'use strict';

app.directive('vrWhsSalesBulkactionTypeImportsuppliertargetmatch', ['WhS_Sales_RatePlanAPIService', 'WhS_Sales_BulkActionUtilsService', 'UtilsService', 'VRUIUtilsService', 'WhS_BE_SalePriceListOwnerTypeEnum', function (WhS_Sales_RatePlanAPIService, WhS_Sales_BulkActionUtilsService, UtilsService, VRUIUtilsService, WhS_BE_SalePriceListOwnerTypeEnum) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var targetMatchBulkActionTypeCtrl = this;
            var targetMatchBulkActionType = new TargetMatchBulkActionType($scope, targetMatchBulkActionTypeCtrl, $attrs);
            targetMatchBulkActionType.initializeController();
        },
        controllerAs: "rateBulkActionTypeCtrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Extensions/BulkAction/ActionTypes/Templates/SupplierTargetMatchImportActionTypeTemplate.html'
    };

    function TargetMatchBulkActionType($scope, rateBulkActionTypeCtrl, $attrs) {

        this.initializeController = initializeController;

    

        function initializeController() {

            $scope.scopeModel = {};
           
            UtilsService.waitMultiplePromises([]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {

             
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Sales.MainExtensions.ImportSupplierTargetMatch, TOne.WhS.Sales.MainExtensions',
                };
            };

            api.getSummary = function () {
                
            };

            if (targetMatchBulkActionTypeCtrl.onReady != null) {
                targetMatchBulkActionTypeCtrl.onReady(api);
            }
        }
    }
}]);