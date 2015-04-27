'use strict';

app.directive('vrColumns', ['$compile', function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {},
        compile: function (tElement, tAttrs) {
            var numberOfColumns = tAttrs.colnum;
                if (numberOfColumns == undefined)
                    numberOfColumns = 1;
              
                var noPadding = (tAttrs.nopadding !== undefined) ? " col-no-padding " : " style-col ";
                var noMargin = (tAttrs.nomargin !== undefined) ? "" : " style-col ";
                if (tAttrs.simplecol !== undefined) {
                    noPadding = "";
                    noMargin = "";
                }
                var otherCol = ' col-md-' + numberOfColumns + ' col-sm-' + numberOfColumns *2;
                var newElement = '<div class="col-lg-' + numberOfColumns + noMargin + noPadding + otherCol + ' ">' + tElement.context.innerHTML + '</div>';
                tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);