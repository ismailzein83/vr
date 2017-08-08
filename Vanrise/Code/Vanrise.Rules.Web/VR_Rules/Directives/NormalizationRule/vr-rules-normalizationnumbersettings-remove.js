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
                    else {
                        $scope.scopeModel.includingText = true;
                    }

                    $scope.scopeModel.removeDirections = UtilsService.getArrayEnum(VR_Rules_RemoveDirectionEnum);
                    $scope.scopeModel.selectedRemoveDirection = removeDirection != undefined ? UtilsService.getItemByVal($scope.scopeModel.removeDirections, removeDirection, 'value') : $scope.scopeModel.removeDirections[0];

                    $scope.scopeModel.textOccurrences = UtilsService.getArrayEnum(VR_Rules_TextOccurrenceEnum);
                    $scope.scopeModel.selectedTextOccurrence = textOccurrence != undefined ? UtilsService.getItemByVal($scope.scopeModel.textOccurrences, textOccurrence, 'value') : $scope.scopeModel.textOccurrences[0];
                };

                api.getData = function () {

                    return {
                        $type: "Vanrise.Rules.Normalization.MainExtensions.RemoveActionSettings, Vanrise.Rules.Normalization",
                        RemoveDirection: $scope.scopeModel.selectedRemoveDirection.value,
                        TextToRemove: $scope.scopeModel.textToRemove,
                        TextOccurrence: $scope.scopeModel.selectedTextOccurrence.value,
                        IncludingText: $scope.scopeModel.includingText
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
