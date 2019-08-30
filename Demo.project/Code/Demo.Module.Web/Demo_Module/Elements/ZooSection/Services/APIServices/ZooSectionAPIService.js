(function (appControllers) {

    'use strict';

    zooSectionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function zooSectionAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var controller = 'Demo_ZooSection';

        function GetFilteredZooSections(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetFilteredZooSections'), input);
        }

        function GetZooSectionTypeConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetZooSectionTypeConfigs'));
        }

        function GetZooSectionTypeAnimalConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetZooSectionTypeAnimalConfigs'));
        }

        function GetZooSectionById(zooSectionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetZooSectionById'), { zooSectionId: zooSectionId });
        }

        function AddZooSection(zooSection) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'AddZooSection'), zooSection);
        }

        function UpdateZooSection(zooSection) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'UpdateZooSection'), zooSection);
        }

        return {
            GetFilteredZooSections: GetFilteredZooSections,
            GetZooSectionTypeConfigs: GetZooSectionTypeConfigs,
            GetZooSectionTypeAnimalConfigs: GetZooSectionTypeAnimalConfigs,
            GetZooSectionById: GetZooSectionById,
            AddZooSection: AddZooSection,
            UpdateZooSection: UpdateZooSection
        };
    }

    appControllers.service('Demo_Module_ZooSectionAPIService', zooSectionAPIService);
})(appControllers);