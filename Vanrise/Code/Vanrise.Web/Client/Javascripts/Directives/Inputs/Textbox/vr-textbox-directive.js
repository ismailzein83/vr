'use strict';

app.directive('vrTextbox', ['ValidationMessagesEnum', 'BaseDirService', function (ValidationMessagesEnum, BaseDirService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        require: '^form',
        scope: {
            value: '=',
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
            if (attrs.type == "email")
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
                        if (iAttrs.type == "number") {
                            var arr = String(newValue).split("");
                            if (arr.length === 0) return;
                            if (arr.length === 1 && (arr[0] == '-' || arr[0] === '.')) return;
                            if (arr.length === 2 && newValue === '-.') return;
                            if (isNaN(newValue)) {
                                ctrl.value = oldValue;
                            }
                        }                       
                        if (iAttrs.onvaluechanged != undefined) {
                            var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onvaluechanged);
                            if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                                onvaluechangedMethod();
                            }
                        }
                    });

                    ctrl.notifyUserChange = function () {
                        isUserChange = true;
                    };

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
            if (attrs.type != undefined && attrs.type == 'password')
                type = 'password';
            var textboxTemplate = '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false">'
                                + '<input id="mainInput" ng-model="ctrl.value" ng-change="ctrl.notifyUserChange()" size="10" class="form-control" data-autoclose="1" type="' + type + '" >'
                            + '</div>';

            var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true,true);

            return startTemplate + labelTemplate + textboxTemplate + validationTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);