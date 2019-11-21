using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
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

            TechnicalCodeManager technicalCodeManager = new TechnicalCodeManager();

            foreach (var sourceRoute in context.SourceRoutes)
            {
                result.Add(new EricssonConvertedRoute()
                {
                    BO = context.TargetBO,
                    Code = sourceRoute.Code,
                    RCNumber = sourceRoute.RCNumber,
                    TRD = sourceRoute.TRD,
                    RouteType = sourceRoute.RouteType
                });
            }

            if (CodeGroupSuffixes != null && context.CodeGroupRoutes != null)
            {
                foreach (var codeGroupRoute in context.CodeGroupRoutes)
                {
                    foreach (var suffix in CodeGroupSuffixes)
                    {
                        TechnicalCodePrefix technicalCodePrefix = technicalCodeManager.GetTechnicalCodeByNumberPrefix(codeGroupRoute.CodeGroup);
                        if (technicalCodePrefix == null)
                            throw new NullReferenceException($"No Technical Code Match is found for the following Code: '{codeGroupRoute.CodeGroup}'");
                        var trd = technicalCodePrefix.ZoneID;

                        result.Add(new EricssonConvertedRoute()
                        {
                            BO = context.TargetBO,
                            Code = string.Concat(codeGroupRoute.CodeGroup, suffix.Suffix),
                            RCNumber = codeGroupRoute.RCNumber,
                            TRD = trd,
                            RouteType = EricssonRouteType.BNumberServiceLanguage
                        });
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
        List<EricssonConvertedRoute> SourceRoutes { get; }

        List<CodeGroupRoute> CodeGroupRoutes { get; }

        int TargetBO { get; }
    }

    public class EricssonSpecialRoutingSettingContext : IEricssonSpecialRoutingSettingContext
    {
        public List<EricssonConvertedRoute> SourceRoutes { get; set; }

        public List<CodeGroupRoute> CodeGroupRoutes { get; set; }

        public int TargetBO { get; set; }
    }
}