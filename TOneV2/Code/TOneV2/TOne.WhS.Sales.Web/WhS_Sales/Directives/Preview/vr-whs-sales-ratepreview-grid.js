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
        var processInstanceId;

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
                        for (var i = 0; i < response.Data.length; i++)
                            extendDataItem(response.Data[i]);
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
                ctrl.isNormalRateGrid = (query.ZoneName == null);
                processInstanceId = query.ProcessInstanceId;
                return gridAPI.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function extendDataItem(dataItem)
        {
            if (dataItem.Entity.IsCurrentRateInherited === true)
                dataItem.Entity.CurrentRate += ' (Inherited)';

            dataItem.onGridReady = function (api) {
                var query = {
                    ProcessInstanceId: processInstanceId,
                    ZoneName: dataItem.Entity.ZoneName
                };
                api.load(query);
            };
        }
    }
}]);