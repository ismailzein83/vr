'use strict';

app.directive('vrDatalist', ['UtilsService', function (UtilsService) {

	var directiveDefinitionObject = {
		//transclude: true,
		restrict: 'E',
		scope: {
			maxitemsperrow: '@',
			datasource: '=',
			onremoveitem: '&',
			autoremoveitem: '=',
			onitemclicked: '=',
			hideremoveicon: '=',
			dragdropsetting: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			if (ctrl.datasource == undefined)
				ctrl.datasource = {};
			ctrl.itemsSortable = { handle: '.handeldrag', animation: 100 };
			if (ctrl.dragdropsetting != undefined && typeof (ctrl.dragdropsetting) == 'object') {
				ctrl.itemsSortable.group = {
					name: ctrl.dragdropsetting.groupCorrelation.getGroupName(),
					pull: ctrl.dragdropsetting.canSend == true && ctrl.dragdropsetting.copyOnSend == true ? "clone" : ctrl.dragdropsetting.canSend,
					put: ctrl.dragdropsetting.canReceive
				};
				ctrl.itemsSortable.sort = ctrl.dragdropsetting.enableSorting;
				ctrl.itemsSortable.onAdd = function (/**Event*/evt) {
					var obj = evt.model;
					if (ctrl.dragdropsetting.onItemReceived != undefined && typeof (ctrl.dragdropsetting.onItemReceived) == 'function')
						obj = ctrl.dragdropsetting.onItemReceived(evt.model, evt.models);
					evt.models[evt.newIndex] = obj;
				};
			}
			ctrl.readOnly = UtilsService.isContextReadOnly($scope) || $attrs.readonly != undefined;
			ctrl.collapsible = $attrs.collapsible != undefined;
			ctrl.onInternalRemove = function (dataItem) {
				if (ctrl.autoremoveitem == true) {
					var index = ctrl.datasource.indexOf(dataItem);
					ctrl.datasource.splice(index, 1);
				}
				else {
					var removeFunction = $scope.$parent.$eval(ctrl.onremoveitem);
					removeFunction(dataItem);
				}
			};
			ctrl.getDataItemTitle = function (dataItem) {
			};
			$scope.ondataitemclicked = function (dataItem) {
				if (ctrl.readOnly)
					return;
				if (typeof (ctrl.onitemclicked) == 'function')
					ctrl.onitemclicked(dataItem);
			};

		},
		controllerAs: 'VRDatalistCtrl',
		bindToController: true,
		compile: function (element, attrs) {

			return {
				pre: function (scope, elem, attrs, ctrl) {
					scope.isDataListScope = true;
					scope.viewScope = scope.$parent;
					while (scope.viewScope.isDataListScope) {
						scope.viewScope = scope.viewScope.$parent;
					}
				}
			};
		},
		template: function (element, attrs) {

			var draggableIconTemplate = '';
			var contentWidth = 0;
			if (attrs.isitemdraggable != undefined) {
				draggableIconTemplate = '<div ng-if="!ctrl.readOnly" style="width: 14px; display:inline-block;height:25px">'
					+ '<i class="glyphicon glyphicon-th-list handeldrag hand-cursor drag-icon"></i>'
					+ '</div>';
				contentWidth += 14;
			}

			var onRemoveAttr = '';
			if (attrs.autoremoveitem != undefined || attrs.onremoveitem != undefined) {
				onRemoveAttr = 'onremove="VRDatalistCtrl.onInternalRemove(dataItem)"';
				contentWidth += 14;
			}


			var onItemClickedAttr = '';
			var itemCssClass = '';
			if (attrs.onitemclicked != undefined) {
				onItemClickedAttr = 'ng-click="ondataitemclicked(dataItem)"';
				itemCssClass = 'class ="vr-list-item-clickable"'
			}
			var title='';
			if (attrs.enabletitle != undefined)
				title = 'title="{{dataItem}}"';
			var template = '<vr-list maxitemsperrow="{{VRDatalistCtrl.maxitemsperrow}}" hideremoveicon="VRDatalistCtrl.hideremoveicon"  iscollapsible="{{VRDatalistCtrl.collapsible}}">'
				+ '<div class="datalist-container" style="white-space: pre-line;" ng-sortable="VRDatalistCtrl.itemsSortable" ng-class="VRDatalistCtrl.dragdropsetting &&  VRDatalistCtrl.datasource.length == 0 ?\'empty-vr-datalist-drop\':\'\'">'
				+ '<vr-listitem ' + onItemClickedAttr + ' ' + itemCssClass + ' ng-repeat="dataItem in VRDatalistCtrl.datasource" ' + onRemoveAttr + ' > '
				+ '<span class="hand-cursor collapse-icon glyphicon" ng-show="VRDatalistCtrl.collapsible" ng-init="expande=true" ng-click="expande=!expande" ng-class="expande?\'glyphicon-collapse-up\':\'glyphicon-collapse-down\'"></span>'
				+ '<span class="listitem-title" ng-show="!expande" ng-if="VRDatalistCtrl.collapsible && dataItem.title"><vr-label>{{dataItem.title}}</vr-label></span>'
				+ draggableIconTemplate
				+ '<div style="width: calc( 100% - ' + contentWidth + 'px); display:inline-block;text-overflow: ellipsis; overflow: hidden; padding:0px 0px;white-space: nowrap;"' + title + ' ng-show="expande==true">' + element.html() + '</div>'
				+ '</vr-listitem>'
				+ '</div>'
				+ '</vr-list>';
			return template;
		}


	};

	return directiveDefinitionObject;



}]);

