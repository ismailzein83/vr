'use strict';

app.directive('vrWhsBeCodesoneachrowMappedvalueBefield', ['WhS_BE_SalePriceListTemplateSettingsCodeOnEachRowBEFieldEnum', 'UtilsService', function (WhS_BE_SalePriceListTemplateSettingsCodeOnEachRowBEFieldEnum, UtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var basicSettingsBEFieldMappedValue = new BasicSettingsBEFieldMappedValue($scope, ctrl, $attrs);
			basicSettingsBEFieldMappedValue.initializeController();
		},
		controllerAs: "codesOnEachRowBeFieldMappedValueCtrl",
		bindToController: true,
		template: function (element, attrs) {
			return getTemplate(attrs);
		}
	};

	function BasicSettingsBEFieldMappedValue($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var selectorAPI;

		function initializeController() {

			$scope.scopeModel = {};
			$scope.scopeModel.beFields = UtilsService.getArrayEnum(WhS_BE_SalePriceListTemplateSettingsCodeOnEachRowBEFieldEnum);

			$scope.scopeModel.onSelectorReady = function (api) {
				selectorAPI = api;
				defineAPI();
			};
		}
		function defineAPI() {

			var api = {};

			api.load = function (payload) {
				if (payload != undefined && payload.mappedValue != undefined) {
					$scope.scopeModel.selectedBEField = UtilsService.getItemByVal($scope.scopeModel.beFields, payload.mappedValue.BEField, 'value');
				}
			};

			api.getData = function getData() {
				return {
				    $type: 'TOne.WhS.BusinessEntity.MainExtensions.CodeOnEachRowBEFieldMappedValue, TOne.WhS.BusinessEntity.MainExtensions',
					BEField: $scope.scopeModel.selectedBEField.value
				};
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}
	}

	function getTemplate() {
	    return '<vr-columns colnum="{{codesOnEachRowBeFieldMappedValueCtrl.normalColNum}}">\
					<vr-select on-ready="scopeModel.onSelectorReady"\
						datasource="scopeModel.beFields"\
						selectedvalues="scopeModel.selectedBEField"\
						datavaluefield="value"\
						datatextfield="description"\
						isrequired="codesOnEachRowBeFieldMappedValueCtrl.isrequired"\
						hideremoveicon="codesOnEachRowBeFieldMappedValueCtrl.isrequired">\
					</vr-select>\
				</vr-columns>';
	}
}]);