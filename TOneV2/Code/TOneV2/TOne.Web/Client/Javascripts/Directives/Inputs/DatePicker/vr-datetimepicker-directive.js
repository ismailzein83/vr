'use strict';

app.directive('vrValidationDatetime', function () {
        return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrlModel) {

            var validate = function (viewValue) {
                if (viewValue != undefined && viewValue != null) {
                    var d = Date.parse(viewValue);
                    if (d == 'NaN')
                        ctrlModel.$setValidity('invalidformat', false);
                    else
                        ctrlModel.$setValidity('invalidformat', true);
                }
                else {
                    ctrlModel.$setValidity('invalidformat', true);
                }

                return viewValue;
            }
            ctrlModel.$parsers.unshift(validate);
            ctrlModel.$formatters.push(validate);

        }
    };
});


app.directive('vrDatetimepicker', ['ValidationMessagesEnum', 'BaseDirService', function (ValidationMessagesEnum, BaseDirService) {
    
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

            var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, element.find('#rootDiv'));
             return {
                 pre: function ($scope, iElem, iAttrs, formCtrl) {
                     var ctrl = $scope.ctrl;

                    var isUserChange;
                    $scope.$watch('ctrl.value', function () {
                        //if (!isValidElem())
                        //    return;
                       
                        if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                            return;
                        isUserChange = false;//reset the flag
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

            var dateTemplate = '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false">'
                                + '<input id="mainInput" bs-datepicker ng-model="ctrl.value" vr-validation-datetime=""  ng-change="ctrl.notifyUserChange()" size="10" class="form-control" data-autoclose="1" placeholder="Date" type="text" >'
                            + '</div>';

            var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true);

            if(attrs.type == 'date')
                return startTemplate + labelTemplate + dateTemplate + validationTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);