"use strict";
app.directive("vrInvoiceBusinessobjectDataprovidersettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var businessObject = new BusinessObject($scope, ctrl, $attrs);
            businessObject.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/VR_Invoice/Directives/Extensions/GenericData/Templates/InvoiceBusinessObjectDataProviderSettingsTemplate.html'
    };


    function BusinessObject($scope, ctrl, $attrs) {
        this.initializeController = initializeController;


        var dataProviderSettingsDirectiveAPI;
        var dataProviderSettingsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.InvoiceSelectorReady = function (api) {
                dataProviderSettingsDirectiveAPI = api;
                dataProviderSettingsSelectorReadyPromiseDeferred.resolve();

                var setLoader = function (value) {
                    $scope.scopeModel.isDirectiveLoading = value;
                };

                var daPayload = {
                    filter: {
                        Filters: []
                    },
                };
                daPayload.filter.Filters.push({ $type: "Vanrise.Invoice.Business.InvoiceBusinessObjectDataProviderSettings, Vanrise.Invoice.Business" });

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataProviderSettingsDirectiveAPI, daPayload, setLoader, dataProviderSettingsSelectorReadyPromiseDeferred);
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var promises = [];
                var loadDataProviderSettingsPromiseDeferred = UtilsService.createPromiseDeferred();

                dataProviderSettingsSelectorReadyPromiseDeferred.promise.then(function () {

                    var dataProviderSettingsPayload = {
                        filter: {
                            Filters: []
                        }
                    };

                    dataProviderSettingsPayload.filter.Filters.push({ $type: "Vanrise.GenericData.Business.BusinessObjectDataRecordStorageSettings, Vanrise.GenericData.Business" });

                    if (payload != undefined) {

                        dataProviderSettingsPayload.selectedIds = payload.extendedSettings.InvoiceTypeId;
                    }
                    VRUIUtilsService.callDirectiveLoad(dataProviderSettingsDirectiveAPI, dataProviderSettingsPayload, loadDataProviderSettingsPromiseDeferred);
                });
                promises.push(loadDataProviderSettingsPromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {

                return {
                    $type: "Vanrise.Invoice.Business.InvoiceBusinessObjectDataProviderSettings, Vanrise.Invoice.Business",
                    InvoiceTypeId: dataProviderSettingsDirectiveAPI.getSelectedIds(),
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}
]);