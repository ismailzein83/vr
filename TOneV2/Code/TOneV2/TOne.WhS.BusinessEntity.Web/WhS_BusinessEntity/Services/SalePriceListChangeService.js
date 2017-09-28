(function (appControllers) {

    'use strict';

    salePriceListPreviewService.$inject = ['VRModalService'];

    function salePriceListPreviewService(vrModalService) {
        return ({
            previewPriceList: previewPriceList,
            salePricelistFilePreview: salePricelistFilePreview,
            sendEmail: sendEmail
        });

        function previewPriceList(priceListId, onSalePriceListPreviewClosed) {
            var modalParameters = {
                PriceListId: priceListId
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSalePriceListPreviewClosed = onSalePriceListPreviewClosed;
            };
            vrModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SalePricelist/SalePriceListChange.html', modalParameters, modalSettings);
        }
        function salePricelistFilePreview(vrFiles) {
            var modalParameters = {
                vrFiles: vrFiles
            };
            var modalSettings = {};
            vrModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SalePricelist/SalePricelistFilePreview.html', modalParameters, modalSettings);
        }
        function sendEmail(emailResponse, onSalePriceListSendingEmail) {

            var parametrs =
                    {
                        evaluatedEmail: emailResponse.EvaluatedTemplate,
                        saleVrFiles: emailResponse.SalePricelistVrFiles
                    };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSalePriceListSendingEmail = onSalePriceListSendingEmail;
            };
            vrModalService.showModal('/Client/Modules/Common/Views/VRMail/VRMailMessageEvaluator.html', parametrs, modalSettings);
        }
    };
    appControllers.service('WhS_BE_SalePriceListChangeService', salePriceListPreviewService);

})(appControllers);
