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
        function GetSupplierSourceTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, "Supplier", "GetSupplierSourceTemplates"));
        }

        function GetSuppliersInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, "Supplier", "GetSuppliersInfo"));
        }

        function DownloadImportSupplierTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, "Supplier", "DownloadImportSupplierTemplate"), {},
                {
                    returnAllResponseParameters: true,
                    responseTypeAsBufferArray: true
                });
        }

        function UploadSuppliers(fileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, "Supplier", "UploadSuppliers"), {
                fileId: fileId
            })
        }

        return ({
            GetFilteredSuppliers: GetFilteredSuppliers,
            GetSupplier: GetSupplier,
            AddSupplier: AddSupplier,
            UpdateSupplier: UpdateSupplier,
            GetSupplierSourceTemplates: GetSupplierSourceTemplates,
            GetSuppliersInfo: GetSuppliersInfo,
            DownloadImportSupplierTemplate: DownloadImportSupplierTemplate,
            UploadSuppliers: UploadSuppliers
        });
    }

    appControllers.service('QM_BE_SupplierAPIService', supplierAPIService);
})(appControllers);