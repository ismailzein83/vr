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

        var IncludeClosedEntitiesSelectorAPI;
        var IncludeClosedEntitiesSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var CodeChangeTypeSettingsGridAPI;
        var CodeChangeTypeSettingsGridReadyDeferred = UtilsService.createPromiseDeferred();

        var RateChangeTypeSettingsGridAPI;
        var RateChangeTypeSettingsGridReadyDeferred = UtilsService.createPromiseDeferred();

        var FileNamePatternAPI;
        var FileNamePatternReadyDeferred = UtilsService.createPromiseDeferred();

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

            ctrl.onIncludeClosedEntitiesSelectorReady = function (api) {
                IncludeClosedEntitiesSelectorAPI = api;
                IncludeClosedEntitiesSelectorReadyDeferred.resolve();
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

            ctrl.onFileNamePatternReady = function (api) {
                FileNamePatternAPI = api;
                FileNamePatternReadyDeferred.resolve();
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
                var includeClosedEntities;
                var codeChangeTypeSettings;
                var rateChangeTypeSettings;
                var fileNamePattern;

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
                    includeClosedEntities = data.IncludeClosedEntities;
                    codeChangeTypeSettings = data.CodeChangeTypeDescriptions;
                    rateChangeTypeSettings = data.RateChangeTypeDescriptions;
                    fileNamePattern = data.SalePricelistFileNamePattern;
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

                if (ctrl.showIncludeClosedEntities) {
                    var loadIncludeClosedEntitiesSelectorPromise = loadIncludeClosedEntitiesSelector(includeClosedEntities);
                    promises.push(loadIncludeClosedEntitiesSelectorPromise);
                }

                if (ctrl.showCodeChangeTypeSettingsGrid) {
                    var loadCodeChangeTypeSettingsGridPromise = loadCodeChangeTypeSettingsGrid(codeChangeTypeSettings);
                    promises.push(loadCodeChangeTypeSettingsGridPromise);
                }

                if (ctrl.showRateChangeTypeSettingsGrid) {
                    var loadRateChangeTypeSettingsGridPromise = loadRateChangeTypeSettingsGrid(rateChangeTypeSettings);
                    promises.push(loadRateChangeTypeSettingsGridPromise);
                }


                if (ctrl.showFileNamePattern) {
                    var loadFileNamePatternPromise = loadFileNamePattern(fileNamePattern);
                    promises.push(loadFileNamePatternPromise);
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    DefaultSalePLMailTemplateId: (mailMsgTemplateSelectorAPI != undefined) ? mailMsgTemplateSelectorAPI.getSelectedIds() : undefined,
                    DefaultSalePLTemplateId: (salePLTemplateSelectorAPI != undefined) ? salePLTemplateSelectorAPI.getSelectedIds() : undefined,
                    PriceListExtensionFormat: (priceListExtensionFormatSelectorAPI != undefined) ? priceListExtensionFormatSelectorAPI.getSelectedIds() : undefined,
                    PriceListType: (priceListTypeSelectorAPI != undefined) ? priceListTypeSelectorAPI.getSelectedIds() : undefined,
                    CompressPriceListFile: (CompressFileSelectorAPI != undefined) ? CompressFileSelectorAPI.getData() : undefined,
                    IncludeClosedEntities: (IncludeClosedEntitiesSelectorAPI != undefined) ? IncludeClosedEntitiesSelectorAPI.getData() : undefined,
                    CodeChangeTypeDescriptions: (CodeChangeTypeSettingsGridAPI != undefined) ? CodeChangeTypeSettingsGridAPI.getData() : undefined,
                    RateChangeTypeDescriptions: (RateChangeTypeSettingsGridAPI != undefined) ? RateChangeTypeSettingsGridAPI.getData() : undefined,
                    SalePricelistFileNamePattern: (FileNamePatternAPI != undefined) ? FileNamePatternAPI.getData() : undefined,

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

        function loadIncludeClosedEntitiesSelector(includeClosedEntities) {
            var includeClosedEntitiesSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            IncludeClosedEntitiesSelectorReadyDeferred.promise.then(function () {
                IncludeClosedEntitiesSelectorReadyDeferred = undefined;
                var includeClosedEntitiesPayload = {
                    selectedValue: includeClosedEntities
                };
                VRUIUtilsService.callDirectiveLoad(IncludeClosedEntitiesSelectorAPI, includeClosedEntitiesPayload, includeClosedEntitiesSelectorLoadDeferred);
            });
            return includeClosedEntitiesSelectorLoadDeferred.promise;
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

        function loadFileNamePattern(fileNamePattern) {
            var FileNamePatternLoadDeferred = UtilsService.createPromiseDeferred();
            FileNamePatternReadyDeferred.promise.then(function () {
                FileNamePatternReadyDeferred = undefined;
                var fileNamePatternPayload = {
                    fileNamePattern: fileNamePattern
                };
                VRUIUtilsService.callDirectiveLoad(FileNamePatternAPI, fileNamePatternPayload, FileNamePatternLoadDeferred);
            });
            return FileNamePatternLoadDeferred.promise;
        }

        function prepareDirectivesViewForContext(directiveContext) {
            var systemEnumValue = WhS_BE_SaleAreaSettingsContextEnum.System.value;
            var companyEnumValue = WhS_BE_SaleAreaSettingsContextEnum.Company.value;
            var customerEnumValue = WhS_BE_SaleAreaSettingsContextEnum.Customer.value;

            ctrl.showPricelistTemplateSelector = (directiveContext == systemEnumValue || directiveContext == customerEnumValue || directiveContext == companyEnumValue);
            ctrl.showPricelistEmailTemplateSelector = (directiveContext == systemEnumValue || directiveContext == companyEnumValue);
            ctrl.showPricelistExtensionFormatSelector = (directiveContext == systemEnumValue || directiveContext == customerEnumValue || directiveContext == companyEnumValue);
            ctrl.showPricelistTypeSelector = (directiveContext == systemEnumValue || directiveContext == customerEnumValue || directiveContext == companyEnumValue);
            ctrl.showCompressPriceListFile = (directiveContext == systemEnumValue || directiveContext == customerEnumValue || directiveContext == companyEnumValue);
            ctrl.showIncludeClosedEntities = (directiveContext == systemEnumValue || directiveContext == customerEnumValue || directiveContext == companyEnumValue);
            ctrl.showCodeChangeTypeSettingsGrid = (directiveContext == systemEnumValue || directiveContext == customerEnumValue || directiveContext == companyEnumValue);
            ctrl.showRateChangeTypeSettingsGrid = (directiveContext == systemEnumValue || directiveContext == customerEnumValue || directiveContext == companyEnumValue);
            ctrl.showFileNamePattern = (directiveContext == systemEnumValue || directiveContext == customerEnumValue || directiveContext == companyEnumValue);
        }

        function prepareDirectivesRequiredForContext(directiveContext) {
            var systemEnumValue = WhS_BE_SaleAreaSettingsContextEnum.System.value;

            ctrl.isPricelistTemplateRequired = (directiveContext == systemEnumValue);
            ctrl.isPricelistEmailTemplateRequired = (directiveContext == systemEnumValue);
            ctrl.isPricelistExtensionFormatRequired = (directiveContext == systemEnumValue);
            ctrl.isPricelistTypeRequired = (directiveContext == systemEnumValue);
            ctrl.isCompressPriceListFileRequired = (directiveContext == systemEnumValue);
            ctrl.isIncludeClosedEntitiesRequired = (directiveContext == systemEnumValue);
            ctrl.isRateChangeTypeSettingsRequired = (directiveContext == systemEnumValue);
            ctrl.isCodeChangeTypeSettingsRequired = (directiveContext == systemEnumValue);
            ctrl.isFileNamePatternRequired = (directiveContext == systemEnumValue);
        }

    }

    return directiveDefinitionObject;
}]);