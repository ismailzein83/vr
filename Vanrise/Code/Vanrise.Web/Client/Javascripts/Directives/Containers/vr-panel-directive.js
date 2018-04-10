'use strict';

app.directive('vrPanel', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var newElement = '<div class="panel panel-primary panel-over-color"><div class="panel-body">' + tElement.html() + '</div></div>';
            tElement.html(newElement);
        }

    };

    return directiveDefinitionObject;

}]);