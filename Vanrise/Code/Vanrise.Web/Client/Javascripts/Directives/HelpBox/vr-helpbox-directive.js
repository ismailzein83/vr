'use strict';
app.directive('vrHelpbox', ['BaseDirService', function (BaseDirService) {
    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            datasource: '=',
            onhelpboxclicked: '=',
            datavaluefield: '@',
            datatextfield: '@',
            dataclassfield: '@',
            labelclassfield: '@'
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
                if (typeof item[ctrl.dataclassfield]) {
                    var color = item[ctrl.dataclassfield];
                    if (typeof color === 'string' || typeof color instanceof String) {
                        return color;
                    }
                    else if (typeof color === 'object' || color instanceof Object) {
                        switch (color.UniqueName) {
                            case "VR_AccountBalance_StyleFormating_CSSClass": return color.ClassName;
                        }
                    }
                }
            };

            ctrl.getLabelClass = function (item) {
                if (ctrl.labelclassfield) return ctrl.getObjectProperty(item, ctrl.labelclassfield);
            };
            ctrl.getObjectValue = function (item) {
                if (ctrl.datavaluefield) return ctrl.getObjectProperty(item, ctrl.datavaluefield);
                return item;
            };
            ctrl.onHelpBoxClicked = function () {
                if (ctrl.onhelpboxclicked != undefined && typeof (ctrl.onhelpboxclicked) == 'function') {
                    ctrl.onhelpboxclicked();
                }
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
            var addedClasses = "";
            if (attrs.isclickable != undefined)
                addedClasses += " clickable";
            var mainTemplate = '<div  class="vr-helpbox' + layoutClass + '" >'
                + '<span style="margin-left: -5px;border: 1px solid #ccc;border-radius: 4px;" class="' + addedClasses + '" ng-click="ctrl.onHelpBoxClicked()" ><div class="item ' + addedClasses + '" ng-repeat="item in ctrl.datasource">'
                + '<div class="label badge-color " ng-class="ctrl.getObjectClass(item)" ><span ng-class="ctrl.getLabelClass(item)">abc</span></div>'
                + '<span title="{{ctrl.getObjectText(item)}}" ng-class="ctrl.getLabelClass(item)" style="text-overflow: ellipsis;overflow: hidden;" >{{ctrl.getObjectText(item)}}</span>'
                + '</div>'
                + '</div></span>';
            return labelTemplate + mainTemplate;
        }

    };

    return directiveDefinitionObject;

}]);





