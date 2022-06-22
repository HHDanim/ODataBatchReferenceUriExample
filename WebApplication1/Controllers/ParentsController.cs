using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace WebApplication1.Controllers {
    public class ParentsController : ODataController {
        public IActionResult Post([FromBody] Parent parent) {
            Parent newParent = new Parent() {
                ParentId = 123
            };

            return Created(newParent);
        }
    }
}
