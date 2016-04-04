﻿(function (app) {

    "use strict";

    vrCellviewer.$inject = ['BaseDirService', 'VRValidationService'];

    function vrCellviewer(BaseDirService, VRValidationService) {

        return {
            restrict: 'E',
            scope: {
                value: '=',
                hint: '@',
                onSelect: "=",
                onUpdate:"=",
                placeholder: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.validate = function () {
                    return VRValidationService.validate(ctrl.value, $scope, $attrs);
                };
            },
            compile: function (element, attrs) {

                
                return {
                    pre: function ($scope, iElem, iAttrs) {
                        var ctrl = $scope.ctrl;

                        var isUserChange;
                        $scope.$watch('ctrl.value', function (newValue, oldValue) {

                            if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                                return;

                            if (newValue == "") {
                                ctrl.value = undefined;
                            }
                            isUserChange = false;//reset the flag

                            if (iAttrs.onvaluechanged != undefined) {
                                var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onvaluechanged);
                                if (onvaluechangedMethod != undefined && typeof (onvaluechangedMethod) == 'function') {
                                    onvaluechangedMethod();
                                }
                            }
                        });
                        $scope.selectCell = function () {
                            var a = parseInt(ctrl.value.row);
                            var b = parseInt(ctrl.value.col);
                            var s = parseInt(ctrl.value.sheet);

                            if (ctrl.onSelect != null)
                                ctrl.onSelect(a,b,s);                       

                        }

                        $scope.updateRange = function (r,c,s) {
                            ctrl.value = {
                                row: r,
                                col: c,
                                sheet:s
                            }
                            if (ctrl.onUpdate != null)
                                ctrl.onUpdate(r, c, s);

                        }
                        ctrl.remove = function () {
                            ctrl.value = null;
                        }
                        ctrl.notifyUserChange = function () {
                            isUserChange = true;
                        };
                        ctrl.readOnly = attrs.readonly != undefined;
                        ctrl.placelHolder = (attrs.placeholder != undefined) ? ctrl.placeholder : '';

                        if (attrs.hint != undefined) {
                            ctrl.hint = attrs.hint;
                        }
                        ctrl.getInputeStyle = function () {
                            return (attrs.hint != undefined) ? {
                                "display": "inline-block",
                                "width": "calc(100% - 15px)",
                                "margin-right": "1px"
                            } : {};
                        }
                        ctrl.adjustTooltipPosition = function (e) {
                            setTimeout(function () {
                                var self = angular.element(e.currentTarget);
                                var selfHeight = $(self).height();
                                var selfOffset = $(self).offset();
                                var tooltip = self.parent().find('.tooltip-info')[0];
                                $(tooltip).css({ display: 'block' });
                                var innerTooltip = self.parent().find('.tooltip-inner')[0];
                                var innerTooltipArrow = self.parent().find('.tooltip-arrow')[0];
                                $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 5, left: selfOffset.left - 30 });
                                $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight, left: selfOffset.left });

                            }, 1)
                        }


                        //BaseDirService.addScopeValidationMethods(ctrl, elementName, formCtrl);

                    }
                }
            },

            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                var startTemplate = '<div id="rootDiv" style="position: relative;">';
                var endTemplate = '</div>';

                var labelTemplate = '';
                if (attrs.label != undefined)
                    labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
                var rows = 3
                if (attrs.rows != undefined)
                    rows = attrs.rows;
                var textboxTemplate = '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false" ng-style="ctrl.getInputeStyle()">'
                        + '<vr-validator validate="ctrl.validate()">'
                        + '<div    id="mainInput" ng-model="ctrl.value" style="border-radius: 4px; padding: 0px; width: 100%; border: 0px;">'
                        + ' <span class="glyphicon glyphicon-circle-arrow-right" style="font-size: 21px;top: 3px;" ng-click="updateRange()"></span>'
                        + '<a ng-show="ctrl.value !=null && ctrl.value !=undefined " class="hand-cursor" style="display: inline-block; width: calc(100% - 45px); position: relative; top: -2px;" ng-click="selectCell()">Row{{ctrl.value.row}};Col{{ctrl.value.col}}</a>'
                        + '<span ng-show="ctrl.value !=null" class="glyphicon glyphicon-remove hand-cursor" style="top: 0px;" aria-hidden="true" ng-click="ctrl.remove()"></span>'
                        + '</div>'
                        + '</vr-validator>'
                    + '</div>'
                   + '<span ng-if="ctrl.hint!=undefined" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor" html="true" style="color:#337AB7"  placement="bottom"  trigger="hover" ng-mouseenter="ctrl.adjustTooltipPosition($event)"  data-type="info" data-title="{{ctrl.hint}}"></span>';

                return startTemplate + labelTemplate + textboxTemplate + endTemplate;
            }

        };

    }

    app.directive('vrCellviewer', vrCellviewer);

})(app);



