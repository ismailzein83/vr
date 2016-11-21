'use strict';


app.directive('vrChoicesFilter', ['UtilsService', '$rootScope', function (UtilsService, $rootScope) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            selectedindex: '=?',
            onselectionchanged: '&'

        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var choiceCtrls = [];
           
            ctrl.addChoiceCtrl = function (choiceCtrl) {
                choiceCtrls.push(choiceCtrl);
                setDefaultChoiceSeletion();
            };
            ctrl.removeTab = function (tabCtrl) {
                choiceCtrls.splice(choiceCtrls.indexOf(tabCtrl), 1);
                UtilsService.safeApply($scope, function () { });
            };

            ctrl.getTabStyle = function (ctrl) {
                var m = 1;
                if (choiceCtrls.indexOf(ctrl) == choiceCtrls.length - 1)
                    m = 0;
                return { 'width': 'calc(' + 100 / choiceCtrls.length + '% - ' + m + 'px )', 'display': 'inline-block !important', 'max-width': '150px', 'vertical-align': 'top' };
            };
            var triggerSelectionChanged = false;
            ctrl.selectChoice = function (choiceCtrl) {
                triggerSelectionChanged = true;
                angular.forEach(choiceCtrls, function (t) {
                    if (t != choiceCtrl)
                        setChoiceSelection(t, false);
                });
                setChoiceSelection(choiceCtrl, true);
            };

            ctrl.unselectChoice = function (choiceCtrl) {
                setChoiceSelection(choiceCtrl, false);
                setDefaultChoiceSeletion();
            };
            ctrl.isradio = $attrs.isradio != undefined;
            function setDefaultChoiceSeletion() {
                if (choiceCtrls.length == 0 || ctrl.selectedindex > -1)
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
                    if (isSelected == true)
                        ctrl.selectedindex = choiceCtrls.indexOf(choiceCtrl);                        
                }
            }

            $scope.$watch("ctrl.selectedindex", function (value) {
                if (choiceCtrls[ctrl.selectedindex] != undefined && !choiceCtrls[ctrl.selectedindex].isSelected)
                    ctrl.selectChoice(choiceCtrls[ctrl.selectedindex]);
                else {
                    if (ctrl.onselectionchanged && typeof (ctrl.onselectionchanged) == 'function') {
                        if (triggerSelectionChanged == true)
                            ctrl.onselectionchanged();
                    }
                    triggerSelectionChanged = false;
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
            //var radioclass = (attrs.isradio != undefined) ? " radio-btn-groupe" : "";
            element.html('<div class="btn-group btn-group-custom vr-tabs vr-choices-filter" >' + element.html() + '</div>');

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        }

    };

    return directiveDefinitionObject;



}]);

