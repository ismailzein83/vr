'use strict';

app.directive('vrColumns', ['$compile', function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {},
        compile: function (tElement, tAttrs) {
            var numberOfColumns = tAttrs.colnum;
                if (numberOfColumns == undefined)
                    numberOfColumns = 1;               
                var newElement = '<div class="col-lg-' + numberOfColumns + ' style-col">' + tElement.context.innerHTML + '</div>';
                tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);