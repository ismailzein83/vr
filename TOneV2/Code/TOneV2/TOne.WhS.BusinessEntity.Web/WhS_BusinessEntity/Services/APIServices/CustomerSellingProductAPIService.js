(function (appControllers) {

    "use strict";
    customerSellingProductAPIService.$inject = ['BaseAPIService', 'WhS_BE_ModuleConfig', 'SecurityService', 'UtilsService'];

    function customerSellingProductAPIService(BaseAPIService, WhS_BE_ModuleConfig, SecurityService, UtilsService) {

        var controllerName = "CustomerSellingProduct";


        function GetFilteredCustomerSellingProducts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredCustomerSellingProducts"), input);
        }

        function GetCustomerSellingProduct(customerSellingProductId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCustomerSellingProduct"), {
                customerSellingProductId: customerSellingProductId
            });
        }

        function AddCustomerSellingProduct(customerSellingProductObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddCustomerSellingProduct"), customerSellingProductObject);
        }

        function DeleteCustomerSellingProduct(customerSellingProductId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "DeleteCustomerSellingProduct"),
                { customerSellingProductId: customerSellingProductId });
        }

        function UpdateCustomerSellingProduct(customerSellingProductObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateCustomerSellingProduct"), customerSellingProductObject);
        }


        function IsCustomerAssignedToSellingProduct(customerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "IsCustomerAssignedToSellingProduct"),
                { customerId: customerId });
        }

        function HasUpdateCustomerSellingProductPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateCustomerSellingProduct']));
        }

        function HasAddCustomerSellingProductPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AddCustomerSellingProduct']));
        }

        function GetCustomerNamesBySellingProductId(sellingProductId) {
        	return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCustomerNamesBySellingProductId"), {
        		sellingProductId: sellingProductId
        	});
        }

        return ({
            GetFilteredCustomerSellingProducts: GetFilteredCustomerSellingProducts,
            AddCustomerSellingProduct: AddCustomerSellingProduct,
            DeleteCustomerSellingProduct: DeleteCustomerSellingProduct,
            GetCustomerSellingProduct: GetCustomerSellingProduct,
            UpdateCustomerSellingProduct: UpdateCustomerSellingProduct,
            IsCustomerAssignedToSellingProduct: IsCustomerAssignedToSellingProduct,
            HasUpdateCustomerSellingProductPermission: HasUpdateCustomerSellingProductPermission,
            HasAddCustomerSellingProductPermission: HasAddCustomerSellingProductPermission,
            GetCustomerNamesBySellingProductId: GetCustomerNamesBySellingProductId
        });
    }

    appControllers.service('WhS_BE_CustomerSellingProductAPIService', customerSellingProductAPIService);
})(appControllers);