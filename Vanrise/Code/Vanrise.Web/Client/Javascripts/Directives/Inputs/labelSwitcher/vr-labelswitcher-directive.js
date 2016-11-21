'use strict';


app.directive('vrLabelswitcher', ['VRValidationService', 'BaseDirService', 'VRNotificationService', 'BaseAPIService', 'UtilsService', 'SecurityService', 'FileAPIService', function (VRValidationService, BaseDirService, VRNotificationService, BaseAPIService, UtilsService, SecurityService, FileAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            value: '=',
            hint: '@',
            modulename: '@',
            validationfunction: '='
        },
        controller: function ($scope, $element, $attrs,$timeout) {
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
                { id: 1, label: "item 1", selected: true },
                { id: 2, label: "item 2", selected: false },
                { id: 3, label: "item 3", selected: false },
                { id: 4, label: "item 4", selected: false },
               // { id: 5, label: "last", selected: false }
            ];
            $scope.items[$scope.items.length] = { id: -1, label: " " };
            $scope.setSelected = function (item, index) {


                $scope.nextindex = index + 1;
                $scope.selectedindex = index;

                if (index == $scope.items.length - 1) {
                    $scope.nextindex = 1;
                    $scope.selectedindex = 0;
                }


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
            var fileTemplate =
                 '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false" ng-class="isUploading == true? \'vr-disabled-div\':\'\'" >'
                  + '<vr-validator validate="ctrl.validate()">'
                     +  '<div id="mainInput" ng-model="ctrl.value" class="form-control vr-label-switcher-container"  style="border-radius: 4px;padding: 0px;">'

                       + '<div><div ng-repeat="item in items"   ng-class="$index ==  selectedindex? \'vr-label-switcher-selected\':\'vr-label-switcher-notselected\'" ng-if=" $index ==  selectedindex || $index ==  nextindex "  ng-click="setSelected(item,$index)" >{{item.label}}</div></div>'

                    + '</div>'
                  + '</vr-validator>'
                  + '<span  ng-if="ctrl.hint!=undefined" ng-mouseenter="ctrl.adjustTooltipPosition($event)" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor" style="color:#337AB7;" html="true" ng-mouseenter="ctrl.adjustTooltipPosition($event)" placement="bottom" trigger="hover" data-type="info" data-title="{{ctrl.hint}}"></span>'
            + '</div>';

            var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined);

            return startTemplate + labelTemplate + fileTemplate + validationTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);

