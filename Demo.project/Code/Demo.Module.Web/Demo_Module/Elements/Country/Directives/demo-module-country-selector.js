'use strict';

app.directive('demoModuleCountrySelector', ['VRNotificationService', 'Demo_Module_CountryAPIService', 'UtilsService', 'VRUIUtilsService',
	function (VRNotificationService, Demo_Module_CountryAPIService, UtilsService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				ismultipleselection: "@",
				onselectionchanged: '=',
				selectedvalues: '=',
				isrequired: "=",
				onselectitem: "=",
				ondeselectitem: "=",
				hideremoveicon: '@',
				normalColNum: '@',
				isdisabled: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				ctrl.datasource = [];

				ctrl.selectedvalues;
				if ($attrs.ismultipleselection != undefined)
					ctrl.selectedvalues = [];

				var countrySelector = new CountrySelector(ctrl, $scope, $attrs);
				countrySelector.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			template: function (element, attrs) {
				return getCountrySelectorTemplate(attrs);
			}
		};

		function getCountrySelectorTemplate(attrs) {

			var multipleselection = "";
			var label = "Country";

			if (attrs.ismultipleselection != undefined) {
				label = "Countries";
				multipleselection = "ismultipleselection";
			}

			var hideremoveicon = "";
			if (attrs.hideremoveicon != undefined) {
				hideremoveicon = "hideremoveicon";
			}

			return '<vr-columns colnum="{{ctrl.normalColNum}}">'
				+ '<span vr-disabled="ctrl.isdisabled">'
				+ '<vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield= "Name" datavaluefield= "CountryId" isrequired= "ctrl.isrequired"'
				+ ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Country" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'
				+ '</vr-select></span></vr-columns>';
		};


		function CountrySelector(ctrl, $scope, attrs) {
			this.initializeController = initializeController;

			var selectorAPI;

			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.onSelectorReady = function (api) {
					selectorAPI = api;
					defineAPI();
				};
			};

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					selectorAPI.clearDataSource();

					var selectedIds;
					var filter;

					if (payload != undefined) {
						selectedIds = payload.selectedIds;
						filter = payload.filter;
					}

					return Demo_Module_CountryAPIService.GetCountriesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
						if (response != null) {
							for (var i = 0; i < response.length; i++) {
								ctrl.datasource.push(response[i]);
							}

							if (selectedIds != undefined) {
								VRUIUtilsService.setSelectedValues(selectedIds, 'CountryId', attrs, ctrl);
							}
						}
					});
				};

				api.getSelectedIds = function () {
					return VRUIUtilsService.getIdSelectedIds('CountryId', attrs, ctrl);
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			};
		};

		return directiveDefinitionObject;
	}]);