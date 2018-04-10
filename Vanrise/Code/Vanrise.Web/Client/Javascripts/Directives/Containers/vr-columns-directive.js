(function (app) {

    "use strict";

    app.directive('vrColumns', function () {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {},
            compile: function (tElement, tAttrs) {
                var numberOfColumns = tAttrs.colnum;
                if (numberOfColumns == undefined)
                    numberOfColumns = 1;

                if (tAttrs.width != undefined) {
                    switch (tAttrs.width) {
                        case "normal": numberOfColumns = 2; break;
                        case "small": numberOfColumns = 1; break;
                        case "large": numberOfColumns = 3; break;
                        case "fullrow": numberOfColumns = 12; break;
                        case "1/2row": numberOfColumns = 6; break;
                        case "1/4row": numberOfColumns = 3; break;
                        case "3/4row": numberOfColumns = 9; break;
                        case "1/3row": numberOfColumns = 4; break;
                        case "2/3row": numberOfColumns = 8; break;
                    }
                }

                var nbrcolsm = (numberOfColumns <= 3) ? numberOfColumns * 2 : 12;

                if (tAttrs.colnumsm != undefined) {
                    nbrcolsm = tAttrs.colnumsm;
                }
                var emptyline = "";
                if (tAttrs.withemptyline != undefined) {
                    emptyline = " empty-line-col ";
                }
                var childcolumnsclass = "";
                if (tAttrs.haschildcolumns != undefined) {
                    childcolumnsclass = " parent-col-container ";
                }
                var otherCol = ' col-md-' + numberOfColumns + ' col-sm-' + nbrcolsm;
                var newElement = '<div class="col-lg-' + numberOfColumns + otherCol + emptyline + childcolumnsclass  + ' " >' + tElement.html() + '</div>';
                tElement.html(newElement);
            }
        };

        return directiveDefinitionObject;

    });


})(app);



