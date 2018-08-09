'use strict';

app.directive('vrSectionV2', ['UtilsService', 'MultiTranscludeService', function (UtilsService, MultiTranscludeService) {

	var directiveDefinitionObject = {
		restrict: 'E',
		scope: {
			menuactions: "=",
			onRemove: "=",
			header: "=",
		    warning:"="
		},
		transclude: true,
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			$scope.classlevel = "1";
			$scope.header = ctrl.header;
			$scope.dragable = $attrs.dragable != undefined;
			$scope.collapsible = $attrs.collapsible != undefined;
			if ($attrs.level != undefined && $attrs.level == "2")
				$scope.classlevel = " panel-vr-child ";
			$scope.expandname = 'expanded_' + UtilsService.replaceAll(UtilsService.guid(), '-', '');
		},
		controllerAs: 'sectionCtrl',
		bindToController: true,
		template: function (attrs) {
			var htmlTempalte = '<div class="panel-primary panel-vr" ng-class="classlevel" >'
				+ '<div class="panel-heading" ng-init="expandname=true" expanded="{{expandname}}">'
				+ '<span class="glyphicon glyphicon-th-list handeldrag hand-cursor drag-icon" ng-show="dragable" ></span>'
				+ '<span style="padding-left: 4px;" class="hand-cursor collapsible-icon glyphicon " ng-show="collapsible" ng-click=" expandname =!expandname " ng-class=" expandname ?\'glyphicon-collapse-up\':\'glyphicon-collapse-down\'" ></span><label>{{header}}</label>'
				+ '<span class="section-menu"  style="absolute: relative; right: 10px;" ng-if="sectionCtrl.menuactions.length > 0" > <vr-button type="SectionAction" menuactions="sectionCtrl.menuactions" isasynchronous="true" ></vr-button></span> '
				+ '<span class="warning-icon"  style="position: absolute;top: 7px;" ng-style="{\'right\': sectionCtrl.menuactions.length > 0 ? \'80px\' : \'20px\'}" ng-if="sectionCtrl.warning!=\'\'" > <vr-warning  value="{{sectionCtrl.warning}}"></vr-warning></span> '
	            + '<span class="hand-cursor glyphicon glyphicon-remove" style="position: absolute; right: 5px; top: 12px;" ng-show="sectionCtrl.onRemove != undefined" ng-click="sectionCtrl.onRemove()" ></span></div>'
				+ '<div class="panel-body" ng-show="expandname"  ng-transclude></div></div>';

			return htmlTempalte;
		}
	};

	return directiveDefinitionObject;

}]);