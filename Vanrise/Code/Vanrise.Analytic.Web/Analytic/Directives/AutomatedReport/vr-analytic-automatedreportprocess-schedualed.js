"use strict";

app.directive("vrAnalyticAutomatedreportprocessSchedualed", ['UtilsService', 'VRAnalytic_AutomatedReportProcessScheduledService',  'VRUIUtilsService',
    function (UtilsService,VRAnalytic_AutomatedReportProcessScheduledService, VRUIUtilsService) {
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
        templateUrl: "/Client/Modules/Analytic/Directives/AutomatedReport/Templates/AutomatedReportProcessScheduled.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;
        
        var handlerSettingsAPI;
        var handlerSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.columns = [];

            var gridAPI;

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.scopeModel.onHandlerSettingsAutomatedReportReady = function (api) {
                handlerSettingsAPI = api;
                handlerSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.addQuery = addQuery;

            function addQuery() { 
                var onQueryAdded = function (obj) {
                    $scope.scopeModel.columns.push(obj);
                };
                VRAnalytic_AutomatedReportProcessScheduledService.addQuery(onQueryAdded);
            }

            $scope.scopeModel.removeColumn = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.columns, dataItem.id, 'id');
                if (index > -1) {
                    $scope.scopeModel.columns.splice(index, 1);
                }
            };


            $scope.scopeModel.validateColumns = function () {
                if ($scope.scopeModel.columns.length == 0) {
                    return 'Please, one record must be added at least.';
                }
                var columnNames = [];
                for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                    if ($scope.scopeModel.columns[i].QueryName != undefined) {
                        columnNames.push($scope.scopeModel.columns[i].QueryName.toUpperCase());
                    }
                }
                while (columnNames.length > 0) {
                    var nameToValidate = columnNames[0];
                    columnNames.splice(0, 1);
                    if (!validateName(nameToValidate, columnNames)) {
                        return 'Two or more columns have the same Name';
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
            var api = {
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Analytic.BP.Arguments.VRAutomatedReportProcessInput,Vanrise.Analytic.BP.Arguments",
                    Name: $scope.scopeModel.name,
                    Queries: $scope.scopeModel.columns.length > 0 ? getColumns() : null,
                    Handler: {
                        Settings: handlerSettingsAPI.getData(),
                    },
                };

                function getColumns() {
                    var columns = [];
                    for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                        var column = $scope.scopeModel.columns[i];
                        columns.push({
                            DefinitionId: column.DefinitionId,
                            QueryName: column.QueryName,
                            Settings: column.Settings, 
                        });
                    }
                    return columns;
                }
            };

            api.load = function (payload) {

                if (payload != undefined && payload.data != undefined) {

                    var Queries = payload.data.Queries;
                    var Handler = payload.data.Handler;

                    for (var i = 0; i < Queries.length; i++) {
                        var gridItem = {
                            id: i,
                            DefinitionId: Queries[i].DefinitionId,
                            QueryName: Queries[i].QueryName,
                            Settings: Queries[i].Settings,
                        };
                        $scope.scopeModel.columns.push(gridItem);
                    }
                }
                loadHandlerSelector();
                function loadHandlerSelector() {
                    var handlerSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    handlerSettingsReadyPromiseDeferred.promise.then(function () {

                        var handlerPayload = Handler != undefined && Handler.Settings != undefined ? Handler.Settings : undefined;

                        VRUIUtilsService.callDirectiveLoad(handlerSettingsAPI, handlerPayload, handlerSettingsLoadPromiseDeferred);

                    });
                    return handlerSettingsLoadPromiseDeferred.promise;
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
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
            VRAnalytic_AutomatedReportProcessScheduledService.editQuery(object, onQueryUpdated);
        }
    }

    return directiveDefinitionObject;
}]);
