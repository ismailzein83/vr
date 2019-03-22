"use strict";

app.directive("businessprocessGenerictasktypeActionsettingsExecute", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var generictasktypeActionExecute = new GenerictasktypeActionExecute($scope, ctrl, $attrs);
                generictasktypeActionExecute.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: ''
        };

        function GenerictasktypeActionExecute($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.ExecuteBPGenericTaskTypeAction, Vanrise.BusinessProcess.MainExtensions"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);