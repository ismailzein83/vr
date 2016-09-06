'use strict';

app.directive('vrModalcontent', [function () {

    var directiveDefinitionObject = {
        restrict: 'A',
        scope: false,
        compile: function (tElement, tAttrs) {
            tElement.attr('class', "modal");
            tElement.attr['tabindex'] = "-1";
            tElement.attr['role'] = "dialog";
            tElement.attr['aria-hidden'] = "true";
            var widthPart = "";
            if (tAttrs.width != undefined)
                widthPart = '\'width\': ' + tAttrs.width;
            //'<div class="modal" tabindex="-1" role="dialog" aria-hidden="true">'
            var style = "";
            if ($('.modal-dialog').length > 0)
                style = "top:" + ($('.modal-dialog').length) * 10 + "px; left:" + ($('.modal-dialog').length) * 10 + "px;";
            var newElement = '<div class="modal-dialog" ng-style="{ ' + widthPart + ' }" style="' + style + '" >'
                                  + '  <div class="modal-content">'
                                    + '    <div class="modal-header" ng-show="title">'
                                      + '      <button type="button" class="close" aria-label="Close" ng-click="modalContext.closeModal()"><span aria-hidden="true">&times;</span></button>'
                                        + '    <h5 class="modal-title" ng-bind="title"></h5>'
                                        + '</div>'
                                + tElement.context.innerHTML
                                    + '</div>'
                                + '</div>';

            tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);