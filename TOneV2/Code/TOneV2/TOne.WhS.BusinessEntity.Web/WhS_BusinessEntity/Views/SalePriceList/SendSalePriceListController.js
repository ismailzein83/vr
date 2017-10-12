(function (appControllers) {

    'use strict';

    SendSalePriceListController.$inject = ['$scope', 'WhS_BE_SalePricelistAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_BE_SalePriceListChangeService', 'WhS_BE_SalePriceListChangeAPIService', 'VRCommon_VRMailAPIService'];

    function SendSalePriceListController($scope, WhS_BE_SalePricelistAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_BE_SalePriceListChangeService, WhS_BE_SalePriceListChangeAPIService, VRCommon_VRMailAPIService) {

        var processInstanceId;

        var salePriceListGridAPI;
        var salePriceListGridReadyDeferred = UtilsService.createPromiseDeferred();
        var salePriceListGridContext;
        var hideSelectedColumn;
        var customerPriceListIds;
        var haveAllEmailsBeenSent;
        var selectAll;
        var singlePricelistToPreview;
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
                singlePricelistToPreview = salePriceListGridAPI.previewIfSinglePriceList();
                if (singlePricelistToPreview != undefined) {
                    sendSinglePricelist(singlePricelistToPreview);
                    return;
                }

                $scope.scopeModel.isLoading = true;
                var sendCustomerPriceListsPromise = UtilsService.createPromiseDeferred();

                WhS_BE_SalePricelistAPIService.SendPricelistsWithCheckNotSendPreviousPricelists(getSendPricelistsInputObject()).then(function (response) {
                    if (response.Customers.length == 0) {
                        haveAllEmailsBeenSent = response.AllEmailsHaveBeenSent;
                        sendCustomerPriceListsPromise.resolve();
                    }
                    else {
                        WhS_BE_SalePriceListChangeService.showSendPricelistsConfirmation(response.Customers)
                            .then(function (confirmationResponse) {
                                if (confirmationResponse.decision) {
                                    sendCustomerPriceLists(response.PricelistIds).then(function (response) {
                                        haveAllEmailsBeenSent = response;
                                        sendCustomerPriceListsPromise.resolve();
                                    });
                                }
                                else {
                                    sendCustomerPriceListsPromise.resolve();
                                    haveAllEmailsBeenSent = undefined;
                                }
                            });
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });

                sendCustomerPriceListsPromise.promise.then(function (response) {
                    if (haveAllEmailsBeenSent != undefined) {
                        loadSalePriceListGrid().then(function (response) {
                            $scope.scopeModel.isLoading = false;
                            if (haveAllEmailsBeenSent === false)
                                VRNotificationService.showWarning('Some pricelists have failed to send');
                            else {
                                $scope.scopeModel.isSendAllButtonDisabled = true;
                                $scope.modalContext.closeModal();
                            }
                        });
                    }
                    else {
                        $scope.scopeModel.isLoading = false;
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function sendSinglePricelist(singlePricelistToPreview) {
            $scope.scopeModel.isLoading = true;
            var sendPromiseDeferred = UtilsService.createPromiseDeferred();
            WhS_BE_SalePricelistAPIService.CheckIfCustomerHasNotSendPricelist(singlePricelistToPreview.Entity.PriceListId).then(function (response) {
                if (response == true) {
                    VRNotificationService.showConfirmation("This Customer has previous Pricelists not sent. Are you sure you want to continue ?").then(function (response) {
                        if (response) {
                            sendSinglePricelistMail(singlePricelistToPreview.Entity).then(function () {
                                sendPromiseDeferred.resolve();
                            });
                        }
                        else {
                            sendPromiseDeferred.resolve();
                        }
                    });
                }
                else {
                    sendSinglePricelistMail(singlePricelistToPreview.Entity).then(function () {
                        sendPromiseDeferred.resolve();
                    });
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
            return sendPromiseDeferred.promise.then(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function sendSinglePricelistMail(pricelistEntity) {
            return WhS_BE_SalePriceListChangeAPIService.GenerateAndEvaluateSalePricelistEmailByPricelistIdAndOwnerId(pricelistEntity.PriceListId, pricelistEntity.OwnerId).then(function (emailResponse) {
                WhS_BE_SalePriceListChangeService.sendEmail(emailResponse, onSinglePricelistMailSent);
            });
        }

        function onSinglePricelistMailSent(evaluatedEmail) {
            $scope.scopeModel.isLoading = true;
            var promises = [];
            evaluatedEmail.CompressFile = $scope.scopeModel.compressPriceListFile;
            var sendEmailPromise = VRCommon_VRMailAPIService.SendEmail(evaluatedEmail);
            promises.push(sendEmailPromise);

            var setPriceListAsSentDeferred = UtilsService.createPromiseDeferred();
            promises.push(setPriceListAsSentDeferred.promise);

            sendEmailPromise.then(function () {
                WhS_BE_SalePriceListChangeAPIService.SetPriceListAsSent(singlePricelistToPreview.Entity.PriceListId).then(function () {
                    setPriceListAsSentDeferred.resolve();
                }).catch(function (error) {
                    setPriceListAsSentDeferred.reject(error);
                });
            });

            return UtilsService.waitMultiplePromises(promises).then(function () {
                loadSalePriceListGrid().then(function (response) { $scope.scopeModel.isLoading = false; });
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);

                $scope.scopeModel.isLoading = false;
            });
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
        function getSendPricelistsInputObject() {
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
            return sendPriceListsInput;
        }

        function sendCustomerPriceLists(pricelistIds) {

            var sendPriceListsInput = {
                PricelistIds: pricelistIds,
                CompressAttachement: $scope.scopeModel.compressPriceListFile
            };

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