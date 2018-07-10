"use strict";

app.directive("vrAnalyticAutomatedreportfilenamepartDate", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this; 

                var ctor = new AutomatedReportDateFileNamePart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Handler/Templates/AutomatedReportDateFileNamePartTemplate.html"

        };

        function AutomatedReportDateFileNamePart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.concatenatedPartSettings != undefined) {
                            $scope.scopeModel.dateFormat = payload.concatenatedPartSettings.DateFormat;
                        }
                        var promises = [];
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.VRAutomatedReportDateFileNamePart, Vanrise.Analytic.MainExtensions",
                        DateFormat: $scope.scopeModel.dateFormat,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);