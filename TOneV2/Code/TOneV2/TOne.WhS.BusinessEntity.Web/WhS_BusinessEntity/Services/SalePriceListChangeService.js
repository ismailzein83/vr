(function (appControllers) {

    'use strict';

    salePriceListPreviewService.$inject = ['VRModalService'];

    function salePriceListPreviewService(vrModalService) {
        return ({
            previewPriceList: previewPriceList,
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
        function sendEmail(emailResponse, onSalePriceListSendingEmail) {
            var parametrs =
                    {
                        evaluatedEmail: emailResponse.EvaluatedTemplate,
                        fileId: emailResponse.FileId
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
