(function (appControllers) {

    "use strict";
    destinationGroupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function destinationGroupAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetFilteredDestinationGroups(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "DestinationGroup", "GetFilteredDestinationGroups"), input);
        }

        function GetDestinationGroup(destinationGroupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "DestinationGroup", "GetDestinationGroup"), {
                destinationGroupId: destinationGroupId
            });

        }

        function UpdateDestinationGroup(destinationGroupObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "DestinationGroup", "UpdateDestinationGroup"), destinationGroupObject);
        }
        function AddDestinationGroup(destinationGroupObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "DestinationGroup", "AddDestinationGroup"), destinationGroupObject);
        }

        function GetGroupTypeTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "DestinationGroup", "GetGroupTypeTemplates"));
        }

        return ({
            GetGroupTypeTemplates: GetGroupTypeTemplates,
            GetFilteredDestinationGroups: GetFilteredDestinationGroups,
            GetDestinationGroup: GetDestinationGroup,
            AddDestinationGroup: AddDestinationGroup,
            UpdateDestinationGroup: UpdateDestinationGroup
        });
    }

    appControllers.service('Demo_DestinationGroupAPIService', destinationGroupAPIService);

})(appControllers);