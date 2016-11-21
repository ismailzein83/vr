'use strict';

app.directive('vrHint', ['SecurityService', function (SecurityService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            value:'@'
        },
        controller: function ($scope, $element) {

            $scope.adjustTooltipPosition = function (e) {
                setTimeout(function () {
                    var self = angular.element(e.currentTarget);
                    var selfHeight = $(self).height();
                    var selfOffset = $(self).offset();
                    var tooltip = self.parent().find('.tooltip-info')[0];
                    $(tooltip).css({ display: 'block !important' });
                    var innerTooltip = self.parent().find('.tooltip-inner')[0];
                    var innerTooltipArrow = self.parent().find('.tooltip-arrow')[0];
                    $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 20, left: selfOffset.left - 5 });
                    $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 15, left: selfOffset.left });
                }, 1);
            };
        },
        link: function (scope, element, attrs, ctrl) {
            if (attrs.value != undefined)
                scope.value = attrs.value;
           
            
        },
        template: function (element, attrs) {
            if (attrs.permissions === undefined || SecurityService.isAllowed(attrs.permissions)) {
               
                return '<span ng-if="value!=undefined" ng-mouseenter="adjustTooltipPosition($event)" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor" style="color:#337AB7;top:2px" html="true" placement="bottom" trigger="hover" data-type="info" data-title="{{value}}"></span>';
            }
            else
                return "";
            
        }

    };

    return directiveDefinitionObject;

}]);