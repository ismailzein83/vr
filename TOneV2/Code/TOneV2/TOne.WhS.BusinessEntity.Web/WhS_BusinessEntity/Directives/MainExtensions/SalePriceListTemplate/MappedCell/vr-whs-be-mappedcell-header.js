'use strict';

app.directive('vrWhsBeMappedcellHeader', ['WhS_BE_SalePricelistTemplateMappedCellHeaderEnum', 'UtilsService', function (WhS_BE_SalePricelistTemplateMappedCellHeaderEnum, UtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var mappedCellHeader = new MappedCellHeader($scope, ctrl, $attrs);
            mappedCellHeader.initializeController();
        },
        controllerAs: "mappedCellHeaderCtrl",
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function MappedCellHeader($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.options = UtilsService.getArrayEnum(WhS_BE_SalePricelistTemplateMappedCellHeaderEnum);

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
        }
        function defineAPI() {

            var api = {};
            api.load = function (payload) {
                if (payload != undefined && payload.mappedCell != undefined) {
                    $scope.scopeModel.selectedHeader = UtilsService.getItemByVal($scope.scopeModel.options, payload.mappedCell.HeaderField, 'value');
                }
            };

            api.getData = function getData() {
                return {
                    $type: 'TOne.WhS.BusinessEntity.MainExtensions.HeaderMappedCell, TOne.WhS.BusinessEntity.MainExtensions',
                    HeaderField: $scope.scopeModel.selectedHeader.value
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getTemplate() {
        return '<vr-columns colnum="{{mappedCellHeaderCtrl.normalColNum}}">\
					<vr-select on-ready="scopeModel.onSelectorReady"\
						datasource="scopeModel.options"\
						selectedvalues="scopeModel.selectedHeader"\
						datavaluefield="value"\
						datatextfield="description"\
						isrequired="mappedCellHeaderCtrl.isrequired"\
						hideremoveicon="mappedCellHeaderCtrl.isrequired">\
					</vr-select>\
				</vr-columns>';
    }
}]);