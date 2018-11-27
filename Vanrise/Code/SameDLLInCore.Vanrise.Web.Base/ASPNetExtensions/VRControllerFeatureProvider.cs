using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Vanrise.Common;

namespace Vanrise.Web.Base
{
    public class VRControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {

            var assembly = Assembly.Load("StandardWebLib");
            var candidates = assembly.GetExportedTypes();

            foreach (var candidate in Utilities.GetAllImplementations<BaseAPIController>())
            {
                if (candidate.GetCustomAttribute<System.Web.Http.RoutePrefixAttribute>() != null)
                    feature.Controllers.Add(candidate.GetTypeInfo());
            }
        }
    }
}
