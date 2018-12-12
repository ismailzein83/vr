﻿'use strict';

app.directive('vrModalbody', ['MobileService', function (MobileService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var maxHeightPart = "";
            var draggablemodal = MobileService.isMobile() ? "" : "draggablemodal";

            var modalHeightTrim = MobileService.isMobile() ? 63 : 122;
            var maxHeigth = window.innerHeight - modalHeightTrim;

            //if (tAttrs.maxheight != undefined)
            //    maxHeigth = tAttrs.maxheight;

            var isMenuPopup = tElement.parents(".modal-dialog").hasClass("centered-model");
            if (tAttrs.stopdrag != undefined)
                draggablemodal = '';
            
            if (MobileService.isMobile() && isMenuPopup && tAttrs.notification == undefined) {
                maxHeigth -= 30;
            }

            maxHeightPart = '\'max-height\': ' + maxHeigth + ', \'overflow\': \'auto\',\'overflow-x\': \'hidden\', \'padding\': \'15px\'';

            if (MobileService.isMobile() && !isMenuPopup && tAttrs.notification == undefined)
                maxHeightPart += ',\'height\': \'100vh\',\'width\': \'100%\'';

            if (MobileService.isMobile() && isMenuPopup && tAttrs.notification == undefined) {
                maxHeightPart += ',\'width\': \'calc(100% - 5px)\'';
            }

            var newElement = '<div class="modal-body"  ' + draggablemodal + ' ng-style="{ ' + maxHeightPart + ' }" >'
                                + tElement.html()
                            + '</div>';
            tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);