(function (appControllers) {

    "use strict";
    switchBrandAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PSTN_BE_ModuleConfig'];

    function switchBrandAPIService(BaseAPIService, UtilsService, PSTN_BE_ModuleConfig) {

        function GetBrands() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "SwitchBrand","GetBrands"));
        }

        function GetFilteredBrands(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "SwitchBrand","GetFilteredBrands"), input);
        }

        function GetBrandById(brandId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "SwitchBrand","GetBrandById"), {
                brandId: brandId
            });
        }

        function AddBrand(brandObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "SwitchBrand","AddBrand"), brandObj);
        }

        function UpdateBrand(brandObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "SwitchBrand","UpdateBrand"), brandObj);
        }

        function DeleteBrand(brandId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "SwitchBrand","DeleteBrand"), {
                brandId: brandId
            });
        }
        return ({
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
