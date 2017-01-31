'use strict';

app.directive('vrSection', ['UtilsService', function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var classlevel = "1";
            var collapsible = tAttrs.collapsible != undefined;
            var title = tAttrs.header || tAttrs.title;
            if (tAttrs.level != undefined && tAttrs.level == "2")
                classlevel = " panel-vr-child ";
            var expandname = 'expanded_' + UtilsService.replaceAll(UtilsService.guid(), '-', '');
            var newElement = '<div class="panel-primary panel-vr ' + classlevel + '" >'
                            + '<div class="panel-heading" ng-init="' + expandname + '=true" expanded="{{' + expandname + '}}"><label><span style="padding-right: 3px;" class="hand-cursor glyphicon " ng-show="' + collapsible + '" ng-click="' + expandname + '=!' + expandname + '" ng-class="' + expandname + '?\'glyphicon-collapse-up\':\'glyphicon-collapse-down\'" ></span>' + title + '</label></div>'
                            + '<div class="panel-body" ng-show="' + expandname + '">' + tElement.context.innerHTML + '</div></div>';
            tElement.html(newElement);
        }

    };

    return directiveDefinitionObject;

}]);