(function (appControllers) {

    "use strict";
    SiteAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Teles_ModuleConfig'];

    function SiteAPIService(BaseAPIService, UtilsService, Retail_Teles_ModuleConfig) {

        var controllerName = "TelesSite";
        function GetEnterpriseSitesInfo(vrConnectionId, enterpriseId, serializedFilter)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetEnterpriseSitesInfo"), {
                vrConnectionId: vrConnectionId,
                enterpriseId:enterpriseId,
                serializedFilter: serializedFilter,
            });
        }

        function GetSiteRoutingGroupsInfo(vrConnectionId, siteId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetSiteRoutingGroupsInfo"), {
                vrConnectionId: vrConnectionId,
                siteId: siteId,
                serializedFilter: serializedFilter,
            });
        }

        function MapSiteToAccount(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "MapSiteToAccount"), input);
        }

        function AddTelesSite(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "AddTelesSite"), input);
        }

        return ({
            GetEnterpriseSitesInfo: GetEnterpriseSitesInfo,
            MapSiteToAccount: MapSiteToAccount,
            AddTelesSite: AddTelesSite,
            GetSiteRoutingGroupsInfo: GetSiteRoutingGroupsInfo
        });
    }

    appControllers.service('Retail_Teles_SiteAPIService', SiteAPIService);

})(appControllers);