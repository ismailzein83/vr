using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.MainExtensions
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericBESerialNumber")]
    public class GenericBESerialNumberController:BaseAPIController
    {
    }
}
