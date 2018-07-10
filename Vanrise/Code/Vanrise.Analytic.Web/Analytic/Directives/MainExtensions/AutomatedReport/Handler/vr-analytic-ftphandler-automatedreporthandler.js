"use strict";
app.directive("vrAnalyticFtphandlerAutomatedreporthandler", ["UtilsService", "VRAnalytic_AutomatedReportHandlerService", "VRUIUtilsService",
function (UtilsService, VRAnalytic_AutomatedReportHandlerService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ftpHandler = new FTPHandler($scope, ctrl, $attrs);
            ftpHandler.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Handler/Templates/FTPHandlerAutomatedReport.html"
    };


    function FTPHandler($scope, ctrl, $attrs) {
        this.initializeController = initializeController;


        var ftpCommunicatorSettingsAPI;
        var ftpCommunicatorSettingsReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onFTPCommunicatorSettingsReady = function (api) {
                ftpCommunicatorSettingsAPI = api;
                ftpCommunicatorSettingsReadyDeferred.resolve();
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
                var ftpCommunicatorSettings;

                if (payload != undefined) {
                    context = payload.context;
                    if (payload.settings != undefined) {
                        var attachementGenerators = payload.settings.AttachementGenerators;
                        ftpCommunicatorSettings = payload.settings.FTPCommunicatorSettings;
                        $scope.scopeModel.subdirectory = payload.settings.Subdirectory;
                        if (attachementGenerators != undefined) {
                            for (var i = 0; i < attachementGenerators.length; i++) {
                                var generator = attachementGenerators[i];
                                var gridItem = {
                                    id: i,
                                    VRAutomatedReportFileGeneratorId: generator.VRAutomatedReportFileGeneratorId,
                                    Name: generator.Name,
                                    Settings: generator.Settings,
                                };
                                $scope.scopeModel.columns.push(gridItem);
                            }
                        }
                    }
                }

                var loadFTPCommunicatorSettingsPromise = loadFTPCommunicatorSettings();
                promises.push(loadFTPCommunicatorSettingsPromise);

                function loadFTPCommunicatorSettings() {
                    var ftpCommunicatorSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    ftpCommunicatorSettingsReadyDeferred.promise.then(function () {
                        var ftpCommunicatorSettingsPayload;
                        if (ftpCommunicatorSettings != undefined)
                            ftpCommunicatorSettingsPayload = { ftpCommunicatorSettings: ftpCommunicatorSettings };

                        VRUIUtilsService.callDirectiveLoad(ftpCommunicatorSettingsAPI, ftpCommunicatorSettingsPayload, ftpCommunicatorSettingsLoadPromiseDeferred);
                    });

                    return ftpCommunicatorSettingsLoadPromiseDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.FTPHandler,Vanrise.Analytic.MainExtensions",
                    AttachementGenerators: $scope.scopeModel.columns.length > 0 ? getColumns() : null,
                    Subdirectory: $scope.scopeModel.subdirectory,
                    FTPCommunicatorSettings: ftpCommunicatorSettingsAPI.getData()
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