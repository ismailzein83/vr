(function (app) {

    "use strict";

    vrTextbox.$inject = ['BaseDirService', 'TextboxTypeEnum', 'VRValidationService', 'UtilsService', 'VRLocalizationService'];

    function vrTextbox(BaseDirService, TextboxTypeEnum, VRValidationService, UtilsService, VRLocalizationService) {

        return {
            restrict: 'E',
            scope: {
                value: '=',
                hint: '@',
                minvalue: '@',
                maxvalue: '@',
                decimalprecision: '@',
                maxlength: '@',
                minlength: '@',
                placeholder: '@',
                type: '@',
                label: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.readOnly = false;
                if ($attrs.stopreadonly == undefined)
                    ctrl.readOnly = UtilsService.isContextReadOnly($scope) || $attrs.readonly != undefined;
                var validationOptions = {};
                if ($attrs.type === TextboxTypeEnum.Email.name || $scope.$parent.$eval(ctrl.type) === TextboxTypeEnum.Email.name)
                    validationOptions.emailValidation = true;
                if ($attrs.type === TextboxTypeEnum.Ip.name || $scope.$parent.$eval(ctrl.type) === TextboxTypeEnum.Ip.name)
                    validationOptions.ipValidation = true;
                if ($attrs.refusepecialcharacter != undefined)
                    validationOptions.specialCharacterValidation = true;
                if ($attrs.type === TextboxTypeEnum.FileName.name || $scope.$parent.$eval(ctrl.type) === TextboxTypeEnum.FileName.name)
                    validationOptions.filenameValidation = true;
                if ($attrs.minlength != undefined) {
                    validationOptions.minlengthValidation = true;
                    validationOptions.minLength = ctrl.minlength;
                }
                else if ($attrs.type === TextboxTypeEnum.Number.name || $scope.$parent.$eval(ctrl.type) === TextboxTypeEnum.Number.name) {
                    validationOptions.numberValidation = true;
                    validationOptions.maxNumber = ctrl.maxvalue;
                    validationOptions.minNumber = ctrl.minvalue;
                    validationOptions.numberPrecision = ctrl.decimalprecision;
                }
                var getInputeStyle = function () {
                    var div = $element.find('div[validator-section]')[0];
                    if ($attrs.hint != undefined) {
                        var styleObj = {
                            "display": "inline-block", "width": "calc(100% - 15px)"
                        };
                        if (VRLocalizationService.isLocalizationRTL())
                            styleObj.marginLeft = "1px";
                        else
                            styleObj.marginRight = "1px";

                        $(div).css(styleObj);

                    }
                }();

                ctrl.tabindex = "";
                setTimeout(function () {
                    if ($($element).hasClass('divDisabled') || $($element).parents('.divDisabled').length > 0) {
                        ctrl.tabindex = "-1"
                    }
                }, 10);

                var $quan = $('.next-input').filter(function () {
                    return !$(this).parents('.divDisabled').length;
                });
                $quan.bind('keyup', function (e) {
                    $('.vr-datagrid-row').removeClass('vr-datagrid-datacells-click');
                    var ind = $quan.index(this);
                    var textbox;
                    if (e.which === 40 || e.which === 13) {
                        textbox = $quan.eq(ind + 1);

                    }
                    if (e.which === 38) {
                        var el = $quan.eq(ind - 1);
                        if (el) {
                            textbox = $quan.eq(ind - 1);
                        }
                    }
                    if (textbox) {
                        $(textbox).focus();
                        $(textbox).closest('.vr-datagrid-row').addClass('vr-datagrid-datacells-click');
                    }
                });
                ctrl.validate = function () {
                    return VRValidationService.validate(ctrl.value, $scope, $attrs, validationOptions);
                };

                if ($attrs.setfocus != undefined)
                    setTimeout(function () {
                        $element.find('.main-input').focus();
                    }, 10);

                $scope.ctrl.onBlurDirective = function (e) {
                    if ($attrs.onblurtextbox != undefined && !ctrl.readOnly) {
                        var onblurtextboxMethod = $scope.$parent.$eval($attrs.onblurtextbox);
                        if (onblurtextboxMethod != undefined && onblurtextboxMethod != null && typeof (onblurtextboxMethod) == 'function') {
                            onblurtextboxMethod();
                        }
                    }
                };
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
                        $scope.$on("$destroy", function () {
                            valueWatch();
                        });
                        var isUserChange;

                        var valueWatch = $scope.$watch('ctrl.value', function (newValue, oldValue) {
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
                                if (String(newValue).indexOf(".") > -1 && ctrl.decimalprecision == 0) {
                                    ctrl.value = oldValue;
                                    return;
                                }
                                //if (arr.length === 0) return;
                                //if (iAttrs.onlypositive != undefined && negativeArray.length > 1)
                                //    ctrl.value = oldValue;
                                //if (iAttrs.allowndecimal == undefined && decimalArray.length > 1)
                                //    ctrl.value = oldValue;
                                //if (decimalArray.length > 1 && ctrl.decimalprecision == 0)
                                //    ctrl.value = oldValue;
                                //if (negativeArray.length > 1 && ctrl.minvalue >= 0)
                                //    ctrl.value = oldValue;
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
                                    retrunedValue = undefined;
                                    return;
                                }


                            }

                            if (iAttrs.maxlength != undefined) {
                                var charArray = String(newValue).split("");
                                if (charArray.length > ctrl.maxlength)
                                    ctrl.value = oldValue;
                                return;
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
                        // ctrl.readOnly = attrs.readonly != undefined;
                        if (attrs.hint != undefined) {
                            ctrl.hint = attrs.hint;
                        }
                        ctrl.onKeyUp = function ($event) {
                            if (iAttrs.onenterpress != undefined) {
                                var onEnterPress = $scope.$parent.$eval(iAttrs.onenterpress);
                                if (onEnterPress != undefined && typeof (onEnterPress) == 'function' && $event.which == 13) {
                                    onEnterPress();
                                }
                            }
                        };

                        ctrl.adjustTooltipPosition = function (e) {
                            setTimeout(function () {
                                var self = angular.element(e.currentTarget);
                                var selfHeight = $(self).height();
                                var selfOffset = $(self).offset();
                                var tooltip = self.parent().find('.tooltip-info')[0];
                                $(tooltip).css({
                                    display: 'block'
                                });
                                var innerTooltip = self.parent().find('.tooltip-inner')[0];
                                var innerTooltipArrow = self.parent().find('.tooltip-arrow')[0];
                                if (iAttrs.largehint != undefined)
                                    $(innerTooltip).addClass('large-hint');
                                var innerTooltipWidth = parseFloat(($(innerTooltip).width() / 2) + 2.5);
                                $(innerTooltip).css({
                                    position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 15, left: selfOffset.left - innerTooltipWidth
                                });
                                $(innerTooltipArrow).css({
                                    position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 10, left: selfOffset.left
                                });

                            }, 1);
                        };

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

                //var label = "";

                //if (attrs.label != undefined) {
                //    label = VRLocalizationService.getResourceValue(attrs.localizedlabel, attrs.label);
                //}

                //if (attrs.label != undefined)
                //    labelTemplate = '<vr-label>' + label + '</vr-label>';

                if (attrs.label != undefined)
                    labelTemplate = '<vr-label>{{ctrl.label}}</vr-label>';
                var type = 'text';
                var isolationFormStart = "";
                var isolationFormEnd = "";
                if (attrs.type != undefined && attrs.type === TextboxTypeEnum.Password.name) {
                    type = 'password';                    
                    isolationFormStart = "<form>";
                    isolationFormEnd = "</form>";
                }
                var keypress = '';
                var keypressclass = '';
                if (attrs.tonextinput != undefined) {
                    keypressclass = 'next-input';
                }
                //var format = "";
                //if (attrs.type === TextboxTypeEnum.Number.name)
                //    format = 'format="number"'; '+format+'
                var textboxTemplate = '<div ng-mouseenter="::(showtd=true)" ng-mouseleave="::(showtd=false)">'
                            + '<vr-validator validate="ctrl.validate()" vr-input>'
                            +   isolationFormStart + ' <input  tabindex="{{ctrl.tabindex}}" autocomplete="false" ng-readonly="::ctrl.readOnly"  placeholder="{{::ctrl.placelHolder}}"  ng-model="ctrl.value" ng-change="::ctrl.notifyUserChange()" size="10" class="form-control vanrise-inpute main-input ' + keypressclass + ' " data-autoclose="1" type="' + type + '" ng-keyup="::ctrl.onKeyUp($event)" ng-blur="::ctrl.onBlurDirective($event)"/>' + isolationFormEnd
                            + '</vr-validator>'
                            + '<span ng-if="(ctrl.hint!=undefined)" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor vr-hint-input" html="true"  placement="bottom"  trigger="hover" ng-mouseenter="::ctrl.adjustTooltipPosition($event)"  data-type="info" data-title="{{ctrl.hint}}"></span>'
                        + '</div>';


                //var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined ,true);

                return startTemplate + labelTemplate + textboxTemplate + endTemplate;
            }

        };

    }

    app.directive('vrTextbox', vrTextbox);

    app.constant('TextboxTypeEnum', {
        Email: {
            name: "email"
        },
        Ip: {
            name: "ip"
        },
        FileName: {
            name: "filename"
        },
        Number: {
            name: "number"
        },
        Password: {
            name: "password"
        }
    });

})(app);

//app.directive('format', ['$filter', function ($filter) {
//    return {
//        require: '?ngModel',
//        link: function (scope, elem, attrs, ctrl) {
//            if (!ctrl) return;
//            ctrl.$formatters.unshift(function (a) {
//                return $filter(attrs.format)(ctrl.$modelValue)
//            });
//            ctrl.$parsers.unshift(function (viewValue) {
//                var plainNumber = viewValue.replace(/[^\d|\-+|\.+]/g, '');
//                elem.val($filter(attrs.format)(plainNumber));
//                return plainNumber;
//            });
//        }
//    };
//}]);


