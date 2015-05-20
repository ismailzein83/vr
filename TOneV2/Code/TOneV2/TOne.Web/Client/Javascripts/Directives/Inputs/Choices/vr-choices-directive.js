'use strict';


app.directive('vrChoices', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            selectedindex: '=?',
            onselectionchanged: '='

        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var choiceCtrls = [];

            ctrl.addChoiceCtrl = function (choiceCtrl) {
                choiceCtrls.push(choiceCtrl);
                setDefaultChoiceSeletion();
            }

            ctrl.selectChoice = function (choiceCtrl) {
                angular.forEach(choiceCtrls, function (t) {
                    if (t != choiceCtrl)
                        setChoiceSelection(t, false);
                });
                setChoiceSelection(choiceCtrl, true);
            }

            ctrl.unselectChoice = function (choiceCtrl) {
                setChoiceSelection(choiceCtrl, false);
                setDefaultChoiceSeletion();
            }

            function setDefaultChoiceSeletion() {
                if (choiceCtrls.length == 0)
                    return;
                var isAnySelected = false;
                angular.forEach(choiceCtrls, function (t) {
                    if (t.isSelected)
                        isAnySelected = true;
                });
                if (!isAnySelected)
                    setChoiceSelection(choiceCtrls[0], true);
            }

            function setChoiceSelection(choiceCtrl, isSelected) {
                if (choiceCtrl.isSelected != isSelected) {
                    choiceCtrl.isSelected = isSelected;
                    choiceCtrl.selectionChanged();
                    if (isSelected)
                        ctrl.selectedindex = choiceCtrls.indexOf(choiceCtrl);
                }
            }

            $scope.$watch("ctrl.selectedindex", function () {
                if (ctrl.onselectionchanged && typeof (ctrl.onselectionchanged) == 'function') {
                    ctrl.onselectionchanged();
                }
            });

            var api = {};
            api.selectChoice = function (choiceIndex) {
                var choiceCtrl = choiceCtrls[choiceIndex];
                if (choiceCtrl != undefined)
                    ctrl.selectChoice(choiceCtrl);
            };
            api.unselectChoice = function (choiceIndex) {
                var choiceCtrl = choiceCtrls[choiceIndex];
                if (choiceCtrl != undefined)
                    ctrl.unselectChoice(choiceCtrl);
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            element.html('<div class="btn-group btn-group-custom" role="group" >' + element.html() + '</div>');

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                   
                }
            }
        }

    };

    return directiveDefinitionObject;



}]);

