﻿'use strict';


app.directive('vrChoice', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        transclude: true,
        scope: {
            isselected: '=?',
            onselectionchanged: '='
        },
        require: '^vrChoices',
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

            //element.html(' <label class="hand-cursor" ng-class="ctrl.isselected?\'clicked-btn\':\'\'">{{ctrl.isselected}}' + element.html() + '</label><label>|</label>');
            return {
                pre: function ($scope, iElem, iAttrs, choicesCtrl) {
                    var ctrl = $scope.ctrl;

                    ctrl.selectionChanged = function () {
                        ctrl.isselected = ctrl.isSelected;
                    };

                    choicesCtrl.addChoiceCtrl(ctrl);

                    ctrl.choiceClicked = function () {
                        choicesCtrl.selectChoice(ctrl);
                    };

                    $scope.$watch("ctrl.isselected", function () {
                        if (ctrl.onselectionchanged && typeof (ctrl.onselectionchanged) == 'function') {
                            ctrl.onselectionchanged();
                        }
                    });
                }
            }
        },
        template: '<label class="hand-cursor" ng-class="ctrl.isSelected?\'clicked-btn\':\'\'" ng-click="ctrl.choiceClicked()" ng-transclude></label>'
    };

    return directiveDefinitionObject;



}]);

