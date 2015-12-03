(function (appControllers) {

    "use strict";
    supplierAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_BE_ModuleConfig'];

    function supplierAPIService(BaseAPIService, UtilsService, QM_BE_ModuleConfig) {

        function GetFilteredSuppliers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, "Supplier", "GetFilteredSuppliers"), input);
        }

        function GetSupplier(supplierId) {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, "Supplier", "GetSupplier"), {
                supplierId: supplierId
            });
        }

        function AddSupplier(supplierObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, "Supplier", "AddSupplier"), supplierObject);
        }

        function UpdateSupplier(supplierObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, "Supplier", "UpdateSupplier"), supplierObject);
        }

        return ({
            GetFilteredSuppliers: GetFilteredSuppliers,
            GetSupplier: GetSupplier,
            AddSupplier: AddSupplier,
            UpdateSupplier: UpdateSupplier
        });
    }

    appControllers.service('QM_BE_SupplierAPIService', supplierAPIService);
})(appControllers);