//(function (appControllers) {

//    "use strict";

//    invoiceFTPActionEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService','VRUIUtilsService', 'VR_Invoice_InvoiceAPIService'];

//    function invoiceFTPActionEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceAPIService) {
//        var invoiceId;
//        var invoiceActionId;
//        var invoiceEntity;
//        var connectionSelectorDirectiveAPI;
//        var connectionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//        defineScope();
//        loadParameters();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);
//            if (parameters != undefined && parameters != null) {
//                invoiceId = parameters.invoiceId;
//                invoiceActionId = parameters.invoiceActionId;
//            }
//        }
//        function defineScope() {
//            $scope.scopeModel = {};

//            $scope.scopeModel.execute = function () {
//                return sendFTPFile();
//            };
//            $scope.scopeModel.close = function () {
//                $scope.modalContext.closeModal();
//            };
//            $scope.scopeModel.onConnectionSelectorReady = function (api) {
//                connectionSelectorDirectiveAPI = api;
//                connectionSelectorReadyPromiseDeferred.resolve();

//            };
//            $scope.scopeModel.disableExecute = function () {
//                if (connectionSelectorDirectiveAPI && connectionSelectorDirectiveAPI.getSelectedIds) return false;
//                return true;
//            };

//            function sendFTPFile() {
//                $scope.scopeModel.isLoading = true;
//                return VR_Invoice_InvoiceAPIService.SendFTPFile(invoiceActionId, invoiceId, connectionSelectorDirectiveAPI.getSelectedIds())
//                    .then(function (response) {
//                        if (response)
//                            VRNotificationService.showInformation("File Successfully Sent");
//                        else
//                            VRNotificationService.showInformation("File Was Not Sent");
//                   $scope.modalContext.closeModal();
//               })
//               .catch(function (error) {
//                   VRNotificationService.notifyException(error, $scope);
//               }).finally(function () {
//                   $scope.scopeModel.isLoading = false;
//               });
//            }
//        }
        
//        function load() {
//            $scope.scopeModel.isLoading = true;
//            loadAllControls().then(function () {
//            }).catch(function (error) {
//                VRNotificationService.notifyExceptionWithClose(error, $scope);
//                $scope.scopeModel.isLoading = false;
//            });.
//        }
//        function loadAllControls() {

//            function setTitle() {
//                $scope.title = "Invoice FTP";
//            }
//            function loadStaticData() {
//                if (invoiceEntity != undefined) {
//                }
//            }
//            function loadVRConnectionSelector() {

//                var connectionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
//                connectionSelectorReadyPromiseDeferred.promise.then(function () {
//                    VRUIUtilsService.callDirectiveLoad(connectionSelectorDirectiveAPI, undefined, connectionSelectorLoadPromiseDeferred);
//                });
//                return connectionSelectorLoadPromiseDeferred.promise;
//            }

//            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRConnectionSelector])
//               .catch(function (error) {
//                   VRNotificationService.notifyExceptionWithClose(error, $scope);
//               })
//              .finally(function () {
//                  $scope.scopeModel.isLoading = false;
//              });
//        }
//    }

//    appControllers.controller('VR_Invoice_InvoiceFTPActionEditorController', invoiceFTPActionEditorController);
//})(appControllers);
