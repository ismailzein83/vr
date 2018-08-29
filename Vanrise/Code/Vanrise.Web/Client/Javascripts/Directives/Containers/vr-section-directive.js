'use strict';

app.directive('vrSection', ['UtilsService','VRLocalizationService', function (UtilsService, VRLocalizationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var classlevel = "1";
            var collapsible = tAttrs.collapsible != undefined;
            var title = tAttrs.header || tAttrs.title;

            var localizedtitle = tAttrs.localizedtitle || tAttrs.localizedheader;
            title = VRLocalizationService.getResourceValue(localizedtitle, title);
           
            

            var expanded = (collapsible == undefined || tAttrs.collapsed == undefined);
            if (tAttrs.level != undefined && tAttrs.level == "2")
                classlevel = " panel-vr-child ";
            if(tAttrs.level != undefined && tAttrs.level == "2" && tAttrs.light!=undefined)
                classlevel = "panel-vr-child light";
            var sectionId = UtilsService.replaceAll(UtilsService.guid(), '-', '');
            var expandname = 'expanded_' + sectionId;
            var focusClass = "";
            if (tAttrs.focusonclick != undefined) {
                focusClass = "vr-clickable-panel";
            }
            var newElement = '<div ng-click="$root.addFocusPanelClass($event)" id="' + sectionId + '" class="panel-primary panel-vr ' + classlevel + ' ' + focusClass + '" >'
                            + '<div class="panel-heading" ng-init="' + expandname + '=' + expanded + '" expanded="{{' + expandname + '}}"><label><span  class="hand-cursor collapsible-icon glyphicon " ng-show="' + collapsible + '" ng-click="' + expandname + '=!' + expandname + '" ng-class="' + expandname + '?\'glyphicon-collapse-up\':\'glyphicon-collapse-down\'" ></span>' + title + '</label></div>'
                            + '<div class="panel-body" ng-show="' + expandname + '">' + tElement.html() + '</div></div>';
            tElement.html(newElement);           
        }

    };

    return directiveDefinitionObject;

}]);