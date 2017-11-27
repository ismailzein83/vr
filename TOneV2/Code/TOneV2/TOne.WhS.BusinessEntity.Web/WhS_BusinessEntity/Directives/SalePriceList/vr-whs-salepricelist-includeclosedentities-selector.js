'use strict';

app.directive('vrWhsSalepricelistIncludeclosedentitiesSelector', ['WhS_BE_SalePricelistIncludeClosedEntitiesEnum', 'UtilsService', function (WhS_BE_SalePricelistIncludeClosedEntitiesEnum, UtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var pricelistIncludeClosedEntities = new PricelistIncludeClosedEntities($scope, ctrl, $attrs);
            pricelistIncludeClosedEntities.initializeController();
        },
        controllerAs: "includeClosedEntitiesCtrl",
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function PricelistIncludeClosedEntities($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.datasource = UtilsService.getArrayEnum(WhS_BE_SalePricelistIncludeClosedEntitiesEnum);

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
        return '<vr-columns colnum="{{includeClosedEntitiesCtrl.normalColNum}}">\
                    <vr-label>Include Closed Entities</vr-label>\
					<vr-select on-ready="scopeModel.onSelectorReady"\
						datasource="scopeModel.datasource"\
						selectedvalues="scopeModel.selectedValue"\
						datavaluefield="value"\
						datatextfield="description"\
						isrequired="includeClosedEntitiesCtrl.isrequired"\
					</vr-select>\
				</vr-columns>';
    }
}]);