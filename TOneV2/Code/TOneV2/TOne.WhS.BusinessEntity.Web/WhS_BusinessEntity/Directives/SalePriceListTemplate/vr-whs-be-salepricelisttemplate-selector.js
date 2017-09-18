﻿'use strict';

app.directive('vrWhsBeSalepricelisttemplateSelector', ['WhS_BE_SalePriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService', 'WhS_BE_SalePriceListTemplateService', function (WhS_BE_SalePriceListTemplateAPIService, UtilsService, VRUIUtilsService, WhS_BE_SalePriceListTemplateService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			ismultipleselection: "@",
			selectedvalues: '=',
			onselectionchanged: '=',
			onselectitem: "=",
			ondeselectitem: "=",
			isrequired: "=",
			hideremoveicon: "@",
			hideselectedvaluessection: "@",
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {

			var ctrl = this;

			ctrl.selectedvalues;
			if ($attrs.ismultipleselection != undefined)
				ctrl.selectedvalues = [];

			ctrl.datasource = [];

			$scope.addNewSalePriceListTemplate = function () {
			    var onSalePriceListTemplateAdded = function (obj) {
			        ctrl.datasource.push(obj.Entity);
			        if ($attrs.ismultipleselection != undefined)
			            ctrl.selectedvalues.push(obj.Entity);
			        else
			            ctrl.selectedvalues = obj.Entity;
			    };
			    WhS_BE_SalePriceListTemplateService.addSalePriceListTemplate(onSalePriceListTemplateAdded);
			};
			ctrl.haspermission = function () {
			    return WhS_BE_SalePriceListTemplateAPIService.HasAddSalePriceListTemplatePermission();
			};


			var salePriceListTemplateSelector = new SalePriceListTemplateSelector(ctrl, $scope, $attrs);
			salePriceListTemplateSelector.initializeController();

		},
		controllerAs: 'ctrl',
		bindToController: true,
		template: function (element, attrs) {
			return getTemplate(attrs);
		}
	};

	function SalePriceListTemplateSelector(ctrl, $scope, $attrs) {

		this.initializeController = initializeController;

		var selectorAPI;

		function initializeController() {
			ctrl.onSelectorReady = function (api) {
				selectorAPI = api;
				defineAPI();
			};
		}
		function defineAPI() {

			var api = {};

			api.load = function (payload) {

				selectorAPI.clearDataSource();

				var selectedIds;

				if (payload != undefined) {
					selectedIds = payload.selectedIds;
				}

				return WhS_BE_SalePriceListTemplateAPIService.GetSalePriceListTemplatesInfo().then(function (response) {
					if (response != null) {
						for (var i = 0; i < response.length; i++)
							ctrl.datasource.push(response[i]);
					}
					if (selectedIds != undefined) {
						VRUIUtilsService.setSelectedValues(selectedIds, 'SalePriceListTemplateId', $attrs, ctrl);
					}
				});
			};

			api.getSelectedIds = function () {
				return VRUIUtilsService.getIdSelectedIds('SalePriceListTemplateId', $attrs, ctrl);
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}
	}

	function getTemplate(attrs) {

		var multipleselection = "";
		var label = "Sale Pricelist Template";
		if (attrs.ismultipleselection != undefined) {
			label = "Sale Pricelist Templates";
			multipleselection = ' ismultipleselection="true"';
		}

		var required = "";
		if (attrs.isrequired == true)
		    required = ' isrequired="true" ';

		var hideremoveicon = "";
		if (attrs.hideremoveicon != undefined)
			hideremoveicon = ' hideremoveicon="true"';

		var hideselectedvaluessection = "";
		if (attrs.hideselectedvaluessection != undefined)
		    hideselectedvaluessection = ' hideselectedvaluessection="true"';

		var addCliked = '';
		if (attrs.showaddbutton != undefined)
		    addCliked = 'onaddclicked="addNewSalePriceListTemplate"';


		return ' <vr-columns colnum="{{ctrl.normalColNum}}"><vr-select on-ready="ctrl.onSelectorReady" datasource="ctrl.datasource" isrequired="{{ctrl.isrequired}}" label="' + label + '" entityName="' + label + '" selectedvalues="ctrl.selectedvalues" datavaluefield="SalePriceListTemplateId" datatextfield="Name"' + multipleselection + ' ' + addCliked + ' onselectionchanged="ctrl.onselectionchanged" onselectitem="ctrl.onselectitem" haspermission="ctrl.haspermission" ondeselectitem="ctrl.ondeselectitem"'  + hideremoveicon + hideselectedvaluessection + '></vr-select></vr-columns>';
	}
}]);