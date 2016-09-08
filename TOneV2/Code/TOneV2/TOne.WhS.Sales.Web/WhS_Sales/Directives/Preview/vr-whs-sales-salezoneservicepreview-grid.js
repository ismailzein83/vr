'use strict';

app.directive('vrWhsSalesSalezoneservicepreviewGrid', ['WhS_Sales_RatePlanPreviewAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (WhS_Sales_RatePlanPreviewAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var saleZoneServicePreviewGrid = new SaleZoneServicePreviewGrid($scope, ctrl, $attrs);
            saleZoneServicePreviewGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Preview/Templates/SaleZoneServicePreviewGridTemplate.html'
    };

    function SaleZoneServicePreviewGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.saleZoneServicePreviews = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady)
            {
                var promises = [];

                var getFilteredPreviewsPromise = WhS_Sales_RatePlanPreviewAPIService.GetFilteredSaleZoneServicePreviews(dataRetrievalInput);
                promises.push(getFilteredPreviewsPromise);

                var loadDataItemsDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadDataItemsDeferred.promise);

                getFilteredPreviewsPromise.then(function (response)
                {
                    var dataItemPromises = [];

                    if (response != null && response.Data != null)
                    {
                        for (var i = 0; i < response.Data.length; i++)
                        {
                            var dataItem = response.Data[i];
                            extendDataItem(dataItem);
                            dataItemPromises.push(dataItem.currentServiceViewerLoadDeferred.promise);
                            dataItemPromises.push(dataItem.newServiceViewerLoadDeferred.promise);
                        }
                    }

                    UtilsService.waitMultiplePromises(dataItemPromises).then(function () {
                        loadDataItemsDeferred.resolve();
                    }).catch(function (error) {
                        loadDataItemsDeferred.reject(error);
                    });

                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

                return UtilsService.waitMultiplePromises(promises);
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

        function extendDataItem(dataItem)
        {
            dataItem.isCurrentServiceInherited = dataItem.Entity.IsCurrentServiceInherited === true;

            dataItem.currentServiceViewerLoadDeferred = UtilsService.createPromiseDeferred();

            if (dataItem.Entity.CurrentServices != null)
            {
                dataItem.currentServiceViewerReadyDeferred = UtilsService.createPromiseDeferred();

                dataItem.onCurrentServiceViewerReady = function (api) {
                    dataItem.currentServiceViewerAPI = api;
                    dataItem.currentServiceViewerReadyDeferred.resolve();
                };

                dataItem.currentServiceViewerReadyDeferred.promise.then(function () {
                    var currentServiceViewerPayload;
                    currentServiceViewerPayload = {
                        selectedIds: UtilsService.getPropValuesFromArray(dataItem.Entity.CurrentServices, 'ServiceId')
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.currentServiceViewerAPI, currentServiceViewerPayload, dataItem.currentServiceViewerLoadDeferred);
                });
            }
            else {
                dataItem.currentServiceViewerLoadDeferred.resolve();
            }

            dataItem.newServiceViewerLoadDeferred = UtilsService.createPromiseDeferred();

            if (dataItem.Entity.NewServices != null)
            {
                dataItem.newServiceViewerReadyDeferred = UtilsService.createPromiseDeferred();

                dataItem.onNewServiceViewerReady = function (api) {
                    dataItem.newServiceViewerAPI = api;
                    dataItem.newServiceViewerReadyDeferred.resolve();
                };

                dataItem.newServiceViewerReadyDeferred.promise.then(function () {
                    var newServiceViewerPayload;
                    newServiceViewerPayload = {
                        selectedIds: UtilsService.getPropValuesFromArray(dataItem.Entity.NewServices, 'ServiceId')
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.newServiceViewerAPI, newServiceViewerPayload, dataItem.newServiceViewerLoadDeferred);
                });
            }
            else {
                dataItem.newServiceViewerLoadDeferred.resolve();
            }
        }
    }
}]);