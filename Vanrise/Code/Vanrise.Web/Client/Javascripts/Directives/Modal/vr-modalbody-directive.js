'use strict';

app.directive('vrModalbody', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var maxHeightPart = "";
            if (tAttrs.maxheight != undefined)
                maxHeightPart = '\'max-height\': ' + tAttrs.maxheight + ', \'overflow\': \'auto\', \'padding\': \'15px\'';
            var newElement = '<div class="modal-body" ng-style="{ ' + maxHeightPart + ' }" >'
                                +  tElement.context.innerHTML                             
                            + '</div>';
            tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);