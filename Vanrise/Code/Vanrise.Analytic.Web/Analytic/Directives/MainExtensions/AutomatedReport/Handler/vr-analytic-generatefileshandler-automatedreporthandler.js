"use strict";
app.directive("vrAnalyticGeneratefileshandlerAutomatedreporthandler", ["UtilsService", "VRAnalytic_AutomatedReportHandlerService", "VRUIUtilsService",
function (UtilsService, VRAnalytic_AutomatedReportHandlerService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var generateFilesHandler = new GenerateFilesHandler($scope, ctrl, $attrs);
            generateFilesHandler.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Handler/Templates/GenerateFilesHandlerAutomatedReport.html"
    };


    function GenerateFilesHandler($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var context;

        var fileGeneratorGridAPI;
        var fileGeneratorGridReadyDeferred = UtilsService.createPromiseDeferred();

        var actionTypeSelectiveAPI;
        var actionTypeSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onFileGeneratorGridReady = function (api) {
                fileGeneratorGridAPI = api;
                fileGeneratorGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onActionTypeSelectiveReady = function (api) {
                actionTypeSelectiveAPI = api;
                actionTypeSelectiveReadyDeferred.resolve();
            };
           
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                var attachmentGenerators;
                if (payload != undefined) {
                    context = payload.context;
                }

                var loadFileGeneratorGridPromise = loadFileGeneratorGrid();
                promises.push(loadFileGeneratorGridPromise);
                var loadActionTypeSelectivePromise = loadActionTypeSelective();
                promises.push(loadActionTypeSelectivePromise);

                function loadFileGeneratorGrid() {
                    var fileGeneratorGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    fileGeneratorGridReadyDeferred.promise.then(function () {
                        var fileGeneratorGridPayload = {
                            context: getContext()
                        };
                        if (payload != undefined && payload.settings) {
                            fileGeneratorGridPayload.attachmentGenerators = payload.settings.AttachementGenerators;
                        }

                        VRUIUtilsService.callDirectiveLoad(fileGeneratorGridAPI, fileGeneratorGridPayload, fileGeneratorGridLoadPromiseDeferred);
                    });

                    return fileGeneratorGridLoadPromiseDeferred.promise;
                }

                function loadActionTypeSelective() {
                    var loadActionTypeSelectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    actionTypeSelectiveReadyDeferred.promise.then(function () {
                        var actionTypeSelectivePayload = {
                            context: getContext()
                        };
                        if (payload != undefined && payload.settings) {
                            actionTypeSelectivePayload.actionType = payload.settings.ActionType;
                        }
                        VRUIUtilsService.callDirectiveLoad(actionTypeSelectiveAPI, actionTypeSelectivePayload, loadActionTypeSelectiveLoadDeferred);
                    });
                    return loadActionTypeSelectiveLoadDeferred.promise;
                }
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.GenerateFilesHandler,Vanrise.Analytic.MainExtensions",
                    AttachementGenerators: fileGeneratorGridAPI.getData(),
                    ActionType: actionTypeSelectiveAPI.getData()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
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