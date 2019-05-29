//(function (app) {

//    'use strict';

//    PermanentfilterConditionfilter.$inject = ['VRUIUtilsService', 'UtilsService', 'VR_Analytic_AnalyticTableService'];

//    function PermanentfilterConditionfilter(VRUIUtilsService, UtilsService, VR_Analytic_AnalyticTableService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '='
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var permanentFilterConfitionFilter = new PermanentFilterConditionFilter($scope, ctrl, $attrs);
//                permanentFilterConfitionFilter.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticReport/PermanentFilter/MainExtensions/Templates/PermanentFilterConditionFilter.html"
//        };

//        function PermanentFilterConditionFilter($scope, ctrl, $attrs) {

//            this.initializeController = initializeController;

//            var gridAPI;
//            var analyticTableId;
//            var logicalOperator;
//            var logicalOperatorDirectiveAPI;
//            var logicalOperatorDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            ctrl.datasource = [];

//            var conditionFilterItems;

//            function initializeController() {
//                $scope.scopeModel = {};

//                ctrl.onGridReady = function (api) {
//                    gridAPI = api;
//                    defineAPI();
//                }; 
//                $scope.scopeModel.addConditionFilterItem = function () {
//                    var onConditionFilterItemAdded = function (row) {
//                        ctrl.datasource.push(row);
//                    };
//                    VR_Analytic_AnalyticTableService.addConditionFilterItem(analyticTableId, onConditionFilterItemAdded);
//                };
//                $scope.scopeModel.onLogicalOperatorDirectiveReady = function (api) {
//                    logicalOperatorDirectiveAPI = api;
//                    logicalOperatorDirectiveReadyPromiseDeferred.resolve();
//                };

//                $scope.scopeModel.onDeleteRow = function (dataItem) {
//                    var index = ctrl.datasource.indexOf(dataItem);
//                    ctrl.datasource.splice(index, 1);
//                };  
//                defineMenuActions();
              
//            }
     
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    if (payload != undefined) {
//                        analyticTableId = payload.analyticTableId;
//                        logicalOperator = payload.settings != undefined ? payload.settings.LogicalOperator : undefined;
//                        conditionFilterItems = payload.settings != undefined ? payload.settings.ConditionFilterItems : undefined;
//                    }

//                    if (conditionFilterItems != undefined && conditionFilterItems.length > 0) {
//                        for (var i = 0; i < conditionFilterItems.length; i++) {
//                            ctrl.datasource.push({ Entity: conditionFilterItems[i] });
//                        }
//                    }
//                    var rootPromiseNode = { promises: [loadLogicalOperatorDirective()] };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {
//                    return {
//                        $type: "Vanrise.Analytic.Entities.ConditionFilterAnalyticTablePermanentFilter,Vanrise.Analytic.Entities",
//                        ConditionFilterItems: getConditionFilterItems(),
//                        LogicalOperator: logicalOperatorDirectiveAPI.getData()
//                    };
//                };

//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
//            }

//            function getConditionFilterItems() {
//                var items = [];
//                if (ctrl.datasource.length > 0) {
//                    for (var j = 0; j < ctrl.datasource.length; j++) {
//                        items.push(ctrl.datasource[j].Entity);
//                    }
//                }
//                return items;
//            }
//            function loadLogicalOperatorDirective() {
//                var logicalOperatorDirectiveLoadedPromiseDeferred = UtilsService.createPromiseDeferred();

//                logicalOperatorDirectiveReadyPromiseDeferred.promise.then(function () {

//                    var logicalOperatorDirectivePayload;
//                    if (conditionFilterItems != undefined) {
//                        logicalOperatorDirectivePayload = { LogicalOperator: logicalOperator };
//                    }
//                    VRUIUtilsService.callDirectiveLoad(logicalOperatorDirectiveAPI, logicalOperatorDirectivePayload, logicalOperatorDirectiveLoadedPromiseDeferred);
//                });

//                return logicalOperatorDirectiveLoadedPromiseDeferred.promise;
//            }

//            function defineMenuActions() {
//                var defaultMenuActions = [
//                    {
//                        name: "Edit",
//                        clicked: editConditionFilterItem
//                    }];

//                ctrl.gridMenuActions = function (dataItem) {
//                    return defaultMenuActions;
//                };
//            }

//            function editConditionFilterItem(dataItem) {
//                var onConditionFilterItemUpdated = function (conditionFilterItem) {
//                    var index = ctrl.datasource.indexOf(dataItem);
//                    ctrl.datasource[index] = conditionFilterItem;
//                };

//                VR_Analytic_AnalyticTableService.editConditionFilterItem(analyticTableId,dataItem.Entity, onConditionFilterItemUpdated);
//            }
//        }
//    }

//    app.directive('vrAnalyticPermanentfilterConditionfilter', PermanentfilterConditionfilter);

//})(app);