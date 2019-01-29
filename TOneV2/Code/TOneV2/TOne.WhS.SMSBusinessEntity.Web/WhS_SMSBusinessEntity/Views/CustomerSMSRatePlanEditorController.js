(function (appControllers) {

    "use strict";

    customerSMSRatePlanEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService', 'VRDateTimeService', 'WhS_SMSBE_SMSRateChangeTypeEnum', 'WhS_SMSBusinessEntity_CustomerRatePlanService'];

    function customerSMSRatePlanEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService, VRDateTimeService, WhS_SMSBE_SMSRateChangeTypeEnum, WhS_SMSBusinessEntity_CustomerRatePlanService) {

        var customerInfo;
        var allCountryLetters = "All";
        var newSmsRateHistories = [];

        var isEffectiveDateChanged = false;

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        var loadCountryItemsDeferred;
        var loadDraftDataDeferred;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                customerInfo = parameters.customerInfo;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.datastore = [];
            $scope.scopeModel.countryLetters = [];

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                $scope.scopeModel.isLoading = true;
                WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.GetFilteredChanges(dataRetrievalInput).then(function (response) {
                    if (response && response.Data)
                        for (var i = 0; i < response.Data.length; i++)
                            setSMSRateChangeIcon(response.Data[i]);

                    onResponseReady(response);

                    if (loadCountryItemsDeferred != undefined)
                        loadCountryItemsDeferred.resolve();

                    if (loadDraftDataDeferred != undefined)
                        loadDraftDataDeferred.resolve();
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.onCountryLetterSelectionChanged = function () {
                var promises = [];
                $scope.scopeModel.isLoading = true;

                loadCountryItemsDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadCountryItemsDeferred.promise);

                if (newSmsRateHistories.length > 0 || isEffectiveDateChanged) {
                    saveChanges().then(function () {
                        newSmsRateHistories = [];
                        newSmsRateHistories.length = 0;
                        loadCountryItems();
                    });
                }
                else {
                    loadCountryItems();
                }

                function loadCountryItems() {
                    var letter;
                    if ($scope.scopeModel.selectedCountryLetterIndex != $scope.scopeModel.countryLetters.length - 1)// case when any letter != All is selected
                        letter = $scope.scopeModel.countryLetters[$scope.scopeModel.selectedCountryLetterIndex];

                    return gridAPI.retrieveData(getCustomerSMSChangesQuery(customerInfo.CarrierAccountId, letter));
                }

                return UtilsService.waitMultiplePromises(promises).finally(function () {
                    loadCountryItemsDeferred = undefined;
                    isEffectiveDateChanged = false;
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.onRateValueChanged = function (currentSMSRateDetails) {

                if (!tryUpdateCustomerSMSRateChanges(currentSMSRateDetails)) {
                    var rate = currentSMSRateDetails.NewRate;
                    if (rate != undefined) {
                        var obj = {
                            MobileNetworkID: currentSMSRateDetails.MobileNetworkID,
                            NewRate: rate
                        };

                        newSmsRateHistories.push(obj);
                    }
                }

                function tryUpdateCustomerSMSRateChanges(currentSMSRateDetails) {
                    for (var i = 0; i < newSmsRateHistories.length; i++) {
                        if (currentSMSRateDetails.MobileNetworkID == newSmsRateHistories[i].MobileNetworkID) {
                            newSmsRateHistories[i].NewRate = currentSMSRateDetails.NewRate;

                            return true;
                        }
                    }
                    return false;
                }
            };

            $scope.scopeModel.onEffectiveDateChanged = function () {
                isEffectiveDateChanged = true;
            };

            $scope.scopeModel.onFutureRateClicked = function (dataItem) {
                WhS_SMSBusinessEntity_CustomerRatePlanService.viewFutureSMSRate(dataItem.MobileNetworkName, dataItem.FutureRate);
            };

            $scope.scopeModel.saveChanges = function () {
                if (newSmsRateHistories.length > 0 || isEffectiveDateChanged) {
                    saveChanges().finally(function () {
                        $scope.modalContext.closeModal();
                    });
                }
                else {
                    $scope.modalContext.closeModal();
                }
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            function loadAllControls() {

                function loadDraftData() {
                    loadDraftDataDeferred = UtilsService.createPromiseDeferred();
                    WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.GetDraftData(getDraftDataQuery()).then(function (response) {
                        if (response != undefined) {

                            if (response.CountryLetters && response.CountryLetters.length > 0) {
                                for (var i = 0; i < response.CountryLetters.length; i++) {
                                    $scope.scopeModel.countryLetters.push(response.CountryLetters[i]);
                                }

                                $scope.scopeModel.countryLetters.push(allCountryLetters);
                                $scope.scopeModel.selectedCountryLetterIndex = 0;
                            }

                            $scope.scopeModel.effectiveDate = response.EffectiveDate != undefined ? response.EffectiveDate : VRDateTimeService.getTodayDate();
                        }

                        gridReadyDeferred.promise.then(function () {
                            var letter = $scope.scopeModel.countryLetters.length > 0 ? $scope.scopeModel.countryLetters[0] : undefined;
                            gridAPI.retrieveData(getCustomerSMSChangesQuery(customerInfo.CarrierAccountId, letter));
                            $scope.scopeModel.isLoading = true;
                        });
                    });

                    return loadDraftDataDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadDraftData]).finally(function () {
                    loadDraftDataDeferred = undefined;
                });
            }

            return loadAllControls().catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function getCustomerSMSChangesQuery(customerID, countryChar) {
            return {
                CustomerID: customerID,
                CountryChar: countryChar
            };
        }

        function getDraftDataQuery() {
            return {
                Query: {
                    CustomerID: customerInfo.CarrierAccountId
                }
            };
        }

        function setSMSRateChangeIcon(dataItem) {

            dataItem.RateChangeTypeIcon = WhS_SMSBE_SMSRateChangeTypeEnum.NotChanged.iconUrl;
            dataItem.RateChangeTypeIconDescription = WhS_SMSBE_SMSRateChangeTypeEnum.NotChanged.description;
            dataItem.RateChangeTypeIcon = WhS_SMSBE_SMSRateChangeTypeEnum.NotChanged.iconType;

            //switch (dataItem.RateChangeType) {
            //    case WhS_SMSBE_SMSRateChangeTypeEnum.New.value:
            //        dataItem.RateChangeTypeIcon = WhS_SMSBE_SMSRateChangeTypeEnum.New.iconUrl;
            //        dataItem.RateChangeTypeIconDescription = WhS_SMSBE_SMSRateChangeTypeEnum.New.description;
            //        dataItem.RateChangeTypeIcon = WhS_SMSBE_SMSRateChangeTypeEnum.New.iconType;
            //        break;

            //    case WhS_SMSBE_SMSRateChangeTypeEnum.Increase.value:
            //        dataItem.RateChangeTypeIconUrl = WhS_SMSBE_SMSRateChangeTypeEnum.Increase.iconUrl;
            //        dataItem.RateChangeTypeIconDescription = WhS_SMSBE_SMSRateChangeTypeEnum.Increase.description;
            //        dataItem.RateChangeTypeIcon = WhS_SMSBE_SMSRateChangeTypeEnum.Increase.iconType;
            //        break;

            //    case WhS_SMSBE_SMSRateChangeTypeEnum.Decrease.value:
            //        dataItem.RateChangeTypeIconUrl = WhS_SMSBE_SMSRateChangeTypeEnum.Decrease.iconUrl;
            //        dataItem.RateChangeTypeIconDescription = WhS_SMSBE_SMSRateChangeTypeEnum.Decrease.description;
            //        dataItem.RateChangeTypeIcon = WhS_SMSBE_SMSRateChangeTypeEnum.Decrease.iconType;
            //        break;
            //    case WhS_SMSBE_SMSRateChangeTypeEnum.NotChanged.value:
            //        dataItem.RateChangeTypeIconUrl = WhS_SMSBE_SMSRateChangeTypeEnum.NotChanged.iconUrl;
            //        dataItem.RateChangeTypeIconDescription = WhS_SMSBE_SMSRateChangeTypeEnum.NotChanged.description;
            //        dataItem.RateChangeTypeIcon = WhS_SMSBE_SMSRateChangeTypeEnum.NotChanged.iconType;
            //        break;
            //}
        }

        function buildDraftFromScope() {
            return {
                CustomerID: customerInfo.CarrierAccountId,
                SMSRates: newSmsRateHistories,
                EffectiveDate: $scope.scopeModel.effectiveDate
            };
        }

        function saveChanges() {

            $scope.scopeModel.isLoading = true;
            var customerDraftToUpdate = buildDraftFromScope();

            return WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.InsertOrUpdateChanges(customerDraftToUpdate).then(function (response) {
                if (response) {
                    VRNotificationService.showSuccess("Draft Saved Successfully");
                    if ($scope.onDraftSaved != undefined && typeof ($scope.onDraftSaved) == "function")
                        $scope.onDraftSaved(response.ProcessDraftID);
                }
                else {
                    VRNotificationService.showInformation("Draft Not Saved");
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

    }

    appControllers.controller('WhS_SMSBusinessEntity_CustomerSMSRatePlanEditorController', customerSMSRatePlanEditorController);
})(appControllers);
