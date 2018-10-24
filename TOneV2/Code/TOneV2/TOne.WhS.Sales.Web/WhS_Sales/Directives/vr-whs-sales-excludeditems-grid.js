'use strict';

app.directive('vrWhsSalesExcludeditemsGrid', ['WhS_Sales_ExcludedItemsAPIService', 'UtilsService', 'VRNotificationService', 'VRValidationService', 'VRUIUtilsService', 'WhS_Sales_ExcludedItemTypeEnum',
    function (WhS_Sales_ExcludedItemsAPIService, UtilsService, VRNotificationService, VRValidationService, VRUIUtilsService, WhS_Sales_ExcludedItemTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new ExcludedItemsGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/ExcludedItemsGridTemplate.html"
        };

        function ExcludedItemsGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;
            var gridQuery;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.excludedItems = [];
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Sales_ExcludedItemsAPIService.GetFilteredExcludedItems(dataRetrievalInput).then(function (response) {
                        if (response != null && response.Data != null) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var dataItem = response.Data[i];
                                //extendExcludedItem(dataItem);
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