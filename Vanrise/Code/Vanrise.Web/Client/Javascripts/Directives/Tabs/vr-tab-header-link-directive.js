'use strict';


app.directive('vrTabHeaderLink', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        transclude: true,
        scope: {
            isselected: '=?',
            onselectionchanged: '=',
            isvisible:"="
        },
        require: '^vrTabHeaderLinks',
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            $element.bind("$destroy", function () {
               ctrl.removeTab(ctrl);
            });


        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

            //element.html(' <label class="hand-cursor" ng-class="ctrl.isselected?\'clicked-btn\':\'\'">{{ctrl.isselected}}' + element.html() + '</label><label>|</label>');
            return {
                pre: function ($scope, iElem, iAttrs, choicesCtrl) {
                    $scope.$on("$destroy", function () {
                        selectedWatch();
                    });
                    var ctrl = $scope.ctrl;

                    ctrl.selectionChanged = function () {
                        ctrl.isselected = ctrl.isSelected;
                    };
                    choicesCtrl.addChoiceCtrl(ctrl);

                    ctrl.choiceClicked = function () {
                        choicesCtrl.selectChoice(ctrl);
                    };
                    ctrl.removeTab = function (ctrl) {
                        choicesCtrl.removeTab(ctrl);
                    };
                    ctrl.getTabStyle = function () {
                        return choicesCtrl.getTabStyle(ctrl);
                    };
                    ctrl.getOuterStyle = function () {
                        return choicesCtrl.getOuterStyle();
                    };
                    ctrl.choiceClicked = function () {
                        choicesCtrl.selectChoice(ctrl);
                    };
                    ctrl.adjustTooltipPosition = function (e) {
                        setTimeout(function () {
                            var self = angular.element(e.currentTarget);
                            var selfHeight = $(self).height();
                            var selfOffset = $(self).offset();
                            var tooltip = self.parent().find('.tooltip-info')[0];
                            $(tooltip).css({ display: 'block' });
                            var innerTooltip = self.parent().find('.tooltip-inner')[0];
                            var innerTooltipArrow = self.parent().find('.tooltip-arrow')[0];
                            var innerTooltipWidth = parseFloat(($(innerTooltip).width() / 2) + 2.5);
                            $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 15, left: selfOffset.left - innerTooltipWidth - $(window).scrollLeft(), backgroundColor: "#2f4f4f" });
                            $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 10, left: selfOffset.left - $(window).scrollLeft(), bordeBottomColor: "#2f4f4f" });

                        }, 1);
                    };

                    var selectedWatch = $scope.$watch("ctrl.isselected", function (value) {
                        if (ctrl.isSelected != value) {
                            if (value)
                                choicesCtrl.selectChoice(ctrl);
                            else
                                choicesCtrl.unselectChoice(ctrl);
                        }
                        if (ctrl.onselectionchanged && typeof (ctrl.onselectionchanged) == 'function') {
                            ctrl.onselectionchanged();
                        }
                    });
                    var selectedWatch = $scope.$watch("ctrl.isvisible", function (value) {
                        if (value == false && ctrl.isselected == true) {
                            ctrl.isSelected = false;
                            ctrl.isselected = false;
                            choicesCtrl.selectedindex = -1;
                            choicesCtrl.setDefaultChoiceSeletion();
                        }
                    });
                },
                post: function (scope, element, attrs) {
                    var ctrl = scope.ctrl;
                    var span = $(element.children().first().find("span"));
                    setTimeout(function () {
                        ctrl.hint = $(span).html();
                    }, 1);
                }

            }
        },
        template: '<span style="position: relative;" ng-style="ctrl.getOuterStyle()"><label class="hand-cursor" ng-style="ctrl.getTabStyle()"   ng-class="ctrl.isSelected?\'clicked-btn\':\'\'"  ng-click="ctrl.choiceClicked()" ng-transclude  title="{{ctrl.hint}}"></label>'
    };

    return directiveDefinitionObject;



}]);

