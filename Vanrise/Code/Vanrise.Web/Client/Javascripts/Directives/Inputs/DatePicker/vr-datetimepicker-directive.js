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
            hint:'@',
            customvalidate: '&',
            placeholder: '@'
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
                    isDate = true;
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
            setTimeout(function () {
                if (divDatePicker.parents('.modal-body').length > 0) {
                    divDatePicker
                     .on('dp.show', function (e) {
                         var self = $(this);
                         var selfHeight = $(this).height();
                         var selfOffset = $(self).offset();
                         var dropDown = self.parent().find('.bootstrap-datetimepicker-widget')[0];
                         $(dropDown).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight, left: 'auto' });

                     });
                    var fixDropdownPosition = function () {
                        divDatePicker.data("DateTimePicker").hide()

                    };
                    $(divDatePicker.parents('div')).scroll(function () {
                        fixDropdownPosition();
                    })
                }

            }, 1)

            var isUserChange = false;
            var selectedDate;
            divDatePicker
            .on('dp.change', function (e) {
                if (!e.date) {
                    $scope.ctrl.value = null;
                    return;
                }
                var dt = e.date;
                selectedDate = convertUTCDateToLocalDate(new Date(dt));
                selectedDate.setSeconds(0);
                selectedDate.setMilliseconds(0);
                var modelValue = $scope.ctrl.value;
                if (modelValue != undefined && !(modelValue instanceof Date))
                    modelValue = new Date(modelValue);
                if (modelValue == undefined || modelValue.toString() != selectedDate.toString()) {
                    isUserChange = true;
                   
                    if ($attrs.type == "time") {
                        $scope.ctrl.value = {
                            Hour: selectedDate.getUTCHours(),
                            Minute: selectedDate.getUTCMinutes(),
                            Second: selectedDate.getUTCSeconds(),
                            Millisecond: selectedDate.getUTCMilliseconds()
                        };
                    }
                    else
                        $scope.ctrl.value = selectedDate;
                }

            });

            function convertUTCDateToLocalDate(date) {
                var newDate = new Date(date.getTime() - date.getTimezoneOffset() * 60 * 1000);

                //var offset = date.getTimezoneOffset() / 60;
                //var hours = date.getHours();

                //newDate.setHours(hours - offset);

                return newDate;
            }


            var ctrl = $scope.ctrl;
            ctrl.placelHolder = ($attrs.placeholder != undefined) ? ctrl.placeholder : '';
            ctrl.updateModelOnKeyUp = function (e) {
                var $this = angular.element(e.currentTarget);
                setTimeout(function () {
                    if (moment($this.val(), format, true).isValid()) {
                            divDatePicker.data("DateTimePicker").date($this.val());
                    }
                }, 1)
                

            }

            if ($attrs.hint != undefined)
                ctrl.hint = $attrs.hint;
            ctrl.getInputeStyle = function () {
                return ($attrs.hint != undefined) ? {
                    "display": "inline-block",
                    "width": "calc(100% - 15px)",
                    "margin-right": "1px"
                } : {
                    "width": "100%",
                };
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
                    $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 5, left: selfOffset.left - 30 });
                    $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight, left: selfOffset.left });
                }, 1)
            }
            $scope.$watch('ctrl.value', function () {
                if (ctrl.value == undefined)
                    return;
                var date;
                if ($attrs.type == "time") {
                    var initialDate = new Date();
                    if (ctrl.value.Hour == undefined)
                        ctrl.value.Hour = 0;
                    if (ctrl.value.Minute == undefined)
                        ctrl.value.Minute = 0;
                    if (ctrl.value.Second == undefined)
                        ctrl.value.Second = 0;
                    if (ctrl.value.Millisecond == undefined)
                        ctrl.value.Millisecond = 0;
                    initialDate.setHours(ctrl.value.Hour, ctrl.value.Minute, ctrl.value.Second, ctrl.value.Millisecond);
                    var convertedDate = convertUTCDateToLocalDate(initialDate);
                    date = convertedDate;
                }
                else {
                    date = ctrl.value instanceof Date ? ctrl.value : (new Date(ctrl.value));
                }
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

            $scope.ctrl.onBlurDirective = function (e) {
                var dateTab = (ctrl.value).toString().split("T")[0].split("-");
                var year = parseInt(dateTab[0].split(" ")[3]);
                if (year < 1970 || isNaN(year)) {
                    ctrl.value = new Date();
                }
                if ($attrs.onblurdatetime != undefined) {
                    var onblurdatetimeMethod = $scope.$parent.$eval($attrs.onblurdatetime);
                    if (onblurdatetimeMethod != undefined && onblurdatetimeMethod != null && typeof (onblurdatetimeMethod) == 'function') {
                        onblurdatetimeMethod();
                    }
                }
            }
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
            else if (attrs.type == undefined)
                icontemplate = ' <span ng-show="showtd" class="input-group-addon vr-small-addon " ng-click="ctrl.toggleDate($event)" ><i class="glyphicon glyphicon-calendar"></i></span>'
                             + ' <span ng-show="showtd"  class="input-group-addon vr-small-addon " ng-click="ctrl.toggleTime($event)" > <i class="glyphicon glyphicon-time"></i></span>';

            var dateTemplate =
                 '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false"  style="height:29px;" >'
                  + '<div id="mainInput" ng-model="ctrl.value" class="form-control " ng-style="ctrl.getInputeStyle()" style="border-radius: 4px;height: auto;padding: 0px;">'
                        + '<div  class="input-group date datetime-controle"  id="divDatePicker"  style="width:100%;"  >'
                                + '<input class="form-control vr-date-input"  placeholder="{{ctrl.placelHolder}}" ng-style="ctrl.getInputeStyle()" style="padding:0px 5px;"  ng-keyup="ctrl.updateModelOnKeyUp($event)" ng-blur="ctrl.onBlurDirective($event)" ng-class="showtd==true? \'fix-border-radius\':\'border-radius\'" data-autoclose="1" placeholder="Date" type="text" ctrltype="' + attrs.type + '">'
                                + icontemplate
                            + '</div>'
                      + '</div>'
                      + '<span  ng-if="ctrl.hint!=undefined"  ng-mouseenter="ctrl.adjustTooltipPosition($event)" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor" style="color:#337AB7;top:-10px" html="true" placement="bottom" trigger="hover" data-type="info" data-title="{{ctrl.hint}}"></span>'

                    + '</div>';

            var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined);

            return startTemplate + labelTemplate + dateTemplate + validationTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);