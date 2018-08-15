"use strict";
app.directive("vrAnalyticSendemailactiontypeGeneratefileshandler", ["UtilsService", "VRAnalytic_AutomatedReportHandlerService",
function (UtilsService, VRAnalytic_AutomatedReportHandlerService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var sendEmailActionType = new SendEmailActionType($scope, ctrl, $attrs);
            sendEmailActionType.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Handler/Templates/SendEmailActionTypeGenerateFilesHandlerTemplate.html"
    };


    function SendEmailActionType($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var context;

        function initializeController() {

            $scope.scopeModel = {};
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    context = payload.context;
                    if (payload.actionType != undefined) {
                        $scope.scopeModel.to = payload.actionType.To;
                        $scope.scopeModel.subject = payload.actionType.Subject;
                        $scope.scopeModel.body = payload.actionType.Body;
                    }
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.SendEmailActionType, Vanrise.Analytic.MainExtensions",
                    To: $scope.scopeModel.to,
                    Subject: $scope.scopeModel.subject,
                    Body: $scope.scopeModel.body,
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