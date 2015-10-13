app.service("BrandAPIService", function (BaseAPIService) {

    return ({
        GetBrands: GetBrands,
        GetFilteredBrands: GetFilteredBrands,
        GetBrandById: GetBrandById,
        AddBrand: AddBrand,
        UpdateBrand: UpdateBrand,
        DeleteBrand: DeleteBrand
    });

    function GetBrands() {
        return BaseAPIService.get("/api/Brand/GetBrands");
    }

    function GetFilteredBrands(input) {
        return BaseAPIService.post("/api/Brand/GetFilteredBrands", input);
    }

    function GetBrandById(brandId) {
        return BaseAPIService.get("/api/Brand/GetBrandById", {
            brandId: brandId
        });
    }

    function AddBrand(brandObj) {
        return BaseAPIService.post("/api/Brand/AddBrand", brandObj);
    }

    function UpdateBrand(brandObj) {
        return BaseAPIService.post("/api/Brand/UpdateBrand", brandObj);
    }

    function DeleteBrand(brandId) {
        return BaseAPIService.get("/api/Brand/DeleteBrand", {
            brandId: brandId
        });
    }
});
