(function (appControllers) {
    "use strict";

    marketViewsManagementController.$inject = ['$scope', 'Demo_Module_BIDsASKsAPIService', 'VRNotificationService', 'UtilsService','VRUIUtilsService'];

    function marketViewsManagementController($scope, Demo_Module_BIDsASKsAPIService, VRNotificationService, UtilsService, VRUIUtilsService) {

        var bidsGridApi;
        var asksGridApi;

        $scope.scopeModel = {};
        var saleZoneMasterPlanDirectiveAPI;
        var saleZoneMasterPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var asksBooleanFilterDirectiveAPI;
        var asksBooleanFilterDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var bidsBooleanFilterDirectiveAPI;
        var bidsBooleanFilterDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        $scope.scopeModel.bids = [];
        $scope.scopeModel.asks = [];

        defineScope();
        load();

        function defineScope() {

            $scope.scopeModel.search = function () {
                bidsGridApi.retrieveData(getBIDsFilter());
                asksGridApi.retrieveData(getASKsFilter());
            };

            $scope.scopeModel.onBIDsGridReady = function (api) {
                bidsGridApi = api;
                bidsGridApi.retrieveData(getBIDsFilter());
            };

            $scope.scopeModel.onASKsGridReady = function (api) {
                asksGridApi = api;
                asksGridApi.retrieveData(getASKsFilter());
            };
            $scope.scopeModel.onBIDsBooleanFilterEditorReady = function (api) {
                bidsBooleanFilterDirectiveAPI = api;
                bidsBooleanFilterDirectiveReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onASKsBooleanFilterEditorReady = function (api) {
                asksBooleanFilterDirectiveAPI = api;
                asksBooleanFilterDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSaleZoneMarketPlanDirectiveReady = function (api) {
                saleZoneMasterPlanDirectiveAPI = api;
                saleZoneMasterPlanReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.bidsDataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                return Demo_Module_BIDsASKsAPIService.GetFilteredBIDs(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            $scope.scopeModel.asksDataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { 

                return Demo_Module_BIDsASKsAPIService.GetFilteredASKs(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }
        function loadSaleZoneMasterPlanDirective() {
            var saleZoneMasterPlanLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            saleZoneMasterPlanReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(saleZoneMasterPlanDirectiveAPI, { fieldTitle: "Destination" }, saleZoneMasterPlanLoadPromiseDeferred);
            });

            return saleZoneMasterPlanLoadPromiseDeferred.promise;
        }
        function loadBIDsBooleanFilterDirective() {
            var bidsBooleanFilterDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            bidsBooleanFilterDirectiveReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(bidsBooleanFilterDirectiveAPI, { fieldTitle:"CLI/NCLI (BIDs)"}, bidsBooleanFilterDirectiveLoadPromiseDeferred);
            });

            return bidsBooleanFilterDirectiveLoadPromiseDeferred.promise;
        }

        function loadASKsBooleanFilterDirective() {
            var asksBooleanFilterDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            asksBooleanFilterDirectiveReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(asksBooleanFilterDirectiveAPI, { fieldTitle: "CLI/NCLI (ASKs)" }, asksBooleanFilterDirectiveLoadPromiseDeferred);
            });

            return asksBooleanFilterDirectiveLoadPromiseDeferred.promise;
        }
        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

        }

        function loadAllControls() {

            return UtilsService.waitPromiseNode({ promises: [loadSaleZoneMasterPlanDirective(), loadBIDsBooleanFilterDirective(), loadASKsBooleanFilterDirective()] })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function getBIDsFilter() {
            return {
                Destination: saleZoneMasterPlanDirectiveAPI != undefined ? saleZoneMasterPlanDirectiveAPI.getSelectedIds() : undefined,
                CLIORNCLI: bidsBooleanFilterDirectiveAPI != undefined ? bidsBooleanFilterDirectiveAPI.getData() : undefined
            };
        }
        function getASKsFilter() {
            return {
                Destination: saleZoneMasterPlanDirectiveAPI != undefined ? saleZoneMasterPlanDirectiveAPI.getSelectedIds() : undefined,
                CLIORNCLI: asksBooleanFilterDirectiveAPI != undefined ? asksBooleanFilterDirectiveAPI.getData() : undefined
            };
        }
    };

    appControllers.controller('Demo_Module_MarketViewManagementController', marketViewsManagementController);
})(appControllers);