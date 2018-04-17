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
            };
            ctrlModel.$parsers.unshift(validate);
            ctrlModel.$formatters.push(validate);

        }
    };
});


app.directive('vrDatetimepicker', ['BaseDirService', 'VRValidationService', 'UtilsService', 'VRDateTimeService', 'VRLocalizationService', function (BaseDirService, VRValidationService, UtilsService, VRDateTimeService, VRLocalizationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            value: '=',
            hint: '@',
            placeholder: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var divDatePicker = $element.find('#divDatePicker');

            $scope.$on("$destroy", function () {
                $element.parents().unbind('scroll', fixDateTimePickerPosition);
                $(window).unbind('scroll', fixDateTimePickerPosition);
                updatewatch();
            });
            $element.on('$destroy', function () {
                $element.parents().unbind('scroll', fixDateTimePickerPosition);
            });


            //var inputElement = $element.find('#mainInput');
            //var validationOptions = {};
            //if ($attrs.isrequired !== undefined)
            //    validationOptions.requiredValue = true;
            //if ($attrs.customvalidate !== undefined)
            //    validationOptions.customValidation = true;
            //var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, $element.find('#rootDiv'));

            var ctrl = this;
            ctrl.readOnly = UtilsService.isContextReadOnly($scope) || $attrs.readonly != undefined;

            ctrl.validate = function () {
                return VRValidationService.validate(ctrl.value, $scope, $attrs);
            };

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
                case "dateHour": format = 'DD/MM/YYYY HH';
                    $scope.ctrl.isDateTime = true;
                    break;
                case "longDateTime": format = 'DD/MM/YYYY HH:mm:ss';
                    $scope.ctrl.isDateTime = true;
                    break;
                case "yearMonth": format = 'MM/YYYY';
                    $scope.ctrl.isDate = true;
                    isDate = true;
                    break;
                case "hour": format = 'HH';
                    $scope.ctrl.isTime = true;
                    isTime = true;
                    break;
                default: format = 'DD/MM/YYYY HH:mm';
                    $scope.ctrl.isDateTime = true;
                    break;
            }
            divDatePicker.datetimepicker({
                format: format,
                showClose: true,
                useCurrent: 'day',
                allowInputToggle: $attrs.disablefocus == undefined,
                // locale:"ar"
            });
            divDatePicker
                .on('dp.show', function (e) {
                    var istop = false;
                    var isright = false;
                    var self = $(this);
                    var selfHeight = $(this).height();
                    var selfOffset = $(self).offset();
                    var dropDown = self.parent().find('.bootstrap-datetimepicker-widget')[0];
                    var basetop = selfOffset.top - $(window).scrollTop() + $(this).height();
                    var eltop = selfOffset.top - $(window).scrollTop();
                    var baseleft = selfOffset.left - $(window).scrollLeft();
                    var elleft = baseleft;
                    $(dropDown).removeClass('pull-right');
                    $(dropDown).removeClass('dropdown-menu.top');
                    var rtl = VRLocalizationService.isLocalizationRTL();
                    if (rtl) {
                        elleft = elleft - (228 - $(this).width());
                        $(dropDown).addClass('vr-datetime-pull-right');
                        if (baseleft + $(this).width() < 228) {
                            elleft = baseleft;
                            $(dropDown).removeClass('vr-datetime-pull-right');
                        }

                    }
                    else {
                        if (innerWidth - elleft < 228) {
                            elleft = elleft - (228 - $(this).width());
                            $(dropDown).addClass('vr-datetime-pull-right');
                        }
                    }
                    if (innerHeight - eltop < 284) {
                        basetop = eltop - (284 + $(this).height() / 2);
                        $(dropDown).addClass('vr-datetime-top');
                    }
                    else
                        $(dropDown).addClass('vr-datetime');
                    setTimeout(function () {

                        $(dropDown).slideDown();
                    }, 100);

                    $(dropDown).css({
                        position: 'fixed', top: basetop, left: elleft, bottom: 'unset'
                    });


                });

            $element.parents().on('scroll', fixDateTimePickerPosition);
            $(window).on('scroll', fixDateTimePickerPosition);
            $scope.$on('start-drag', function (event, args) {
                fixDateTimePickerPosition();
            });
            function fixDateTimePickerPosition() {
                if (divDatePicker.data != undefined && divDatePicker.data("DateTimePicker"))
                    divDatePicker.data("DateTimePicker").hide();
            }
            ctrl.tabindex = "";
            setTimeout(function () {
                if ($($element).hasClass('divDisabled') || $($element).parents('.divDisabled').length > 0) {
                    ctrl.tabindex = "-1";
                }
            }, 10);

            var isUserChange = false;
            var selectedDate;
            divDatePicker
            .on('dp.change', function (e) {
                if (!e.date) {
                    $scope.ctrl.value = null;
                    return;
                }
                var dt = e.date;
                //selectedDate = convertUTCDateToLocalDate(new Date(dt));
                selectedDate = new Date(dt);
                //selectedDate.setSeconds(0);
                //selectedDate.setMilliseconds(0);
                var modelValue = $scope.ctrl.value;
                if (modelValue != undefined && !(modelValue instanceof Date))
                    modelValue = UtilsService.createDateFromString(modelValue);

                if (modelValue == undefined || modelValue.toString() != selectedDate.toString()) {
                    isUserChange = true;
                    var unspecifiedHour = 0;
                    var unspecifiedMinute = 0;
                    var unspecifiedSecond = 0;
                    var unspecifiedMillisecond = 0;
                    if ($attrs.applytimemax != undefined) {

                        unspecifiedHour = 23;
                        unspecifiedMinute = 59;
                        unspecifiedSecond = 59;
                        unspecifiedMillisecond = 998;
                    }
                    if ($attrs.type == "time" || $attrs.type == "hour") {
                        $scope.ctrl.value = {
                            $type: 'Vanrise.Entities.Time, Vanrise.Entities',
                            Hour: selectedDate.getHours(),
                            Minute: $attrs.type == "hour" ? unspecifiedMinute : selectedDate.getMinutes(),
                            Second: $attrs.type == "hour" ? unspecifiedSecond : selectedDate.getSeconds(),
                            Millisecond: $attrs.type == "hour" ? unspecifiedMillisecond : selectedDate.getMilliseconds()
                        };
                    }

                    else if ($attrs.type == "date") {
                        var date = new Date(selectedDate.setHours(unspecifiedHour, unspecifiedMinute, unspecifiedSecond, unspecifiedMillisecond));
                        //var date = moment.utc(selectedDate).format("L LT");
                        $scope.ctrl.value = date;
                    }
                    else if ($attrs.type == "dateHour") {
                        var date = new Date(selectedDate.setHours(selectedDate.getHours(), unspecifiedMinute, unspecifiedSecond, unspecifiedMillisecond));
                        //var date = moment.utc(selectedDate).format("L LT");
                        $scope.ctrl.value = date;
                    }
                    else if ($attrs.type == "yearMonth") {
                        selectedDate.setDate(1);
                        var date = new Date(selectedDate.setHours(unspecifiedMinute, unspecifiedSecond, unspecifiedMillisecond));
                        $scope.ctrl.value = date;
                    }
                    else if ($attrs.type == "longDateTime") {
                        var date = new Date(selectedDate.setHours(selectedDate.getHours(), selectedDate.getMinutes(), selectedDate.getSeconds(), unspecifiedMillisecond));
                        //var date = moment.utc(selectedDate).format("L LT");
                        $scope.ctrl.value = date;
                    }
                    else {
                        var date = new Date(selectedDate.setHours(selectedDate.getHours(), selectedDate.getMinutes(), unspecifiedSecond, unspecifiedMillisecond));
                        //var date = moment.utc(selectedDate).format("L LT");
                        $scope.ctrl.value = date;

                    }
                }

            });

            function cloneDateTime(date) {
                return new Date(date).toUTCString().replace(' Z', '');
            }
            function convertUTCDateToLocalDate(date) {
                //var newDate = new Date(date.getTime() - date.getTimezoneOffset() * 60 * 1000);

                //var offset = date.getTimezoneOffset() / 60;
                //var hours = date.getHours();

                //newDate.setHours(hours - offset);

                //return newDate;
            }


            var ctrl = $scope.ctrl;
            ctrl.placelHolder = ($attrs.placeholder != undefined) ? ctrl.placeholder : '';
            ctrl.updateModelOnKeyUp = function (e) {
                var $this = angular.element(e.currentTarget);
                setTimeout(function () {
                    if (moment($this.val(), format, true).isValid() && divDatePicker.data("DateTimePicker")) {
                        divDatePicker.data("DateTimePicker").date($this.val());
                    }
                }, 1);
            };

            if ($attrs.hint != undefined)
                ctrl.hint = $attrs.hint;
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
            };
            getInputeStyle();

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
                    var innerTooltipWidth = parseFloat(($(innerTooltip).width() / 2) + 2.5);
                    $(innerTooltip).css({
                        position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 15, left: selfOffset.left - innerTooltipWidth
                    });
                    $(innerTooltipArrow).css({
                        position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 10, left: selfOffset.left
                    });

                }, 1);
            };

            var updatewatch = $scope.$watch('ctrl.value', function (newValue, oldValue) {
                if (newValue == null && oldValue != null && $attrs.firechangeifempty != undefined) {
                    triggerOnDateTimeValueChangeEvent();
                }
                if (ctrl.value == null)
                    $element.find('#divDatePicker').find('.vr-date-input').val('');
                if (ctrl.value == undefined) {
                    selectedDate = undefined;
                    return;
                }
                var date;
                if ($attrs.type == "time" || $attrs.type == "hour") {
                    var initialDate = VRDateTimeService.getNowDateTime();
                    if (ctrl.value.Hour == undefined)
                        ctrl.value.Hour = 0;
                    if (ctrl.value.Minute == undefined || $attrs.type == "hour")
                        ctrl.value.Minute = 0;
                    if (ctrl.value.Second == undefined || $attrs.type == "hour")
                        ctrl.value.Second = 0;
                    if (ctrl.value.Millisecond == undefined || $attrs.type == "hour")
                        ctrl.value.Millisecond = 0;

                    initialDate.setHours(ctrl.value.Hour, ctrl.value.Minute, ctrl.value.Second, ctrl.value.Millisecond);
                    date = initialDate;
                    //var convertedDate = convertUTCDateToLocalDate(initialDate);
                    //date = convertedDate;
                }
                else {
                    date = ctrl.value instanceof Date ? ctrl.value : (UtilsService.createDateFromString(ctrl.value));

                }
                if ((selectedDate == undefined || (date != undefined && selectedDate.toString() != date.toString())) && divDatePicker.data("DateTimePicker")) {
                    divDatePicker.data("DateTimePicker").date(date);
                }
                else {
                    if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                        return;
                    isUserChange = false;//reset the flag
                    triggerOnDateTimeValueChangeEvent();
                }

            });
            function triggerOnDateTimeValueChangeEvent() {
                if ($attrs.onvaluechanged != undefined) {
                    var onvaluechangedMethod = $scope.$parent.$eval($attrs.onvaluechanged);
                    if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                        onvaluechangedMethod();
                    }
                }
            }
            $scope.ctrl.onBlurDirective = function (e) {
                var dateTab = new Date(ctrl.value).toDateString().split(" ");
                var year = parseInt(dateTab[3]);
                if ($attrs.type != "time" && $attrs.type != "hour" && (year < 1970 || isNaN(year)) && !ctrl.readOnly && ctrl.value != undefined) {
                    ctrl.value = VRDateTimeService.getNowDateTime();
                }
                if ($attrs.onblurdatetime != undefined) {
                    var onblurdatetimeMethod = $scope.$parent.$eval($attrs.onblurdatetime);
                    if (onblurdatetimeMethod != undefined && onblurdatetimeMethod != null && typeof (onblurdatetimeMethod) == 'function') {
                        onblurdatetimeMethod();
                    }
                }
            };
            $scope.ctrl.toggleDate = function (e) {

                e.preventDefault();
                e.stopPropagation();

                $('.date-section').addClass('in');
                $('.time-section').removeClass('in');

                // switch icon to time
                $('.btn-switcher').addClass("glyphicon-time");
                $('.btn-switcher').removeClass("glyphicon-calendar");
            };
            $scope.ctrl.toggleTime = function (e) {

                e.preventDefault();
                e.stopPropagation();
                $('.time-section').addClass('in');
                $('.date-section').removeClass('in');

                // switch icon to date
                $('.btn-switcher').removeClass("glyphicon-time");
                $('.btn-switcher').addClass("glyphicon-calendar");
            };
            //BaseDirService.addScopeValidationMethods(ctrl, elementName, this);

        },
        compile: function (element, attrs) {
            //var inputElement = element.find('#mainInput');
            //var validationOptions = {};
            //if (attrs.isrequired !== undefined)
            //    validationOptions.requiredValue = true;
            //if (attrs.customvalidate !== undefined)
            //    validationOptions.customValidation = true;

            //var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, element.find('#rootDiv'));
            return {
                pre: function ($scope, iElem, iAttrs) {
                    //var ctrl = $scope.ctrl;
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
            var n = 0;
            if (attrs.label != undefined)
                labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
            var icontemplate = "";
            if (attrs.type == 'date' || attrs.type == 'dateTime' || attrs.type == 'dateHour' || attrs.type == 'longDateTime' || attrs.type == 'yearMonth') {
                n++;
                icontemplate += ' <span   class="input-group-addon vr-small-addon vanrise-inpute" ng-click="::ctrl.toggleDate($event)" ><i class="glyphicon glyphicon-calendar" ></i></span>';

            }
            if (attrs.type == 'time' || attrs.type == 'dateTime' || attrs.type == 'dateHour' || attrs.type == 'longDateTime' || attrs.type == 'hour') {
                n++;
                icontemplate += ' <span   class="input-group-addon vr-small-addon vanrise-inpute" ng-click="::ctrl.toggleTime($event)" > <i class="glyphicon glyphicon-time" ></i></span>';

            }
            else if (attrs.type == undefined) {
                n = 2;
                icontemplate = ' <span  class="input-group-addon vr-small-addon vanrise-inpute" ng-click="::ctrl.toggleDate($event)" ><i class="glyphicon glyphicon-calendar" ></i></span>'
                             + ' <span  class="input-group-addon vr-small-addon vanrise-inpute" ng-click="::ctrl.toggleTime($event)" > <i class="glyphicon glyphicon-time" ></i></span>';

            }

            var iconposition = VRLocalizationService.isLocalizationRTL() ? 'left:' : 'right:-';
            var dateTemplate =
                 '<div   >'
                  + '<vr-validator validate="ctrl.validate()" vr-input>'
                  + '<div id="divDatePicker" ng-mouseenter="::(showtd=true)" ng-mouseleave="::(showtd=false)"  ng-model="ctrl.value" class="input-group form-control vr-datetime-container vanrise-inpute" >'
                            + '<input tabindex="{{ctrl.tabindex}}" ng-readonly="::ctrl.readOnly" class="vr-date-input vanrise-inpute" placeholder="{{::ctrl.placelHolder}}" ng-keyup="::ctrl.updateModelOnKeyUp($event)" ng-blur="::ctrl.onBlurDirective($event)" ng-class="showtd==true? \'fix-border-radius\':\'border-radius\'" data-autoclose="1" placeholder="Date" type="text" ctrltype="' + attrs.type + '">'
                            + '<div  ng-show="showtd && !ctrl.readOnly"  class="hand-cursor datetime-icon-container" style="max-width:' + 20 * n + 'px;' + iconposition + n * 10 + 'px;" >' + icontemplate + '</div>'
                      + '</div>'
                  + '</vr-validator>'
                      + '<span ng-if="(ctrl.hint!=undefined)"  ng-mouseenter="::ctrl.adjustTooltipPosition($event)" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor vr-hint-input"  html="true" placement="bottom" trigger="hover" data-type="info" data-title="{{ctrl.hint}}"></span>'
                + '</div>';

            //var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined);

            return startTemplate + labelTemplate + dateTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);