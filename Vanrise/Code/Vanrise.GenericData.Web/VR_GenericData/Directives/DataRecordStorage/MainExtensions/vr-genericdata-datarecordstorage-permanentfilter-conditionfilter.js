//(function (app) {

//    'use strict';

//    PermanentfilterConditionfilter.$inject = ['VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordStorageService'];

//    function PermanentfilterConditionfilter(VRUIUtilsService, UtilsService, VR_GenericData_DataRecordStorageService) {
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
//            templateUrl: "/Client/Modules/Common/Directives/PermanentFilterConditionFilter.html"

//                //'/Client/Modules/VR_GenericData/Directives/DataRecordStorage/MainExtensions/Templates/PermanentfilterRecordfilter.html'
//        };

//        function PermanentFilterConditionFilter($scope, ctrl, $attrs) {

//            this.initializeController = initializeController;

//            var gridAPI;
//            var dataRecordTypeId;
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
//                    VR_GenericData_DataRecordStorageService.addConditionFilterItem(dataRecordTypeId, onConditionFilterItemAdded);
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
//                        dataRecordTypeId = payload.dataRecordTypeId;
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
//                        $type: "Vanrise.GenericData.Entities.ConditionFilterDataRecordStoragePermanentFilter,Vanrise.GenericData.Entities",
//                        ConditionFilterItems: getConditionFilterItems(),
//                        LogicalOperator: logicalOperatorDirectiveAPI.getData(),
//                    };
//                };

//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
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

//            function getConditionFilterItems() {
//                var items = [];
//                if (ctrl.datasource.length > 0) {
//                    for (var j = 0; j < ctrl.datasource.length; j++) {
//                        items.push(ctrl.datasource[j].Entity);
//                    }
//                }
//                return items;
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

//                VR_GenericData_DataRecordStorageService.editConditionFilterItem(dataRecordTypeId,dataItem.Entity, onConditionFilterItemUpdated);
//            }
//        }
//    }

//    app.directive('vrGenericdataDatarecordstoragePermanentfilterConditionfilter', PermanentfilterConditionfilter);

//})(app);