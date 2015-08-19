(function (app) {

    "use strict";

    vrTextbox.$inject = ['BaseDirService', 'TextboxTypeEnum'];

    function vrTextbox(BaseDirService, TextboxTypeEnum) {

        return {
            restrict: 'E',
            require: '^form',
            scope: {
                value: '=',
                hint:'@',
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
                if (attrs.type === TextboxTypeEnum.Email.name)
                    validationOptions.emailValidation = true;

                var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, element.find('#rootDiv'));
                return {
                    pre: function ($scope, iElem, iAttrs, formCtrl) {
                        var ctrl = $scope.ctrl;

                        var isUserChange;
                        $scope.$watch('ctrl.value', function (newValue, oldValue) {
                            //if (!isValidElem())
                            //    return;

                            if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                                return;
                            isUserChange = false;//reset the flag
                            if (iAttrs.type === TextboxTypeEnum.Number.name) {
                                var arr = String(newValue).split("");
                                if (arr.length === 0) return;
                                if (arr.length === 1 && (arr[0] === '-' || arr[0] === '.')) return;
                                if (arr.length === 2 && newValue === '-.') return;
                                if (iAttrs.maxvalue != undefined && newValue > parseFloat(iAttrs.maxvalue)) {
                                    ctrl.value = oldValue
                                }
                                if (iAttrs.minvalue != undefined && newValue < parseFloat(iAttrs.minvalue)) {
                                    ctrl.value = oldValue;
                                }
                                if (isNaN(newValue)) {
                                    ctrl.value = oldValue;
                                }
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
                        if (attrs.hint != undefined) {
                            ctrl.hint = attrs.hint;
                            console.log(attrs.hint);
                        }
                            ctrl.hint = attrs.hint;
                        ctrl.getInputeStyle = function () {
                            return (attrs.hint != undefined) ? {
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
                                $(tooltip).css({ display: 'block !important' });
                                var innerTooltip = self.parent().find('.tooltip-inner')[0];
                                var innerTooltipArrow = self.parent().find('.tooltip-arrow')[0];
                                $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight +5, left: selfOffset.left - 30 });
                                $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight , left: selfOffset.left  });

                            }, 1)
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
                var type = 'text';
                if (attrs.type != undefined && attrs.type === TextboxTypeEnum.Password.name)
                    type = 'password';
                    if (attrs.hint != undefined) {
                        console.log(attrs.hint);
                    }
                    var textboxTemplate = '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false">'
                            + '<input  ng-readonly="ctrl.readOnly" id="mainInput" ng-style="ctrl.getInputeStyle()" ng-model="ctrl.value" ng-change="ctrl.notifyUserChange()" size="10" class="form-control" data-autoclose="1" type="' + type + '" >'
                            + '<span ng-if="ctrl.hint!=undefined" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor" html="true" style="color:#337AB7"  placement="bottom"  trigger="hover" ng-mouseenter="ctrl.adjustTooltipPosition($event)" ng data-type="info" data-title="{{ctrl.hint}}"></span>'
                        + '</div>';
                

                var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined);

                return startTemplate + labelTemplate + textboxTemplate + validationTemplate + endTemplate;
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



