(function (appControllers) {

    "use strict";
    supplierAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_BE_ModuleConfig', 'SecurityService'];

    function supplierAPIService(BaseAPIService, UtilsService, QM_BE_ModuleConfig, SecurityService) {

        var controllerName = 'Supplier';

        function GetFilteredSuppliers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSuppliers"), input);
        }

        function GetSupplier(supplierId) {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "GetSupplier"), {
                supplierId: supplierId
            });
        }

        function AddSupplier(supplierObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "AddSupplier"), supplierObject);
        }
        function HasAddSupplierPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(QM_BE_ModuleConfig.moduleName, controllerName, ['AddSupplier']));
        }

        function HasDeleteSupplierPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(QM_BE_ModuleConfig.moduleName, controllerName, ['DeleteSupplier']));
        }

        function UpdateSupplier(supplierObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "UpdateSupplier"), supplierObject);
        }

        function DeleteSupplier(supplierObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "DeleteSupplier"), supplierObject);
        }

        function HasEditSupplierPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(QM_BE_ModuleConfig.moduleName, controllerName, ['UpdateSupplier']));
        }

        function GetSupplierSourceTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "GetSupplierSourceTemplates"));
        }

        function GetSuppliersInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "GetSuppliersInfo"));
        }

        function DownloadImportSupplierTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "DownloadImportSupplierTemplate"), {},
                {
                    returnAllResponseParameters: true,
                    responseTypeAsBufferArray: true
                });
        }
        function HasDownloadSupplierPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(QM_BE_ModuleConfig.moduleName, controllerName, ['DownloadImportSupplierTemplate']));
        }


        function UploadSuppliersList(fileId, allowUpdate) {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "UploadSuppliersList"), {
                fileId: fileId,
                allowUpdate: allowUpdate
            });
        }
        function HasUploadSupplierPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(QM_BE_ModuleConfig.moduleName, controllerName, ['UploadSuppliersList']));
        }

        return ({
            HasDownloadSupplierPermission: HasDownloadSupplierPermission,
            HasUploadSupplierPermission: HasUploadSupplierPermission,
            HasEditSupplierPermission: HasEditSupplierPermission,
            HasAddSupplierPermission: HasAddSupplierPermission,
            HasDeleteSupplierPermission: HasDeleteSupplierPermission,
            GetFilteredSuppliers: GetFilteredSuppliers,
            GetSupplier: GetSupplier,
            AddSupplier: AddSupplier,
            UpdateSupplier: UpdateSupplier,
            DeleteSupplier: DeleteSupplier,
            GetSupplierSourceTemplates: GetSupplierSourceTemplates,
            GetSuppliersInfo: GetSuppliersInfo,
            DownloadImportSupplierTemplate: DownloadImportSupplierTemplate,
            UploadSuppliersList: UploadSuppliersList
        });
    }

    appControllers.service('QM_BE_SupplierAPIService', supplierAPIService);
})(appControllers);