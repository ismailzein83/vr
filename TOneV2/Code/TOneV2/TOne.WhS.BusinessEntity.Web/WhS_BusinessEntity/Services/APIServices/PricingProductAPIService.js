(function (appControllers) {

    "use strict";
    pricingProductAPIService.$inject = ['BaseAPIService'];

    function pricingProductAPIService(BaseAPIService) {

        function GetFilteredPricingProducts(input) {
            return BaseAPIService.post("/api/PricingProduct/GetFilteredPricingProducts", input);
        }

        function GetPricingProduct(pricingProductId) {
            return BaseAPIService.get("/api/PricingProduct/GetPricingProduct", {
                pricingProductId: pricingProductId
            });
        }
        function GetAllPricingProduct() {
            return BaseAPIService.get("/api/PricingProduct/GetAllPricingProduct");
        }
        function AddPricingProduct(pricingProductObject) {
            return BaseAPIService.post("/api/PricingProduct/AddPricingProduct", pricingProductObject);
        }

        function UpdatePricingProduct(pricingProductObject) {
            return BaseAPIService.post("/api/PricingProduct/UpdatePricingProduct", pricingProductObject);
        }
        function DeletePricingProduct(pricingProductId) {
            return BaseAPIService.get("/api/PricingProduct/DeletePricingProduct", { pricingProductId: pricingProductId });
        }

        return ({
            GetFilteredPricingProducts: GetFilteredPricingProducts,
            AddPricingProduct: AddPricingProduct,
            UpdatePricingProduct: UpdatePricingProduct,
            DeletePricingProduct: DeletePricingProduct,
            GetPricingProduct: GetPricingProduct,
            GetAllPricingProduct: GetAllPricingProduct
        });
    }

    appControllers.service('WhS_BE_PricingProductAPIService', pricingProductAPIService);
})(appControllers);