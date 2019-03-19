using System;

namespace CoreLib
{
    [Microsoft.AspNetCore.Mvc.Route("api/ModName/CoreAPI")]
    public class CoreAPIController : Microsoft.AspNetCore.Mvc.Controller
    {
        public string GetItemFromArray(int itmIndex)
        {
            return new string[] { "Item 1", "Item 2", "Item 3" }[itmIndex];
        }
    }
}
