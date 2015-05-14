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
            var newElement = '<div class="modal-dialog" ng-style="{ ' + widthPart + ' }">'
                                  + '  <div class="modal-content">'
                                    + '    <div class="modal-header" ng-show="title">'
                                      + '      <button type="button" class="close" aria-label="Close" ng-click="modalContext.closeModal()"><span aria-hidden="true">&times;</span></button>'
                                        + '    <h4 class="modal-title" ng-bind="title"></h4>'
                                        + '</div>'                                       
                                +  tElement.context.innerHTML   
                                    +'</div>'
                                + '</div>';
            
            tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);