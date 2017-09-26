'use strict';

app.directive('vrWhsSalesBulkactionTypeCancelpendingrates', ['WhS_Sales_BulkActionUtilsService', 'UtilsService', 'VRUIUtilsService', function (WhS_Sales_BulkActionUtilsService, UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var bedBulkActionType = new BEDBulkActionType($scope, ctrl, $attrs);
            bedBulkActionType.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };
    function BEDBulkActionType($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var bulkActionContext;

        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();
        }
        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                if (payload != undefined) {
                    bulkActionContext = payload.bulkActionContext;
                }
            };
            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Sales.MainExtensions.CancelPendingRatesBulkAction, TOne.WhS.Sales.MainExtensions'
                };
            };
            api.getSummary = function () {
                return null;
            };
            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
    function getTemplate(attrs) {
        return '';
    }
}]);