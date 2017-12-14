'use strict';

app.directive('vrCheck', ['SecurityService', 'UtilsService', 'VRLocalizationService', function (SecurityService, UtilsService, VRLocalizationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            value: '=',
            hint: '@',
            label: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var isUserChange;
            if (ctrl.value == undefined)
                ctrl.value = false;
            ctrl.readOnly = UtilsService.isContextReadOnly($scope) || $attrs.readonly != undefined;

            ctrl.toogleCheck = function (e) {
                if (ctrl.readOnly)
                    return;
                ctrl.value = !ctrl.value;
                isUserChange = true;
                if ($element.parents('.vr-datagrid-celltext').length > 0)
                    e.stopPropagation();
            };
            $scope.$watch('ctrl.value', function () {
                if (!isUserChange)//this condition is used because the event will occurs in two cases: if the user changed the value, and if the value is received from the view controller
                    return;
                isUserChange = false;//reset the flag
                if ($attrs.onvaluechanged != undefined) {
                    var onvaluechangedMethod = $scope.$parent.$eval($attrs.onvaluechanged);
                    if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                        onvaluechangedMethod();
                    }
                }
            });
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


        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            if (attrs.permissions === undefined || SecurityService.isAllowed(attrs.permissions)) {
              var label = VRLocalizationService.getResourceValue(attrs.localizedlabel, attrs.label);
                if (label == undefined)
                    label = '';
                return '<vr-label ng-if="withLable">' + label + '</vr-label><div><span ng-model="ctrl.value"  ng-click="ctrl.toogleCheck($event)" class="hand-cursor" style="font-weight: bold; font-size: 15px;" ng-style="ctrl.value==true? {\'color\':\'#64BD63\'} : {\'color\':\'#d2d2d2\'}" >✔</span>'
                    + '<span ng-if="hint!=undefined" ng-mouseenter="adjustTooltipPosition($event)" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor vr-hint-input" style="color:#337AB7;top:-1px" html="true" placement="bottom" trigger="hover" data-type="info" data-title="{{hint}}"></span>'
                    + '</div>';
            }
            else
                return "";

        }

    };

    return directiveDefinitionObject;

}]);