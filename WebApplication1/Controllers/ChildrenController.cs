using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplication1.Controllers {
    public class ChildrenController : ODataController {
        public IActionResult Post([FromBody] Child child) {
            Assert.IsNotNull(child, "child should be set");
            Assert.IsNotNull(child.Parent, "child.Parent should be set");
            Assert.AreEqual(123, child.Parent.ParentId, "child.Parent.ParentId should be 123, not default value (0)");

            Child newChild = new Child() {
                ChildId = 1,
                ParentId = child.ParentId
            };

            return Created(newChild);
        }
    }
}
