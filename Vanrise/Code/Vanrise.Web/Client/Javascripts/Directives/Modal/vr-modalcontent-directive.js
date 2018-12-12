﻿'use strict';

app.directive('vrModalcontent', ['VRLocalizationService', 'MobileService', function (VRLocalizationService, MobileService) {

    var directiveDefinitionObject = {
        restrict: 'A',
        scope: false,
        compile: function (tElement, tAttrs) {
            tElement.attr('class', "modal");
            tElement.attr['tabindex'] = "-1";
            tElement.attr['role'] = "dialog";
            tElement.attr['aria-hidden'] = "true";
            var resClass = "";
            var classmodal = "";
            var titleShowValue= 'ng-show="title"'
            if (tAttrs.resclass != undefined)
                resClass = 'ng-class="' + tAttrs.resclass + '"';
            else {
                var num;
                if (tAttrs.width != undefined) {
                    num = parseFloat(tAttrs.width.substring(1, tAttrs.width.length - 1));
                }
                else
                    num = 50;
                var sizeOptions = {
                    small: "vr-modal-sm",
                    medium: "vr-modal-md",
                    large: "vr-modal-lg",
                    xlarge: "vr-modal-xl"
                };
                if (tAttrs.size != undefined) {
                    classmodal = sizeOptions[tAttrs.size];
                }
                else {
                    classmodal = "vr-modal-sm";
                    if (num >= 50)
                        classmodal = "vr-modal-md";
                    if (num >= 80)
                        classmodal = "vr-modal-lg";
                    if(num > 90)
                      classmodal = "vr-modal-xl";

                }
            }
            var modalCssClass = "";
            if (MobileService.isMobile() ) {
                if (tAttrs.ismenu != undefined){
                    titleShowValue = "";
                    modalCssClass = "centered-model";
                }
                else if  (tAttrs.textboxmultiple != undefined)
                    titleShowValue = "";
                else
                    modalCssClass = "full-view";
            }
            var style = "";
            var direction = VRLocalizationService.isLocalizationRTL() && 'right' || 'left';
               if ($('.modal-dialog').length > 0) {
                if (!MobileService.isMobile()) {
                    style = "top:" + ($('.modal-dialog').length) * 10 + "px;" + direction + ":" + ($('.modal-dialog').length) * 10 + "px;";
                    if ($('.modal-header').eq($('.modal-dialog').length - 1).attr('readonly') == undefined) {
                        $('.modal-header').eq($('.modal-dialog').length - 1).removeClass('vr-modal-header');
                        $('.modal-header').eq($('.modal-dialog').length - 1).addClass('vr-modal-header-inback');
                    }
                }               
            }
               var newElement = '<div class="modal-dialog ' + classmodal  +' '+' '+ modalCssClass +'" ' + resClass + ' style="' + style + '" >'
                                  + '  <div class="modal-content" >'
                                    + '    <div class="modal-header vr-modal-header" ' + titleShowValue + '>'
                                      + '      <button type="button" class="close" aria-label="Close" ng-click="modalContext.closeModal()"><span aria-hidden="true">&times;</span></button>'
                                        + '    <h5 class="modal-title" ng-bind="title"></h5>'
                                        + '</div>'
                                + tElement.html()
                                    + '</div>'
                                + '</div>';

            tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);