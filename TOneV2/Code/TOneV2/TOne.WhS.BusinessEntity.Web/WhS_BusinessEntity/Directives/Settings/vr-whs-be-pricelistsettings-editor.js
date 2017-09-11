'use strict';

app.directive('vrWhsBePricelistsettingsEditor', ['UtilsService', 'VRUIUtilsService','WhS_BE_SaleAreaSettingsContextEnum',
function (UtilsService, VRUIUtilsService,WhS_BE_SaleAreaSettingsContextEnum) {

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

                    if (payload != undefined) {
                        data = payload.data;
                        directiveContext = payload.directiveContext;
                    }

                    prepareDirectivesViewForContext(directiveContext);
                    prepareDirectivesRequiredForContext(directiveContext);

                    if (data != undefined) {
                        selectedMailMsgTemplateId = data.DefaultSalePLMailTemplateId;
                        selectedSalePLTemplateId = data.DefaultSalePLTemplateId;
                        selectedPriceListExtensionFormatId = data.PriceListExtensionFormat;
                        pricelistTypeId = data.PriceListType;
                        compressPriceListFile = data.CompressPriceListFile;
                    }

                    if (ctrl.showCompressPriceListFile) {

                        if (compressPriceListFile != undefined) {
                            ctrl.compressPriceListFile = compressPriceListFile;
                            if (ctrl.showCompressPriceListFileEditCheck)
                                ctrl.isCompressPriceListFileEditCheck = true;
                        }
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

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        DefaultSalePLMailTemplateId: (mailMsgTemplateSelectorAPI != undefined) ? mailMsgTemplateSelectorAPI.getSelectedIds() : undefined,
                        DefaultSalePLTemplateId: (salePLTemplateSelectorAPI != undefined) ? salePLTemplateSelectorAPI.getSelectedIds() : undefined,
                        PriceListExtensionFormat: (priceListExtensionFormatSelectorAPI != undefined) ? priceListExtensionFormatSelectorAPI.getSelectedIds() : undefined,
                        PriceListType: (priceListTypeSelectorAPI != undefined) ? priceListTypeSelectorAPI.getSelectedIds() : undefined,
                        CompressPriceListFile: (ctrl.isCompressPriceListFileEditCheck == false) ? undefined : ctrl.compressPriceListFile
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

            function prepareDirectivesViewForContext(directiveContext) {

                ctrl.showPricelistTemplateSelector = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.showPricelistEmailTemplateSelector = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.showPricelistExtensionFormatSelector = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);
                ctrl.showPricelistTypeSelector = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);
                ctrl.showCompressPriceListFile = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);
                ctrl.showCompressPriceListFileEditCheck = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);
            }

            function prepareDirectivesRequiredForContext(directiveContext) {

                ctrl.isPricelistTemplateRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.isPricelistEmailTemplateRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.isPricelistEmailTemplateRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.isPricelistTypeRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.isCompressPriceListFileRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
            }

        }

        return directiveDefinitionObject;
    }]);