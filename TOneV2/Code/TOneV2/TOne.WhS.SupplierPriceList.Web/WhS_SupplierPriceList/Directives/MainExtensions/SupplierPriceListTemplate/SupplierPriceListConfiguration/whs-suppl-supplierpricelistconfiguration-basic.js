(function (app) {

    'use strict';

    SupplierpricelistconfigurationBasic.$inject = ["UtilsService", "VRUIUtilsService", 'WhS_SupPL_CodeLayoutEnum', 'VR_ExcelConversion_FieldTypeEnum'];

    function SupplierpricelistconfigurationBasic(UtilsService, VRUIUtilsService, WhS_SupPL_CodeLayoutEnum, VR_ExcelConversion_FieldTypeEnum) {
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
            var rateListAPI;
            var rateListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var codeListAPI;
            var codeListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var rateTypeAPI;
            var rateTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var context;
            var configDetails;
            $scope.intPutFieldMappings;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.validate = function () {
                    if ($scope.scopeModel.delimiterValue == $scope.scopeModel.rangeSeparator) {

                        return "Range separator should not be the same as delimiter.";
                    }
                    return null;
                }
                $scope.scopeModel.onRateTypeSelectorReady = function(api)
                {
                    rateTypeAPI = api;
                    rateTypeReadyPromiseDeferred.resolve();
                }

                $scope.scopeModel.rateTypesSelected = [];

                $scope.scopeModel.onSelectRateType = function (item)
                {
                    var rateTypeTabe = {
                        Name: item.Name,
                        RateTypeId :item.RateTypeId,
                        onRateListMappingReady : function (api) {
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
                    }
                    $scope.scopeModel.rateTypesSelected.push(rateTypeTabe);
                }
                $scope.scopeModel.onDeselectRateType = function (item) {

                    $scope.scopeModel.codeTabObject.isSelected = true;
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.rateTypesSelected, item.RateTypeId, "RateTypeId");
                    $scope.scopeModel.rateTypesSelected.splice(index,1);
                }
                $scope.scopeModel.hideNormalRate = false;
                $scope.scopeModel.codeLayouts = UtilsService.getArrayEnum(WhS_SupPL_CodeLayoutEnum);
                $scope.scopeModel.dateTimeFormat = "yyyy/MM/dd";
                $scope.scopeModel.onCodeListMappingReady = function (api) {
                    codeListAPI = api;
                    codeListMappingReadyPromiseDeferred.resolve();
                }
                $scope.scopeModel.onCodeLayoutSelectionChanged = function () {
                    if ($scope.scopeModel.selectedCodeLayout != undefined && $scope.scopeModel.selectedCodeLayout.value == WhS_SupPL_CodeLayoutEnum.Delimitedcode.value) {
                        //  if ($scope.scopeModel.delimiterValue == undefined)
                        $scope.scopeModel.delimiterValue = ',';
                        $scope.scopeModel.showDelimiter = true;
                    }
                    else {
                        $scope.scopeModel.hasCodeRange = false;
                        $scope.scopeModel.delimiterValue = undefined;
                        $scope.scopeModel.showDelimiter = false;
                    }
                }
                $scope.scopeModel.onRateListMappingReady = function (api) {
                    rateListAPI = api;

                    rateListMappingReadyPromiseDeferred.resolve();
                }
                $scope.scopeModel.onCodeRangeValueChanged = function () {
                    if ($scope.scopeModel.hasCodeRange) {
                        $scope.scopeModel.rangeSeparator = '-';
                    } else {
                        $scope.scopeModel.rangeSeparator = undefined;
                    }
                }
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
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
        
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        configDetails = payload.configDetails
                        if (configDetails != undefined) {
                            $scope.scopeModel.dateTimeFormat = configDetails.DateTimeFormat;
                            $scope.scopeModel.hasCodeRange = configDetails.HasCodeRange;
                            $scope.scopeModel.rangeSeparator = configDetails.RangeSeparator;
                            $scope.scopeModel.delimiterValue = configDetails.Delimiter;
                            $scope.scopeModel.isCommaDecimalSeparator = configDetails.IsCommaDecimalSeparator;
                            $scope.scopeModel.selectedCodeLayout = UtilsService.getItemByVal($scope.scopeModel.codeLayouts, configDetails.CodeLayout, "value");
                            loadOtherRateListMapping(promises);
                        }
                    }
                    promises.push(loadRateTypeSelector());
                    promises.push(loadRateListMapping());
                    promises.push(loadCodeListMapping());

                    return UtilsService.waitMultiplePromises(promises);

                    function loadRateTypeSelector() {
                        var loadRateTypePromiseDeferred = UtilsService.createPromiseDeferred();
                        rateTypeReadyPromiseDeferred.promise.then(function () {
                            var payload;
                            if (configDetails != undefined && configDetails.OtherRateListMapping != undefined)
                            {
                                var rateTypeIds = [];
                                for(var i=0;i<configDetails.OtherRateListMapping.length;i++)
                                {
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

                    function loadRateListMapping() {
                        var loadRateListMappingPromiseDeferred = UtilsService.createPromiseDeferred();
                        rateListMappingReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                context: getContext(),
                                fieldMappings: [{ FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value }, { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value }],
                                listName: "RateList"
                            };
                            if (configDetails != undefined && configDetails.NormalRateListMapping) {
                                payload.listMappingData = configDetails.NormalRateListMapping;
                            }
                            VRUIUtilsService.callDirectiveLoad(rateListAPI, payload, loadRateListMappingPromiseDeferred);
                        });

                        return loadRateListMappingPromiseDeferred.promise;
                    }

                    function loadCodeListMapping() {
                        var loadCodeListMappingPromiseDeferred = UtilsService.createPromiseDeferred();
                        codeListMappingReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                context: getContext(),
                                fieldMappings: [{ FieldName: "Code", FieldTitle: "Code", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "CodeGroup", FieldTitle: "Code Group", isRequired: false, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value }],
                                listName: "CodeList"
                            };
                            if (configDetails != undefined && configDetails.CodeListMapping) {
                                payload.listMappingData = configDetails.CodeListMapping;
                            }
                            VRUIUtilsService.callDirectiveLoad(codeListAPI, payload, loadCodeListMappingPromiseDeferred);
                        });

                        return loadCodeListMappingPromiseDeferred.promise;
                    }

                    function loadOtherRateListMapping(promises)
                    {
                        if(configDetails != undefined && configDetails.OtherRateListMapping != undefined)
                        {
                            for(var i=0;i<configDetails.OtherRateListMapping.length;i++)
                            {
                                var otherRate = configDetails.OtherRateListMapping[i];
                                var rateTypeTab = {
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                }
                                promises.push(rateTypeTab.loadPromiseDeferred.promise);
                                addRateTypeAPIExtension(rateTypeTab, otherRate);
                            }
                        }
                    }
                };
                api.getData = getData;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
                function getData() {
                    var listCodeMapping = codeListAPI.getData();
                    var listNormalRateMapping = rateListAPI.getData();
                    var otherRatesListMappings = [];
                    if ($scope.scopeModel.rateTypesSelected.length > 0)
                    {
                        for (var i = 0; i < $scope.scopeModel.rateTypesSelected.length; i++) {
                            var rateTypeSelected = $scope.scopeModel.rateTypesSelected[i];
                            otherRatesListMappings.push(
                                {
                                    RateTypeId: rateTypeSelected.RateTypeId,
                                    RateListMapping: rateTypeSelected.rateListAPI.getData()
                                });
                        }
                    }
                    var basicConfiguration = {
                        $type: "TOne.WhS.SupplierPriceList.MainExtensions.SupplierPriceListSettings.BasicSupplierPriceListSettings,TOne.WhS.SupplierPriceList.MainExtensions",
                        CodeListMapping: listCodeMapping,
                        NormalRateListMapping: listNormalRateMapping,
                        OtherRateListMapping:otherRatesListMappings,
                        DateTimeFormat: $scope.scopeModel.dateTimeFormat,
                        CodeLayout: $scope.scopeModel.selectedCodeLayout != undefined ? $scope.scopeModel.selectedCodeLayout.value : undefined,
                        HasCodeRange: $scope.scopeModel.hasCodeRange,
                        IsCommaDecimalSeparator: $scope.scopeModel.isCommaDecimalSeparator
                    }

                    if ($scope.scopeModel.selectedCodeLayout != undefined && $scope.scopeModel.selectedCodeLayout.value == WhS_SupPL_CodeLayoutEnum.Delimitedcode.value) {
                        basicConfiguration.Delimiter = $scope.scopeModel.delimiterValue;
                    }
                    if ($scope.scopeModel.hasCodeRange) {
                        basicConfiguration.RangeSeparator = $scope.scopeModel.rangeSeparator;
                    }
                    return basicConfiguration;
                }


            }

            function addRateTypeAPIExtension(rateType, payloadRateType) {
                rateType.Name = payloadRateType.RateListMapping.ListName;
                rateType.RateTypeId = payloadRateType.RateTypeId;
                rateType.onRateListMappingReady = function (api) {
                    rateType.rateListAPI = api;
                    rateType.readyPromiseDeferred.resolve();
                }
                rateType.readyPromiseDeferred.promise.then(function () {
                    var payload = {
                        context: getContext(),
                        fieldMappings: [{ FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value }, { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value }],
                        listName: rateType.Name,
                    };
                    if (payloadRateType != undefined && payloadRateType.RateListMapping) {
                        payload.listMappingData = payloadRateType.RateListMapping;
                    }
                    VRUIUtilsService.callDirectiveLoad(rateType.rateListAPI, payload, rateType.loadPromiseDeferred)
                });
                $scope.scopeModel.rateTypesSelected.push(rateType);
            }
            function getContext() {

                if (context != undefined) {
                    var currentContext = UtilsService.cloneObject(context);
                    return currentContext;
                }
            }
        }
    }

    app.directive('whsSupplSupplierpricelistconfigurationBasic', SupplierpricelistconfigurationBasic);

})(app);