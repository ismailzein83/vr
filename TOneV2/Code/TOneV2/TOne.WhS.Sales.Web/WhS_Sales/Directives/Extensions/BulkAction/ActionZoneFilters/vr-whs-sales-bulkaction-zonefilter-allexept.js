'use strict';

app.directive('vrWhsSalesBulkactionZonefilterAllexept', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var allExeptBulkActionZoneFilter = new AllExeptBulkActionZoneFilter($scope, ctrl, $attrs);
            allExeptBulkActionZoneFilter.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Extensions/BulkAction/ActionZoneFilters/Templates/AllExeptBulkActionZoneFilterTemplate.html'
    };
    function AllExeptBulkActionZoneFilter($scope, ctrl, $attrs) {
        var zoneFilter;
        var bulkActionContext;
        this.initializeController = initializeController;
        var bulkActionZoneFilterAPI;
        var bulkActionZoneFilterReadyDeferred = UtilsService.createPromiseDeferred();
        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.bulkactionzoneFilterOnReady = function (api) {
                bulkActionZoneFilterAPI = api;
                bulkActionZoneFilterReadyDeferred.resolve();
            };

            UtilsService.waitMultiplePromises([bulkActionZoneFilterReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    zoneFilter = payload.zoneFilter;
                    bulkActionContext = payload.bulkActionContext;
                }
                var promises = [];

                var loadBulkActionZoneFilterPromise = loadBulkActionZoneFilter();
                promises.push(loadBulkActionZoneFilterPromise);

                return UtilsService.waitMultiplePromises(promises);
            };
            api.getData = function () {

                var data = {
                    $type: 'TOne.WhS.Sales.MainExtensions.AllExeptApplicableZones, TOne.WhS.Sales.MainExtensions',
                    ExceptCountryZones: bulkActionZoneFilterAPI.getData().CountryZonesByCountry,
                    ExceptZoneIds: bulkActionZoneFilterAPI.getData().IncludedZoneIds
                };
                return data;
            };
            api.getSummary = function () {


                return bulkActionZoneFilterAPI.getSummary();
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
        function loadBulkActionZoneFilter() {
            var bulkActionZoneFilterLoadDeferred = UtilsService.createPromiseDeferred();

            var bulkActionZoneFilterPayload = {
                zoneFilter: zoneFilter,
                bulkActionContext: bulkActionContext
            };
            VRUIUtilsService.callDirectiveLoad(bulkActionZoneFilterAPI, bulkActionZoneFilterPayload, bulkActionZoneFilterLoadDeferred);

            return bulkActionZoneFilterLoadDeferred.promise;
        }
    }
}]);