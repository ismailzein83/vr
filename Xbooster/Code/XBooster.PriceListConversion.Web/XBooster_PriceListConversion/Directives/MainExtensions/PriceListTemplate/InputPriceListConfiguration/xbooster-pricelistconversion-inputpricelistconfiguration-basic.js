(function (app) {

    'use strict';

    InputpricelistconfigurationBasic.$inject = ["UtilsService", "VRUIUtilsService",'XBooster_PriceListConversion_CodeLayoutEnum'];

    function InputpricelistconfigurationBasic(UtilsService, VRUIUtilsService, XBooster_PriceListConversion_CodeLayoutEnum) {
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


               $scope.scopeModel.codeLayouts=UtilsService.getArrayEnum(XBooster_PriceListConversion_CodeLayoutEnum);
                $scope.scopeModel.dateTimeFormat = "yyyy/MM/dd";
                $scope.scopeModel.onCodeListMappingReady = function (api) {
                    codeListAPI = api;
                    codeListMappingReadyPromiseDeferred.resolve();
                }
                $scope.scopeModel.onCodeLayoutSelectionChanged= function()
                {
                    if ($scope.scopeModel.selectedCodeLayout !=undefined && $scope.scopeModel.selectedCodeLayout.value == XBooster_PriceListConversion_CodeLayoutEnum.CammaSeparated.value)
                    {
                        if ($scope.scopeModel.delimiterValue == undefined)
                             $scope.scopeModel.delimiterValue = ',';
                        $scope.scopeModel.showDelimiter = true;
                    }
                    else
                    {
                        $scope.scopeModel.delimiterValue = undefined;
                        $scope.scopeModel.showDelimiter = false;
                    }
                }
                $scope.scopeModel.onRateListMappingReady = function (api) {
                    rateListAPI = api;
                    rateListMappingReadyPromiseDeferred.resolve();
                }
                $scope.scopeModel.onCodeRangeValueChanged = function()
                {
                    if ($scope.scopeModel.hasCodeRange)
                    {
                        $scope.scopeModel.rangeSeparator = '-';
                    }else
                    {
                        $scope.scopeModel.rangeSeparator = undefined;
                    }
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
                        if (configDetails != undefined && configDetails.ExcelConversionSettings !=undefined)
                        {
                            $scope.scopeModel.dateTimeFormat = configDetails.ExcelConversionSettings.DateTimeFormat;
                            $scope.scopeModel.hasCodeRange = configDetails.HasCodeRange;
                            $scope.scopeModel.rangeSeparator = configDetails.RangeSeparator;
                            $scope.scopeModel.delimiterValue = configDetails.Delimiter;
                            $scope.scopeModel.selectedCodeLayout = UtilsService.getItemByVal($scope.scopeModel.codeLayouts, configDetails.CodeLayout, "value")
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
                                fieldMappings: [{ FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell" }, { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell" }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: false, type: "cell" }],
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
                                fieldMappings: [{ FieldName: "Code", FieldTitle: "Code", isRequired: true, type: "cell" }, { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell" }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: false, type: "cell" }],
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
                    }
                    var basicConfiguration = {
                        $type: "XBooster.PriceListConversion.MainExtensions.InputPriceListSettings.BasicInputPriceListSettings,XBooster.PriceListConversion.MainExtensions",
                        ExcelConversionSettings: obj,
                        CodeLayout: $scope.scopeModel.selectedCodeLayout != undefined ? $scope.scopeModel.selectedCodeLayout.value : undefined,
                        HasCodeRange: $scope.scopeModel.hasCodeRange,
                      
                    }
                   
                    if ($scope.scopeModel.selectedCodeLayout != undefined && $scope.scopeModel.selectedCodeLayout.value == XBooster_PriceListConversion_CodeLayoutEnum.CammaSeparated.value)
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