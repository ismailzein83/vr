﻿"use strict";

app.directive("bpTaskTestEditor", ["UtilsService",
function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPTask/Templates/BPTaskTestEditor.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        this.initializeController = initializeController;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.getData = function (bpTaskId) {
                var comment = {
                    $type: "Vanrise.BusinessProcess.Entities.BPTaskComment, Vanrise.BusinessProcess.Entities",
                    Comment: $scope.comments
                };

                var input = {
                    $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
                    TaskId: bpTaskId,
                    Comment: comment
                };
                return input;
            };

            api.load = function (payload) {
                var promises = [];
                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
