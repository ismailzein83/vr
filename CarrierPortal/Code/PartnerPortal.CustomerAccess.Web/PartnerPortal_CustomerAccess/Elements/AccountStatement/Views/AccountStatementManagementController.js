(function (appControllers) {

    "use strict";

    accountStatementManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'PartnerPortal_CustomerAccess_AccountStatementAPIService', 'VRNotificationService', 'VRDateTimeService'];

    function accountStatementManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, PartnerPortal_CustomerAccess_AccountStatementAPIService, VRNotificationService, VRDateTimeService) {
        var viewId;

        var  accountStatementSelectorApi;
        var  accountStatementSelectorPromiseDeferred = UtilsService.createPromiseDeferred();


        var gridAPI;
        defineScope();
        loadParameters();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                viewId = parameters.viewId;
            }
        };

        function defineScope() {
            $scope.scopeModel = {};
            var date = VRDateTimeService.getNowDateTime();

            $scope.scopeModel.fromDate = new Date(date.getFullYear(), date.getMonth() - 1, 1, 0, 0, 0, 0);

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.scopeModel.onAccountStatementSelectorReady = function (api) {
                accountStatementSelectorApi = api;
                accountStatementSelectorPromiseDeferred.resolve();
            };
            $scope.scopeModel.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };
        };
        function load()
        {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadAccountStatementSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function loadAccountStatementSelector()
        {
            var accountStatementSelectorLoadDeferred = UtilsService.createPromiseDeferred();
             accountStatementSelectorPromiseDeferred.promise.then(function () {
                 var accountStatementSelectorPayload = { viewId: viewId };
                 VRUIUtilsService.callDirectiveLoad(accountStatementSelectorApi, accountStatementSelectorPayload, accountStatementSelectorLoadDeferred);
            });
            return accountStatementSelectorLoadDeferred.promise;
        }
        function getFilterObject() {
            var query = {
                FromDate: $scope.scopeModel.fromDate,
                ViewId: viewId,
                AccountId: accountStatementSelectorApi.getSelectedIds()
            };
            return query;
        };
    }

    appControllers.controller('PartnerPortal_CustomerAccess_AccountStatementManagementController', accountStatementManagementController);
})(appControllers);