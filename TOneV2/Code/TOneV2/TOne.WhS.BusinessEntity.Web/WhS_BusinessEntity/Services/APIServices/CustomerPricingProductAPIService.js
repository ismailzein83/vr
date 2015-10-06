(function (appControllers) {

    "use strict";
    customerPricingProductAPIService.$inject = ['BaseAPIService'];

    function customerPricingProductAPIService(BaseAPIService) {

        function GetFilteredCustomerPricingProducts(input) {
            return BaseAPIService.post("/api/CustomerPricingProduct/GetFilteredCustomerPricingProducts", input);
        }

        function GetCustomerPricingProduct(customerPricingProductId) {
            return BaseAPIService.get("/api/CustomerPricingProduct/GetCustomerPricingProduct", {
                customerPricingProductId: customerPricingProductId
            });
        }

        function AddCustomerPricingProduct(customerPricingProductObject) {
            return BaseAPIService.post("/api/CustomerPricingProduct/AddCustomerPricingProduct", customerPricingProductObject);
        }

        function DeleteCustomerPricingProduct(customerPricingProductId) {
            return BaseAPIService.get("/api/CustomerPricingProduct/DeleteCustomerPricingProduct", { customerPricingProductId: customerPricingProductId });
        }

        return ({
            GetFilteredCustomerPricingProducts: GetFilteredCustomerPricingProducts,
            AddCustomerPricingProduct: AddCustomerPricingProduct,
            DeleteCustomerPricingProduct: DeleteCustomerPricingProduct,
            GetCustomerPricingProduct: GetCustomerPricingProduct
        });
    }

    appControllers.service('WhS_BE_CustomerPricingProductAPIService', customerPricingProductAPIService);
})(appControllers);