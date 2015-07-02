'use strict';

app.directive('vrRow', ['$compile', function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var removeline = tAttrs.removeline;
            var newElement = '<div class="row' + (removeline != undefined ? ' remove-line' : ' style-row') + '">' + tElement.context.innerHTML + '</div>';
            tElement.html(newElement);            
        }
    };

    return directiveDefinitionObject;

}]);