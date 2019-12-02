(function (app) {

    "use strict";

    app.directive('vrLabelDynamic', function ($compile) {

        var directiveDefinitionObject = {
            restrict: 'E',
            transclude: true,
            scope: {
                color: "@",
                onlabelclick: "="
            },
            controller: function ($scope) {
                var ctrl = this;
                ctrl.onLabelClickHandler = function () {
                    if (ctrl.onlabelclick != undefined && typeof (ctrl.onlabelclick) == "function") ctrl.onlabelclick();
                };

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {

                var isStandalone = attrs.standalone;
                var isValue = attrs.isvalue != undefined;
                var color = (attrs.color != undefined) ? attrs.color : "";
                var hintSection = "";
                if (attrs.hint != undefined)
                    hintSection = '<vr-hint value="' + attrs.hint + '"></vr-hint>';

                var hasbackgroundcolorSection = "";
                if (attrs.hasbackgroundcolor != undefined)
                    hasbackgroundcolorSection = ' color:white';
                var clickclass = "";
                if (attrs.isclickable != undefined)
                    clickclass = " hand-cursor ";

               
                var newElement = '<label ng-click="ctrl.onLabelClickHandler()" ng-class="ctrl.onlabelclick != undefined ? \'clickable\':\'\'"  class="control-label vr-control-label ' + color + clickclass + ' " style="' + (isStandalone === "true" ? 'padding-top:6px;' : '') + (isValue ? 'font-weight:normal;' : '') + hasbackgroundcolorSection + ' " >'
                + ' <span ng-transclude></span></label>' + hintSection;
                
                return newElement;
            }

        };

        return directiveDefinitionObject;

    });

})(app);
