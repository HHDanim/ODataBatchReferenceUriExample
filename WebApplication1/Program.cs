using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

// OData Model
ODataConventionModelBuilder odataBuilder = new ODataConventionModelBuilder();
odataBuilder.EntitySet<Child>("Children");
odataBuilder.EntitySet<Parent>("Parents");

IEdmModel? model = odataBuilder.GetEdmModel();


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddOData(options => { options.AddRouteComponents("odata", model, new DefaultODataBatchHandler()); });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.UseODataBatching();
app.UseRouting();

app.MapControllers();

app.Run();
