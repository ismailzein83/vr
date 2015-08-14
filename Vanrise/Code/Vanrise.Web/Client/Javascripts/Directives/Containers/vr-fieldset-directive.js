'use strict';

app.directive('vrFieldset', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var newElement = '<div class="panel-primary fieldset-vr"><div class="panel-heading"><span class="title">' + tAttrs.title + '</span></div><div class="panel-body">' + tElement.context.innerHTML + '</div></div>';
            tElement.html(newElement);
        }

    };

    return directiveDefinitionObject;

}]);