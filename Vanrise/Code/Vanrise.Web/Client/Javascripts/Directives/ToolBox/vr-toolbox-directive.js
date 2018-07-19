'use strict';
app.directive('vrToolbox', ['BaseDirService', function (BaseDirService) {
    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            datasource: '=',          
            datavaluefield: '@',
            datatextfield: '@',
            dataiconfield: '@',
            groupname:'@'
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

            ctrl.getObjectIcon = function (item) {
                 return ctrl.getObjectProperty(item, ctrl.dataiconfield);
            };
            ctrl.getObjectValue = function (item) {
                if (ctrl.datavaluefield) return ctrl.getObjectProperty(item, ctrl.datavaluefield);
                return item;
            };
            ctrl.config = {
                group: {
                    name: ctrl.groupname,
                    pull: "clone",
                    put: false
                },
                sort: false
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
				layoutClass = "vertical-alignment";
			var mainTemplate = '<div  ng-sortable="ctrl.config" class="vr-toolbox ' + layoutClass+'" >'
                                 + '<div class="item" ng-repeat="item in ctrl.datasource">'
                                     + icontemplate
                                     + '<span title="{{ctrl.getObjectText(item)}}">{{ctrl.getObjectText(item)}}</span>'
                                 + '</div>'
                              + '</div>';
            return  labelTemplate + mainTemplate ;
        }

    };

    return directiveDefinitionObject;

}]);

