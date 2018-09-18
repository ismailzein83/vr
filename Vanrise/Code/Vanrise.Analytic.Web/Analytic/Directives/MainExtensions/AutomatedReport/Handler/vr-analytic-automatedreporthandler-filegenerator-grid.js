"use strict";
app.directive("vrAnalyticAutomatedreporthandlerFilegeneratorGrid", ["UtilsService", "VRAnalytic_AutomatedReportHandlerService", "VRUIUtilsService",
function (UtilsService, VRAnalytic_AutomatedReportHandlerService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var fileGeneratorGrid = new FileGeneratorGrid($scope, ctrl, $attrs);
            fileGeneratorGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Handler/Templates/AutomatedReportFileGeneratorGridTemplate.html"
    };


    function FileGeneratorGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var context;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.columns = [];

            $scope.scopeModel.removeColumn = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.columns, dataItem.id, 'id');
                if (index > -1) {
                    $scope.scopeModel.columns.splice(index, 1);
                }
            };

            $scope.scopeModel.addAttachementGenerator = function () {
                var onAttachementGeneratorAdded = function (obj) {
                    $scope.scopeModel.columns.push(obj);
                };
                VRAnalytic_AutomatedReportHandlerService.addAttachementGenerator(onAttachementGeneratorAdded, getContext());
            };


            $scope.scopeModel.validateColumns = function () {
                if ($scope.scopeModel.columns.length == 0) {
                    return 'At least one record must be added.';
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

            api.load = function (payload) {
                var promises = [];

                if (payload != undefined) {
                    context = payload.context;
                    if (payload.attachmentGenerators != undefined) {
                        for (var i = 0; i < payload.attachmentGenerators.length; i++) {
                            var generator = payload.attachmentGenerators[i];
                                var gridItem = {
                                    id: i,
                                    VRAutomatedReportFileGeneratorId: generator.VRAutomatedReportFileGeneratorId,
                                    Name: generator.Name,
                                    Settings: generator.Settings,
                                    CompressFile:generator.CompressFile
                                };
                                $scope.scopeModel.columns.push(gridItem);
                            }
                        
                    }
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {

                function getColumns() {
                    var columns = [];
                    for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                        var column = $scope.scopeModel.columns[i];
                        columns.push({
                            VRAutomatedReportFileGeneratorId: column.VRAutomatedReportFileGeneratorId,
                            Name: column.Name,
                            Settings: column.Settings,
                            CompressFile: column.CompressFile
                        });
                    }
                    return columns;
                }
                return $scope.scopeModel.columns.length > 0 ? getColumns() : null;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editAttachementGenerator,
            }];
        }

        function editAttachementGenerator(object) {

            var onAttachementGeneratorUpdated = function (obj) {
                var index = $scope.scopeModel.columns.indexOf(object);
                $scope.scopeModel.columns[index] = obj;
            };
            VRAnalytic_AutomatedReportHandlerService.editAttachementGenerator(object, onAttachementGeneratorUpdated, getContext());
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined) {
                currentContext = {};
            }
            return currentContext;
        }
    }

    return directiveDefinitionObject;
}
]);