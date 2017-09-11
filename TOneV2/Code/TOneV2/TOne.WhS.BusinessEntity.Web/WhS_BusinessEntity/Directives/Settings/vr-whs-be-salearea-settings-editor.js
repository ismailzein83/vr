'use strict';

app.directive('vrWhsBeSaleareaSettingsEditor', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_PrimarySaleEntityEnum', 'VRCommon_CurrencyAPIService', 'VRNotificationService', 'WhS_BE_SaleAreaSettingsContextEnum',
    function (UtilsService, VRUIUtilsService, WhS_BE_PrimarySaleEntityEnum, VRCommon_CurrencyAPIService, VRNotificationService, WhS_BE_SaleAreaSettingsContextEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/SaleAreaSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;

            ctrl.fixedKeywords = [];
            ctrl.mobileKeywords = [];
            ctrl.primarySaleEntities = UtilsService.getArrayEnum(WhS_BE_PrimarySaleEntityEnum);

            var priceListSettingsEditorAPI;
            var priceListSettingsEditorReadyDeferred = UtilsService.createPromiseDeferred();

            var pricingSettingsEditorAPI;
            var pricingSettingsEditorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                ctrl.disabledAddFixedKeyword = true;
                ctrl.disabledAddMobileKeyword = true;

                ctrl.addFixedKeyword = function () {
                    ctrl.fixedKeywords.push({ fixedKeyword: ctrl.fixedKeywordvalue });
                    ctrl.fixedKeywordvalue = undefined;
                    ctrl.disabledAddFixedKeyword = true;
                };

                ctrl.onFixedKeywordValueChange = function (value) {
                    ctrl.disabledAddFixedKeyword = (value == undefined && ctrl.fixedKeywordvalue.length - 1 < 1) || UtilsService.getItemIndexByVal(ctrl.fixedKeywords, value, "fixedKeyword") != -1;
                };

                ctrl.validateAddFixedKeyWords = function () {
                    if (ctrl.fixedKeywords != undefined && ctrl.fixedKeywords.length == 0)
                        return "Enter at least one keyword.";
                    return null;
                };

                ctrl.addMobileKeyword = function () {
                    ctrl.mobileKeywords.push({ mobileKeyword: ctrl.mobileKeywordvalue });
                    ctrl.mobileKeywordvalue = undefined;
                    ctrl.disabledAddMobileKeyword = true;
                };

                ctrl.onMobileKeywordValueChange = function (value) {
                    ctrl.disabledAddMobileKeyword = (value == undefined && ctrl.mobileKeywordvalue.length - 1 < 1) || UtilsService.getItemIndexByVal(ctrl.mobileKeywords, value, "mobileKeyword") != -1;
                };

                ctrl.validateAddMobileKeyWords = function () {
                    if (ctrl.mobileKeywords != undefined && ctrl.mobileKeywords.length == 0)
                        return "Enter at least one keyword.";
                    return null;
                };

                ctrl.onPrimarySaleEntitySelectorReady = function (api) {
                    defineAPI();
                };

                ctrl.onPricingSettingsEditorReady = function (api) {
                    pricingSettingsEditorAPI = api;
                    pricingSettingsEditorReadyDeferred.resolve();
                };

                ctrl.onPriceListSettingsEditorReady = function (api) {
                    priceListSettingsEditorAPI = api;
                    priceListSettingsEditorReadyDeferred.resolve();
                };

            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var data;
                    var pricelistSettings;
                    var pricingSettings;

                    if (payload != undefined) {
                        data = payload.data;
                    }

                    if (data != undefined) {
                        pricelistSettings = data.PricelistSettings;
                        pricingSettings = data.PricingSettings;
                    }

                    loadStaticData(data);

                    var loadPriceListSettingsPromise = loadPriceListSettings(pricelistSettings);
                    promises.push(loadPriceListSettingsPromise);

                    var loadPricingSettingsPromise = loadPricingSettings(pricingSettings);
                    promises.push(loadPricingSettingsPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.SaleAreaSettingsData, TOne.WhS.BusinessEntity.Entities",
                        FixedKeywords: UtilsService.getPropValuesFromArray(ctrl.fixedKeywords, "fixedKeyword"),
                        MobileKeywords: UtilsService.getPropValuesFromArray(ctrl.mobileKeywords, "mobileKeyword"),
                        PrimarySaleEntity: ctrl.primarySaleEntity.value,
                        PricelistSettings: priceListSettingsEditorAPI.getData(),
                        PricingSettings: pricingSettingsEditorAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadStaticData(data) {
                if (data == undefined)
                    return;

                ctrl.primarySaleEntity = UtilsService.getItemByVal(ctrl.primarySaleEntities, data.PrimarySaleEntity, 'value');

                if (data.FixedKeywords != null) {
                    for (var i = 0; i < data.FixedKeywords.length; i++)
                        ctrl.fixedKeywords.push({ fixedKeyword: data.FixedKeywords[i] });
                }
                if (data.MobileKeywords != null) {
                    for (var i = 0; i < data.MobileKeywords.length; i++)
                        ctrl.mobileKeywords.push({ mobileKeyword: data.MobileKeywords[i] });
                }
            }

            function loadPriceListSettings(data) {
                var priceListSettingsEditorLoadDeferred = UtilsService.createPromiseDeferred();
                priceListSettingsEditorReadyDeferred.promise.then(function () {
                    var payload = {
                        data: data,
                        directiveContext: WhS_BE_SaleAreaSettingsContextEnum.System.value
                    };
                    VRUIUtilsService.callDirectiveLoad(priceListSettingsEditorAPI, payload, priceListSettingsEditorLoadDeferred);
                });
                return priceListSettingsEditorLoadDeferred.promise;
            }

            function loadPricingSettings(data) {
                var pricingSettingsEditorLoadDeferred = UtilsService.createPromiseDeferred();
                pricingSettingsEditorReadyDeferred.promise.then(function () {
                    var payload = {
                        data: data,
                        directiveContext: WhS_BE_SaleAreaSettingsContextEnum.System.value
                    };
                    VRUIUtilsService.callDirectiveLoad(pricingSettingsEditorAPI, payload, pricingSettingsEditorLoadDeferred);
                });
                return pricingSettingsEditorLoadDeferred.promise;
            }

        }

        return directiveDefinitionObject;
    }]);