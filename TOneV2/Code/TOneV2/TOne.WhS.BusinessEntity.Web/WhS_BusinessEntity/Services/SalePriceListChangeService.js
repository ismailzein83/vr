(function (appControllers) {

    'use strict';

    salePriceListPreviewService.$inject = ['VRModalService', 'UtilsService'];

    function salePriceListPreviewService(vrModalService, UtilsService) {
        return ({
            previewPriceList: previewPriceList,
            salePricelistFilePreview: salePricelistFilePreview,
            sendEmail: sendEmail,
            showSendPricelistsConfirmation: showSendPricelistsConfirmation,
            openCheckPriceListPreview: openCheckPriceListPreview
        });

        function previewPriceList(priceListId, onSalePriceListPreviewClosed, shouldOpenEmailPage) {
            var modalParameters = {
                PriceListId: priceListId,
                shouldOpenEmailPage: shouldOpenEmailPage
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSalePriceListPreviewClosed = onSalePriceListPreviewClosed;
            };
            vrModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SalePricelist/SalePriceListChange.html', modalParameters, modalSettings);
        }
        function openCheckPriceListPreview(processInstanceId) {
            var modalParameters = {
                processInstanceId: processInstanceId
            };
            var modalSettings = {};
            vrModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SalePricelist/TemporarySalePriceListsEditorTemplate.html', modalParameters, modalSettings);
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

        function showSendPricelistsConfirmation(customers) {
            var parameters = {
            customers: customers,
            };

            var modalSettings = {};

            var deferred = UtilsService.createPromiseDeferred();

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCancelSendSalePricelist = function () {
                    modalScope.modalContext.closeModal();
                    var response={
                        decision: false
                };
                    deferred.resolve(response);
                };
                modalScope.onContinueSendSalePricelist = function () {
                    modalScope.modalContext.closeModal();
                        var response={
                        decision: true
                    };
                    deferred.resolve(response);
                };
            };

            vrModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SalePricelist/SendSalePricelistConfirmation.html', parameters, modalSettings);
            return deferred.promise;
        }
    }
    appControllers.service('WhS_BE_SalePriceListChangeService', salePriceListPreviewService);

})(appControllers);
