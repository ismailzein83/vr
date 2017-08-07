"use strict";

app.directive("vrRulesNormalizationnumbersettingsRemove", ['UtilsService', 'VR_Rules_RemoveDirectionEnum', 'VR_Rules_TextOccurrenceEnum',
    function (UtilsService, VR_Rules_RemoveDirectionEnum, VR_Rules_TextOccurrenceEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DirectiveConstructor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/VR_Rules/Directives/NormalizationRule/Templates/RemoveDirectiveTemplate.html'
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var removeDirection;
                    var textOccurrence;

                    if (payload != undefined) {
                        removeDirection = payload.RemoveDirection;
                        textOccurrence = payload.TextOccurrence;

                        $scope.scopeModel.textToRemove = payload.TextToRemove;
                        $scope.scopeModel.includingText = payload.IncludingText;
                    }

                    $scope.scopeModel.removeDirections = UtilsService.getArrayEnum(VR_Rules_RemoveDirectionEnum);
                    $scope.scopeModel.selectedRemoveDirection = UtilsService.getItemByVal($scope.scopeModel.removeDirections, removeDirection, 'value');

                    $scope.scopeModel.textOccurrences = UtilsService.getArrayEnum(VR_Rules_TextOccurrenceEnum);
                    $scope.scopeModel.selectedTextOccurrence = UtilsService.getItemByVal($scope.scopeModel.textOccurrences, textOccurrence, 'value');
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Rules.Normalization.MainExtensions.RemoveActionSettings, Vanrise.Rules.Normalization",
                        RemoveDirection: $scope.scopeModel.selectedRemoveDirection,
                        TextToRemove: $scope.scopeModel.textToRemove,
                        TextOccurrence: $scope.scopeModel.selectedTextOccurrence,
                        IncludingText: $scope.scopeModel.includingText
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
