(function (appControllers) {

    "use strict";

    CustomerRouteManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService'];
    function CustomerRouteManagementController($scope, utilsService, vruiUtilsService) {

        var gridAPI;
        var carrierAccountDirectiveApi;
        var carrierAccountReadyPromiseDeferred = utilsService.createPromiseDeferred();
        defineScope();

        function defineScope() {
            loadCustomers();
            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveApi = api;
                carrierAccountReadyPromiseDeferred.resolve();
            };
            function loadCustomers() {
                var loadPromiseDeffered = utilsService.createPromiseDeferred();
                carrierAccountReadyPromiseDeferred.promise.then(function () {
                    vruiUtilsService.callDirectiveLoad(carrierAccountDirectiveApi, undefined, loadPromiseDeffered);
                });
                return loadPromiseDeffered.promise;
            }
        }
        function buildGridQuery() {
            return {
                CustomerId: carrierAccountDirectiveApi.getSelectedIds(),
                CodePrefix: $scope.CodePrefix,
                Top: $scope.TopRecords
            };
        }
    }
    appControllers.controller('NP_IVSwitch_CustomerRouteManagementController', CustomerRouteManagementController);

})(appControllers);