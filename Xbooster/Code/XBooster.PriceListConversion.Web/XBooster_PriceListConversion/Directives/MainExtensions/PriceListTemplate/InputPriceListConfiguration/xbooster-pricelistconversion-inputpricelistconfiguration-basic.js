(function (app) {

    'use strict';

    InputpricelistconfigurationBasic.$inject = ["UtilsService", "VRUIUtilsService",'XBooster_PriceListConversion_CodeLayoutEnum','VR_ExcelConversion_FieldTypeEnum'];

    function InputpricelistconfigurationBasic(UtilsService, VRUIUtilsService, XBooster_PriceListConversion_CodeLayoutEnum,VR_ExcelConversion_FieldTypeEnum) {
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
                var inputpricelistconfiguration = new Inputpricelistconfiguration($scope, ctrl, $attrs);
                inputpricelistconfiguration.initializeController();
            },
            controllerAs: "outputbasicCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/XBooster_PriceListConversion/Directives/MainExtensions/PriceListTemplate/InputPriceListConfiguration/Templates/BasicInputPriceListConfiguration.html"
        };

        function Inputpricelistconfiguration($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var rateListAPI;
            var rateListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var codeListAPI;
            var codeListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var context;
            var configDetails;
            $scope.outPutFieldMappings;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.validate = function () {
                    if ($scope.scopeModel.delimiterValue == $scope.scopeModel.rangeSeparator) {

                        return "Range separator should not be the same as delimiter.";
                    }
                    return null;
                };

               $scope.scopeModel.codeLayouts=UtilsService.getArrayEnum(XBooster_PriceListConversion_CodeLayoutEnum);
                $scope.scopeModel.dateTimeFormat = "yyyy/MM/dd";
                $scope.scopeModel.onCodeListMappingReady = function (api) {
                    codeListAPI = api;
                    codeListMappingReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCodeLayoutSelectionChanged = function () {
                    if ($scope.scopeModel.selectedCodeLayout != undefined && $scope.scopeModel.selectedCodeLayout.value == XBooster_PriceListConversion_CodeLayoutEnum.Delimitedcode.value) {
                        //  if ($scope.scopeModel.delimiterValue == undefined)
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
                    rateListMappingReadyPromiseDeferred.resolve();
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
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        configDetails = payload.configDetails;
                        if (configDetails != undefined && configDetails.ExcelConversionSettings !=undefined)
                        {
                            $scope.scopeModel.dateTimeFormat = configDetails.ExcelConversionSettings.DateTimeFormat;
                            $scope.scopeModel.hasCodeRange = configDetails.HasCodeRange;
                            $scope.scopeModel.rangeSeparator = configDetails.RangeSeparator;
                            $scope.scopeModel.delimiterValue = configDetails.Delimiter;
                            $scope.scopeModel.isCommaDecimalSeparator =   configDetails.IsCommaDecimalSeparator;
                            $scope.scopeModel.selectedCodeLayout = UtilsService.getItemByVal($scope.scopeModel.codeLayouts, configDetails.CodeLayout, "value");
                        }
                     
                    }
                    promises.push(loadRateListMapping());
                    promises.push(loadCodeListMapping());
                    return UtilsService.waitMultiplePromises(promises);

                    function loadRateListMapping() {
                        var loadRateListMappingPromiseDeferred = UtilsService.createPromiseDeferred();
                        rateListMappingReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                context: getContext(),
                                fieldMappings: [{ FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value }, { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: false, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value }],
                                listName: "RateList"
                            };
                            if (configDetails != undefined && configDetails.ExcelConversionSettings && configDetails.ExcelConversionSettings.ListMappings.length >0)
                            {
                                payload.listMappingData = configDetails.ExcelConversionSettings.ListMappings[1];
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
                                fieldMappings: [{ FieldName: "Code", FieldTitle: "Code", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "CodeGroup", FieldTitle: "Code Group", isRequired: false, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: false, type: "cell", FieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value }],
                                listName: "CodeList"
                            };
                            if (configDetails != undefined && configDetails.ExcelConversionSettings && configDetails.ExcelConversionSettings.ListMappings.length > 0) {
                                payload.listMappingData = configDetails.ExcelConversionSettings.ListMappings[0];
                            }
                            VRUIUtilsService.callDirectiveLoad(codeListAPI, payload, loadCodeListMappingPromiseDeferred);
                        });

                        return loadCodeListMappingPromiseDeferred.promise;
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var listMappings = [];
                    if (codeListAPI != undefined)
                        listMappings.push(codeListAPI.getData());
                    if (rateListAPI != undefined)
                        listMappings.push(rateListAPI.getData());

                    var obj = {
                        ListMappings: listMappings,
                        FieldMappings: null,
                        DateTimeFormat: $scope.scopeModel.dateTimeFormat
                    };
                    var basicConfiguration = {
                        $type: "XBooster.PriceListConversion.MainExtensions.InputPriceListSettings.BasicInputPriceListSettings,XBooster.PriceListConversion.MainExtensions",
                        ExcelConversionSettings: obj,
                        CodeLayout: $scope.scopeModel.selectedCodeLayout != undefined ? $scope.scopeModel.selectedCodeLayout.value : undefined,
                        HasCodeRange: $scope.scopeModel.hasCodeRange,
                        IsCommaDecimalSeparator: $scope.scopeModel.isCommaDecimalSeparator
                    };
                   
                    if ($scope.scopeModel.selectedCodeLayout != undefined && $scope.scopeModel.selectedCodeLayout.value == XBooster_PriceListConversion_CodeLayoutEnum.Delimitedcode.value)
                    {
                        basicConfiguration.Delimiter = $scope.scopeModel.delimiterValue;
                    }
                    if ($scope.scopeModel.hasCodeRange) {
                        basicConfiguration.RangeSeparator = $scope.scopeModel.rangeSeparator;
                    }
                    return basicConfiguration;
                }

                function getContext() {

                    if (context != undefined) {
                        var currentContext = UtilsService.cloneObject(context);
                        return currentContext;
                    }
                }
            }
        }
    }

    app.directive('xboosterPricelistconversionInputpricelistconfigurationBasic', InputpricelistconfigurationBasic);

})(app);