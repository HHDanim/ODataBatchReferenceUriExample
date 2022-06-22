# OData Reference URIs in request body

Take the following entities with a one-to-many relationship between `Parent` and `Child`:

```cs
public class Parent {
    public int ParentId {
        get; set;
    }

    public virtual ICollection<Child> Children {
        get; set;
    }

    public Parent() {
        Children = new HashSet<Child>();
    }
}


public class Child {
    public int ChildId {
        get; set;
    }

    public int? ParentId {
        get; set;
    }

    public virtual Parent? Parent {
        get; set;
    }
}
```

I'd like to create one new entity of both types within a single changeset and have the child linked to it's parent by using the reference URI `"Parent@odata.bind": "$1"`. This is an example request:

```txt
Content-Type:application/json
Accept:application/json
OData-MaxVersion:4.0
OData-Version:4.0
```

```json
{
  "requests": [
    {
      "id": "1",
      "method": "POST",
      "url": "http://localhost:5254/odata/Parents",
      "headers": {
        "OData-Version": "4.0",
        "Content-Type": "application/json;odata.metadata=minimal",
        "Accept": "application/json;odata.metadata=minimal"
      },
      "body": {}
    },
    {
      "id": "2",
      "method": "POST",
      "url": "http://localhost:5254/odata/Children",
      "headers": {
        "OData-Version": "4.0",
        "Content-Type": "application/json;odata.metadata=minimal",
        "Accept": "application/json;odata.metadata=minimal"
      },
      "body": {
        "Parent@odata.bind": "$1"
      }
    }
  ]
}
```

The `Post(Parent parent)` action method of the `ParentsController` always returns a new Parent object with ParentId set to 123. I would expect the `Child` object passed in as parameter to the `Post(Child child)` action method of the `ChildrenController` to have a nested `Parent` object. Although this is the case, the `ParentId` property value of the nested object is always 0 instead of 123.

Expected:

```cs
{
	ChildId: 0,
	ParentId: null,
	Parent: {
		ParentId: 123  // <----- expected
	}
}
```

Actual:

```cs
{
	ChildId: 0,
	ParentId: null,
	Parent: {
		ParentId: 0  // <----- actual
	}
}
```

If you use a full qualified url like 'http://localhost:5254/odata/Parents(111)' instead of $1 in the request the id is extracted correctly and available through the parameter object. This makes me believe that there is an issue with the substitution of the placeholders with the concrete urls from the content-id map. I traced the calls down to the method `ODataJsonLightDeserializer.ReadAndValidateAnnotationStringValueAsUriAsync()`:

```cs
internal async Task<Uri> ReadAndValidateAnnotationStringValueAsUriAsync(string annotationName)
{
	string stringValue = await this.ReadAndValidateAnnotationStringValueAsync(annotationName)
		.ConfigureAwait(false);
	return this.ReadingResponse ? this.ProcessUriFromPayload(stringValue) : new Uri(stringValue, UriKind.RelativeOrAbsolute);
}
```

`ReadAndValidateAnnotationStringValueAsync(annotationName)` returns "$1" and `ReadingResponse` is false for my request. So a new Uri "$1" is constructed, which is not a valid OData Uri an does not contain any entity key. I suspect that to be the reason why I always get the default value 0 for the `ParentId` property.

If I change the code to execute `this.ProcessUriFromPayload(stringValue)` it returns the desired url "http://localhost:5254/odata/Parents(123)" and the `ParentId` value is set to 123.
