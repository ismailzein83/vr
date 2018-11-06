using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
  [RoutePrefix(Constants.ROUTE_PREFIX+"Color")]
    [JSONWithTypeAttribute]
    public class ColorController : BaseAPIController
     {
      ColorManager colorManager = new ColorManager();
      [HttpGet]
      [Route("GetColorsInfo")]
      public IEnumerable<ColorInfo> ColorInfo(string filter = null)
      {
          ColorInfoFilter ColorInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<ColorInfoFilter>(filter) : null;
          return colorManager.GetColorsInfo(ColorInfoFilter);
      }

      [HttpGet]
      [Route("GetColorById")]
      public Color GetColorById(int colorId)
      {
          return colorManager.GetColorById(colorId);

      }

     }
}