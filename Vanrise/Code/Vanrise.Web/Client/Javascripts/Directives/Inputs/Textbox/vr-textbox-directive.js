(function (app) {

    "use strict";

    vrTextbox.$inject = ['BaseDirService', 'TextboxTypeEnum', 'VRValidationService'];

    function vrTextbox(BaseDirService, TextboxTypeEnum, VRValidationService) {

        return {
            restrict: 'E',
            scope: {
                value: '=',
                hint: '@',
                minvalue:'@',
                maxvalue: '@',
                decimalprecision: '@',
                maxlength:'@',
                placeholder: '@',
                type: '@',
                label: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var validationOptions = {};
                if ($attrs.type === TextboxTypeEnum.Email.name || $scope.$parent.$eval(ctrl.type) === TextboxTypeEnum.Email.name)
                    validationOptions.emailValidation = true;
                else if ($attrs.type === TextboxTypeEnum.Number.name || $scope.$parent.$eval(ctrl.type) === TextboxTypeEnum.Number.name) {
                    validationOptions.numberValidation = true;
                    validationOptions.maxNumber = ctrl.maxvalue;
                    validationOptions.minNumber = ctrl.minvalue;
                    validationOptions.numberPrecision = ctrl.decimalprecision;
                }
                ctrl.tabindex = "";
                setTimeout(function () {
                    if ($($element).hasClass('divDisabled') || $($element).parents('.divDisabled').length > 0) {
                        ctrl.tabindex = "-1"
                    }
                }, 10)
               
                var $quan = $('.next-input');
                $quan.on('keyup', function (e) {
                    if (e.which === 40) {
                        var ind = $quan.index(this);
                        $quan.eq(ind + 1).focus()
                    }
                    if (e.which === 38) {
                        var ind = $quan.index(this);
                        var  el = $quan.eq(ind - 1);
                        if(el)
                          $quan.eq(ind - 1).focus();
                    }
                });
                ctrl.validate = function () {                    
                    return VRValidationService.validate(ctrl.value, $scope, $attrs, validationOptions);
                };

                $scope.ctrl.onBlurDirective = function (e) {
                    if ($attrs.onblurtextbox != undefined) {
                        var onblurtextboxMethod = $scope.$parent.$eval($attrs.onblurtextbox);
                        if (onblurtextboxMethod != undefined && onblurtextboxMethod != null && typeof (onblurtextboxMethod) == 'function') {
                            onblurtextboxMethod();
                        }
                    }
                }
            },
            compile: function (element, attrs) {

                //var inputElement = element.find('#mainInput');
                //var validationOptions = {};
                //if (attrs.isrequired !== undefined)
                //    validationOptions.requiredValue = true;
                //if (attrs.customvalidate !== undefined)
                //    validationOptions.customValidation = true;
                //if (attrs.type === TextboxTypeEnum.Email.name)
                //    validationOptions.emailValidation = true;
                //if (attrs.type === TextboxTypeEnum.Number.name)
                //    validationOptions.numberValidation = true;
                //var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, element.find('#rootDiv'));
                return {
                    pre: function ($scope, iElem, iAttrs) {
                        var ctrl = $scope.ctrl;

                        
                        var isUserChange;
                        $scope.$watch('ctrl.value', function (newValue, oldValue) {
                            if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                                return;
                          
                            var retrunedValue;
                            isUserChange = false;//reset the flag
                          
                            if (newValue == "") {
                                
                                setTimeout(function () {
                                    ctrl.value = undefined;
                                    $scope.$apply();
                                });
                               
                            }
                            if (!newValue == "") {

                                retrunedValue = newValue;
                            }
                            
                            if (iAttrs.type === TextboxTypeEnum.Number.name || $scope.$parent.$eval(ctrl.type) === TextboxTypeEnum.Number.name) {
                                var arr = String(newValue).split("");
                                var decimalArray = String(newValue).split(".");
                                var negativeArray = String(newValue).split("-");
                                //if (arr.length === 0) return;
                                //if (iAttrs.onlypositive != undefined && negativeArray.length > 1)
                                //    ctrl.value = oldValue;
                                //if (iAttrs.allowndecimal == undefined && decimalArray.length > 1)
                                //    ctrl.value = oldValue;
                                if (decimalArray.length > 1 && ctrl.decimalprecision == 0)
                                    ctrl.value = oldValue;
                                if (negativeArray.length > 1 && ctrl.minvalue >=0)
                                    ctrl.value = oldValue;
                                if (decimalArray.length > 2)
                                    ctrl.value = oldValue;
                                if (negativeArray.length > 2)
                                    ctrl.value = oldValue;
                               // if (arr.length === 0) return;
                               //if (arr.length === 1 && (arr[0] === '-' || arr[0] === '.')) return;
                                if (arr.length === 2 && newValue === '-.') return;
                                if (!isNaN(newValue) && newValue != "") {
                                    retrunedValue = newValue;
                                }
                                if (isNaN(newValue) && (newValue != '-') && (newValue != '.')) {
                                    ctrl.value = oldValue;
                                    retrunedValue=undefined;
                                }
                               
                            
                            }
                           
                            if (iAttrs.maxlength != undefined) {
                                var charArray = String(newValue).split("");
                                if(charArray.length > ctrl.maxlength)
                                    ctrl.value = oldValue;

                            }

                            if (iAttrs.onvaluechanged != undefined) {
                                var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onvaluechanged);
                                if (onvaluechangedMethod != undefined && typeof (onvaluechangedMethod) == 'function') {
                                    onvaluechangedMethod(retrunedValue);
                                }
                            }
                           
                        });

                        ctrl.notifyUserChange = function () {
                            isUserChange = true;
                        };
                        ctrl.placelHolder = (attrs.placeholder != undefined) ? ctrl.placeholder : '';
                        ctrl.readOnly = attrs.readonly != undefined;
                        if (attrs.hint != undefined) {
                            ctrl.hint = attrs.hint;
                        }
                        var getInputeStyle = function () {
                            var div = element.find('div[validator-section]')[0];
                            if (attrs.hint != undefined) {
                                $(div).css({ "display": "inline-block", "width": "calc(100% - 15px)", "margin-right": "1px" })
                            };
                        }
                        getInputeStyle();

                        ctrl.adjustTooltipPosition = function (e) {
                            setTimeout(function() {
                                var self = angular.element(e.currentTarget);
                                var selfHeight = $(self).height();
                                var selfOffset = $(self).offset();
                                var tooltip = self.parent().find('.tooltip-info')[0];
                                $(tooltip).css({ display: 'block' });
                                var innerTooltip = self.parent().find('.tooltip-inner')[0];
                                var innerTooltipArrow = self.parent().find('.tooltip-arrow')[0];
                                var innerTooltipWidth = parseFloat (( $(innerTooltip).width() / 2 ) + 2.5);
                                $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 15, left: selfOffset.left - innerTooltipWidth });
                                $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 10, left: selfOffset.left });

                            }, 1);
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
                    labelTemplate = '<vr-label>{{ctrl.label}}</vr-label>';
                var type = 'text';
                if (attrs.type != undefined && attrs.type === TextboxTypeEnum.Password.name)
                    type = 'password';
                var keypress = '';
                var keypressclass = '';
                if (attrs.tonextinput != undefined) {
                    keypressclass = 'next-input';
                }
                var textboxTemplate = '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false">'
                            + '<vr-validator validate="ctrl.validate()" >'
                            + '<input  tabindex="{{ctrl.tabindex}}" ng-readonly="ctrl.readOnly" id="mainInput" placeholder="{{ctrl.placelHolder}}"  ng-model="ctrl.value" ng-change="ctrl.notifyUserChange()" size="10" class="form-control ' + keypressclass + ' " data-autoclose="1" type="' + type + '" ng-blur="ctrl.onBlurDirective($event)">'
                            + '</vr-validator>'
                            + '<span ng-if="ctrl.hint!=undefined" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor" html="true" style="color:#337AB7"  placement="bottom"  trigger="hover" ng-mouseenter="ctrl.adjustTooltipPosition($event)"  data-type="info" data-title="{{ctrl.hint}}"></span>'
                        + '</div>';
                

                    //var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined ,true);

                return startTemplate + labelTemplate + textboxTemplate + endTemplate;
            }

        };

    }

    app.directive('vrTextbox', vrTextbox);

    app.constant('TextboxTypeEnum', {
        Email: { name: "email" },
        Number: { name: "number" },
        Password: { name: "password" }
    });

})(app);



