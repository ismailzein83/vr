app.service("SwitchBrandAPIService", function (BaseAPIService) {

    return ({
        GetBrands: GetBrands,
        GetFilteredBrands: GetFilteredBrands,
        GetBrandById: GetBrandById,
        AddBrand: AddBrand,
        UpdateBrand: UpdateBrand,
        DeleteBrand: DeleteBrand
    });

    function GetBrands() {
        return BaseAPIService.get("/api/SwitchBrand/GetBrands");
    }

    function GetFilteredBrands(input) {
        return BaseAPIService.post("/api/SwitchBrand/GetFilteredBrands", input);
    }

    function GetBrandById(brandId) {
        return BaseAPIService.get("/api/SwitchBrand/GetBrandById", {
            brandId: brandId
        });
    }

    function AddBrand(brandObj) {
        return BaseAPIService.post("/api/SwitchBrand/AddBrand", brandObj);
    }

    function UpdateBrand(brandObj) {
        return BaseAPIService.post("/api/SwitchBrand/UpdateBrand", brandObj);
    }

    function DeleteBrand(brandId) {
        return BaseAPIService.get("/api/SwitchBrand/DeleteBrand", {
            brandId: brandId
        });
    }
});
