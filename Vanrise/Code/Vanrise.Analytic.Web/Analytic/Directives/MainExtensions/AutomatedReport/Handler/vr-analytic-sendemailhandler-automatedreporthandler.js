"use strict";
app.directive("vrAnalyticSendemailhandlerAutomatedreporthandler", ["UtilsService", "VRAnalytic_SendEmailHandlerService",
function (UtilsService, VRAnalytic_SendEmailHandlerService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var sendEmailHandler = new SendEmailHandler($scope, ctrl, $attrs);
            sendEmailHandler.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Handler/Templates/SendEmailHandlerAutomatedReport.html"
    };


    function SendEmailHandler($scope, ctrl, $attrs) {
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
                VRAnalytic_SendEmailHandlerService.addAttachementGenerator(onAttachementGeneratorAdded, getContext());
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
                if (payload != undefined) {
                    context = payload.context;
                    if (payload.settings != undefined) {
                        $scope.scopeModel.to = payload.settings.To;
                        $scope.scopeModel.subject = payload.settings.Subject;
                        $scope.scopeModel.body = payload.settings.Body;

                        var attachementGenerators = payload.settings.AttachementGenerators;

                        if (attachementGenerators != undefined) {
                            for (var i = 0; i < attachementGenerators.length; i++) {
                                var gridItem = {
                                    id: i,
                                    VRAutomatedReportFileGeneratorId: attachementGenerators[i].VRAutomatedReportFileGeneratorId,
                                    Name: attachementGenerators[i].Name,
                                    Settings: attachementGenerators[i].Settings,
                                };
                                $scope.scopeModel.columns.push(gridItem);
                            }
                        }
                    }
                }
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.SendEmailHandler,Vanrise.Analytic.MainExtensions",
                    To: $scope.scopeModel.to,
                    Subject: $scope.scopeModel.subject,
                    Body: $scope.scopeModel.body,
                    AttachementGenerators: $scope.scopeModel.columns.length > 0 ? getColumns() : null,
                };

                function getColumns() {
                    var columns = [];
                    for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                        var column = $scope.scopeModel.columns[i];
                        columns.push({
                            VRAutomatedReportFileGeneratorId: column.VRAutomatedReportFileGeneratorId,
                            Name: column.Name,
                            Settings: column.Settings,
                        });
                    }
                    return columns;
                }
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
            VRAnalytic_SendEmailHandlerService.editAttachementGenerator(object, onAttachementGeneratorUpdated, getContext());
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