'use strict';

app.directive('vrColumns', ['$compile', function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {},
        compile: function (tElement, tAttrs) {
            var numberOfColumns = tAttrs.colnum;
                if (numberOfColumns == undefined)
                    numberOfColumns = 1;
              
                var noPadding = (tAttrs.nopadding !== undefined) ? "col-no-padding" : "style-col ";
                var noMargin = (tAttrs.nomargin !== undefined) ? "" : " style-col ";
                var newElement = '<div class="col-lg-' + numberOfColumns + noMargin + noPadding + '">' + tElement.context.innerHTML + '</div>';
                tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);