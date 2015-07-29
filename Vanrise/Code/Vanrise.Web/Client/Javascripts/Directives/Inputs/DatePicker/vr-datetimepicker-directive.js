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
        controller: function ($scope, $element, $attrs) {
            var divDatePicker = $element.find('#divDatePicker');
            var inputElement = $element.find('#mainInput');
            var validationOptions = {};
            if ($attrs.isrequired !== undefined)
                validationOptions.requiredValue = true;
            if ($attrs.customvalidate !== undefined)
                validationOptions.customValidation = true;
           var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, $element.find('#rootDiv'));

            var format;
            var isDate;
            var isTime;
            var isDateTime;
            switch ($attrs.type) {
                case "date": format = 'DD/MM/YYYY';
                    $scope.ctrl.isDate = true;
                    break;
                case "time": format = 'HH:mm';
                    $scope.ctrl.isTime = true;
                    break;
                default: format = 'DD/MM/YYYY HH:mm';
                    $scope.ctrl.isDateTime = true;
                    break;
            }
            divDatePicker.datetimepicker({
                format: format,
                showClose: true
            });
            if (divDatePicker.closest('.modal-body').length > 0) {

                divDatePicker
                 .on('dp.show', function (e) {
                     var self = $(this);
                     var selfHeight = $(this).parent().height();
                     var selfOffset = $(self).offset();
                     var dropDown = self.parent().find('.bootstrap-datetimepicker-widget')[0];
                     $(dropDown).css({ position: 'fixed', top: selfOffset.top + selfHeight, left: 'auto' });
                 });
                var fixDropdownPosition = function () {
                    divDatePicker.data("DateTimePicker").hide()

                };

                $(".modal-body").unbind("scroll");
                $(".modal-body").scroll(function () {
                    fixDropdownPosition();
                });               
                $(window).resize(function () {
                    fixDropdownPosition();
                });
            }
            var isUserChange = false;
            var selectedDate;
            divDatePicker
            .on('dp.change', function (e) {
                if (!e.date) {
                    $scope.ctrl.value = null;
                    return;
                }
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
                if (ctrl.value==null) {
                    console.log("is null in control");
                }

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

                    if ($attrs.onvaluechanged != undefined) {
                        var onvaluechangedMethod = $scope.$parent.$eval($attrs.onvaluechanged);
                        if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                            onvaluechangedMethod();
                        }
                    }
                }

            });
            $scope.ctrl.toggleDate = function (e) {
              
                e.preventDefault();
                e.stopPropagation();
               
                $('.date-section').addClass('in');
                $('.time-section').removeClass('in');

                // switch icon to time
                $('.btn-switcher').addClass("glyphicon-time");
                $('.btn-switcher').removeClass("glyphicon-calendar");
            }
            $scope.ctrl.toggleTime = function (e) {
             
                e.preventDefault();
                e.stopPropagation();
                $('.time-section').addClass('in');
                $('.date-section').removeClass('in');

                // switch icon to date
                $('.btn-switcher').removeClass("glyphicon-time");
                $('.btn-switcher').addClass("glyphicon-calendar");
            }
            BaseDirService.addScopeValidationMethods(ctrl, elementName, this);
            
        },
        compile: function (element, attrs) {
            //var divDatePickerId = BaseDirService.generateHTMLElementName();
            //element.attr('id', divDatePickerId);
            //var divDatePicker = element.find('#divDatePicker');//$('#' + divDatePickerId);//
            //console.log(element.find('#divDatePicker'))
            //var format;
            //var isDate;
            //var isTime;
            //var isDateTime;
            //switch (attrs.type) {
            //    case "date": format = 'DD/MM/YYYY';
            //        isDate = true;
            //        break;
            //    case "time": format = 'HH:mm';
            //        isTime = true;
            //        break;
            //    default: format = 'DD/MM/YYYY HH:mm';
            //        isDateTime = true;
            //        break;
            //}
            //divDatePicker.datetimepicker({
            //    format: format,
            //    showClose: true
            //});
            var inputElement = element.find('#mainInput');
            var validationOptions = {};
            if (attrs.isrequired !== undefined)
                validationOptions.requiredValue = true;
            if (attrs.customvalidate !== undefined)
                validationOptions.customValidation = true;

            var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, element.find('#rootDiv'));
             return {
                 pre: function ($scope, iElem, iAttrs, formCtrl) {

                    // var isUserChange = false;
                    // var selectedDate;
                    // //divDatePicker.datetimepicker({
                    // //    format: 'DD/MM/YYYY',
                    // //    showClose: true
                    // //});
                    //// $scope.frmctrl = formCtrl
                    // divDatePicker
                    //       .on('dp.change', function (e) {                              
                    //          selectedDate = new Date(e.date);
                    //           var modelValue = $scope.ctrl.value;
                    //           if (modelValue != undefined && !(modelValue instanceof Date))
                    //               modelValue = new Date(modelValue);

                    //           if (modelValue == undefined || modelValue.toString() != selectedDate.toString()) {
                    //               isUserChange = true;
                    //               $scope.ctrl.value = selectedDate;                                  
                    //           }

                    //       });                   
                    // if (divDatePicker.closest('.modal-body').length > 0) {

                    //     divDatePicker
                    //      .on('dp.show', function (e) {
                    //          var self = $(this);
                    //          var selfHeight = $(this).parent().height();
                    //          var selfOffset = $(self).offset();
                    //          var dropDown = self.parent().find('.bootstrap-datetimepicker-widget')[0];
                    //          $(dropDown).css({ position: 'fixed', top: selfOffset.top + selfHeight, left: 'auto' });
                    //      });
                         
                    // }
                    var ctrl = $scope.ctrl;

                    //$scope.$watch('ctrl.value', function () {
                    //    if (ctrl.value == undefined)
                    //        return;
                        
                    //    var date = ctrl.value instanceof Date ? ctrl.value : (new Date(ctrl.value));
                    //    if (selectedDate == undefined || selectedDate.toString() != date.toString()) {                            
                    //        divDatePicker.data("DateTimePicker").date(date);
                    //    }
                    //    else {
                    //        if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                    //            return;
                    //        isUserChange = false;//reset the flag
                           
                    //        if (iAttrs.onvaluechanged != undefined) {
                    //            var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onvaluechanged);
                    //            if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                    //                onvaluechangedMethod();
                    //            }
                    //        }
                    //    }
                        
                    //});

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
            var icontemplate = "";
            if (attrs.type == 'date' || attrs.type == 'dateTime')
                icontemplate += ' <span ng-show="showtd" class="input-group-addon vr-small-addon " ng-click="ctrl.toggleDate($event)" ><i class="glyphicon glyphicon-calendar"></i></span>';
            if (attrs.type == 'time' || attrs.type == 'dateTime')
                icontemplate += ' <span ng-show="showtd"  class="input-group-addon vr-small-addon " ng-click="ctrl.toggleTime($event)" > <i class="glyphicon glyphicon-time"></i></span>';
            var dateTemplate =
                 '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false"  >'
                  + '<div id="mainInput" ng-model="ctrl.value" class="form-control " style="border-radius: 4px;height: auto;padding: 0px;">'
                 + '<div  class="input-group date datetime-controle" style="width:100%" id="divDatePicker"  >'
                                + '<input class="form-control vr-date-input" style="height:30px;" ng-class="showtd==true? \'fix-border-radius\':\'border-radius\'" data-autoclose="1" placeholder="Date" type="text" ctrltype="' + attrs.type + '">'
                                + icontemplate
                            + '</div>'
                      + '</div>'
                    + '</div>';
           
            var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true,true, attrs.label != undefined);

            return startTemplate + labelTemplate + dateTemplate + validationTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);