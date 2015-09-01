(function (app) {

    "use strict";

    vrNumeric.$inject = ['BaseDirService'];

    function vrNumeric(BaseDirService) {

        return {
            restrict: 'E',
            require: '^form',
            scope: {
                value: '=',
                hint: '@',
                minvalue:'@',
                maxvalue: '@',
                unit: '@',
                stepvalue: '@',
                upcolor: '@',
                downcolor:'@',
                customvalidate: '&'
            },
            controller: function ($scope, $element) {

            },
            compile: function (element, attrs) {

                var inputElement = element.find('#mainInput');
                var validationOptions = {};
                if (attrs.isrequired !== undefined)
                    validationOptions.requiredValue = true;
                if (attrs.customvalidate !== undefined)
                    validationOptions.customValidation = true;
                var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, element.find('#rootDiv'));
                return {
                    pre: function ($scope, iElem, iAttrs, formCtrl) {
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
                                if (arr.indexOf(".") > -1)
                                    ctrl.value = oldValue;
                                if (arr.length === 0) return;
                                if (arr.length === 1 && (arr[0] == '-')) return;
                                if (ctrl.maxValue != undefined && parseFloat(newValue) > ctrl.maxValue) {
                                    ctrl.value = oldValue;
                                }
                                if (ctrl.minValue != undefined && parseFloat(newValue) < ctrl.minValue) {
                                    ctrl.value = oldValue;
                                }
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
                        ctrl.readOnly = attrs.readonly != undefined;
                        ctrl.upColor = (attrs.upcolor != undefined) ? ctrl.upcolor : 'green';
                        ctrl.downColor = (attrs.downcolor != undefined) ? ctrl.downcolor : 'red';
                        ctrl.minValue = (attrs.minvalue != undefined) ? parseInt(ctrl.minvalue) : undefined;
                        ctrl.maxValue = (attrs.maxvalue != undefined) ? parseInt(ctrl.maxvalue) : undefined;
                        ctrl.stepValue = (attrs.stepvalue != undefined) ? parseInt(ctrl.stepvalue) : 1;
                        ctrl.unitValue = (attrs.unit != undefined) ? ctrl.unit : '';
                        ctrl.increment = function () {
                            var avrege;
                            if (isNaN(parseFloat(ctrl.value))) {
                                if (ctrl.minValue == undefined || ctrl.maxValue == undefined)
                                    avrege = parseInt( 0 );
                                else
                                    avrege = (ctrl.maxValue + ctrl.minValue) / 2 + (((ctrl.maxValue + ctrl.minValue) / 2 % ctrl.stepValue));
                                ctrl.value = avrege ;
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
                                if (ctrl.minValue == undefined || ctrl.maxValue == undefined)
                                    avrege = 0;
                                else
                                    avrege = (ctrl.maxValue + ctrl.minValue) / 2 + (((ctrl.maxValue + ctrl.minValue) / 2 % ctrl.stepValue));
                                ctrl.value = avrege;
                                ctrl.notifyUserChange();

                                return;
                            }
                            var newvalue = parseFloat(ctrl.value) - ctrl.stepValue;
                            if (ctrl.minValue != undefined &&  newvalue < ctrl.minValue)
                                return;
                            else
                                ctrl.value = newvalue;


                        };
                        if (attrs.hint != undefined) {
                            ctrl.hint = attrs.hint;
                        }
                        ctrl.getInputeStyle = function () {
                            return  (attrs.hint != undefined) ? {
                                "display": "inline-block",
                                "width": "calc(100% - 15px)",
                                "margin-right": "1px"
                            } :{} ;
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
                                $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight +5, left: selfOffset.left - 30 });
                                $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight , left: selfOffset.left  });

                            }, 1)
                        }
                        ctrl.getNumericControlClass = function () {
                            var classes = '';
                            if (attrs.hint != undefined)
                                classes += ' with-hint ';
                            if(attrs.label != undefined)
                                classes += ' with-label ';
                            return classes;
                        }
                        BaseDirService.addScopeValidationMethods(ctrl, elementName, formCtrl);

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
                var numericTemplate = '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false">'
                                            + '<div  class="vr-numeric" ng-style="ctrl.getInputeStyle()">'
                                                   + '<input class="form-control  border-radius input-box" type="text" ng-change="ctrl.notifyUserChange()" id="mainInput" ng-model="ctrl.value" >'
                                                    + '<div class="vr-numeric-control" ng-class="ctrl.getNumericControlClass()">'
                                                        + '<span class="unit" ng-bind="ctrl.unitValue"></span>'
                                                        + '<div class="hand-cursor arrow-box" ng-click="ctrl.increment()" ng-style="{\'color\':ctrl.upColor}">'
                                                        + '<div class="caret-up" ></div>'
                                                        + '</div>'
                                                        + '<div class="hand-cursor arrow-box " ng-click="ctrl.decrement()" ng-style="{\'color\':ctrl.downColor}">'
                                                        + '<div class="caret-down"></div>'
                                                        + '</div>'
                                                    + '</div>'
                                             + '</div>'
                                        + '<span ng-if="ctrl.hint!=undefined"  bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor" html="true" style="color:#337AB7;right: -10px;"  placement="bottom"  trigger="hover" ng-mouseenter="ctrl.adjustTooltipPosition($event)"  data-type="info" data-title="{{ctrl.hint}}"></span>'
                                    + '</div>';  
                     var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined, true);

                    return startTemplate + labelTemplate + numericTemplate + validationTemplate + endTemplate;
            }

        };

    }

    app.directive('vrNumeric', vrNumeric);

    
})(app);



