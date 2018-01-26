'use strict';

app.directive('vrWhsSalesChangedcountrypreviewGrid', ['WhS_Sales_RatePlanPreviewAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (WhS_Sales_RatePlanPreviewAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var changedCountryPreviewGrid = new ChangedCountryPreviewGrid($scope, ctrl, $attrs);
            changedCountryPreviewGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Preview/Templates/ChangedCountryPreviewGridTemplate.html'
    };

    function ChangedCountryPreviewGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.previews = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Sales_RatePlanPreviewAPIService.GetFilteredChangedCustomerCountryPreviews(dataRetrievalInput).then(function (response) {
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };
        }
        function defineAPI() {

            var api = {};

            api.load = function (query) {
                if (query != null)
                    $scope.scopeModel.showCustomerName = query.ShowCustomerName;
                return gridAPI.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);