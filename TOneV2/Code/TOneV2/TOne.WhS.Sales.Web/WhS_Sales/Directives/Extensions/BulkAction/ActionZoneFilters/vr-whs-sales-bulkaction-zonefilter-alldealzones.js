'use strict';

app.directive('vrWhsSalesBulkactionZonefilterAlldealzones', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var allDealZonesBulkActionZoneFilter = new AllDealZonesBulkActionZoneFilter($scope, ctrl, $attrs);
            allDealZonesBulkActionZoneFilter.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function AllDealZonesBulkActionZoneFilter($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();
        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {

            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Sales.MainExtensions.AllDealZones, TOne.WhS.Sales.MainExtensions'
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
        return null;
    }
}]);