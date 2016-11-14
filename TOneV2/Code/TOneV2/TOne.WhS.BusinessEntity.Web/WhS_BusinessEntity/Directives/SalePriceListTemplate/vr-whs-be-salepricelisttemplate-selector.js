'use strict';

app.directive('vrWhsBeSalepricelisttemplateSelector', ['WhS_BE_SalePriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService', function (WhS_BE_SalePriceListTemplateAPIService, UtilsService, VRUIUtilsService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			ismultipleselection: "@",
			selectedvalues: '=',
			onselectionchanged: '=',
			onselectitem: "=",
			ondeselectitem: "=",
			isrequired: "@",
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
		if (attrs.isrequired != undefined)
			required = ' isrequired="true"';

		var hideremoveicon = "";
		if (attrs.hideremoveicon != undefined)
			hideremoveicon = ' hideremoveicon="true"';

		var hideselectedvaluessection = "";
		if (attrs.hideselectedvaluessection != undefined)
			hideselectedvaluessection = ' hideselectedvaluessection="true"';

		return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select on-ready="ctrl.onSelectorReady" datasource="ctrl.datasource" label="' + label + '" entityName="' + label + '" selectedvalues="ctrl.selectedvalues" datavaluefield="SalePriceListTemplateId" datatextfield="Name"' + multipleselection + ' onselectionchanged="ctrl.onselectionchanged" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + required + hideremoveicon + hideselectedvaluessection + '></vr-select></vr-columns>';
	}
}]);