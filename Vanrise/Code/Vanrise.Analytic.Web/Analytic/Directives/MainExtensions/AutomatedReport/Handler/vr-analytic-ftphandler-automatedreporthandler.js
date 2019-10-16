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

        var fileGeneratorGridAPI;
        var fileGeneratorGridReadyDeferred = UtilsService.createPromiseDeferred();

        var ftpCommunicatorSettingsAPI;
        var ftpCommunicatorSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        var context;

        function initializeController() {

            $scope.scopeModel = {};


            $scope.scopeModel.onFileGeneratorGridReady = function (api) {
                fileGeneratorGridAPI = api;
                fileGeneratorGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onFTPCommunicatorSettingsReady = function (api) {
                ftpCommunicatorSettingsAPI = api;
                ftpCommunicatorSettingsReadyDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                var ftpCommunicatorSettings;
                var attachmentGenerators;
                if (payload != undefined) {
                    context = payload.context;
                    if (payload.settings != undefined) {
                        attachmentGenerators =  payload.settings.AttachementGenerators;
                        ftpCommunicatorSettings = payload.settings.FTPCommunicatorSettings;
                    }
                }

                var loadFTPCommunicatorSettingsPromise = loadFTPCommunicatorSettings();
                promises.push(loadFTPCommunicatorSettingsPromise);

                var loadFileGeneratorGridPromise = loadFileGeneratorGrid();
                promises.push(loadFileGeneratorGridPromise);
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

                function loadFileGeneratorGrid() {
                    var fileGeneratorGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    fileGeneratorGridReadyDeferred.promise.then(function () {
                           var fileGeneratorGridPayload = {
                                context: getContext()
                            };
                            if (attachmentGenerators != undefined) {
                                fileGeneratorGridPayload.attachmentGenerators = attachmentGenerators;
                            }

                        VRUIUtilsService.callDirectiveLoad(fileGeneratorGridAPI, fileGeneratorGridPayload, fileGeneratorGridLoadPromiseDeferred);
                    });

                    return fileGeneratorGridLoadPromiseDeferred.promise;
                }


                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.FTPHandler,Vanrise.Analytic.MainExtensions",
                    AttachementGenerators: fileGeneratorGridAPI.getData(),
                    FTPCommunicatorSettings: ftpCommunicatorSettingsAPI.getData()
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