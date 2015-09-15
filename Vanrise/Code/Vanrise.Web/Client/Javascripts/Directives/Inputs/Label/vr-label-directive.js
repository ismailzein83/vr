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
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (tElement, tAttrs) {
                var isStandalone = tAttrs.standalone;
                var isValue = tAttrs.isvalue != undefined;
                var color = (tAttrs.color != undefined) ? tAttrs.color : "";
                var newElement = '<label class="control-label vr-control-label ' + color +' " style="' + (isStandalone === "true" ? 'padding-top:6px;' : '') + (isValue ? 'font-weight:bold;' : '') + '" >'
                    + tElement.context.innerHTML + '</label>';
                tElement.html(newElement);
            }
        };

        return directiveDefinitionObject;

    });

})(app);


