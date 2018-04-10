'use strict';

app.directive('vrModalbody', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var maxHeightPart = "";
            var draggablemodal = "draggablemodal";

            var maxHeigth = window.innerHeight - 122 ;

            //if (tAttrs.maxheight != undefined)
            //    maxHeigth = tAttrs.maxheight;

            maxHeightPart = '\'max-height\': ' + maxHeigth + ', \'overflow\': \'auto\',\'overflow-x\': \'hidden\', \'padding\': \'15px\'';
            if (tAttrs.stopdrag != undefined)
                draggablemodal = '';

            var newElement = '<div class="modal-body"  ' + draggablemodal + ' ng-style="{ ' + maxHeightPart + ' }" >'
                                +  tElement.html()                             
                            + '</div>';
            tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);