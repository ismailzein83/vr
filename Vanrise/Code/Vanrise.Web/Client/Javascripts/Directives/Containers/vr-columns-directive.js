'use strict';

app.directive('vrColumns', ['$compile', function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {},
        compile: function (tElement, tAttrs) {
            var numberOfColumns = tAttrs.colnum;
            if (numberOfColumns == undefined)
                numberOfColumns = 1;
            var nbrcolsm = (numberOfColumns <= 3) ? numberOfColumns * 2 : 12;

            if (tAttrs.colnumsm != undefined) {
                nbrcolsm = tAttrs.colnumsm
            }
            var otherCol = ' col-md-' + numberOfColumns + ' col-sm-' + nbrcolsm;
            var newElement = '<div class="col-lg-' + numberOfColumns + otherCol + ' " >' + tElement.context.innerHTML + '</div>';
            tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);