'use strict';

app.directive('vrRow', ['$compile', function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var removeline = tAttrs.removeline;
            var newElement = '<div class="row' + (removeline != undefined ? ' remove-line' : ' style-row') + '">' + tElement.html() + '</div>';
            //if (removeline === undefined)
            //    newElement += '<div style="padding: 0px 10px;"><div style="width:100%; border-bottom : 1px solid #F0F0F0;"> </div></div>';
            tElement.html(newElement);            
        }
    };

    return directiveDefinitionObject;

}]);