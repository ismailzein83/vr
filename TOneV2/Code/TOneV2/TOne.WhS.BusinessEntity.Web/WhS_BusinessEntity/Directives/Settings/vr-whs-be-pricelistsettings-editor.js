'use strict';

app.directive('vrWhsBePricelistsettingsEditor', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_SaleAreaSettingsContextEnum',
function (UtilsService, VRUIUtilsService, WhS_BE_SaleAreaSettingsContextEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new pricelistSettingEditorCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/PriceListSettingsTemplate.html"
    };

    function pricelistSettingEditorCtor(ctrl, $scope, $attrs) {

        this.initializeController = initializeController;

        var salePLTemplateSelectorAPI;
        var salePLTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var mailMsgTemplateSelectorAPI;
        var mailMsgTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var priceListExtensionFormatSelectorAPI;
        var priceListExtensionFormatSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var priceListTypeSelectorAPI;
        var priceListTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var CompressFileSelectorAPI;
        var CompressFileSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var CodeChangeTypeSettingsGridAPI;
        var CodeChangeTypeSettingsGridReadyDeferred = UtilsService.createPromiseDeferred();

        var RateChangeTypeSettingsGridAPI;
        var RateChangeTypeSettingsGridReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            ctrl.onPriceListEmailTemplateSelectorReady = function (api) {
                mailMsgTemplateSelectorAPI = api;
                mailMsgTemplateSelectorReadyDeferred.resolve();
            };

            ctrl.onPriceListExtensionFormatSelectorReady = function (api) {
                priceListExtensionFormatSelectorAPI = api;
                priceListExtensionFormatSelectorReadyDeferred.resolve();
            };

            ctrl.onPriceListTypeSelectorReady = function (api) {
                priceListTypeSelectorAPI = api;
                priceListTypeSelectorReadyDeferred.resolve();
            };

            ctrl.onSalePLTemplateSelectorReady = function (api) {
                salePLTemplateSelectorAPI = api;
                salePLTemplateSelectorReadyDeferred.resolve();
            };

            ctrl.onCompressFileSelectorReady = function (api) {
                CompressFileSelectorAPI = api;
                CompressFileSelectorReadyDeferred.resolve();
            };

            ctrl.onCodeChangeTypeSettingsGridReady = function (api) {
                CodeChangeTypeSettingsGridAPI = api;
                CodeChangeTypeSettingsGridReadyDeferred.resolve();
            };

            ctrl.onRateChangeTypeSettingsGridReady = function (api) {
                RateChangeTypeSettingsGridAPI = api;
                RateChangeTypeSettingsGridReadyDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                var promises = [];
                var data;
                var directiveContext;
                var selectedMailMsgTemplateId;
                var selectedSalePLTemplateId;
                var selectedPriceListExtensionFormatId;
                var pricelistTypeId;
                var compressPriceListFile;
                var codeChangeTypeSettings;
                var rateChangeTypeSettings;

                if (payload != undefined) {
                    data = payload.data;
                    directiveContext = payload.directiveContext;
                }

                prepareDirectivesRequiredForContext(directiveContext);
                prepareDirectivesViewForContext(directiveContext);

                if (data != undefined) {
                    selectedMailMsgTemplateId = data.DefaultSalePLMailTemplateId;
                    selectedSalePLTemplateId = data.DefaultSalePLTemplateId;
                    selectedPriceListExtensionFormatId = data.PriceListExtensionFormat;
                    pricelistTypeId = data.PriceListType;
                    compressPriceListFile = data.CompressPriceListFile;
                    codeChangeTypeSettings = data.CodeChangeTypeDescriptions;
                    rateChangeTypeSettings = data.RateChangeTypeDescriptions;
                }

                if (ctrl.showPricelistEmailTemplateSelector) {
                    var loadMailMsgTemplateSelectorPromise = loadMailMsgTemplateSelector(selectedMailMsgTemplateId);
                    promises.push(loadMailMsgTemplateSelectorPromise);
                }
                if (ctrl.showPricelistTemplateSelector) {
                    var loadSalePLTemplateSelectorPromise = loadSalePLTemplateSelector(selectedSalePLTemplateId);
                    promises.push(loadSalePLTemplateSelectorPromise);
                }
                if (ctrl.showPricelistExtensionFormatSelector) {
                    var loadPriceListExtensionFormatSelectorPromise = loadPriceListExtensionFormatSelector(selectedPriceListExtensionFormatId);
                    promises.push(loadPriceListExtensionFormatSelectorPromise);
                }
                if (ctrl.showPricelistTypeSelector) {
                    var loadPriceListTypeSelectorPromise = loadPriceListTypeSelector(pricelistTypeId);
                    promises.push(loadPriceListTypeSelectorPromise);
                }
                if (ctrl.showCompressPriceListFile) {
                    var loadCompressFileSelectorPromise = loadCompressFileSelector(compressPriceListFile);
                    promises.push(loadCompressFileSelectorPromise);
                }
                
                var loadCodeChangeTypeSettingsGridPromise = loadCodeChangeTypeSettingsGrid(codeChangeTypeSettings);
                promises.push(loadCodeChangeTypeSettingsGridPromise);

                var loadRateChangeTypeSettingsGridPromise = loadRateChangeTypeSettingsGrid(rateChangeTypeSettings);
                promises.push(loadRateChangeTypeSettingsGridPromise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    DefaultSalePLMailTemplateId: (mailMsgTemplateSelectorAPI != undefined) ? mailMsgTemplateSelectorAPI.getSelectedIds() : undefined,
                    DefaultSalePLTemplateId: (salePLTemplateSelectorAPI != undefined) ? salePLTemplateSelectorAPI.getSelectedIds() : undefined,
                    PriceListExtensionFormat: (priceListExtensionFormatSelectorAPI != undefined) ? priceListExtensionFormatSelectorAPI.getSelectedIds() : undefined,
                    PriceListType: (priceListTypeSelectorAPI != undefined) ? priceListTypeSelectorAPI.getSelectedIds() : undefined,
                    CompressPriceListFile: (CompressFileSelectorAPI != undefined) ? CompressFileSelectorAPI.getData() : undefined,
                    CodeChangeTypeDescriptions: (CodeChangeTypeSettingsGridAPI != undefined) ? CodeChangeTypeSettingsGridAPI.getData() : undefined,
                    RateChangeTypeDescriptions: (RateChangeTypeSettingsGridAPI != undefined) ? RateChangeTypeSettingsGridAPI.getData() : undefined,

                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadMailMsgTemplateSelector(selectedId) {
            var mailMsgTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            mailMsgTemplateSelectorReadyDeferred.promise.then(function () {
                var mailMsgTemplateSelectorPayload = {
                    selectedIds: selectedId,
                    filter: {
                        VRMailMessageTypeId: "f61f0b87-ee5b-4794-8b0f-6c0777006441"
                    }
                };
                VRUIUtilsService.callDirectiveLoad(mailMsgTemplateSelectorAPI, mailMsgTemplateSelectorPayload, mailMsgTemplateSelectorLoadDeferred);
            });
            return mailMsgTemplateSelectorLoadDeferred.promise;
        }

        function loadSalePLTemplateSelector(selectedId) {
            var salePLTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            salePLTemplateSelectorReadyDeferred.promise.then(function () {
                var salePLTemplateSelectorPayload = {
                    selectedIds: selectedId
                };
                VRUIUtilsService.callDirectiveLoad(salePLTemplateSelectorAPI, salePLTemplateSelectorPayload, salePLTemplateSelectorLoadDeferred);
            });
            return salePLTemplateSelectorLoadDeferred.promise;
        }

        function loadPriceListExtensionFormatSelector(selectedIds) {
            var priceListExtensionFormatSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            priceListExtensionFormatSelectorReadyDeferred.promise.then(function () {
                priceListExtensionFormatSelectorReadyDeferred = undefined;
                var priceListExtensionFormatSelectorPayload = {
                    selectedIds: selectedIds
                };
                VRUIUtilsService.callDirectiveLoad(priceListExtensionFormatSelectorAPI, priceListExtensionFormatSelectorPayload, priceListExtensionFormatSelectorLoadDeferred);
            });
            return priceListExtensionFormatSelectorLoadDeferred.promise;
        }

        function loadPriceListTypeSelector(selectedIds) {
            var priceListTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            priceListTypeSelectorReadyDeferred.promise.then(function () {
                priceListTypeSelectorReadyDeferred = undefined;
                var priceListTypeSelectorPayload = {
                    selectedIds: selectedIds
                };
                VRUIUtilsService.callDirectiveLoad(priceListTypeSelectorAPI, priceListTypeSelectorPayload, priceListTypeSelectorLoadDeferred);
            });
            return priceListTypeSelectorLoadDeferred.promise;
        }

        function loadCompressFileSelector(compressPriceListFile) {
            var compressFileSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            CompressFileSelectorReadyDeferred.promise.then(function () {
                CompressFileSelectorReadyDeferred = undefined;
                var compressFilePayload = {
                    selectedValue: compressPriceListFile
                };
                VRUIUtilsService.callDirectiveLoad(CompressFileSelectorAPI, compressFilePayload, compressFileSelectorLoadDeferred);
            });
            return compressFileSelectorLoadDeferred.promise;
        }

        function loadCodeChangeTypeSettingsGrid(codeChangeTypeSettings) {
            var CodeChangeTypeSettingsGridLoadDeferred = UtilsService.createPromiseDeferred();
            CodeChangeTypeSettingsGridReadyDeferred.promise.then(function () {
                CodeChangeTypeSettingsGridReadyDeferred = undefined;
                var codeChangeTypeSettingsGridPayload = {
                    codeChangeTypeSettings: codeChangeTypeSettings
                };
                VRUIUtilsService.callDirectiveLoad(CodeChangeTypeSettingsGridAPI, codeChangeTypeSettingsGridPayload, CodeChangeTypeSettingsGridLoadDeferred);
            });
            return CodeChangeTypeSettingsGridLoadDeferred.promise;
        }

        function loadRateChangeTypeSettingsGrid(rateChangeTypeSettings) {
            var RateChangeTypeSettingsGridLoadDeferred = UtilsService.createPromiseDeferred();
            RateChangeTypeSettingsGridReadyDeferred.promise.then(function () {
                RateChangeTypeSettingsGridReadyDeferred = undefined;
                var rateChangeTypeSettingsGridPayload = {
                    rateChangeTypeSettings: rateChangeTypeSettings
                };
                VRUIUtilsService.callDirectiveLoad(RateChangeTypeSettingsGridAPI, rateChangeTypeSettingsGridPayload, RateChangeTypeSettingsGridLoadDeferred);
            });
            return RateChangeTypeSettingsGridLoadDeferred.promise;
        }

        function prepareDirectivesViewForContext(directiveContext) {

            ctrl.showPricelistTemplateSelector = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);
            ctrl.showPricelistEmailTemplateSelector = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
            ctrl.showPricelistExtensionFormatSelector = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);
            ctrl.showPricelistTypeSelector = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);
            ctrl.showCompressPriceListFile = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);
            ctrl.showCodeChangeTypeSettingsGrid = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);
            ctrl.showRateChangeTypeSettingsGrid = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);

        }

        function prepareDirectivesRequiredForContext(directiveContext) {

            ctrl.isPricelistTemplateRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
            ctrl.isPricelistEmailTemplateRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
            ctrl.isPricelistExtensionFormatRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
            ctrl.isPricelistTypeRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
            ctrl.isCompressPriceListFileRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);

        }

    }

    return directiveDefinitionObject;
}]);