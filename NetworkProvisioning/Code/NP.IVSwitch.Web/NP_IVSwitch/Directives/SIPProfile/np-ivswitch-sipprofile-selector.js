'use strict';
app.directive('npIvswitchSipprofileSelector', ['NP_IVSwitch_SIPProfileAPIService', 'VRUIUtilsService', 'UtilsService',
	function (NP_IVSwitch_SIPProfileAPIService, VRUIUtilsService, UtilsService) {

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
				isdisabled: "=",
				customlabel: "@",
				onitemadded: "=",
				customvalidate: "="
			},
			controller: function ($scope, $element, $attrs) {

				var ctrl = this;
				ctrl.datasource = [];

				ctrl.selectedvalues;
				if ($attrs.ismultipleselection != undefined)
					ctrl.selectedvalues = [];

				var ctor = new endPointCtor(ctrl, $scope, $attrs);
				ctor.initializeController();

			},
			controllerAs: 'ctrl',
			bindToController: true,
			template: function (element, attrs) {
				return getUserTemplate(attrs);
			}
		};


		function getUserTemplate(attrs) {

			var multipleselection = "";

			var label = "SIP Profile";
			if (attrs.ismultipleselection != undefined) {
				label = "SIP Profiles";
				multipleselection = "ismultipleselection";
			}

			if (attrs.customlabel != undefined)
				label = attrs.customlabel;

			return '<span vr-disabled="ctrl.isdisabled"><vr-select ' + multipleselection + '  datatextfield="Description" datavaluefield="ProfileName" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate"'
				+ ' label="' + label + '"  datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" haspermission="ctrl.haspermission"></vr-select></span>'
		}

		function endPointCtor(ctrl, $scope, attrs) {

			var selectorApi;

			function initializeController() {
				ctrl.onSelectorReady = function (api) {
					selectorApi = api;
					defineAPI();
				};
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					selectorApi.clearDataSource();
					var selectedIds;
					var selectedAllIds = [];
					var filter;
					var selectAll;
					if (payload != undefined) {
						filter = payload.filter;
						selectedIds = payload.selectedIds;
						selectAll = payload.selectAll;
					}

					return NP_IVSwitch_SIPProfileAPIService.GetSIPProfilesInfo(UtilsService.serializetoJson(filter)).then(function (response) {

						if (response) {
							for (var i = 0; i < response.length; i++) {
								ctrl.datasource.push(response[i]);
								if (selectAll == true) {
									selectedAllIds.push(response[i].ProfileName);
								}
							}

						}

						if (selectedIds != undefined) {
							VRUIUtilsService.setSelectedValues(selectedIds, 'ProfileName', attrs, ctrl);
						}
						if (selectedAllIds !=undefined && selectedAllIds.length>0) {
						VRUIUtilsService.setSelectedValues(selectedAllIds, 'ProfileName', attrs, ctrl);
						}
					});
				};

				api.getSelectedIds = function () {
					return VRUIUtilsService.getIdSelectedIds('ProfileName', attrs, ctrl);
				};
				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			this.initializeController = initializeController;
		}

		return directiveDefinitionObject;
	}]);