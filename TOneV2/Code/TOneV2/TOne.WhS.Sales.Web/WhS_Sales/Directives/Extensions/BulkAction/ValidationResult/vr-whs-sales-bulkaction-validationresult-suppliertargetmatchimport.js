'use strict';

app.directive('vrWhsSalesBulkactionValidationresultuppliertargetmatchimport', ['WhS_Sales_RatePlanUtilsService', 'UtilsService', 'VRUIUtilsService', '$filter', function (WhS_Sales_RatePlanUtilsService, UtilsService, VRUIUtilsService, $filter) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var targetMatchValidationResult = new TargetMatchValidationResult($scope, ctrl, $attrs);
            targetMatchValidationResult.initializeController();
        },
        controllerAs: "rateValidationResultCtrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Extensions/BulkAction/ValidationResult/Template/SupplierTargetMatchImportValidationResultTemplate.html'
    };

    function TargetMatchValidationResult($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

      

        function initializeController() {
            $scope.scopeModel = {};

            defineAPI();
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                return UtilsService.waitMultiplePromises();
            };

            api.getData = function () {
              
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }

    }
}]);