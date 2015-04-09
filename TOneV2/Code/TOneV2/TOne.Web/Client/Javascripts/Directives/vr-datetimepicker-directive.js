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


app.directive('vrDatetimepicker', ['ValidationMessagesEnum', function (ValidationMessagesEnum) {
    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
    }
    function escapeRegExp(string) {
        return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
    }
    function replaceAll(string, find, replace) {
        return string.replace(new RegExp(escapeRegExp(find), 'g'), replace);
    }
    var directiveDefinitionObject = {
        restrict: 'E',
        require: '^form',
        scope: {
            value: '='
        },
        controller: function ($scope, $element) {
            $scope.ValidationMessagesEnum = ValidationMessagesEnum;
            $scope.tooltip = false;

            $scope.showtooltip = function () {
                $scope.tooltip = true;
            };

            $scope.hidetooltip = function () {
                $scope.tooltip = false;
            };

            
            
        },
        link: function (scope, element, attrs, formCtrl) {

            
            
        },
        compile: function (element, attrs) {
            var inputId = element.find('#hiddenId').attr('value');
            
            if (attrs.isrequired !== undefined) {
                var inputElement = element.find('#' + inputId);
                inputElement.attr('vr-validation-value', '');
            }

             return {
                pre: function ($scope, iElem, iAttrs, formCtrl) {
                    $scope.inputId = inputId;

                    var isUserChange;
                    $scope.$watch('value', function () {
                        if (!isValidElem())
                            return;
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

                    $scope.notifyUserChange = function () {
                        isUserChange = true;
                    };

                    var isValidElem = function () {
                       return formCtrl[$scope.inputId].$valid;
                    };

                    $scope.getErrorObject = function () {
                        return formCtrl[$scope.inputId].$error;
                    }

                    $scope.isvisibleTooltip = function () {
                        if (isValidElem()) return false;
                        return $scope.tooltip;
                    };

                    var validationClass = {
                        invalid: "required-inpute", valid: ''
                    };

                    $scope.isvalidcomp = function () {

                        if (isValidElem()) return validationClass.valid;

                        return validationClass.invalid;
                    }
                    
                }
            }
        },

        //controllerAs: 'ctrl',
        //bindToController: true,
        template: function (element, attrs) {
            var id = 'input_' + replaceAll(guid(), '-', '');
            var startTemplate = '<div ng-mouseenter="showtooltip(); " ng-mouseleave="hidetooltip()" ><hidden id="hiddenId" value="' + id + '"></hidden>';
            var endTemplate = '</div>';
            var labelTemplate = '';
            if (attrs.label != undefined)
                labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
            var dateTemplate = '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false">'
                                + '<input id="' + id + '" name="' + id + '" bs-datepicker ng-model="value" vr-validation-datetime  ng-class="isvalidcomp()" ng-change="notifyUserChange()" size="10" class="form-control" data-autoclose="1" placeholder="Date" type="text" >'
                            + '</div>';
            var validationTemplate = '<div class="tooltip-error disable-animations " ng-show="isvisibleTooltip()" ng-messages="getErrorObject()">'
                                           + '<div ng-message="requiredvalue">{{ ValidationMessagesEnum.required }}</div>'
                                           + '<div ng-message="invalidformat">invalid format</div>'
                                           + '<div ng-message="customvalidation">{{ customMessage }}</div>'
                                        +'</div>'
            if(attrs.type == 'date')
                return startTemplate + labelTemplate + dateTemplate + validationTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);