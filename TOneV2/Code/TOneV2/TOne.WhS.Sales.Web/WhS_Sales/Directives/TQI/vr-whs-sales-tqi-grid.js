'use strict';

app.directive('vrWhsSalesTqiGrid', ['WhS_Sales_RatePlanAPIService', 'UtilsService', 'VRNotificationService', 'VRValidationService', function (WhS_Sales_RatePlanAPIService, UtilsService, VRNotificationService, VRValidationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var tqiGrid = new TQIGrid($scope, ctrl, $attrs);
            tqiGrid.initializeController();
        },
        controllerAs: 'soldCountryCtrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/TQI/Templates/TQIGridTemplate.html'
    };

    function TQIGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;
        var selectedCountryIds;
        var effectiveDateDayOffset;

        function initializeController() {

            $scope.analyticsData = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Sales_RatePlanAPIService.GetTQISuppliersInfo(dataRetrievalInput)
                    .then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.analyticsData.push(response[i]);
                                mapNeededData(response[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                var query = {};
                if (payload != undefined && payload.rpRouteDetail != undefined) {
                    query.RPRouteDetail = payload.rpRouteDetail;
                    query.PeriodType = payload.periodType;
                    query.PeriodValue = payload.periodValue;
                }
                return gridAPI.retrieveData(query);
            };

            api.getData = function () {

                if (selectedCountryIds.length == 0)
                    return null;

                return {
                    CountryIds: selectedCountryIds,
                    EED: $scope.scopeModel.endEffectiveDate
                };
            };



            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function mapNeededData(dataItem)
        {
            dataItem.onServiceReady = function (api) {
                dataItem.ServieApi = api;
                dataItem.ServieApi.load({ selectedIds: dataItem.ZoneServices });
            };
        }
    }
}]);