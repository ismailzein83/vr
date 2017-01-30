(function (app) {

    'use strict';

    AdvancedMeasureOrderOptions.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_Analytic_AnalyticTypeEnum', 'VR_Analytic_AnalyticItemConfigAPIService', 'VR_Analytic_OrderDirectionEnum'];

    function AdvancedMeasureOrderOptions(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticTypeEnum, VR_Analytic_AnalyticItemConfigAPIService,VR_Analytic_OrderDirectionEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                customvalidate: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AdvancedMeasureOrderOptionsDirective($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "gridCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/AnalyticConfiguration/MainExtensions/OrderType/Templates/AdvancedMeasureOrderOptionsTemplate.html",
        };
        function AdvancedMeasureOrderOptionsDirective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var measureSelectorAPI;
            var measureReadyDeferred = UtilsService.createPromiseDeferred();



            var orderTypeSelectorAPI;
            var orderTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var tableIds;
            var measures;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.orderDirectionList = UtilsService.getArrayEnum(VR_Analytic_OrderDirectionEnum);

                $scope.scopeModel.measures = [];

                $scope.scopeModel.onMeasureSelectorDirectiveReady = function (api) {
                    measureSelectorAPI = api;
                    measureReadyDeferred.resolve();
                };

                $scope.scopeModel.isValidMeasures = function () {

                    if ($scope.scopeModel.measures.length > 0)
                        return null;
                    return "At least one measure should be selected.";
                };

                $scope.scopeModel.onSelectMeasureItem = function (measure) {
                    var dataItem = {
                        AnalyticItemConfigId: measure.AnalyticItemConfigId,
                        SelectedOrderDirection: VR_Analytic_OrderDirectionEnum.Ascending,
                        Name: measure.Name
                    };
                    $scope.scopeModel.measures.push(dataItem);
                };

                $scope.scopeModel.onDeselectMeasureItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.measures, dataItem.Name, 'Name');
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.removeMeasure = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedMeasures, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedMeasures.splice(index, 1);
                    var datasourceIndex = $scope.scopeModel.measures.indexOf(dataItem);
                    $scope.scopeModel.measures.splice(datasourceIndex, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.tableIds != undefined) {
                        tableIds = payload.tableIds;
                        var promises = [];
                        var selectedMeasureIds;
                        if (payload.advancedOrderOptions != undefined)
                        {
                            if (payload.advancedOrderOptions.MeasureOrders != undefined && payload.advancedOrderOptions.MeasureOrders.length > 0) {
                                selectedMeasureIds = [];

                                for (var i = 0; i < payload.advancedOrderOptions.MeasureOrders.length; i++) {
                                    var measure = payload.advancedOrderOptions.MeasureOrders[i];
                                    var measureOrder = {
                                        payload: measure,
                                    };
                                    selectedMeasureIds.push(measure.MeasureName);
                                    addMeasureGridWidthAPI(measureOrder);
                                }

                            }
                        }

                        var loadMeasureDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        measureReadyDeferred.promise.then(function () {
                            var payloadFilterDirective = {
                                filter: { TableIds: tableIds },
                                selectedIds: selectedMeasureIds
                            };
                            VRUIUtilsService.callDirectiveLoad(measureSelectorAPI, payloadFilterDirective, loadMeasureDirectivePromiseDeferred);
                        });
                        promises.push(loadMeasureDirectivePromiseDeferred.promise);

                        promises.push(getAllMeasures());


                        return UtilsService.waitMultiplePromises(promises);
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var measures;
                    if ($scope.scopeModel.measures != undefined && $scope.scopeModel.measures.length > 0) {
                        measures = [];
                        for (var i = 0; i < $scope.scopeModel.measures.length; i++) {
                            var measure = $scope.scopeModel.measures[i];
                            measures.push({
                                MeasureName: measure.Name,
                                OrderDirection: measure.SelectedOrderDirection.value,
                            });
                        }
                    }
                    var data = {
                        $type: "Vanrise.Analytic.Entities.AnalyticQueryAdvancedMeasureOrderOptions,Vanrise.Analytic.Entities ",
                        MeasureOrders: measures,
                    };
                    return data;
                }

            }

            function addMeasureGridWidthAPI(measureOrder) {
               
                var dataItem = {};
                if (measureOrder.payload != undefined) {
                    dataItem.Name = measureOrder.payload.MeasureName;
                    dataItem.SelectedOrderDirection =  UtilsService.getItemByVal($scope.scopeModel.orderDirectionList, measureOrder.payload.OrderDirection, 'value')
                }
                $scope.scopeModel.measures.push(dataItem);
            }
            function getAllMeasures() {
                var input = {
                    TableIds: tableIds,
                    ItemType: VR_Analytic_AnalyticTypeEnum.Measure.value,
                };
                return VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                    measures = response;
                });
            }

            function getContext() {
                var context = {
                    getMeasures: function () {
                        var selectedMeasures = [];
                        for (var i = 0; i < $scope.scopeModel.selectedMeasures.length; i++) {
                            var selectedMeasure = $scope.scopeModel.selectedMeasures[i];
                            var matchItem = UtilsService.getItemByVal(measures, selectedMeasure.Name, "Name");
                            if (matchItem != undefined) {
                                selectedMeasure.FieldType = matchItem.Config.FieldType;
                            }
                            selectedMeasures.push(selectedMeasure);
                        }
                        return selectedMeasures;
                    }
                };
                return context;
            }
        }
    }

    app.directive('vrAnalyticOrdertypeAdvancedmeasureorderoptions', AdvancedMeasureOrderOptions);

})(app);