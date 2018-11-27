using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Reflection;

namespace Vanrise.Web.Base
{
    public class FromBodyApplicationModelConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                foreach (var action in controller.Actions)
                {
                    if (action.ActionMethod.GetCustomAttribute<System.Web.Http.HttpPostAttribute>() != null && action.Parameters.Count == 1)
                    {
                        action.Parameters[0].BindingInfo = new Microsoft.AspNetCore.Mvc.ModelBinding.BindingInfo { BindingSource = new Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource("Body", "Body", true, true) };
                    }
                }
            }
        }
    }
}