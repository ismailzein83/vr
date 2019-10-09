"use strict";
app.directive("vrAnalyticSendemailhandlerAutomatedreporthandler", ["UtilsService", "VRAnalytic_AutomatedReportHandlerService","VRUIUtilsService",
function (UtilsService, VRAnalytic_AutomatedReportHandlerService, VRUIUtilsService) {
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

        var fileGeneratorGridAPI;
        var fileGeneratorGridReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.onFileGeneratorGridReady = function (api) {
                fileGeneratorGridAPI = api;
                fileGeneratorGridReadyDeferred.resolve();
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
                    if (payload.settings != undefined) {
                        $scope.scopeModel.to = payload.settings.To;
                        $scope.scopeModel.cc = payload.settings.CC;
                        $scope.scopeModel.subject = payload.settings.Subject;
                        $scope.scopeModel.body = payload.settings.Body;
                        attachmentGenerators = payload.settings.AttachementGenerators;
                    }
                }

                var loadFileGeneratorGridPromise = loadFileGeneratorGrid();
                promises.push(loadFileGeneratorGridPromise);

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
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.SendEmailHandler,Vanrise.Analytic.MainExtensions",
                    To: $scope.scopeModel.to,
                    CC: $scope.scopeModel.cc,
                    Subject: $scope.scopeModel.subject,
                    Body: $scope.scopeModel.body,
                    AttachementGenerators: fileGeneratorGridAPI.getData(),
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