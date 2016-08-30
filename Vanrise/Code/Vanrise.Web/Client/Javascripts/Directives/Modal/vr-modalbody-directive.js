'use strict';

app.directive('vrModalbody', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var maxHeightPart = "";
            var draggablemodal = "draggablemodal";
            if (tAttrs.maxheight != undefined)
                maxHeightPart = '\'max-height\': ' + tAttrs.maxheight + ', \'overflow\': \'auto\', \'padding\': \'15px\'';

            if (tAttrs.stopdrag != undefined)
                draggablemodal = '';

            var newElement = '<div class="modal-body"  ' + draggablemodal + ' ng-style="{ ' + maxHeightPart + ' }" >'
                                +  tElement.context.innerHTML                             
                            + '</div>';
            tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);