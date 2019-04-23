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


app.directive('vrScrollableContainer', ['$compile', 'MobileService', function ($compile, MobileService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var margin = parseInt(tAttrs.margin);
            var containerHeightTrim = MobileService.isMobile() ? 63 : 122;
            var maxHeigth = window.innerHeight - (containerHeightTrim + margin);
            var finalMaxHeigth = maxHeigth > 200 && maxHeigth || 200;
            var newElement = '<div style="max-height:' + finalMaxHeigth + 'px;overflow-y:auto;overflow-x:hidden;">' + tElement.html() + '</div>';
            tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);