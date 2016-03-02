(function (appControllers) {

    "use strict";
    switchBrandAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PSTN_BE_ModuleConfig', 'SecurityService'];

    function switchBrandAPIService(BaseAPIService, UtilsService, PSTN_BE_ModuleConfig, SecurityService) {

        var controllerName = 'SwitchBrand';

        function GetBrands() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetBrands"));
        }

        function GetFilteredBrands(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetFilteredBrands"), input);
        }

        function GetBrandById(brandId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetBrandById"), {
                brandId: brandId
            });
        }

        function AddBrand(brandObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "AddBrand"), brandObj);
        }

        function UpdateBrand(brandObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "UpdateBrand"), brandObj);
        }

        function DeleteBrand(brandId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "DeleteBrand"), {
                brandId: brandId
            });
        }

        function HasAddSwitchBrandPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(PSTN_BE_ModuleConfig.moduleName, controllerName, ['AddBrand']));
        }

        function HasUpdateSwitchBrandPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(PSTN_BE_ModuleConfig.moduleName, controllerName, ['UpdateBrand']));
        }

        function HasDeleteSwitchBrandPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(PSTN_BE_ModuleConfig.moduleName, controllerName, ['DeleteBrand']));
        }

        return ({
            HasAddSwitchBrandPermission: HasAddSwitchBrandPermission,
            HasUpdateSwitchBrandPermission: HasUpdateSwitchBrandPermission,
            HasDeleteSwitchBrandPermission: HasDeleteSwitchBrandPermission,
            GetBrands: GetBrands,
            GetFilteredBrands: GetFilteredBrands,
            GetBrandById: GetBrandById,
            AddBrand: AddBrand,
            UpdateBrand: UpdateBrand,
            DeleteBrand: DeleteBrand
        });
    }

    appControllers.service('CDRAnalysis_PSTN_SwitchBrandAPIService', switchBrandAPIService);

})(appControllers);
