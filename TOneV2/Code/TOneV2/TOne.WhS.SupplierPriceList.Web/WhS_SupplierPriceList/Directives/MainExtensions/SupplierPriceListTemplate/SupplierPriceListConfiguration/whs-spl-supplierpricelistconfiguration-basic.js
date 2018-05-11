(function (app) {

    'use strict';

    SupplierpricelistconfigurationBasic.$inject = ["UtilsService", "VRUIUtilsService", 'WhS_SupPL_CodeLayoutEnum', 'VR_ExcelConversion_FieldTypeEnum', 'WhS_SupPL_CodeRateMappingEnum', 'WhS_SupPL_RatePrecisionTypeEnum'];

    function SupplierpricelistconfigurationBasic(UtilsService, VRUIUtilsService, WhS_SupPL_CodeLayoutEnum, VR_ExcelConversion_FieldTypeEnum, WhS_SupPL_CodeRateMappingEnum, WhS_SupPL_RatePrecisionTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var supplierpricelistconfiguration = new Supplierpricelistconfiguration($scope, ctrl, $attrs);
                supplierpricelistconfiguration.initializeController();
            },
            controllerAs: "supplierbasicCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/MainExtensions/SupplierPriceListTemplate/SupplierPriceListConfiguration/Templates/BasicSupplierPriceListConfiguration.html"
        };

        function Supplierpricelistconfiguration($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var codeRateMappingAPI;
            var codeRateMappingSelectionChangedDeferred;

            var rateListAPI;
            var rateListMappingReadyPromiseDeferred;

            var codeListAPI;
            var codeListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var rateTypeAPI;
            var rateTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var flaggedServiceAPI;
            var flaggedServiceReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var context;
            var configDetails;

            var rateTabsAPI;
            var serviceTabsAPI;

            var isCodeLayoutSelected;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.normalRateTabObject = { showTab: false };
                $scope.scopeModel.hideNormalRate = false;
                $scope.scopeModel.code = false;
                $scope.scopeModel.codeLayouts = UtilsService.getArrayEnum(WhS_SupPL_CodeLayoutEnum);
                $scope.scopeModel.codeTabObject = { header: "Codes" };

                $scope.scopeModel.dateTimeFormat = "yyyy/MM/dd";
                $scope.scopeModel.rateTypesSelected = [];
                $scope.scopeModel.servicesSelected = [];
                $scope.scopeModel.codeRateMappings = [];
                $scope.scopeModel.ratePrecisionTypes = [];
                $scope.scopeModel.onRateTabsReady = function (api) {
                    rateTabsAPI = api;
                    rateTabsAPI.setTabSelected(0);
                };

                $scope.scopeModel.ratePrecicionType = WhS_SupPL_RatePrecisionTypeEnum.RoundRate;
                $scope.scopeModel.ratePrecisionTypes = UtilsService.getArrayEnum(WhS_SupPL_RatePrecisionTypeEnum);

                $scope.scopeModel.precisionValidate = function () {
                    if ($scope.scopeModel.precisionValue == undefined)
                        return null;
                    var precisionValueAsNumber = Number($scope.scopeModel.precisionValue);
                    if (isNaN(precisionValueAsNumber))
                        return 'Value is an invalid number';
                    if ($scope.scopeModel.precisionValue.toString().indexOf(".") > -1)
                        return 'Value must be Integer';
                    if (precisionValueAsNumber <= 0)
                        return 'Value must be greater than zero';
                    if (precisionValueAsNumber > 8)
                        return 'Value must be smaller than 9';
                    return null;
                };

                $scope.scopeModel.onCodeRateMappingReady = function (api) {
                    codeRateMappingAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onCodeRateMappingSelectionChanged = function (selectedObject) {
                    if ($scope.scopeModel.selectedCodeRateMapping != undefined) {
                        $scope.scopeModel.normalRateTabObject.showTab = $scope.scopeModel.selectedCodeRateMapping.showRateMapping;
                        $scope.scopeModel.codeTabObject.header = $scope.scopeModel.selectedCodeRateMapping.showRateMapping ? "Codes" : "Codes And Rates";
                        if (codeRateMappingSelectionChangedDeferred != undefined) {
                            codeRateMappingSelectionChangedDeferred.resolve();
                        }
                        else {
                            loadCodeListMapping();
                        }
                    }

                    function loadCodeListMapping() {
                        var codeListMappingPayload = {
                            context: getContext(),
                            fieldMappings: [],
                            listName: "CodeList",
                            listTitle: $scope.scopeModel.selectedCodeRateMapping.showRateMapping ? "Codes" : "Codes And Rates",
                            showDateFormat: $scope.scopeModel.selectedCodeRateMapping.showRateMapping
                        };
                        codeListMappingPayload.fieldMappings.push({ FieldName: "Code", FieldTitle: "Code", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value });
                        codeListMappingPayload.fieldMappings.push({ FieldName: "CodeGroup", FieldTitle: "Code Group", isRequired: false, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value });
                        codeListMappingPayload.fieldMappings.push({ FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value });
                        if ($scope.scopeModel.selectedCodeRateMapping.value == WhS_SupPL_CodeRateMappingEnum.SameSheet.value) {
                            codeListMappingPayload.fieldMappings.push({ FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value });
                        }
                        codeListMappingPayload.fieldMappings.push({ FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value });

                        var setLoader = function (value) {
                            $scope.scopeModel.isCodeListMappingLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, codeListAPI, codeListMappingPayload, setLoader, undefined);
                    }
                };

                $scope.scopeModel.onServiceTabsReady = function (api) {
                    serviceTabsAPI = api;
                    serviceTabsAPI.setTabSelected(0);
                };

                $scope.scopeModel.validate = function () {
                    if ($scope.scopeModel.delimiterValue == $scope.scopeModel.rangeSeparator) {

                        return "Range separator should not be the same as delimiter.";
                    }
                    return null;
                };

                $scope.scopeModel.onRateTypeSelectorReady = function (api) {
                    rateTypeAPI = api;
                    rateTypeReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFlaggedServiceSelectorReady = function (api) {
                    flaggedServiceAPI = api;
                    flaggedServiceReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSelectRateType = function (item) {
                    var rateTypeTabe = {
                        Name: item.Name,
                        RateTypeId: item.RateTypeId,
                        onRateListMappingReady: function (api) {
                            rateTypeTabe.rateListAPI = api;
                            var payload = {
                                context: getContext(),
                                fieldMappings: [{ FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value }, { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value }],
                                listName: item.Name,
                            };
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingSupplierPriceListTemplate = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, rateTypeTabe.rateListAPI, payload, setLoader);
                        }
                    };
                    $scope.scopeModel.rateTypesSelected.push(rateTypeTabe);
                    rateTabsAPI.setTabSelected($scope.scopeModel.rateTypesSelected.length - 1);

                };

                $scope.scopeModel.onDeselectRateType = function (item) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.rateTypesSelected, item.RateTypeId, "RateTypeId");
                    $scope.scopeModel.rateTypesSelected.splice(index, 1);
                    rateTabsAPI.setTabSelected($scope.scopeModel.rateTypesSelected.length - 1);
                };

                $scope.scopeModel.onSelectFlaggedService = function (item) {
                    var serviceTabe = {
                        Symbol: item.Symbol,
                        ZoneServiceConfigId: item.ZoneServiceConfigId,
                        onServiceListMappingReady: function (api) {
                            serviceTabe.flaggedServiceAPI = api;
                            var payload = {
                                context: getContext(),
                                fieldMappings: [{ FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value }],
                                listName: item.Symbol,
                            };
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingSupplierPriceListTemplate = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, serviceTabe.flaggedServiceAPI, payload, setLoader);
                        }
                    };
                    $scope.scopeModel.servicesSelected.push(serviceTabe);
                    serviceTabsAPI.setTabSelected($scope.scopeModel.servicesSelected.length - 1);

                };

                $scope.scopeModel.onDeselectFlaggedService = function (item) {
                    //  $scope.scopeModel.codeTabObject.isSelected = true;
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.servicesSelected, item.ZoneServiceConfigId, "ZoneServiceConfigId");
                    $scope.scopeModel.servicesSelected.splice(index, 1);
                    serviceTabsAPI.setTabSelected($scope.scopeModel.servicesSelected.length - 1);

                };

                $scope.scopeModel.onCodeListMappingReady = function (api) {
                    codeListAPI = api;
                    codeListMappingReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCodeLayoutSelectionChanged = function () {

                    var isDelimitedCodeLayout =
						($scope.scopeModel.selectedCodeLayout != undefined && $scope.scopeModel.selectedCodeLayout.value == WhS_SupPL_CodeLayoutEnum.Delimitedcode.value);

                    if (isCodeLayoutSelected === false) {
                        isCodeLayoutSelected = true;
                        $scope.scopeModel.showDelimiter = isDelimitedCodeLayout;
                        return;
                    }
                    if (isDelimitedCodeLayout) {
                        $scope.scopeModel.delimiterValue = ',';
                        $scope.scopeModel.showDelimiter = true;
                    }
                    else {
                        $scope.scopeModel.hasCodeRange = false;
                        $scope.scopeModel.delimiterValue = undefined;
                        $scope.scopeModel.showDelimiter = false;
                    }
                };

                $scope.scopeModel.onRateListMappingReady = function (api) {
                    rateListAPI = api;

                    var rateListMappingPayload = {
                        context: getContext(),
                        fieldMappings: [{ FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value }, { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value }],
                        listName: "RateList",
                    };

                    var setLoader = function (value) {
                        $scope.scopeModel.isRateListMappingLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, rateListAPI, rateListMappingPayload, setLoader, rateListMappingReadyPromiseDeferred);
                };

                $scope.scopeModel.onCodeRangeValueChanged = function () {
                    if ($scope.scopeModel.hasCodeRange) {
                        $scope.scopeModel.rangeSeparator = '-';
                    } else {
                        $scope.scopeModel.rangeSeparator = undefined;
                    }
                };

                $scope.scopeModel.checkUsesOfEffectiveDate = function () {
                    if (codeListAPI != undefined) {
                        var codeListData = codeListAPI.getData();
                        if (codeListData != undefined && codeListData.FieldMappings != undefined) {
                            for (var i = 0 ; i < codeListData.FieldMappings.length; i++) {
                                var fieldMappingForCodeList = codeListData.FieldMappings[i];
                                if (fieldMappingForCodeList.FieldType == VR_ExcelConversion_FieldTypeEnum.DateTime.value) {
                                    return true;
                                }
                            }
                        }
                    }
                    if (rateListAPI != undefined) {
                        var rateListData = rateListAPI.getData();
                        if (rateListData != undefined && rateListData.FieldMappings != undefined) {
                            for (var i = 0 ; i < rateListData.FieldMappings.length; i++) {
                                var fieldMappingForRateList = rateListData.FieldMappings[i];
                                if (fieldMappingForRateList.FieldType == VR_ExcelConversion_FieldTypeEnum.DateTime.value) {
                                    return true;
                                }

                            }
                        }
                    }
                    return false;
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    codeRateMappingAPI.clearDataSource();
                    $scope.scopeModel.codeRateMappings.length = 0;
                    var showRateMapping = false;

                    codeRateMappingSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                    isCodeLayoutSelected = undefined;

                    $scope.scopeModel.rateTypesSelected.length = 0;
                    $scope.scopeModel.servicesSelected.length = 0;

                    $scope.scopeModel.codeRateMappings = UtilsService.getArrayEnum(WhS_SupPL_CodeRateMappingEnum);

                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        configDetails = payload.configDetails;
                        if (configDetails != undefined) {
                            $scope.scopeModel.dateTimeFormat = configDetails.DateTimeFormat;
                            $scope.scopeModel.hasCodeRange = configDetails.HasCodeRange;
                            $scope.scopeModel.rangeSeparator = configDetails.RangeSeparator;
                            $scope.scopeModel.delimiterValue = configDetails.Delimiter;
                            $scope.scopeModel.isCommaDecimalSeparator = configDetails.IsCommaDecimalSeparator;
                            $scope.scopeModel.ratePrecicionType = UtilsService.getItemByVal($scope.scopeModel.ratePrecisionTypes, configDetails.RatePrecicionType, "value");
                            $scope.scopeModel.precisionValue = configDetails.Precision;

                            isCodeLayoutSelected = false;
                            $scope.scopeModel.selectedCodeLayout = UtilsService.getItemByVal($scope.scopeModel.codeLayouts, configDetails.CodeLayout, "value");

                            $scope.scopeModel.includeServices = configDetails.IncludeServices;

                            showRateMapping = UtilsService.getEnum(WhS_SupPL_CodeRateMappingEnum, 'value', configDetails.CodeRateMapping).showRateMapping;
                            loadOtherRateListMapping(promises);
                            loadFlaggedServiceListMapping(promises);
                        }
                        else {
                            codeRateMappingSelectionChangedDeferred.resolve();
                            $scope.scopeModel.includeServices = false;
                        }
                    }
                    setTimeout(function () {
                        if (configDetails != undefined) {
                            $scope.scopeModel.selectedCodeRateMapping = UtilsService.getEnum(WhS_SupPL_CodeRateMappingEnum, 'value', configDetails.CodeRateMapping);
                        }
                        else {
                            $scope.scopeModel.selectedCodeRateMapping = UtilsService.getEnum(WhS_SupPL_CodeRateMappingEnum, 'value', WhS_SupPL_CodeRateMappingEnum.SameSheet.value);
                        }
                    });


                    var codeRateMappingPromises = [];

                    promises.push(loadRateTypeSelector());
                    promises.push(loadFlaggedServiceSelector());

                    if (showRateMapping) {
                        rateListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                        var loadRateListMappingPromise = loadRateListMapping();

                        promises.push(loadRateListMappingPromise);
                        codeRateMappingPromises.push(loadRateListMappingPromise);
                    }
                    $scope.scopeModel.codeTabObject.header = showRateMapping ? "Codes" : "Codes And Rates";

                    var loadCodeListMappingPromise = loadCodeListMapping(showRateMapping);
                    promises.push(loadCodeListMappingPromise);
                    codeRateMappingPromises.push(loadCodeListMappingPromise);

                    if (rateTabsAPI != undefined) {
                        rateTabsAPI.setTabSelected(0);
                    }

                    if (serviceTabsAPI != undefined) {
                        serviceTabsAPI.setTabSelected(0);
                    }

                    function loadRateTypeSelector() {
                        var loadRateTypePromiseDeferred = UtilsService.createPromiseDeferred();
                        rateTypeReadyPromiseDeferred.promise.then(function () {
                            var payload;
                            if (configDetails != undefined && configDetails.OtherRateListMapping != undefined) {
                                var rateTypeIds = [];
                                for (var i = 0; i < configDetails.OtherRateListMapping.length; i++) {
                                    var otherRate = configDetails.OtherRateListMapping[i];
                                    rateTypeIds.push(otherRate.RateTypeId);
                                }
                                payload = {
                                    selectedIds: rateTypeIds
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(rateTypeAPI, payload, loadRateTypePromiseDeferred);
                        });

                        return loadRateTypePromiseDeferred.promise;
                    }

                    function loadFlaggedServiceSelector() {
                        var loadFlaggedServicePromiseDeferred = UtilsService.createPromiseDeferred();
                        flaggedServiceReadyPromiseDeferred.promise.then(function () {
                            var payload;
                            if (configDetails != undefined && configDetails.FlaggedServiceListMapping != undefined) {
                                var flaggedServicesIds = [];
                                for (var i = 0; i < configDetails.FlaggedServiceListMapping.length; i++) {
                                    var flaggedService = configDetails.FlaggedServiceListMapping[i];
                                    flaggedServicesIds.push(flaggedService.ZoneServiceConfigId);
                                }
                                payload = {
                                    selectedIds: flaggedServicesIds
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(flaggedServiceAPI, payload, loadFlaggedServicePromiseDeferred);
                        });

                        return loadFlaggedServicePromiseDeferred.promise;
                    }

                    function loadRateListMapping() {
                        var loadRateListMappingPromiseDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([rateListMappingReadyPromiseDeferred.promise, codeRateMappingSelectionChangedDeferred.promise]).then(function () {
                            rateListMappingReadyPromiseDeferred = undefined;

                            var payload = {
                                context: getContext(),
                                fieldMappings: [{ FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value }, { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value }],
                                listName: "RateList",
                            };
                            if (configDetails != undefined && configDetails.NormalRateListMapping) {
                                payload.listMappingData = configDetails.NormalRateListMapping;
                            }
                            VRUIUtilsService.callDirectiveLoad(rateListAPI, payload, loadRateListMappingPromiseDeferred);
                        });

                        return loadRateListMappingPromiseDeferred.promise;
                    }

                    function loadCodeListMapping(showRateMapping) {
                        var loadCodeListMappingPromiseDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([codeListMappingReadyPromiseDeferred.promise, codeRateMappingSelectionChangedDeferred.promise]).then(function () {
                            var payload = {
                                context: getContext(),
                                fieldMappings: [],
                                listName: "CodeList",
                                listTitle: showRateMapping ? "Codes" : "Codes And Rates",
                                showDateFormat: showRateMapping
                            };
                            payload.fieldMappings.push({ FieldName: "Code", FieldTitle: "Code", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value });
                            payload.fieldMappings.push({ FieldName: "CodeGroup", FieldTitle: "Code Group", isRequired: false, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value });
                            payload.fieldMappings.push({ FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value });
                            if (!showRateMapping) {
                                payload.fieldMappings.push({ FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value });
                            }
                            payload.fieldMappings.push({ FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value });

                            if (configDetails != undefined && configDetails.CodeListMapping) {
                                payload.listMappingData = configDetails.CodeListMapping;
                            }
                            VRUIUtilsService.callDirectiveLoad(codeListAPI, payload, loadCodeListMappingPromiseDeferred);
                        });

                        return loadCodeListMappingPromiseDeferred.promise;
                    }

                    function loadOtherRateListMapping(promises) {
                        if (configDetails != undefined && configDetails.OtherRateListMapping != undefined) {
                            for (var i = 0; i < configDetails.OtherRateListMapping.length; i++) {
                                var otherRate = configDetails.OtherRateListMapping[i];
                                var rateTypeTab = {
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                };
                                promises.push(rateTypeTab.loadPromiseDeferred.promise);
                                addRateTypeAPIExtension(rateTypeTab, otherRate);
                            }
                        }
                    }

                    function loadFlaggedServiceListMapping(promises) {
                        if (configDetails != undefined && configDetails.FlaggedServiceListMapping != undefined) {
                            for (var i = 0; i < configDetails.FlaggedServiceListMapping.length; i++) {
                                var flaggedService = configDetails.FlaggedServiceListMapping[i];
                                var flaggedServiceTab = {
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                };
                                promises.push(flaggedServiceTab.loadPromiseDeferred.promise);
                                addFlaggedServiceAPIExtension(flaggedServiceTab, flaggedService);
                            }
                        }
                    }

                    UtilsService.waitMultiplePromises(codeRateMappingPromises).then(function () {
                        codeRateMappingSelectionChangedDeferred = undefined;
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function getData() {
                    var listCodeMapping = codeListAPI.getData();
                    var listNormalRateMapping = $scope.scopeModel.selectedCodeRateMapping.showRateMapping ? rateListAPI.getData() : undefined;

                    var otherRatesListMappings = [];
                    if ($scope.scopeModel.rateTypesSelected.length > 0) {
                        for (var i = 0; i < $scope.scopeModel.rateTypesSelected.length; i++) {
                            var rateTypeSelected = $scope.scopeModel.rateTypesSelected[i];
                            otherRatesListMappings.push(
                                {
                                    RateTypeId: rateTypeSelected.RateTypeId,
                                    RateListMapping: rateTypeSelected.rateListAPI.getData()
                                });
                        }
                    }

                    var flaggedServiceListMapping = [];
                    if ($scope.scopeModel.servicesSelected.length > 0) {
                        for (var i = 0; i < $scope.scopeModel.servicesSelected.length; i++) {
                            var flaggedServiceSelected = $scope.scopeModel.servicesSelected[i];
                            flaggedServiceListMapping.push(
                                {
                                    ZoneServiceConfigId: flaggedServiceSelected.ZoneServiceConfigId,
                                    ServiceListMapping: flaggedServiceSelected.flaggedServiceAPI.getData()
                                });
                        }
                    }

                    var basicConfiguration = {
                        $type: "TOne.WhS.SupplierPriceList.MainExtensions.SupplierPriceListSettings.BasicSupplierPriceListSettings,TOne.WhS.SupplierPriceList.MainExtensions",
                        CodeRateMapping: $scope.scopeModel.selectedCodeRateMapping.value,
                        CodeListMapping: listCodeMapping,
                        NormalRateListMapping: listNormalRateMapping,
                        OtherRateListMapping: otherRatesListMappings,
                        FlaggedServiceListMapping: flaggedServiceListMapping,
                        IncludeServices: $scope.scopeModel.includeServices,
                        DateTimeFormat: $scope.scopeModel.dateTimeFormat,
                        CodeLayout: $scope.scopeModel.selectedCodeLayout != undefined ? $scope.scopeModel.selectedCodeLayout.value : undefined,
                        HasCodeRange: $scope.scopeModel.hasCodeRange,
                        IsCommaDecimalSeparator: $scope.scopeModel.isCommaDecimalSeparator,
                        Precision: $scope.scopeModel.precisionValue,
                            RatePrecicionType: ($scope.scopeModel.ratePrecicionType != null) ? $scope.scopeModel.ratePrecicionType.value: WhS_SupPL_RatePrecisionTypeEnum.RoundRate,
                    };

                    if ($scope.scopeModel.selectedCodeLayout != undefined && $scope.scopeModel.selectedCodeLayout.value == WhS_SupPL_CodeLayoutEnum.Delimitedcode.value) {
                        basicConfiguration.Delimiter = $scope.scopeModel.delimiterValue;
                    }

                    if ($scope.scopeModel.hasCodeRange) {
                        basicConfiguration.RangeSeparator = $scope.scopeModel.rangeSeparator;
                    }
                    return basicConfiguration;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function addRateTypeAPIExtension(rateType, payloadRateType) {
                rateType.Name = payloadRateType.RateListMapping.ListName;
                rateType.RateTypeId = payloadRateType.RateTypeId;
                rateType.onRateListMappingReady = function (api) {
                    rateType.rateListAPI = api;
                    rateType.readyPromiseDeferred.resolve();
                };

                rateType.readyPromiseDeferred.promise.then(function () {
                    var payload = {
                        context: getContext(),
                        fieldMappings: [{ FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value }, {
                            FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value
                        }, {
                            FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value
                        }],
                        listName: rateType.Name
                    };
                    if (payloadRateType != undefined && payloadRateType.RateListMapping) {
                        payload.listMappingData = payloadRateType.RateListMapping;
                    }
                    VRUIUtilsService.callDirectiveLoad(rateType.rateListAPI, payload, rateType.loadPromiseDeferred);
                });

                $scope.scopeModel.rateTypesSelected.push(rateType);
            }

            function addFlaggedServiceAPIExtension(flaggedService, payloadFlaggedService) {
                flaggedService.Symbol = payloadFlaggedService.ServiceListMapping.ListName;
                flaggedService.ZoneServiceConfigId = payloadFlaggedService.ZoneServiceConfigId;
                flaggedService.onServiceListMappingReady = function (api) {
                    flaggedService.flaggedServiceAPI = api;
                    flaggedService.readyPromiseDeferred.resolve();
                };

                flaggedService.readyPromiseDeferred.promise.then(function () {
                    var payload = {
                        context: getContext(),
                        fieldMappings: [{ FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, {
                            FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value
                        }],
                        listName: flaggedService.Symbol,
                    };
                    if (payloadFlaggedService != undefined && payloadFlaggedService.ServiceListMapping) {
                        payload.listMappingData = payloadFlaggedService.ServiceListMapping;
                    }
                    VRUIUtilsService.callDirectiveLoad(flaggedService.flaggedServiceAPI, payload, flaggedService.loadPromiseDeferred);
                });
                $scope.scopeModel.servicesSelected.push(flaggedService);
            }

            function getContext() {
                if (context != undefined) {
                    var currentContext = UtilsService.cloneObject(context);
                    return currentContext;
                }
            }
        }
    }

    app.directive('whsSplSupplierpricelistconfigurationBasic', SupplierpricelistconfigurationBasic);

})(app);