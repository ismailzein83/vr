
app.service('WhS_SupPL_SupplierPriceListService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {
        return ({
            previewSupplierPriceList: previewSupplierPriceList
        });

        function previewSupplierPriceList(priceListId) {
           
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Supplier PriceList Preview";

            };
            var parameters = {
                PriceListId: priceListId
            };

            VRModalService.showModal('/Client/Modules/WhS_SupplierPriceList/Views/SupplierPriceListPreview.html', parameters, settings);
        }
      
 }]);
