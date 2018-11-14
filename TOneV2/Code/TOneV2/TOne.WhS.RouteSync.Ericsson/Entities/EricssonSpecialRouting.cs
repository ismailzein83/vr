using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public abstract class EricssonSpecialRoutingSetting
    {
        public abstract Guid ConfigId { get; }

        public abstract List<EricssonConvertedRoute> Execute(IEricssonSpecialRoutingSettingContext context);
    }

    public class EricssonSpecialRoutingServiceLanguage : EricssonSpecialRoutingSetting
    {
        public override Guid ConfigId { get { return new Guid("A9497EF9-2074-4A1E-A999-08253B68F448"); } }

        public List<CodeGroupSuffix> CodeGroupSuffixes { get; set; }

        public override List<EricssonConvertedRoute> Execute(IEricssonSpecialRoutingSettingContext context)
        {
            List<EricssonConvertedRoute> result = new List<EricssonConvertedRoute>();
            CodeGroupManager codeGroupManager = new CodeGroupManager();

            if (context.SourceRoutes == null || context.SourceRoutes.Count == 0)
                return null;

            foreach (var sourceRoute in context.SourceRoutes)
            {
                EricssonRouteType targetRouteRouteType;

                if (sourceRoute.RouteType == EricssonRouteType.ANumber)
                    targetRouteRouteType = EricssonRouteType.ANumberServiceLanguage;
                else if (sourceRoute.RouteType == EricssonRouteType.BNumber)
                    targetRouteRouteType = EricssonRouteType.BNumberServiceLanguage;
                else
                    throw new ArgumentException(string.Format("{0} can not be source route type.", sourceRoute.RouteType));

                result.Add(new EricssonConvertedRoute()
                {
                    BO = context.TargetBO,
                    Code = sourceRoute.Code,
                    RCNumber = sourceRoute.RCNumber,
                    RouteType = targetRouteRouteType
                });

                if (CodeGroupSuffixes != null && CodeGroupSuffixes.Count > 0)
                {
                    var codeGroupObject = codeGroupManager.GetMatchCodeGroup(sourceRoute.Code);
                    codeGroupObject.ThrowIfNull(string.Format("No Code Group found for code '{0}'.", sourceRoute.Code));

                    if (string.Compare(codeGroupObject.Code, sourceRoute.Code) == 0)
                    {
                        foreach (var suffix in CodeGroupSuffixes)
                        {
                            result.Add(new EricssonConvertedRoute()
                            {
                                BO = context.TargetBO,
                                Code = sourceRoute.Code + suffix.Suffix,
                                RCNumber = sourceRoute.RCNumber,
                                RouteType = targetRouteRouteType
                            });
                        }
                    }
                }
            }
            return result;
        }
    }

    public class CodeGroupSuffix
    {
        public string Suffix { get; set; }
    }

    public class EricssonSpecialRoutingSettingConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_EricssonSpecialRoutingServiceLanguageConfig";

        public string Editor { get; set; }
    }

    public interface IEricssonSpecialRoutingSettingContext
    {
        List<EricssonConvertedRoute> SourceRoutes { get; set; }
        string TargetBO { get; set; }
    }

    public class EricssonSpecialRoutingSettingContext : IEricssonSpecialRoutingSettingContext
    {
        public List<EricssonConvertedRoute> SourceRoutes { get; set; }
        public string TargetBO { get; set; }
    }
}
