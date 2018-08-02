"use strict";

app.directive("vrAnalyticVrautomatedreportquery", ['UtilsService', 'VRAnalytic_AutomatedReportProcessScheduledService' ,  
    function (UtilsService, VRAnalytic_AutomatedReportProcessScheduledService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return { 
                pre: function ($scope, iElem, iAttrs, ctrl) {
                }
            };
        },
        templateUrl: "/Client/Modules/Analytic/Directives/AutomatedReport/Templates/VRAutomatedReportQuery.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;        

        var context;
        var queries;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.columns = [];

            var gridAPI;

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.scopeModel.addQuery = function () {

                var onQueryAdded = function (obj) {
                    $scope.scopeModel.columns.push(obj);
                };
                VRAnalytic_AutomatedReportProcessScheduledService.addQuery(onQueryAdded, getContext());
            };

            $scope.scopeModel.removeColumn = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.columns, dataItem.VRAutomatedReportQueryId, 'VRAutomatedReportQueryId');
                if (index > -1) {
                    $scope.scopeModel.columns.splice(index, 1);
                }
            };


            $scope.scopeModel.validateColumns = function () {
                if ($scope.scopeModel.columns.length == 0) {
                    return 'At least one column must be added.';
                }
                var columnNames = [];
                for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                    var column = $scope.scopeModel.columns[i];
                    if (column.QueryTitle != undefined) {
                        columnNames.push(column.QueryTitle.toUpperCase());
                    }
                }
                while (columnNames.length > 0) {
                    var nameToValidate = columnNames[0];
                    columnNames.splice(0, 1);
                    if (!validateName(nameToValidate, columnNames)) {
                        return 'Two or more columns have the same name.';
                    }
                }
                return null;
                function validateName(name, array) {
                    for (var j = 0; j < array.length; j++) {
                        if (array[j] == name)
                            return false;
                    }
                    return true;
                }
            };

            defineMenuActions();
            defineAPI();
        }
        function defineAPI() {
            var api = {};
            api.getData = function () {
                return $scope.scopeModel.columns.length > 0 ? getColumns() : null;       
            };

            api.load = function (payload) {
                                
                if (payload != undefined) {
                    context = payload.context;
                    var queries = payload.Queries;
                    if (queries != undefined) {
                        for (var i = 0; i < queries.length; i++) {
                            var query = queries[i];
                            var gridItem = {
                                DefinitionId: query.DefinitionId,
                                VRAutomatedReportQueryId: query.VRAutomatedReportQueryId,
                                QueryTitle: query.QueryTitle,
                                Settings: query.Settings,
                            };
                            $scope.scopeModel.columns.push(gridItem);
                        }
                    }
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getColumns() {
            var columns = [];
            for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                var column = $scope.scopeModel.columns[i];
                columns.push({
                    $type: "Vanrise.Analytic.Entities.VRAutomatedReportQuery, Vanrise.Analytic.Entities",
                    DefinitionId: column.DefinitionId,
                    VRAutomatedReportQueryId: column.VRAutomatedReportQueryId,
                    QueryTitle: column.QueryTitle,
                    Settings: column.Settings
                });
            }
            return columns;
        }

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editQuery,
            }];
        }

        function editQuery(object) {
            var onQueryUpdated = function (obj) {
                var index = $scope.scopeModel.columns.indexOf(object);
                $scope.scopeModel.columns[index] = obj;
            };
            VRAnalytic_AutomatedReportProcessScheduledService.editQuery(object, onQueryUpdated, getContext());
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            currentContext.getQueryInfo = function () {
                return getColumns();
            };
            return currentContext;
        };        

       
    }

    return directiveDefinitionObject;
}]);
