'use strict';


app.directive('vrForm', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var newElement = '<form name="' + tAttrs.name + '"  novalidate >' + tElement.context.innerHTML + '</form>';
            tElement.html(newElement);            
        }
    };

    return directiveDefinitionObject;

}]);