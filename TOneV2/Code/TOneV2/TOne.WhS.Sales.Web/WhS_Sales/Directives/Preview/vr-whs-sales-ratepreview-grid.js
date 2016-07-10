'use strict';

app.directive('vrWhsSalesRatepreviewGrid', ['WhS_Sales_RatePlanPreviewAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (WhS_Sales_RatePlanPreviewAPIService, UtilsService, VRUIUtilsService, VRNotificationService)
{
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ratePreviewGrid = new RatePreviewGrid($scope, ctrl, $attrs);
            ratePreviewGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Preview/Templates/RatePreviewGridTemplate.html'
    };

    function RatePreviewGrid($scope, ctrl, $attrs)
    {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController()
        {
            $scope.scopeModel = {};
            $scope.scopeModel.ratePreviews = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Sales_RatePlanPreviewAPIService.GetFilteredRatePreviews(dataRetrievalInput).then(function (response) {
                    if (response != null && response.Data != null) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var dataItem = response.Data[i];
                            if (dataItem.Entity.IsCurrentRateInherited === true)
                                dataItem.Entity.CurrentRate += ' (Inherited)';
                        }
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (query) {
                return gridAPI.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);