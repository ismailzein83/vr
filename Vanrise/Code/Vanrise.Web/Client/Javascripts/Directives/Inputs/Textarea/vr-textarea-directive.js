(function (app) {

    "use strict";

    vrTextarea.$inject = ['BaseDirService', 'VRValidationService', 'UtilsService','VRLocalizationService'];

    function vrTextarea(BaseDirService, VRValidationService, UtilsService, VRLocalizationService) {

        return {
            restrict: 'E',
            scope: {
                value: '=',
                hint: '@',
                placeholder:'@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.validate = function () {
                    return VRValidationService.validate(ctrl.value, $scope, $attrs);
                };
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
                        var ctrl = $scope.ctrl;
                        $scope.$on("$destroy", function () {
                            valueWatch();
                        });
                        var isUserChange;

                        var valueWatch = $scope.$watch('ctrl.value', function (newValue, oldValue) {

                            if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                                return;

                            if (newValue == "") {
                                ctrl.value = undefined;
                            }
                            isUserChange = false;//reset the flag

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
                        ctrl.readOnly = UtilsService.isContextReadOnly($scope) || iAttrs.readonly != undefined;
                        ctrl.placelHolder = (attrs.placeholder != undefined) ? ctrl.placeholder : '';

                        if (attrs.hint != undefined) {
                            ctrl.hint = attrs.hint;
                        }
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
                var label = "";

                if (attrs.label != undefined)
                    label = VRLocalizationService.getResourceValue(attrs.localizedlabel, attrs.label);
                 
                if (attrs.label != undefined)
                    labelTemplate = '<vr-label>' + label + '</vr-label>';
               

                var rows = 3;
                if ($(element).parents('.vr-datagrid-celltext').length > 0)
                    rows = 2;
                if (attrs.rows != undefined)
                    rows = attrs.rows;
                var textboxTemplate = '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false" >'
                        + '<vr-validator validate="ctrl.validate()" vr-textarea-element>'
                        + '<textarea tabindex="{{ctrl.tabindex}}" readonly="ctrl.readOnly"  placeholder="{{ctrl.placelHolder}}" ng-readonly="ctrl.readOnly" id="mainInput"  ng-model="ctrl.value" ng-change="ctrl.notifyUserChange()" rows="' + rows + '" class="form-control vanrise-inpute" style="width: 100%; resize: none;" ></textarea>'
                        + '</vr-validator>'
                        + '<span ng-if="ctrl.hint!=undefined" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor vr-hint-input" html="true"   placement="bottom"  trigger="hover" ng-mouseenter="ctrl.adjustTooltipPosition($event)"  data-type="info" data-title="{{ctrl.hint}}"></span>';
                + '</div>';


                //var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined ,true);

                return startTemplate + labelTemplate + textboxTemplate + endTemplate;
            }

        };

    }

    app.directive('vrTextarea', vrTextarea);
    
})(app);



