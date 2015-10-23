(function (appControllers) {

    "use strict";
    sellingProductAPIService.$inject = ['BaseAPIService'];

    function sellingProductAPIService(BaseAPIService) {

        function GetFilteredSellingProducts(input) {
            return BaseAPIService.post("/api/SellingProduct/GetFilteredSellingProducts", input);
        }

        function GetSellingProduct(sellingProductId) {
            return BaseAPIService.get("/api/SellingProduct/GetSellingProduct", {
                sellingProductId: sellingProductId
            });
        }
        function GetAllSellingProduct() {
            return BaseAPIService.get("/api/SellingProduct/GetAllSellingProduct");
        }
        function AddSellingProduct(sellingProductObject) {
            return BaseAPIService.post("/api/SellingProduct/AddSellingProduct", sellingProductObject);
        }

        function UpdateSellingProduct(sellingProductObject) {
            return BaseAPIService.post("/api/SellingProduct/UpdateSellingProduct", sellingProductObject);
        }
        function DeleteSellingProduct(sellingProductId) {
            
            return BaseAPIService.get("/api/SellingProduct/DeleteSellingProduct", { sellingProductId: sellingProductId });
        }

        return ({
            GetFilteredSellingProducts: GetFilteredSellingProducts,
            AddSellingProduct: AddSellingProduct,
            UpdateSellingProduct: UpdateSellingProduct,
            DeleteSellingProduct: DeleteSellingProduct,
            GetSellingProduct: GetSellingProduct,
            GetAllSellingProduct: GetAllSellingProduct
        });
    }

    appControllers.service('WhS_BE_SellingProductAPIService', sellingProductAPIService);
})(appControllers);