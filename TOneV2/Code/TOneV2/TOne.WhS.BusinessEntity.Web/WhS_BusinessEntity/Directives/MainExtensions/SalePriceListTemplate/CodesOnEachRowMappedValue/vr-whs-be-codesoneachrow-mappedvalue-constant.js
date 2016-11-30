'use strict';

app.directive('vrWhsBeCodesoneachrowMappedvalueConstant', [function () {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var basicSettingsConstantMappedValue = new BasicSettingsConstantMappedValue($scope, ctrl, $attrs);
			basicSettingsConstantMappedValue.initializeController();
		},
		controllerAs: "codesOnEachRowConstMappedValueCtrl",
		bindToController: true,
		template: function (element, attrs) {
			return getTemplate(attrs);
		}
	};

	function BasicSettingsConstantMappedValue($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		function initializeController() {
			$scope.scopeModel = {};
			defineAPI();
		}
		function defineAPI() {

			var api = {};

			api.load = function (payload) {
				if (payload != undefined && payload.mappedValue != undefined) {
					$scope.scopeModel.value = payload.mappedValue.Value;
				}
			};

			api.getData = function getData() {
				return {
				    $type: 'TOne.WhS.BusinessEntity.MainExtensions.CodeOnEachRowConstantMappedValue, TOne.WhS.BusinessEntity.MainExtensions',
					Value: $scope.scopeModel.value
				};
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}
	}

	function getTemplate() {
	    return '<vr-columns colnum={{codesOnEachRowConstMappedValueCtrl.normalColNum}}>\
					<vr-textbox value="scopeModel.value" isrequried="codesOnEachRowConstMappedValueCtrl.isrequried"></vr-textbox>\
				</vr-columns>';
	}
}]);