(function (app) {

    "use strict";

    vrTextboxMultiple.$inject = ['BaseDirService', 'TextboxTypeEnum', 'VRValidationService', 'UtilsService', 'VRLocalizationService'];

    function vrTextboxMultiple(BaseDirService, TextboxTypeEnum, VRValidationService, UtilsService, VRLocalizationService) {

        return {
            restrict: 'E',
            scope: {
                value: '=',
                hint: '@',
                minvalue: '@',
                maxvalue: '@',
                decimalprecision: '@',
                maxlength: '@',
                placeholder: '@',
                datatype: '@',
                label: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var validationOptions = {};
                if ($attrs.isrequired != undefined)
                    validationOptions.arrayValidation = true;

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

                var $quan = $('.next-input');
                $quan.on('keyup', function (e) {
                    if (e.which === 40) {
                        var ind = $quan.index(this);
                        $quan.eq(ind + 1).focus()
                    }
                    if (e.which === 38) {
                        var ind = $quan.index(this);
                        var el = $quan.eq(ind - 1);
                        if (el)
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
                };
            },
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs) {
                        $scope.$on("$destroy", function () {
                            valueWatch();
                        });
                        $scope.$on("$destroy", function () {
                            $(window).unbind('scroll', fixMultipleTextDropdownPosition);
                            $(window).unbind('resize', fixMultipleTextDropdownPosition);
                            $(window).off("resize.Viewport");
                            valueWatch();

                        });
                        iElem.on('$destroy', function () {
                            $('#' + ctrl.id).parents('div').unbind('scroll', fixMultipleTextDropdownPosition);
                        });


                        var ctrl = $scope.ctrl;

                        if (ctrl.value == undefined)
                            ctrl.value = [];
                        var isUserChange;
                        var valueWatch = $scope.$watch('ctrl.value', function (newValue, oldValue) {
                            if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                                return;

                            var retrunedValue;
                            isUserChange = false;//reset the flag


                            if (iAttrs.onvaluechanged != undefined) {
                                var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onvaluechanged);
                                if (onvaluechangedMethod != undefined && typeof (onvaluechangedMethod) == 'function') {
                                    onvaluechangedMethod(retrunedValue);
                                }
                            }

                        });
                        ctrl.id = BaseDirService.generateHTMLElementName();
                        ctrl.notifyUserChange = function () {
                            isUserChange = true;
                        };
                        ctrl.placelHolder = (attrs.placeholder != undefined) ? ctrl.placeholder : '';
                        ctrl.readOnly = UtilsService.isContextReadOnly($scope) || iAttrs.readonly != undefined;
                        if (attrs.hint != undefined) {
                            ctrl.hint = attrs.hint;
                        }

                        ctrl.muteAction = function (e) {
                            BaseDirService.muteAction(e);
                        };
                        var validationOptionsInput = {};
                        if (iAttrs.datatype === TextboxTypeEnum.Email.name || $scope.$parent.$eval(ctrl.datatype) === TextboxTypeEnum.Email.name)
                            validationOptionsInput.emailValidation = true;
                        if (iAttrs.datatype === TextboxTypeEnum.Ip.name || $scope.$parent.$eval(ctrl.datatype) === TextboxTypeEnum.Ip.name)
                            validationOptionsInput.ipValidation = true;
                        else if (iAttrs.datatype === TextboxTypeEnum.Number.name || $scope.$parent.$eval(ctrl.datatype) === TextboxTypeEnum.Number.name) {
                            validationOptionsInput.numberValidation = true;
                            validationOptionsInput.maxNumber = ctrl.maxvalue;
                            validationOptionsInput.minNumber = ctrl.minvalue;
                            validationOptionsInput.numberPrecision = ctrl.decimalprecision;
                        }
                        function IsInvalide(value) {
                            if (value == "" || value == undefined || value == null || VRValidationService.validate(value, $scope, { isrequired: false }, validationOptionsInput) != null || ctrl.value.indexOf(ctrl.valuetext) > -1
                                || (!isNaN(ctrl.valuetext) && ctrl.value.indexOf(parseInt(ctrl.valuetext)) > -1))
                                return true;

                            return false;
                        }
                        ctrl.validateInpute = function () {
                            return VRValidationService.validate(ctrl.valuetext, $scope, { isrequired: false }, validationOptionsInput);
                        };
                        ctrl.addNewValue = function () {
                            if (!IsInvalide(ctrl.valuetext)) {
                                ctrl.value.push(ctrl.valuetext);
                                ctrl.valuetext = null;
                            }

                        };
                        ctrl.removeValue = function (obj) {
                            var index = ctrl.value.indexOf(obj);
                            ctrl.value.splice(index, 1);
                        };

                        ctrl.clearAllSelected = function (e) {
                            BaseDirService.muteAction(e);
                            ctrl.value.length = 0;
                        };
                        ctrl.disabledAdd = function () {
                            return IsInvalide(ctrl.valuetext);
                        };

                        ctrl.getLabelText = function () {
                            if (ctrl.value == 0) return "Select";
                            else if (ctrl.value != undefined) return ctrl.value.join(";");
                        };
                        ctrl.adjustTooltipPosition = function (e) {
                            setTimeout(function () {
                                var self = angular.element(e.currentTarget);
                                var selfHeight = $(self).height();
                                var selfOffset = $(self).offset();
                                var tooltip = self.parent().find('.tooltip-info')[0];
                                $(tooltip).css({ display: 'block' });
                                var innerTooltip = self.parent().find('.tooltip-inner')[0];
                                var innerTooltipArrow = self.parent().find('.tooltip-arrow')[0];
                                var innerTooltipWidth = parseFloat(($(innerTooltip).width() / 2) + 2.5);
                                $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 15, left: selfOffset.left - innerTooltipWidth });
                                $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 10, left: selfOffset.left });

                            }, 1);
                        };

                        setTimeout(function () {
                            $('#' + ctrl.id).on('show.bs.dropdown', function () {
                                setTimeout(function () {
                                    $('#' + ctrl.id).find('input').first().focus();
                                }, 1);

                                var selfHeight = $(this).height();
                                var selfOffset = $(this).offset();
                                var basetop = selfOffset.top - $(window).scrollTop() + selfHeight;

                                var heigth = selfHeight + 120;

                                if (innerHeight - basetop < heigth) {
                                    var div = $(this).find('.dropdown-menu');
                                    var height = div.css({
                                        display: "block"
                                    }).height();

                                    div.css({
                                        overflow: "hidden",
                                        marginTop: height,
                                        height: 0
                                    }).animate({
                                        marginTop: 0,
                                        height: height
                                    }, 1000, function () {
                                        $(this).css({
                                            display: "",
                                            overflow: "",
                                            height: "",
                                            marginTop: ""
                                        });
                                    });

                                }
                                else
                                    $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
                            });

                            $('#' + ctrl.id).on('hide.bs.dropdown', function () {
                                $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
                            });
                        }, 100);
                        setTimeout(function () {

                            $('#' + ctrl.id).on('click', '.dropdown-toggle', function () {
                                $scope.$root.$broadcast("hide-all-select");
                                var self = $(this);
                                var selfHeight = $(this).parent().height();
                                var selfOffset = $(self).offset();
                                var dropDown = self.parent().find('ul');
                                var top = 0;
                                var basetop = selfOffset.top - $(window).scrollTop() + selfHeight;
                                var baseleft = selfOffset.left - $(window).scrollLeft();

                                var heigth = self.height() + 120;
                                if (innerHeight - basetop < heigth)
                                    top = basetop - (heigth + (selfHeight * 2.7));
                                else
                                    top = selfOffset.top - $(window).scrollTop() + selfHeight;

                                if (VRLocalizationService.isLocalizationRTL()) {
                                    baseleft += (self.parent().find('.dropdown-toggle').outerWidth() - self.width());
                                }

                                $scope.$root.$broadcast("hideallselect");
                                $(dropDown).css({ position: 'fixed', top: top, left: baseleft, width: self.width() });
                            });

                            $('#' + ctrl.id).parents('div').scroll(function () {
                                fixMultipleTextDropdownPosition();
                            });
                            $(window).scroll(function () {
                                fixMultipleTextDropdownPosition();
                            });
                            $(window).resize(function () {
                                fixMultipleTextDropdownPosition();
                            });

                        }, 1);

                        $scope.$on('start-drag', function (event, args) {
                            fixMultipleTextDropdownPosition();
                        });

                        $scope.$on('hide-all-menu', function (event, args) {
                            fixMultipleTextDropdownPosition();
                        });

                        var fixMultipleTextDropdownPosition = function () {
                            $('.vr-multiple').find('.dropdown-menu').hide();
                            $('#' + ctrl.id).removeClass("open");
                        };

                    },

                }
            },

            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                var startTemplate = '<div id="rootDiv" style="position: relative;">';
                var endTemplate = '</div>';

                var labelTemplate = '';
                var label = '';
                if (attrs.label != undefined) {
                    label = VRLocalizationService.getResourceValue(attrs.localizedlabel, attrs.label);
                    labelTemplate = '<vr-label>' + label + '</vr-label>';
                }
                var type = 'text';
                if (attrs.type != undefined && attrs.type === TextboxTypeEnum.Password.name)
                    type = 'password';
                var keypress = '';
                var keypressclass = '';
                if (attrs.tonextinput != undefined) {
                    keypressclass = 'next-input';
                }

                var textboxTemplate = '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false">'
                            + '<vr-validator validate="ctrl.validate()" vr-input >'
                            + '<div class="dropdown vr-multiple vanrise-inpute" id="{{ctrl.id}}" >'
                             + '<button  class="btn btn-default dropdown-toggle vanrise-inpute" style="width:100%;text-align: left;" type="button" data-toggle="dropdown" '
                                + ' aria-expanded="true"  >'
                                    + '<span class="vanrise-inpute"  style="float: left; margin: 0px;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;display: inline-block;width:calc(100% - 11px ); " ng-style="!ctrl.isHideRemoveIcon() ? {\'width\':\'calc(100% - 11px)\'}:{\'width\':\'100%\'} " > {{ctrl.getLabelText()}} </span>'
                                    + '<span class="caret vr-multiple-caret vanrise-inpute" ></span>'
                                + '</button>'
                                + '<ul class="dropdown-menu" role="menu" aria-labelledby="dropdownMenuCu-test" style="width : 100%">'
                                    + '<li role="presentation" ng-click="ctrl.muteAction($event);">'
                                        + '<div style="width : 100%"  ng-if="!ctrl.readOnly" > '
                                        + '<div style="display:inline-block;width:calc(100% - 30px );padding: 0px 3px;" >'
                                                + ' <vr-textbox value="ctrl.valuetext" type="ctrl.datatype" customvalidate="ctrl.validateInpute()" onenterpress="ctrl.addNewValue"></vr-textbox>'
                                         + '</div>'
                                        + '<div style="display:inline-block;width:30px" ><vr-button vr-disabled="ctrl.disabledAdd()" type="Add"  standalone data-onclick="ctrl.addNewValue" ></vr-button></div>'
                                         + '</div>'
                                        + '<div style="width : 100%" > '
                                        + '<div  calss="items-container" >'
                                            + '<div style="padding:5px ;height:29px">'
                                                + 'Values:'
                                                + '<a href="" class="link" style="position: absolute; right: 5px;" ng-click="ctrl.clearAllSelected($event); "  ng-if="!ctrl.readOnly">'
                                                + ' Clear all'
                                                + '<i class="glyphicon glyphicon-trash hand-cursor "></i>'
                                                + '</a>'
                                            + ' </div>'
                                            + ' <div style="height: 100px; overflow: auto;padding-top:5px">'
                                               + '<span class="label custom-label" ng-repeat="obj in ctrl.value track by $index">'
                                                  + '{{obj}} '
                                                   + '<span class="glyphicon glyphicon-remove hand-cursor" aria-hidden="true" ng-click="ctrl.removeValue(obj);"  ng-if="!ctrl.readOnly"></span>'
                                                + '</span>'
                                            + '</div>'
                                       + '</div>'
                                    + '</li>'
                                + '</ul>'
                            + '</div>'
                            + '</vr-validator>'
                            + '<span ng-if="ctrl.hint!=undefined"  bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor vr-hint-input" style="right: 1px;left: 1px;" html="true"  placement="bottom"  trigger="hover" ng-mouseenter="ctrl.adjustTooltipPosition($event)"  data-type="info" data-title="{{ctrl.hint}}"></span>';

                ;

                return startTemplate + labelTemplate + textboxTemplate + endTemplate;
            }

        };

    }

    app.directive('vrTextboxMultiple', vrTextboxMultiple);

    app.constant('TextboxTypeEnum', {
        Email: { name: "email" },
        Ip: { name: "ip" },
        Number: { name: "number" },
        Password: { name: "password" }
    });

})(app);



