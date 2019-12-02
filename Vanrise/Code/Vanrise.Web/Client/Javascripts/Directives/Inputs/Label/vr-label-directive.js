﻿(function (app) {

    "use strict";

    app.directive('vrLabel', function () {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                color: "@"
            },
            controller: function ($scope) {
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (tElement, tAttrs) {
                var isStandalone = tAttrs.standalone;
                var isValue = tAttrs.isvalue != undefined;
                var color = (tAttrs.color != undefined) ? tAttrs.color : "";
                var hintSection = "";
                if (tAttrs.hint != undefined)
                    hintSection = '<vr-hint value="' + tAttrs.hint + '"></vr-hint>';

                var hasbackgroundcolorSection = "";
                if (tAttrs.hasbackgroundcolor != undefined)
                    hasbackgroundcolorSection = ' color:white';
                var clickclass = "";
                if (tAttrs.isclickable != undefined)
                    clickclass = " hand-cursor ";                
             
                var newElement = '<label  class="control-label vr-control-label ' + color + clickclass + ' " style="' + (isStandalone === "true" ? 'padding-top:6px;' : '') + (isValue ? 'font-weight:normal;' : '') + hasbackgroundcolorSection + ' " >'
                    + tElement.html() + '</label>' + hintSection;
                tElement.html(newElement);
            }

        };

        return directiveDefinitionObject;

    });

})(app);
