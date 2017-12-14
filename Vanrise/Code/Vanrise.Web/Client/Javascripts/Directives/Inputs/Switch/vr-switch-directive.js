'use strict';

app.directive('vrSwitch', ['SecurityService', 'UtilsService','VRLocalizationService', function (SecurityService, UtilsService, VRLocalizationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            value: '=',
            hint: '@'
        },
        controller: function ($scope, $element, $attrs) {
          
            $scope.adjustTooltipPosition = function (e) {
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
            var ctrl = this;
            if (ctrl.value == undefined)
                ctrl.value = false;
        },
        link: function (scope, element, attrs, ctrl) {
            scope.$on("$destroy", function () {
                valueWatch();
            });
            ctrl.readOnly = UtilsService.isContextReadOnly(scope) || attrs.readonly != undefined;

            var isUserChange;
            var valueWatch = scope.$watch('ctrl.value', function (newValue, oldValue) {
                if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                    return;
                isUserChange = false;//reset the flag
                if (attrs.onvaluechanged != undefined) {
                    var onvaluechangedMethod = scope.$parent.$eval(attrs.onvaluechanged);
                    if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                        onvaluechangedMethod();
                    }
                }
            });
            ctrl.toogleCheck = function () {
                if (ctrl.readOnly)
                    return;
                ctrl.value = !ctrl.value;
                isUserChange = true;

            };
            scope.withLable = false;
            if (attrs.label != undefined)
                scope.withLable = true;

            if (attrs.hint != undefined)
                scope.hint = attrs.hint;
          

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            if (attrs.permissions === undefined || SecurityService.isAllowed(attrs.permissions)) {
                var label = "";

                if (attrs.label != undefined) {
                    label = VRLocalizationService.getResourceValue(attrs.localizedlabel, attrs.label);
                }


                var onspan = '';
                if (attrs.on != undefined)
                    onspan = '<span class="on">' + attrs.on + '</span>';
                var offspan = '';
                if (attrs.off != undefined)
                    offspan ='<span class="off">' + attrs.off + '</span>';

                var template = '<vr-label ng-if="withLable">' + label + '</vr-label>'
                               + '<div class="vr-switch vanrise-inpute">'
                                   + '<span  class="switch green" ng-class="ctrl.value == true? \'checked\':\'\'" ng-click="ctrl.toogleCheck()">'
                                   + '<small></small>'
                                   + '<input type="checkbox" ng-model="ctrl.value" style="display:none;"/>'
                                   + '</span>'
                                   + '<span class="switch-text">'
                                   + onspan + offspan
                                   + '</span>'
                                   + '<span ng-if="hint!=undefined" ng-mouseenter="adjustTooltipPosition($event)" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor vr-hint-input"  html="true" placement="bottom" trigger="hover" data-type="info" data-title="{{hint}}"></span>'
                               + '</div>';
                return template;
            }
            else
                return "";

        }

    };

    return directiveDefinitionObject;

}]);