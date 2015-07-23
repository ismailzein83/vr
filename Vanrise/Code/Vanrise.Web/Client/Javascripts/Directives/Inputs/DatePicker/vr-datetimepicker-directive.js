'use strict';

app.directive('vrValidationDatetime', function () {
        return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrlModel) {

            var validate = function (viewValue) {
                if (viewValue == "invalid")                
                        ctrlModel.$setValidity('invalidformat', false);
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
            //var divDatePickerId = BaseDirService.generateHTMLElementName();
            //element.attr('id', divDatePickerId);
            var divDatePicker =  element.find('#divDatePicker');//$('#' + divDatePickerId);//
            var format;
            var isDate;
            var isTime;
            var isDateTime;
            switch (attrs.type) {
                case "date": format = 'DD/MM/YYYY';
                    isDate = true;
                    break;
                case "time": format = 'HH:mm';
                    isTime = true;
                    break;
                default: format = 'DD/MM/YYYY HH:mm';
                    isDateTime = true;
                    break;
            }
            divDatePicker.datetimepicker({
                format: format,
                showClose: true
            });
            var inputElement = element.find('#mainInput');
            var validationOptions = {};
            if (attrs.isrequired !== undefined)
                validationOptions.requiredValue = true;
            if (attrs.customvalidate !== undefined)
                validationOptions.customValidation = true;

            var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, element.find('#rootDiv'));
             return {
                 pre: function ($scope, iElem, iAttrs, formCtrl) {

                     var isUserChange = false;
                     var selectedDate;
                     divDatePicker
                           .on('dp.change', function (e) {                              
                              selectedDate = new Date(e.date);
                               var modelValue = $scope.ctrl.value;
                               if (modelValue != undefined && !(modelValue instanceof Date))
                                   modelValue = new Date(modelValue);

                               if (modelValue == undefined || modelValue.toString() != selectedDate.toString()) {
                                   isUserChange = true;
                                   $scope.ctrl.value = selectedDate;                                  
                               }

                           });

                     var ctrl = $scope.ctrl;

                    $scope.$watch('ctrl.value', function () {
                        if (ctrl.value == undefined)
                            return;
                        
                        var date = ctrl.value instanceof Date ? ctrl.value : (new Date(ctrl.value));
                        if (selectedDate == undefined || selectedDate.toString() != date.toString()) {                            
                            divDatePicker.data("DateTimePicker").date(date);
                        }
                        else {
                            if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                                return;
                            isUserChange = false;//reset the flag
                           
                            if (iAttrs.onvaluechanged != undefined) {
                                var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onvaluechanged);
                                if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                                    onvaluechangedMethod();
                                }
                            }
                        }
                        
                    });

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

            var dateTemplate =
                 '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false">'
                  + '<div id="mainInput" ng-model="ctrl.value">'
                 + '<div  class="input-group date datetime-controle" id="divDatePicker"  >'
                                + '<input class="form-control" data-autoclose="1" placeholder="Date" type="text" >'                                
                            + '</div>'
                      + '</div>'
                    + '</div>';

            var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true);

            //if(attrs.type == 'date')
                return startTemplate + labelTemplate + dateTemplate + validationTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);