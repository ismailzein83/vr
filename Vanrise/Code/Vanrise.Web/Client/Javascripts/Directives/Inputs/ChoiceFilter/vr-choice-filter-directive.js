﻿'use strict';


app.directive('vrChoiceFilter', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        transclude: true,
        scope: {
            isselected: '=?',
            onselectionchanged: '='
        },
        require: '^vrChoicesFilter',
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            $element.bind("$destroy", function () {
                ctrl.removeTab(ctrl);
            });

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, choicesCtrl) {
                    var ctrl = $scope.ctrl;

                    ctrl.selectionChanged = function () {
                        ctrl.isselected = ctrl.isSelected;
                    };
                    ctrl.removeTab = function (ctrl) {
                        choicesCtrl.removeTab(ctrl);
                    };
                    ctrl.getTabStyle = function () {
                        return choicesCtrl.getTabStyle();
                    };
                    choicesCtrl.addChoiceCtrl(ctrl);

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
                    }
                    $scope.$watch("ctrl.isselected", function (value) {
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
                },
                post: function (scope, element, attrs) {
                    var ctrl = scope.ctrl;
                    var span = $(element.children().first().find("span"));
                    setTimeout(function () {
                        ctrl.hint = $(span).html();
                    },1)
                }

           
            }
        },
        template: '<label class="hand-cursor" ng-style="ctrl.getTabStyle()" ng-mouseenter="ctrl.adjustTooltipPosition($event)" ng-class="ctrl.isSelected?\'clicked-btn\':\'\'"  ng-click="ctrl.choiceClicked()" ng-transclude bs-tooltip placement="bottom" data-title="{{ctrl.hint}}"></label>'
    };

    return directiveDefinitionObject;



}]);

