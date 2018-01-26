'use strict';

app.directive('vrWhsSalesNewcountrypreviewGrid', ['WhS_Sales_RatePlanPreviewAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (WhS_Sales_RatePlanPreviewAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var newCountryPreviewGrid = new NewCountryPreviewGrid($scope, ctrl, $attrs);
            newCountryPreviewGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Preview/Templates/NewCountryPreviewGridTemplate.html'
    };

    function NewCountryPreviewGrid($scope, ctrl, $attrs) {

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
                return WhS_Sales_RatePlanPreviewAPIService.GetFilteredNewCustomerCountryPreviews(dataRetrievalInput).then(function (response) {
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