'use strict';

app.directive('vrSectionV2', ['UtilsService', 'MultiTranscludeService', function (UtilsService, MultiTranscludeService) {

	var directiveDefinitionObject = {
		restrict: 'E',
		scope: {
			menuactions: "=",
			onRemove: "=",
			header: "=",
			warning: "=",
			dataitem: "=",
		    settings:"="
		},
		transclude: true,
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			$scope.classlevel = "1";
			$scope.dragable = $attrs.dragable != undefined;
			$scope.collapsible = $attrs.collapsible != undefined;
			if ($attrs.level != undefined && $attrs.level == "2")
				$scope.classlevel = " panel-vr-child ";
			$scope.sectionId = UtilsService.replaceAll(UtilsService.guid(), '-', '');
			$scope.expandname = 'expanded_' + $scope.sectionId;
			if (ctrl.settings != undefined) {
			    ctrl.oneditclicked = ctrl.settings.oneditclicked != undefined ? ctrl.settings.oneditclicked : undefined;
			    ctrl.enablesortable = ctrl.settings.sortable != undefined ? ctrl.settings.sortable : false;
			    ctrl.headerEditable = ctrl.settings.headerEditable != undefined ? ctrl.settings.headerEditable : undefined;
			}

		},
		controllerAs: 'sectionCtrl',
		bindToController: true,
		template: function (attrs) {
			var focusClass = "";
			if (attrs.focusonclick != undefined) {
				focusClass = "vr-clickable-panel";
			}
			var htmlTempalte = '<div class="panel-primary panel-vr ' + focusClass + '"  ng-class="classlevel" >'
				    + '<div class="panel-heading" ng-init="expandname=true" expanded="{{expandname}}" id="{{sectionId}}" ng-click="$root.addFocusPanelClass($event)">'
				        + '<span class="glyphicon glyphicon-th-list handeldrag hand-cursor drag-icon" ng-show="dragable || sectionCtrl.enablesortable" ></span>'
                        + '<span style="padding:0px 2px;" class="hand-cursor collapsible-icon glyphicon " ng-show="collapsible" ng-click=" expandname =!expandname " ng-class=" expandname ?\'glyphicon-collapse-up\':\'glyphicon-collapse-down\'" ></span>'
                        + '<label ng-if="!sectionCtrl.headerEditable">{{ sectionCtrl.header }}</label>'
                        + '<span  ng-if="sectionCtrl.headerEditable"><span title="{{sectionCtrl.header}}" class="edit-header-container single-line {{dragable || sectionCtrl.enablesortable ? \'share-space\' : \'\'}} {{ !sectionCtrl.header ? \'required-header\' : \'\' }}"   contenteditable="true" ng-model="sectionCtrl.header"></span></span>'
                        + '<span ng-if="sectionCtrl.validationContext.validate() != null" class="hand-cursor section-validation-sign"  title="has validation errors!">*</span> '
                        + '<span class="section-menu"  style="position: absolute; right: 9px;top:-1px;" ng-if="sectionCtrl.menuactions.length > 0" > <vr-button type="SectionAction" menuactions="sectionCtrl.menuactions" isasynchronous="true" ></vr-button></span> '
                        + '<span class="warning-icon {{sectionCtrl.menuactions.length > 0 ? \'share-space\' : \'\'}}"  style="position: absolute;top:4px;" ng-style="{\'right\': sectionCtrl.menuactions.length > 0 ? \'80px\' : \'20px\'}" ng-if="sectionCtrl.warning != undefined && sectionCtrl.warning!=\'\'" > <vr-warning  value="{{sectionCtrl.warning}}"></vr-warning></span>'
                        + '<i ng-if="sectionCtrl.oneditclicked != undefined"  class="glyphicon glyphicon-pencil edit hand-cursor {{sectionCtrl.onRemove ? \'share-space\' : \'\' }}" ng-click="sectionCtrl.oneditclicked(sectionCtrl.dataitem)"></i>'
				        + '<span class="hand-cursor glyphicon glyphicon-remove remove {{ sectionCtrl.oneditclicked ? \'share-space\' : \'\' }}"  ng-if="sectionCtrl.onRemove != undefined" ng-click="sectionCtrl.onRemove(sectionCtrl.dataitem)" ></span>'
                        + ' </div>'
                    + ' <vr-validation-group validationcontext="sectionCtrl.validationContext">'
                        + ' <vr-textbox value="sectionCtrl.header"  ng-if="sectionCtrl.headerEditable" ng-show="false" isrequired="true"></vr-textbox>'
                        + ' <div class="panel-body" ng-show="expandname"  ng-transclude></div>'
                    + ' </vr-validation-group>'
                + ' </div>';

			return htmlTempalte;
		}
	};

	return directiveDefinitionObject;

}]);