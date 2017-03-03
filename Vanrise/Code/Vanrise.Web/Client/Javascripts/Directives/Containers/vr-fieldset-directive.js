'use strict';

app.directive('vrFieldset', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var title = tAttrs.header || tAttrs.title;
            var newElement = '<div class="panel-primary fieldset-vr"><div class="panel-heading"><span class="title">' + title + '</span></div><div class="panel-body">' + tElement.context.innerHTML + '</div></div>';
            tElement.html(newElement);
        }

    };

    return directiveDefinitionObject;

}]);