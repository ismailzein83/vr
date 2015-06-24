﻿'use strict';

app.directive('vrColumns', ['$compile', function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {},
        compile: function (tElement, tAttrs) {
            var numberOfColumns = tAttrs.colnum;
            if (numberOfColumns == undefined)
                numberOfColumns = 1;

            if (tAttrs.width != undefined) {
                switch(tAttrs.width)
                {
                    case "normal": numberOfColumns = 2; break;
                    case "small": numberOfColumns = 1; break;
                    case "large": numberOfColumns = 3; break;
                    case "fullrow": numberOfColumns = 12; break;
                    case "1/2row": numberOfColumns = 6; break;
                    case "1/4row": numberOfColumns = 3; break;
                    case "1/3row": numberOfColumns = 4; break;
                }
            }

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