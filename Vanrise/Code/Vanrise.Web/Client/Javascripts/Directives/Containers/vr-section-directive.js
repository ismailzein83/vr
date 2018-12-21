'use strict';

app.directive('vrSection', ['UtilsService', 'VRLocalizationService', function (UtilsService, VRLocalizationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var classlevel = "1";
            var collapsible = tAttrs.collapsible != undefined;
            var title = tAttrs.header || tAttrs.title;
            var localizedtitle = tAttrs.localizedtitle || tAttrs.localizedheader;
            title = VRLocalizationService.getResourceValue(localizedtitle, title);

            if (title === undefined) title = "";

            var expanded = (collapsible == undefined || tAttrs.collapsed == undefined);
            if (tAttrs.level != undefined && tAttrs.level == "2")
                classlevel = " panel-vr-child ";
            if (tAttrs.level != undefined && tAttrs.level == "2" && tAttrs.light != undefined)
                classlevel = "panel-vr-child light";
            if (title === "")
                classlevel += " hidden-title";
            var sectionId = UtilsService.replaceAll(UtilsService.guid(), '-', '');
            var expandname = 'expanded_' + sectionId;
            var focusClass = "";
            if (tAttrs.focusonclick != undefined) {
                focusClass = "vr-clickable-panel";
            }
            var newElement = '<div  class="panel-primary panel-vr ' + classlevel + ' ' + focusClass + '" ng-init="' + expandname + '=' + expanded + '" expanded="{{' + expandname + '}}" >'
            +((title !== "") ? '<div class="panel-heading" ng-click="$root.addFocusPanelClass($event)" id="' + sectionId + '" ><label><span  class="hand-cursor collapsible-icon glyphicon " ng-show="' + collapsible + '" ng-click="' + expandname + '=!' + expandname + '" ng-class="' + expandname + '?\'glyphicon-collapse-up\':\'glyphicon-collapse-down\'" ></span>' + title + '</label></div>':'')
            +((title === "" && collapsible) ? '<div class="collapsible-container"><span  class="hand-cursor collapsible-icon glyphicon " ng-show="' + collapsible + '" ng-click="' + expandname + '=!' + expandname + '" ng-class="' + expandname + '?\'glyphicon-collapse-up\':\'glyphicon-collapse-down\'" ></span></div>' : '')
            +'<div class="panel-body" ng-show="' + expandname + '">' + tElement.html() + '</div></div>';
            tElement.html(newElement);
        }

    };

    return directiveDefinitionObject;

}]);