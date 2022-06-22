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
