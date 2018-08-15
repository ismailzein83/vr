"use strict";
app.directive("vrAnalyticFtpactiontypeGeneratefileshandler", ["UtilsService", "VRAnalytic_AutomatedReportHandlerService", "VRUIUtilsService",
function (UtilsService, VRAnalytic_AutomatedReportHandlerService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ftpActionType = new FTPActionType($scope, ctrl, $attrs);
            ftpActionType.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Handler/Templates/FTPActionTypeGenerateFilesHandlerTempate.html"
    };


    function FTPActionType($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var ftpCommunicatorSettingsAPI;
        var ftpCommunicatorSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        var context;

        function initializeController() {

            $scope.scopeModel = {};

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
                if (payload != undefined) {
                    context = payload.context;
                    if (payload.actionType != undefined) {
                        ftpCommunicatorSettings = payload.actionType.FTPCommunicatorSettings;
                        $scope.scopeModel.subdirectory = payload.actionType.Subdirectory;
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
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.FTPActionType,Vanrise.Analytic.MainExtensions",
                    Subdirectory: $scope.scopeModel.subdirectory,
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