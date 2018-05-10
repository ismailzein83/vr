"use strict";
app.directive("vrGenericdataDatarecordstoragesettingsBusinessobject", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
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
        templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/Templates/DataRecordStorageSettingsBusinessObjectTemplate.html"
    };


    function BusinessObject($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var dataProviderSettingsDirectiveAPI;
        var dataProviderSettingsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.onDataProviderSettingsSelectorReady = function (api) {
                dataProviderSettingsDirectiveAPI = api;
                dataProviderSettingsSelectorReadyPromiseDeferred.resolve();

                var setLoader = function (value) {
                    $scope.scopeModel.isDirectiveLoading = value;
                };

                var daPayload = {

                    extendedSettings: dataProviderSettingsDirectiveAPI.getData()
                };


                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataProviderSettingsDirectiveAPI, daPayload, setLoader, dataProviderSettingsSelectorReadyPromiseDeferred);
            };
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                var promises = [];
                var dataProviderSettingsPayload;

                var loadDataProviderSettingsPromiseDeferred = UtilsService.createPromiseDeferred();

                dataProviderSettingsSelectorReadyPromiseDeferred.promise.then(function () {

                    var dataProviderSettingsPayload;

                    if (payload != undefined && payload.Settings != undefined) {
                        dataProviderSettingsPayload = {
                            extendedSettings: payload.Settings.ExtendedSettings
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(dataProviderSettingsDirectiveAPI, dataProviderSettingsPayload, loadDataProviderSettingsPromiseDeferred);
                });
                promises.push(loadDataProviderSettingsPromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {

                return {
                   $type: "Vanrise.GenericData.Business.BusinessObjectDataRecordStorageSettings, Vanrise.GenericData.Business",
                   Settings:{
                       ExtendedSettings: dataProviderSettingsDirectiveAPI.getData(),
                   },
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}
]);