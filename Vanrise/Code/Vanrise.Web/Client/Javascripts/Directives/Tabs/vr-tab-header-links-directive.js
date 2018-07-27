'use strict';


app.directive('vrTabHeaderLinks', ['UtilsService', function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            selectedindex: '=?',
            onselectionchanged: '&'

        },
        controller: function ($scope, $element, $attrs) {
            $scope.$on("$destroy", function () {
                selectedindexWatch();
            });
            var ctrl = this;

            var choiceCtrls = [];

            ctrl.addChoiceCtrl = function (choiceCtrl) {
                choiceCtrls.push(choiceCtrl);
                setDefaultChoiceSeletion();
            };
            ctrl.removeTab = function (tabCtrl) {
                choiceCtrls.splice(choiceCtrls.indexOf(tabCtrl), 1);
                setTimeout(function () {
                    UtilsService.safeApply($scope);
                }, 1)
            };

            ctrl.getTabStyle = function (ctrl) {
                if ($attrs.vertical != undefined)
                    return { 'display': 'block', 'margin-bottom': '0px', 'border-radius': '0px' };
                var m = 1;
                if (choiceCtrls.indexOf(ctrl) == choiceCtrls.length - 1)
                    m = 0;

                return { 'width': 'calc(' + 100 / choiceCtrls.length + '% - ' + m + 'px )', 'display': 'inline-block !important', 'max-width': '150px', 'vertical-align': 'top' };



            };

            ctrl.getOuterStyle = function () {
                if ($attrs.vertical != undefined)
                    return { 'display': 'block' };
                return {};
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
            var setDefaultChoiceSeletion = function () {
                if (choiceCtrls.length == 0 || ctrl.selectedindex > -1)
                    return;
                var isAnySelected = false;
                var firstVisibleChoiceIndex = -1;
                for (var i = 0 ; i < choiceCtrls.length ; i++) {
                    var t = choiceCtrls[i];
                    if (t.isSelected)
                        isAnySelected = true;
                    if (firstVisibleChoiceIndex == -1 && t.isvisible == true)
                        firstVisibleChoiceIndex = i;
                }
                if (!isAnySelected && firstVisibleChoiceIndex != -1)
                    setChoiceSelection(choiceCtrls[firstVisibleChoiceIndex], true);
            };
            ctrl.setDefaultChoiceSeletion = setDefaultChoiceSeletion;


            function setChoiceSelection(choiceCtrl, isSelected) {
                if (choiceCtrl.isSelected != isSelected) {
                    choiceCtrl.isSelected = isSelected;
                    choiceCtrl.selectionChanged();
                    if (isSelected == true)
                        ctrl.selectedindex = choiceCtrls.indexOf(choiceCtrl);
                }
            }

            var selectedindexWatch = $scope.$watch("ctrl.selectedindex", function (value) {
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
            var verticalflag = attrs.vertical != undefined ? "vertical" : " ";
            element.html('<div class="btn-group btn-group-custom vr-tabs"  ' + verticalflag + ' >' + element.html() + '</div>');

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        }

    };

    return directiveDefinitionObject;



}]);

