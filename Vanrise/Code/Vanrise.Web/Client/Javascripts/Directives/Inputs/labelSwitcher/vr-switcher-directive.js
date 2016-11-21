
'use strict';

app.directive('vrSwitcher', ['VRValidationService', 'BaseDirService', 'VRNotificationService', 'BaseAPIService', 'UtilsService', 'SecurityService', 'FileAPIService', function (VRValidationService, BaseDirService, VRNotificationService, BaseAPIService, UtilsService, SecurityService, FileAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            value: '=',
            hint: '@',
            modulename: '@',
            validationfunction: '='
        },
        controller: function ($scope, $element, $attrs, $timeout) {
            var ctrl = this;

            ctrl.validate = function () {
                return VRValidationService.validate(ctrl.value, $scope, $attrs);
            };
            ctrl.tabindex = "";
            setTimeout(function () {
                if ($($element).hasClass('divDisabled') || $($element).parents('.divDisabled').length > 0) {
                    ctrl.tabindex = "-1"
                }
            }, 10);



            var isInternalSetValue;
            $scope.selectedindex = 0;
            $scope.nextindex = 1;
            $scope.items = [
                { id: 1, label: "item 1" },
                { id: 2, label: "item 2" },
                { id: 3, label: "item 3" },
                { id: 4, label: "item 4" },
                { id: 5, label: "item 5" },
                { id: 6, label: "item 6" }

            ];
            var firsttime = true;
            ctrl.increment = function () {

                if ($scope.selectedindex + 1 == $scope.items.length) {
                    $scope.selectedindex = 0;
                }
                else
                    $scope.selectedindex = $scope.selectedindex + 1;

            };
            ctrl.decrement = function () {

                if ($scope.selectedindex == 0) {
                    $scope.selectedindex = $scope.items.length - 1;
                }
                else
                    $scope.selectedindex = $scope.selectedindex - 1;

            };
            $scope.getClass = function (index) {

                if (index == $scope.selectedindex) {
                    return "vr-label-switcher-selected bgcolor-0";
                }
                if (index == $scope.nextindex) {
                    return "vr-label-switcher-notselected  bgcolor-1";
                }
                return "am-slide-top";
            };


            $scope.$watch('ctrl.value', function () {


            });

            if ($attrs.hint != undefined)
                ctrl.hint = $attrs.hint;

            var getInputeStyle = function () {
                var div = $element.find('div[validator-section]')[0];
                if ($attrs.hint != undefined) {
                    $(div).css({ "display": "inline-block", "width": "calc(100% - 15px)", "margin-right": "1px" });
                }
            };
            getInputeStyle();

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

            function getModuleName() {
                return (ctrl.modulename == undefined || ctrl.modulename == null) ? null : ctrl.modulename;
            }
        },
        controllerAs: 'ctrl',
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
                        }, 1);
                    };

                    //BaseDirService.addScopeValidationMethods(ctrl, elementName, formCtrl);

                }
            };
        },
        bindToController: true,
        template: function (element, attrs) {
            var startTemplate = '<div id="rootDiv" style="position: relative;">';
            var endTemplate = '</div>';

            var labelTemplate = '';
            if (attrs.label != undefined)
                labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
            var fileTemplate = '<div  class="vr-numeric form-control  border-radius input-box" >'
                    + '<vr-validator validate="ctrl.validate()">'
                       + '<div ng-repeat="item in items"   ng-show=" $index ==  selectedindex "  >{{item.label}}</div>'
                                + '</vr-validator>'
                        + '<span ng-if="ctrl.hint!=undefined"  bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor" html="true" style="color:#337AB7;right: -10px;"  placement="bottom"  trigger="hover" ng-mouseenter="ctrl.adjustTooltipPosition($event)"  data-type="info" data-title="{{ctrl.hint}}"></span>'
                        + '<div class="vr-numeric-control" ng-class="ctrl.getNumericControlClass()">'
                        + '<span class="unit" ng-bind="ctrl.unitValue"></span>'
                        + '<div class="hand-cursor arrow-box" ng-click="ctrl.increment()" ng-style="{\'color\':ctrl.upColor}">'
                        + '<div class="caret-up" ></div>'
                        + '</div>'
                        + '<div class="hand-cursor arrow-box " ng-click="ctrl.decrement()" ng-style="{\'color\':ctrl.downColor}">'
                        + '<div class="caret-down"></div>'
                        + '</div>{{}}'
                            + '</div>'
                        + '</div>';

            var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined);

            return startTemplate + labelTemplate + fileTemplate + validationTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);