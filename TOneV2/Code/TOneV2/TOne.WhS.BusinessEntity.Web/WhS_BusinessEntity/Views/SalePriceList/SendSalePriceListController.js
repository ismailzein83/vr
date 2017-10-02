(function (appControllers) {

    'use strict';

    SendSalePriceListController.$inject = ['$scope', 'WhS_BE_SalePricelistAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function SendSalePriceListController($scope, WhS_BE_SalePricelistAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var processInstanceId;

        var salePriceListGridAPI;
        var salePriceListGridReadyDeferred = UtilsService.createPromiseDeferred();
        var salePriceListGridContext;
        var hideSelectedColumn;
        var customerPriceListIds;

        var selectAll;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters) {
                processInstanceId = parameters.processInstanceId;
                hideSelectedColumn = parameters.HideSelectedColumn;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.optionTab = { showTab: false };

            setSalePriceListGridContext();
            $scope.scopeModel.onSalePriceListGridReady = function (api) {
                salePriceListGridAPI = api;
                salePriceListGridReadyDeferred.resolve();
            };
            $scope.scopeModel.selectAll = function () {
                selectAll = true;
                toggleSalePriceListGridSelection();
            };
            $scope.scopeModel.deselectAll = function () {
                selectAll = false;
                toggleSalePriceListGridSelection();
            };

            $scope.scopeModel.disableSendButton = function () {
                if (customerPriceListIds.length == 1)
                    return false;
                if (salePriceListGridAPI == undefined)
                    return true;
                var salePriceListData = salePriceListGridAPI.getData();
                return (salePriceListData == undefined || salePriceListData.selectedPriceListIds == undefined || salePriceListData.selectedPriceListIds.length == 0);
            };
            $scope.scopeModel.send = function () {
                if (salePriceListGridAPI.previewIfSinglePriceList())
                    return;

                $scope.scopeModel.isLoading = true;
                var promises = [];

                var haveAllEmailsBeenSent;

                var sendCustomerPriceListsPromise = sendCustomerPriceLists();
                promises.push(sendCustomerPriceListsPromise);

                var loadSalePriceListGridDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadSalePriceListGridDeferred.promise);

                sendCustomerPriceListsPromise.then(function (response) {
                    haveAllEmailsBeenSent = response;
                    loadSalePriceListGrid().then(function () {
                        loadSalePriceListGridDeferred.resolve();
                    }).catch(function (error) {
                        loadSalePriceListGridDeferred.reject(error);
                    });
                });

                return UtilsService.waitMultiplePromises(promises).then(function () {
                    if (haveAllEmailsBeenSent === false)
                        VRNotificationService.showWarning('Some pricelists have failed to send');
                    else {
                        $scope.scopeModel.isSendAllButtonDisabled = true;
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            getSalePriceListIdsByProcessInstanceId().then(function () {
                $scope.scopeModel.doCustomerPriceListsExist = doCustomerPriceListsExist();
                if ($scope.scopeModel.doCustomerPriceListsExist)
                    $scope.scopeModel.optionTab.showTab = true;
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getSalePriceListIdsByProcessInstanceId() {
            return WhS_BE_SalePricelistAPIService.GetSalePriceListIdsByProcessInstanceId(processInstanceId).then(function (response) {
                if (response != undefined) {
                    customerPriceListIds = [];
                    for (var i = 0; i < response.length; i++) {
                        customerPriceListIds.push(response[i]);
                    }
                }
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadSalePriceListGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = 'Send Pricelists';
        }
        function loadSalePriceListGrid() {
            var salePriceListGridLoadDeferred = UtilsService.createPromiseDeferred();

            salePriceListGridReadyDeferred.promise.then(function () {
                var salePriceListGridPayload = {
                    query: {
                        IncludedSalePriceListIds: customerPriceListIds
                    },
                    context: salePriceListGridContext,
                    HideSelectedColumn: hideSelectedColumn
                };
                VRUIUtilsService.callDirectiveLoad(salePriceListGridAPI, salePriceListGridPayload, salePriceListGridLoadDeferred);
            });

            return salePriceListGridLoadDeferred.promise;
        }

        function sendCustomerPriceLists() {
            var sendPriceListsInput = {
                ProcessInstanceId: processInstanceId,
                SelectAll: selectAll,
                CompressAttachement: $scope.scopeModel.compressPriceListFile
            };

            var salePriceListData = salePriceListGridAPI.getData();
            if (salePriceListData != undefined) {
                sendPriceListsInput.SelectedPriceListIds = salePriceListData.selectedPriceListIds;
                sendPriceListsInput.NotSelectedPriceListIds = salePriceListData.notSelectedPriceListIds;
            }

            return WhS_BE_SalePricelistAPIService.SendCustomerPriceLists(sendPriceListsInput);
        }

        function setSalePriceListGridContext() {
            salePriceListGridContext = {
                processInstanceId: processInstanceId,
                onSalePriceListPreviewClosed: onSalePriceListPreviewClosed,
                getSelectAllValue: getSelectAllValue
            };
        }
        function onSalePriceListPreviewClosed() {
            $scope.scopeModel.isLoading = true;

            return loadSalePriceListGrid().catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function getSelectAllValue() {
            return selectAll;
        }

        function doCustomerPriceListsExist() {
            return (customerPriceListIds != undefined && customerPriceListIds.length > 0);
        }
        function toggleSalePriceListGridSelection() {
            salePriceListGridAPI.toggleSelection(selectAll);
        }
    }

    appControllers.controller('WhS_BE_SendSalePriceListController', SendSalePriceListController);

})(appControllers);