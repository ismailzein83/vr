'use strict';

app.directive('whsBeCompanypricelistsettingsRuntime', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_SaleAreaSettingsContextEnum',
    function (UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_SaleAreaSettingsContextEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new companyPricelistSettingsCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/CompanyPricelistSettings/Templates/CompanyPricelistSettingsTemplate.html"
        };

        function companyPricelistSettingsCtor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;

            var priceListSettingsEditorAPI;
            var priceListSettingsEditorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                ctrl.onPriceListSettingsEditorReady = function (api) {
                    priceListSettingsEditorAPI = api;
                    priceListSettingsEditorReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var pricelistSettings;

                    if (payload != undefined) {
                        pricelistSettings = payload.PricelistSettings;
                    }

                    return loadPriceListSettings(pricelistSettings);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.CompanyPricelistSettings, TOne.WhS.BusinessEntity.MainExtensions",
                        PricelistSettings: priceListSettingsEditorAPI.getData(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function loadPriceListSettings(data) {
                var priceListSettingsEditorLoadDeferred = UtilsService.createPromiseDeferred();
                priceListSettingsEditorReadyDeferred.promise.then(function () {
                    var payload = {
                        data: data,
                        directiveContext: WhS_BE_SaleAreaSettingsContextEnum.Company.value
                    };
                    VRUIUtilsService.callDirectiveLoad(priceListSettingsEditorAPI, payload, priceListSettingsEditorLoadDeferred);
                });
                return priceListSettingsEditorLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);