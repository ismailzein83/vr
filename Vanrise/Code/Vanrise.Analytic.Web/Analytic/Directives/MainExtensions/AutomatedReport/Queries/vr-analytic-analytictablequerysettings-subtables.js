"use strict";
app.directive("vrAnalyticAnalytictablequerysettingsSubtables", ["UtilsService", "VRUIUtilsService", "VR_Analytic_AnalyticTableQuerySettingsSubtablesService",
function (UtilsService, VRUIUtilsService, VR_Analytic_AnalyticTableQuerySettingsSubtablesService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var analyticSubtables = new AnalyticSubtables($scope, ctrl, $attrs);
            analyticSubtables.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Queries/Templates/AnalyticSubtablesTemplate.html"
    };


    function AnalyticSubtables($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        var analyticTableId;
        var context;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.subtables = [];
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.addSubtable = function () {
                var onSubtableAdded = function (obj) {
                    $scope.scopeModel.subtables.push(obj);
                };
                VR_Analytic_AnalyticTableQuerySettingsSubtablesService.addSubtable(analyticTableId, onSubtableAdded, getContext());
            };

            $scope.scopeModel.removeSubtable = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.subtables, dataItem.id, 'id');
                if (index > -1) {
                    $scope.scopeModel.subtables.splice(index, 1);
                }
            };

            defineMenuActions();

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                ctrl.onReady(getDirectiveAPI());
            }
        }

        function getDirectiveAPI() {

            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    analyticTableId = payload.analyticTableId;
                    if (payload.subtables != undefined) {
                        for (var i = 0; i < payload.subtables.length; i++) {
                            var subtable = payload.subtables[i];
                            var gridItem = {
                                SubTableId: subtable.SubTableId,
                                Title: subtable.Title,
                                Measures: subtable.Measures,
                                Dimensions: subtable.Dimensions,
                                OrderType: subtable.OrderType,
                                AdvancedOrderOptions: subtable.AdvancedOrderOptions
                            };
                            $scope.scopeModel.subtables.push(gridItem);
                        }
                    }
                }
                var promises = [];

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var subtables;
                if($scope.scopeModel.subtables!=undefined && $scope.scopeModel.subtables.length>0){
                    subtables = [];
                    for (var i = 0; i < $scope.scopeModel.subtables.length; i++)
                    {
                        var subtable = $scope.scopeModel.subtables[i];
                        subtables.push({
                            SubTableId: subtable.SubTableId,
                            Title: subtable.Title,
                            Dimensions:subtable.Dimensions,
                            Measures: subtable.Measures,
                            OrderType: subtable.OrderType,
                            AdvancedOrderOptions:subtable.AdvancedOrderOptions
                            });
                    }
                }
                return subtables;
            };

            return api;
        }
        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editSubtable,
            }];
        }

        function editSubtable(subtableObj) {
            var onSubtableUpdated = function (subtable) {
                var index = $scope.scopeModel.subtables.indexOf(subtableObj);
                $scope.scopeModel.subtables[index] = subtable;
            };
            VR_Analytic_AnalyticTableQuerySettingsSubtablesService.editSubtable(analyticTableId, subtableObj, onSubtableUpdated, getContext());
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {
                };
            return currentContext;
        }
    }

    return directiveDefinitionObject;
}
]);