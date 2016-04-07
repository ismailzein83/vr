﻿(function (appControllers) {

    "use strict";
    customerSellingProductAPIService.$inject = ['BaseAPIService'];

    function customerSellingProductAPIService(BaseAPIService) {

        function GetFilteredCustomerSellingProducts(input) {
            return BaseAPIService.post("/api/CustomerSellingProduct/GetFilteredCustomerSellingProducts", input);
        }

        function GetCustomerSellingProduct(customerSellingProductId) {
            return BaseAPIService.get("/api/CustomerSellingProduct/GetCustomerSellingProduct", {
                customerSellingProductId: customerSellingProductId
            });
        }

        function AddCustomerSellingProduct(customerSellingProductObject) {
            return BaseAPIService.post("/api/CustomerSellingProduct/AddCustomerSellingProduct", customerSellingProductObject);
        }

        function DeleteCustomerSellingProduct(customerSellingProductId) {
            return BaseAPIService.get("/api/CustomerSellingProduct/DeleteCustomerSellingProduct", { customerSellingProductId: customerSellingProductId });
        }
        function UpdateCustomerSellingProduct(customerSellingProductObject) {
            return BaseAPIService.post("/api/CustomerSellingProduct/UpdateCustomerSellingProduct", customerSellingProductObject);
        }
        function IsCustomerAssignedToSellingProduct(customerId) {
            return BaseAPIService.get("/api/CustomerSellingProduct/IsCustomerAssignedToSellingProduct", { customerId: customerId });
        }

        return ({
            GetFilteredCustomerSellingProducts: GetFilteredCustomerSellingProducts,
            AddCustomerSellingProduct: AddCustomerSellingProduct,
            DeleteCustomerSellingProduct: DeleteCustomerSellingProduct,
            GetCustomerSellingProduct: GetCustomerSellingProduct,
            UpdateCustomerSellingProduct: UpdateCustomerSellingProduct,
            IsCustomerAssignedToSellingProduct: IsCustomerAssignedToSellingProduct
        });
    }

    appControllers.service('WhS_BE_CustomerSellingProductAPIService', customerSellingProductAPIService);
})(appControllers);