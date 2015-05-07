'use strict';

app.directive('vrSection', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var newElement = '<div class="panel-primary panel-vr"><div class="panel-heading">' + tAttrs.title + '</div><div class="panel-body">' + tElement.context.innerHTML + '</div></div>';
            tElement.html(newElement);
        }

    };

    return directiveDefinitionObject;

}]);