(function (appControllers) {

    'use strict';

    SellingProductService.$inject = ['WhS_BE_SellingProductAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService', 'UtilsService'];

    function SellingProductService(WhS_BE_SellingProductAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService, UtilsService) {
        return ({
            addSellingProduct: addSellingProduct,
            editSellingProduct: editSellingProduct,
            getEntityUniqueName: getEntityUniqueName,
            registerHistoryViewAction: registerHistoryViewAction
        });

        function addSellingProduct(onSellingProductAdded, sellingNumberPlanId) {
            var settings = {};
            var parameters = {
                sellingNumberPlanId: sellingNumberPlanId
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSellingProductAdded = onSellingProductAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/SellingProductEditor.html', parameters, settings);
        }

        function editSellingProduct(sellingProductObj, onSellingProductUpdated) {
            var modalSettings = {
            };
            var parameters = {
                SellingProductId: sellingProductObj.SellingProductId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSellingProductUpdated = onSellingProductUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/SellingProductEditor.html', parameters, modalSettings);
        }

        function viewHistorySellingProduct(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/SellingProductEditor.html', modalParameters, modalSettings);
        };
        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "WhS_BusinessEntity_SellingProduct_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistorySellingProduct(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }

        function getEntityUniqueName() {
            return "WhS_BusinessEntity_SellingProduct";
        }
    }

    appControllers.service('WhS_BE_SellingProductService', SellingProductService);

})(appControllers);
