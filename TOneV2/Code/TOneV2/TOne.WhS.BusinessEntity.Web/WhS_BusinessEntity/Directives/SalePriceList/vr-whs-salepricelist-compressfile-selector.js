'use strict';

app.directive('vrWhsSalepricelistCompressfileSelector', ['WhS_BE_SalePriceistCompressFileEnum', 'UtilsService', function (WhS_BE_SalePriceistCompressFileEnum, UtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var pricelistCompressFile = new PricelistCompressFile($scope, ctrl, $attrs);
            pricelistCompressFile.initializeController();
        },
        controllerAs: "compressFileCtrl",
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function PricelistCompressFile($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.datasource = UtilsService.getArrayEnum(WhS_BE_SalePriceistCompressFileEnum);

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
        }
        function defineAPI() {

            var api = {};
            api.load = function (payload) {
                if (payload != undefined && payload.selectedValue != undefined) {
                    $scope.scopeModel.selectedValue = UtilsService.getItemByVal($scope.scopeModel.datasource, payload.selectedValue, 'value');
                }
            };

            api.getData = function getData() {
                if ($scope.scopeModel.selectedValue != undefined)
                    return ($scope.scopeModel.selectedValue.value);
                else return null;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getTemplate() {
        return '<vr-columns colnum="{{compressFileCtrl.normalColNum}}">\
                    <vr-label>Compress Pricelist File</vr-label>\
					<vr-select on-ready="scopeModel.onSelectorReady"\
						datasource="scopeModel.datasource"\
						selectedvalues="scopeModel.selectedValue"\
						datavaluefield="value"\
						datatextfield="description"\
						isrequired="compressFileCtrl.isrequired"\
					</vr-select>\
				</vr-columns>';
    }
}]);