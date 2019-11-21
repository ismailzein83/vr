"use strict";

app.directive("retailBillingChargetypeAnalyticquery", ["UtilsService", "VRUIUtilsService", "VR_Analytic_AnalyticTypeEnum", "VR_Analytic_AnalyticItemConfigAPIService",
    function (UtilsService, VRUIUtilsService, VR_Analytic_AnalyticTypeEnum, VR_Analytic_AnalyticItemConfigAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new BillingChargeTypeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingChargeType/MainExtensions/Templates/AnalyticQueryChargeTypeTemplate.html"
        };

        function BillingChargeTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var dimensions;
            var analyticTableId;

            var tableSelectorAPI;
            var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedTablePromiseDeferred;

            var billingAccountDimensionSelectorAPI;
            var billingAccountDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var amountMeasureSelectorAPI;
            var amountMeasureReadyDeferred = UtilsService.createPromiseDeferred();

            var currencyMeasureSelectorAPI;
            var currencyMeasureReadyDeferred = UtilsService.createPromiseDeferred();

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onTableSelectorDirectiveReady = function (api) {
                    tableSelectorAPI = api;
                    tableSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onBillingAccountDimensionSelectorDirectiveReady = function (api) {
                    billingAccountDimensionSelectorAPI = api;
                    billingAccountDimensionReadyDeferred.resolve();
                };

                $scope.scopeModel.onAmountMeasureSelectorDirectiveReady = function (api) {
                    amountMeasureSelectorAPI = api;
                    amountMeasureReadyDeferred.resolve();
                };

                $scope.scopeModel.onCurrencyMeasureSelectorDirectiveReady = function (api) {
                    currencyMeasureSelectorAPI = api;
                    currencyMeasureReadyDeferred.resolve();
                };

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onTableSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedTablePromiseDeferred != undefined)
                            selectedTablePromiseDeferred.resolve();
                        else {
                            analyticTableId = value.AnalyticTableId;
                            loadDimensions().then(function () {
                                var payload = {
                                    filter: {
                                        TableIds: [analyticTableId]
                                    }
                                };

                                var setBillingAccountDimensionSelectorLoader = function (value) {
                                    $scope.scopeModel.isBillingAccountDimensionSelectorLoading = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, billingAccountDimensionSelectorAPI, payload, setBillingAccountDimensionSelectorLoader);

                                var setAmmountMeasureSelecorLoader = function (value) {
                                    $scope.scopeModel.isAmmountMeasureSelectorLoading = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, amountMeasureSelectorAPI, payload, setAmmountMeasureSelecorLoader);

                                var setCurrencyMeasureSelectorLoader = function (value) {
                                    $scope.scopeModel.isCurrencyMeasureSelectorLoading = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, currencyMeasureSelectorAPI, payload, setCurrencyMeasureSelectorLoader);

                                var setFilterGroupSelectorLoader = function (value) {
                                    $scope.scopeModel.isFilterGroupSelectorLoading = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, recordFilterDirectiveAPI, { context: buildContext() }, setFilterGroupSelectorLoader);
                            });
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettings;
                    var promises = [];

                    selectedTablePromiseDeferred = UtilsService.createPromiseDeferred();

                    if (payload != undefined) {
                        extendedSettings = payload.extendedSettings;
                    }
                    if (extendedSettings != undefined)
                        analyticTableId = extendedSettings.AnalyticTableId;

                    promises.push(loadAnalyticTableSelector());

                    return UtilsService.waitPromiseNode({
                        promises: promises,
                        getChildNode: function () {
                            if (analyticTableId != undefined) {
                                return {
                                    promises: [loadDimensions()],
                                    getChildNode: function () {
                                        var childPromises = [];
                                        childPromises.push(loadAmountMeasureSelector());
                                        childPromises.push(loadCurrenyMeasureSelector());
                                        childPromises.push(loadBillingAccountDimensionSelector());
                                        childPromises.push(loadRecordFilterDirective());
                                        return { promises: childPromises };
                                    }
                                }
                            }
                            else {
                                return { promises: [] };
                            }
                        }

                    }).then(function () {
                        selectedTablePromiseDeferred = undefined;
                    });

                    function loadBillingAccountDimensionSelector() {
                        var billingAccountDimensionLoadDeferred = UtilsService.createPromiseDeferred();
                        billingAccountDimensionReadyDeferred.promise.then(function () {
                            var payloadGroupingDirective = {
                                filter: {
                                    TableIds: [analyticTableId]
                                },
                                selectedIds: extendedSettings.BillingAccountDimensionName
                            };

                            VRUIUtilsService.callDirectiveLoad(billingAccountDimensionSelectorAPI, payloadGroupingDirective, billingAccountDimensionLoadDeferred);
                        });
                        return billingAccountDimensionLoadDeferred.promise;
                    }
                    function loadCurrenyMeasureSelector() {
                        var currencyMeasureLoadDeferred = UtilsService.createPromiseDeferred();
                        currencyMeasureReadyDeferred.promise.then(function () {
                            var payloadFilterDirective = {
                                filter: {
                                    TableIds: [analyticTableId]
                                },
                                selectedIds: extendedSettings.CurrencyMeasureName
                            };

                            VRUIUtilsService.callDirectiveLoad(currencyMeasureSelectorAPI, payloadFilterDirective, currencyMeasureLoadDeferred);
                        });
                        return currencyMeasureLoadDeferred.promise;
                    }
                    function loadAmountMeasureSelector() {
                        var amountMeasureLoadDeferred = UtilsService.createPromiseDeferred();
                        amountMeasureReadyDeferred.promise.then(function () {
                            var payloadFilterDirective = {
                                filter: {
                                    TableIds: [analyticTableId]
                                },
                                selectedIds: extendedSettings.AmountMeasureName
                            };

                            VRUIUtilsService.callDirectiveLoad(amountMeasureSelectorAPI, payloadFilterDirective, amountMeasureLoadDeferred);
                        });
                        return amountMeasureLoadDeferred.promise;
                    }
                    function loadAnalyticTableSelector() {
                        var loadTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        tableSelectorReadyDeferred.promise.then(function () {
                                var  payLoad = {
                                    selectedIds: analyticTableId
                                };

                            VRUIUtilsService.callDirectiveLoad(tableSelectorAPI, payLoad, loadTableSelectorPromiseDeferred);
                        });
                        return loadTableSelectorPromiseDeferred.promise;
                    }
                    function loadRecordFilterDirective() {
                        var loadRecordFilterDirectiveDeferred = UtilsService.createPromiseDeferred();

                        recordFilterDirectiveReadyDeferred.promise.then(function () {

                            var recordFilterDirectivePayload = {
                                FilterGroup: extendedSettings.FilterGroup,
                                context: buildContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, loadRecordFilterDirectiveDeferred);
                        });

                        return loadRecordFilterDirectiveDeferred.promise;
                    }
                };

                api.getData = function () {

                    var recordFilter;
                    var data = recordFilterDirectiveAPI.getData();

                    if (data != undefined && data.filterObj != undefined) {
                        recordFilter = data.filterObj;
                    }

                    var obj = {
                        $type: "Retail.Billing.MainExtensions.RetailBillingChargeType.RetailBillingAnalyticQueryChargeType,Retail.Billing.MainExtensions",
                        AnalyticTableId: tableSelectorAPI.getSelectedIds(),
                        BillingAccountDimensionName: billingAccountDimensionSelectorAPI.getSelectedIds(),
                        AmountMeasureName: amountMeasureSelectorAPI.getSelectedIds(),
                        CurrencyMeasureName: currencyMeasureSelectorAPI.getSelectedIds(),
                        FilterGroup: recordFilter
                    };

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadDimensions() {
                var input = {
                    TableIds: [analyticTableId],
                    ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value,
                };
                return VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                    dimensions = response;
                });
            }
            function buildContext() {

                var fields = [];
                for (var i = 0; i < dimensions.length; i++) {
                    var dimension = dimensions[i];

                    fields.push({
                        FieldName: dimension.Name,
                        FieldTitle: dimension.Title,
                        Type: dimension.Config.FieldType,
                    });
                }

                var context = {
                    getFields: function () {
                        return fields;
                    }
                };
                return context;
            }
        }
        return directiveDefinitionObject;
    }
]);