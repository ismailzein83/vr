(function (appControllers) {
    "use strict";

    callCenterCustomerManagementController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_ContractAPIService'];

    function callCenterCustomerManagementController($scope, VRNotificationService, UtilsService, VRUIUtilsService, Demo_Module_ContractAPIService) {

        var customerInfoReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var contractReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var CDRReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var billingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var complaintReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var customerInfoApi;
        var contractApi;
        var CDRApi;
        var billingApi;
        var complaintGridApi;
        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onCustomerInfoReady = function (api) {
                customerInfoApi = api;
                customerInfoReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onContractReady = function (api) {
                contractApi = api;
                contractReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onCDRReady = function (api) {
                CDRApi = api;
                CDRReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onBillingGridReady = function (api) {
                billingApi = api;
                billingReadyPromiseDeferred.resolve();
            };
         
            $scope.scopeModel.onComplaintReady = function (api) {
                complaintGridApi = api;
                complaintReadyPromiseDeferred.resolve();
            }
        };

      

        function load() {

            var promises = [];

            var loadCustomerInfoPromise = loadCustomerInfo();
            promises.push(loadCustomerInfoPromise);

            function loadCustomerInfo() {
                var customerInfoLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                customerInfoReadyPromiseDeferred.promise.then(function () {
                 
                    VRUIUtilsService.callDirectiveLoad(customerInfoApi, customerInfoLoadPromiseDeferred);
                });
                return customerInfoLoadPromiseDeferred.promise;
            }

            var loadContractPromise = loadContract();
            promises.push(loadContractPromise);

            function loadContract() {
                var contractLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                contractReadyPromiseDeferred.promise.then(function () {

                    VRUIUtilsService.callDirectiveLoad(contractApi, contractLoadPromiseDeferred);
                });
                return contractLoadPromiseDeferred.promise;
            }

            var loadCDRPromise = loadCDR();
            promises.push(loadCDRPromise);

            function loadCDR() {
                var CDRLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                CDRReadyPromiseDeferred.promise.then(function () {

                    VRUIUtilsService.callDirectiveLoad(CDRApi, CDRLoadPromiseDeferred);
                });
                return CDRLoadPromiseDeferred.promise;
            }

            var loadBillingPromise = loadBilling();
            promises.push(loadBillingPromise);

            function loadBilling() {
                var billingLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                billingReadyPromiseDeferred.promise.then(function () {

                    VRUIUtilsService.callDirectiveLoad(billingApi, billingLoadPromiseDeferred);
                });
                return billingLoadPromiseDeferred.promise;
            }

            var loadComplaintPromise = loadComplaint();
            promises.push(loadComplaintPromise);

            function loadComplaint() {
                var complaintLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                complaintReadyPromiseDeferred.promise.then(function () {

                    VRUIUtilsService.callDirectiveLoad(complaintGridApi, complaintLoadPromiseDeferred);
                });
                return complaintLoadPromiseDeferred.promise;
            }
        
            return UtilsService.waitMultiplePromises(promises);

        }

    };

    appControllers.controller('Demo_Module_CallCenterCustomerManagementController', callCenterCustomerManagementController);
})(appControllers);