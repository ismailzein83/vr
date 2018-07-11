'use strict';

app.directive('vrModalbody', ['MobileService', function (MobileService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var maxHeightPart = "";
            var draggablemodal = MobileService.isMobile() ? "" : "draggablemodal";

            var modalHeightTrim = MobileService.isMobile() ? 100 : 122;
            var maxHeigth = window.innerHeight - modalHeightTrim;

            //if (tAttrs.maxheight != undefined)
            //    maxHeigth = tAttrs.maxheight;

            maxHeightPart = '\'max-height\': ' + maxHeigth + ', \'overflow\': \'auto\',\'overflow-x\': \'hidden\', \'padding\': \'15px\'';
            if (tAttrs.stopdrag != undefined)
                draggablemodal = '';

            var newElement = '<div class="modal-body"  ' + draggablemodal + ' ng-style="{ ' + maxHeightPart + ' }" >'
                                + tElement.html()
                            + '</div>';
            tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);