'use strict';
app.directive('vrHelpbox', ['BaseDirService', function (BaseDirService) {
    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            datasource: '=',
            datavaluefield: '@',
            datatextfield: '@',
            dataclassfield: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            
            ctrl.getObjectProperty = function (item, property) {
                return BaseDirService.getObjectProperty(item, property);
            };
            ctrl.getObjectText = function (item) {
                if (ctrl.datatextfield) return ctrl.getObjectProperty(item, ctrl.datatextfield);
                return item;
            };

            ctrl.getObjectClass = function (item) {
                if (ctrl.dataclassfield) return ctrl.getObjectProperty(item, ctrl.dataclassfield);
                return item;
            };
            ctrl.getObjectValue = function (item) {
                if (ctrl.datavaluefield) return ctrl.getObjectProperty(item, ctrl.datavaluefield);
                return item;
            };
        

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            var labelTemplate = '';
            if (attrs.label != undefined)
                labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
            var icontemplate = '';
            if (attrs.dataiconfield != undefined && attrs.dataiconfield != '')
                icontemplate = '<span class="icon"><vr-icon icontype="ctrl.getObjectIcon(item)"></vr-icon></span>';

            var layoutClass = "";
            if (attrs.verticallayout != undefined)
                layoutClass = " vertical-alignment";
            if (attrs.leftalignment != undefined)
                layoutClass += " left-alignment";

            var mainTemplate = '<div  class="vr-helpbox' + layoutClass + '" >'
				+ '<span style="margin-left: -5px;border: 1px solid #ccc;border-radius: 4px;"><div class="item" ng-repeat="item in ctrl.datasource">'
				+ '<div class="badge-color" ng-class="ctrl.getObjectClass(item)"></div>'
				+ '<span title="{{ctrl.getObjectText(item)}}">{{ctrl.getObjectText(item)}}</span>'
				+ '</div>'
				+ '</div></span>';
            return labelTemplate + mainTemplate;
        }

    };

    return directiveDefinitionObject;

}]);





