(function (app) {

    "use strict";

    app.directive('vrLabel', function () {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                color:"@"
            },
            controller: function () {

            },
            compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, formCtrl) {
                    var ctrl = $scope.ctrl;
                    
                    if (attrs.hint != undefined) {
                        ctrl.hint = attrs.hint;
                    }
                   
                    ctrl.adjustTooltipPosition = function (e) {
                        setTimeout(function () {
                            var self = angular.element(e.currentTarget);
                            var selfHeight = $(self).height();
                            var selfOffset = $(self).offset();
                            var tooltip = self.parent().find('.tooltip-info')[0];
                            $(tooltip).css({  position: 'fixed', top: selfOffset.top - $(window).scrollTop() + 5 , left: selfOffset.left - 10 });
                            var innerTooltipArrow = self.parent().find('.tooltip-arrow')[0];
                            $(innerTooltipArrow).css({   left: 10 });

                        }, 1)
                    }
                }
            }
        },

        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            var isStandalone = attrs.standalone;
            var isValue = attrs.isvalue != undefined;
            var color = (attrs.color != undefined) ? attrs.color : "";
            var newElement = '<label class="control-label vr-control-label ' + color +' " style="' + (isStandalone === "true" ? 'padding-top:6px;' : '') + (isValue ? 'font-weight:normal;' : '') + '" >'
                    + element.context.innerHTML + '</label><span ng-if="ctrl.hint!=undefined"  bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor" html="true" style="color:#337AB7;right: -10px;"  placement="bottom"  trigger="hover" ng-mouseenter="ctrl.adjustTooltipPosition($event)"  data-type="info" data-title="{{ctrl.hint}}"></span>';


            return newElement;
        }

        };

        return directiveDefinitionObject;

    });

})(app);


