(function (app) {

    "use strict";

    vrNumeric.$inject = ['BaseDirService', 'VRValidationService', 'UtilsService','VRLocalizationService'];

    function vrNumeric(BaseDirService, VRValidationService, UtilsService, VRLocalizationService) {

        return {
            restrict: 'E',
            scope: {
                value: '=',
                hint: '@',
                minvalue:'@',
                maxvalue: '@',
                unit: '@',
                stepvalue: '@',
                upsign: '@',
                downsign: '@',
                placeholder: '@'

            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.readOnly = UtilsService.isContextReadOnly($scope) || $attrs.readonly != undefined;

                ctrl.validate = function () {
                    if (ctrl.maxValue != undefined && parseFloat(ctrl.value) > ctrl.maxValue) {
                        return "value should be less than or equal to " + ctrl.maxValue;
                    }
                    if (ctrl.minValue != undefined && parseFloat(ctrl.value) < ctrl.minValue) {
                        return "value should be greater than or equal to " + ctrl.minValue;
                    }
                    return VRValidationService.validate(ctrl.value, $scope, $attrs);
                };

                var getInputeStyle = function () {
                    var div = $element.find('div[validator-section]')[0];
                    if ($attrs.hint != undefined) {
                        $(div).css({ "display": "inline-block", "width": "calc(100% - 15px)", "margin-right": "1px" });
                    }
                }();
            },
            compile: function (element, attrs) {

                //var inputElement = element.find('#mainInput');
                //var validationOptions = {};
                //if (attrs.isrequired !== undefined)
                //    validationOptions.requiredValue = true;
                //if (attrs.customvalidate !== undefined)
                //    validationOptions.customValidation = true;
                //var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, element.find('#rootDiv'));
                return {
                    pre: function ($scope, iElem, iAttrs) {
                        var ctrl = $scope.ctrl;
                        var isUserChange;
                        $scope.$watch('ctrl.value', function (newValue, oldValue) {
                            if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                                return;
                            isUserChange = false;//reset the flag
                            var arr = String(newValue).split("");
                            var negativeArray = String(newValue).split("-");
                            if (arr.length === 0) return;
                            if (negativeArray.length > 2)
                                ctrl.value = oldValue;
                            if (negativeArray.length > 1 && ctrl.minValue != undefined && ctrl.minValue >= 0)
                                ctrl.value = oldValue;
                            if (arr.indexOf(".") > -1)
                                ctrl.value = oldValue;
                            if (arr.length === 0) return;
                            if (arr.length === 1 && (arr[0] == '-')) return;
                            //if (ctrl.maxValue != undefined && parseFloat(newValue) > ctrl.maxValue) {
                            //    ctrl.value = oldValue;
                            //}
                            //if (ctrl.minValue != undefined && parseFloat(newValue) < ctrl.minValue) {
                            //    ctrl.value = oldValue;
                            //}
                            if (isNaN(newValue)) {
                                ctrl.value = oldValue;
                            }

                            if (iAttrs.onvaluechanged != undefined) {
                                var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onvaluechanged);
                                if (onvaluechangedMethod != undefined && typeof (onvaluechangedMethod) == 'function') {
                                    onvaluechangedMethod();
                                }
                            }
                        });

                        ctrl.notifyUserChange = function () {
                            isUserChange = true;
                        };
                        var defaultcolor = '#666';
                        ctrl.upColor = defaultcolor;
                        ctrl.downColor = defaultcolor;
                        if (attrs.upsign != undefined) {
                            if (ctrl.upsign == "critical")
                                ctrl.upColor = '#ff0000';
                            else if (ctrl.upsign == "safe")
                                ctrl.upColor = '#009900';
                            else
                                ctrl.upColor = defaultcolor;

                        }
                        if (attrs.downsign != undefined) {
                            if (ctrl.downsign == "critical")
                                ctrl.downColor = '#ff0000';
                            else if (ctrl.downsign == "safe")
                                ctrl.downColor = '#009900';
                            else
                                ctrl.downColor = defaultcolor;

                        }
                        ctrl.minValue = (attrs.minvalue != undefined) ? parseInt(ctrl.minvalue) : undefined;
                        ctrl.maxValue = (attrs.maxvalue != undefined) ? parseInt(ctrl.maxvalue) : undefined;
                        ctrl.stepValue = (attrs.stepvalue != undefined) ? parseInt(ctrl.stepvalue) : 1;
                        ctrl.unitValue = (attrs.unit != undefined) ? ctrl.unit : '';
                        ctrl.placelHolder = (attrs.placeholder != undefined) ? ctrl.placeholder : '';
                        ctrl.increment = function () {
                            var avrege;
                            if (isNaN(parseFloat(ctrl.value))) {
                                if (ctrl.maxValue != undefined) {
                                    avrege = ctrl.maxValue;
                                }
                                if (ctrl.minValue != undefined)
                                    avrege = ctrl.minValue;

                                if (ctrl.minValue == undefined && ctrl.maxValue == undefined)
                                    avrege = parseInt(0);

                                if (ctrl.minValue != undefined && ctrl.maxValue != undefined)
                                    avrege = (ctrl.maxValue + ctrl.minValue) / 2 + (((ctrl.maxValue + ctrl.minValue) / 2 % ctrl.stepValue));

                                ctrl.value = avrege;
                                ctrl.notifyUserChange();

                                return;
                            }
                            var newvalue = parseFloat(ctrl.value) + ctrl.stepValue;
                            if (ctrl.maxValue != undefined && newvalue > ctrl.maxValue)
                                return;
                            else
                                ctrl.value = newvalue;
                        };
                        ctrl.decrement = function () {
                            var avrege;
                            if (isNaN(parseFloat(ctrl.value))) {
                                if (ctrl.maxValue != undefined) {
                                    avrege = ctrl.maxValue;
                                }
                                if (ctrl.minValue != undefined)
                                    avrege = ctrl.minValue;

                                if (ctrl.minValue == undefined && ctrl.maxValue == undefined)
                                    avrege = parseInt(0);

                                if (ctrl.minValue != undefined && ctrl.maxValue != undefined)
                                    avrege = (ctrl.maxValue + ctrl.minValue) / 2 + (((ctrl.maxValue + ctrl.minValue) / 2 % ctrl.stepValue));

                                ctrl.value = avrege;
                                ctrl.notifyUserChange();

                                return;
                            }
                            var newvalue = parseFloat(ctrl.value) - ctrl.stepValue;
                            if (ctrl.minValue != undefined && newvalue < ctrl.minValue)
                                return;
                            else
                                ctrl.value = newvalue;


                        };
                        if (attrs.hint != undefined) {
                            ctrl.hint = attrs.hint;
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
                                var innerTooltipWidth = parseFloat(($(innerTooltip).width() / 2) + 2.5);
                                $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 15, left: selfOffset.left - innerTooltipWidth });
                                $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 10, left: selfOffset.left });

                            }, 1);
                        };

                        ctrl.tabindex = "";
                        setTimeout(function () {
                            if ($(element).hasClass('divDisabled') || $(element).parents('.divDisabled').length > 0) {
                                ctrl.tabindex = "-1"
                            }
                        }, 10);
                        //BaseDirService.addScopeValidationMethods(ctrl, elementName, formCtrl);

                    }
                };
            },

            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                var startTemplate = '<div id="rootDiv" style="position: relative;">';
                var endTemplate = '</div>';
                var labelTemplate = '';
                var label = VRLocalizationService.getResourceValue(attrs.localizedlabel, attrs.label);

                if (label != undefined)
                {
                    labelTemplate = '<vr-label>' + label + '</vr-label>';
                }
                var numericTemplate = '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false">'
                                            + '<div  class="vr-numeric" >'
                                                + '<vr-validator validate="ctrl.validate()" vr-input>'
                                                        +'<div style="position: relative;">'
                                                            + '<input ng-readonly="ctrl.readOnly" tabindex="{{ctrl.tabindex}}" class="form-control  border-radius input-box" type="text" placeholder="{{ctrl.placelHolder}}" ng-change="ctrl.notifyUserChange()" id="mainInput" ng-model="ctrl.value" />'
                                                            + '<div class="vr-numeric-control" ng-if="!ctrl.readOnly">'
                                                            + '<span class="unit" ng-bind="ctrl.unitValue"></span>'
                                                            + '<div class="hand-cursor arrow-box" ng-click="ctrl.increment()" ng-style="{\'color\':ctrl.upColor}">'
                                                                + '<div class="caret-up" ></div>'
                                                            + '</div>'
                                                            + '<div class="hand-cursor arrow-box " ng-click="ctrl.decrement()" ng-style="{\'color\':ctrl.downColor}">'
                                                                + '<div class="caret-down"></div>'
                                                            + '</div>'
                                                        + '</div>'
                                                + '</div>'
                                               + '</vr-validator>'
                                                + '<span ng-if="ctrl.hint!=undefined"  bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor vr-hint-input" style="right: 1px;left: 1px;" html="true"  placement="bottom"  trigger="hover" ng-mouseenter="ctrl.adjustTooltipPosition($event)"  data-type="info" data-title="{{ctrl.hint}}"></span>'
                                             + '</div>'
                                    + '</div>';  
                     //var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined, true);

                    return startTemplate + labelTemplate + numericTemplate + endTemplate;
            }

        };

    }

    app.directive('vrNumeric', vrNumeric);

    
})(app);



