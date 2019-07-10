﻿'use strict';

app.directive('whsBeCompanypurchasepricelistsettingsRuntime', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_PurchaseSettingsContextEnum',
    function (UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_PurchaseSettingsContextEnum) {

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
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/CompanyPricelistSettings/Templates/CompanyPurchasePricelistSettingsTemplate.html"
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

                    var pricelistSettings;

                    if (payload != undefined) {
                        pricelistSettings = payload.PricelistSettings;
                    }

                    return loadPriceListSettings(pricelistSettings);
                };

                api.getData = function () {
                    var obj = {
                        $type: "TOne.WhS.BusinessEntity.Entities.CompanyPurchasePricelistSettings, TOne.WhS.BusinessEntity.Entities",
                        PricelistSettings: priceListSettingsEditorAPI.getData()
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function loadPriceListSettings(data) {
                var priceListSettingsEditorLoadDeferred = UtilsService.createPromiseDeferred();
                priceListSettingsEditorReadyDeferred.promise.then(function () {
                    var payload = {
                        data: data,
                        directiveContext: WhS_BE_PurchaseSettingsContextEnum.Company.value
                    };
                    VRUIUtilsService.callDirectiveLoad(priceListSettingsEditorAPI, payload, priceListSettingsEditorLoadDeferred);
                });
                return priceListSettingsEditorLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);