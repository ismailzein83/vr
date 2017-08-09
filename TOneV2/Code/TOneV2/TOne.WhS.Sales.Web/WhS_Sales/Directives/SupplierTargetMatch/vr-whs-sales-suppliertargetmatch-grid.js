'use strict';

app.directive('vrWhsSalesSuppliertargetmatchGrid', ['WhS_Sales_SupplierTargetMatchAPIService', 'UtilsService', 'VRNotificationService', 'VRValidationService',
    function (WhS_Sales_SupplierTargetMatchAPIService, UtilsService, VRNotificationService, VRValidationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new SupplierTargetMatchGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Sales/Directives/SupplierTargetMatch/Templates/SupplierTargetMatchGrid.html'
        };

        function SupplierTargetMatchGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;
            var gridQuery;

            var selectedCountryIds;
            var effectiveDate;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.targetMatches = [];
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Sales_SupplierTargetMatchAPIService.GetFilteredSupplierTargetMatches(dataRetrievalInput).then(function (response) {
                        if (response != null && response.Data != null) {
                            for (var i = 0; i < response.Data.length; i++) {

                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                $scope.scopeModel.search = function () {
                    return gridAPI.retrieveData(gridQuery);
                };
            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {


                    if (payload != undefined) {
                        gridQuery = payload;
                    }

                    return gridAPI.retrieveData(gridQuery);
                };

                api.getData = function () {

                    return {

                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);