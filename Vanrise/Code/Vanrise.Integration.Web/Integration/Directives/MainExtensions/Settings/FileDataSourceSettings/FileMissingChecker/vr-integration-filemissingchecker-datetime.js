"use strict";

app.directive("vrIntegrationFilemissingcheckerDatetime", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DateTimeCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Integration/Directives/MainExtensions/Settings/FileDataSourceSettings/FileMissingChecker/Templates/DateTimeFileMissingCheckerTemplate.html"
        };

        function DateTimeCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Integration.MainExtensions.FileMissingChecker.DateTimeFileMissingChecker, Vanrise.Integration.MainExtensions"
                    };
                };

                if (ctrl.onReady != null && typeof ctrl.onReady == "function") {
                    ctrl.onReady(api);
                }
            }
        }

        return directiveDefinitionObject;
    }
]);