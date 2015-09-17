"use strict";

SuspiciousNumberDetailsController.$inject = ["$scope", "CaseManagementAPIService", "NormalCDRAPIService", "NumberProfileAPIService", "StrategyAPIService", "UsersAPIService", "SuspicionLevelEnum", "CaseStatusEnum", "CallTypeEnum", "LabelColorsEnum", "UtilsService", "VRNavigationService", "VRNotificationService", "VRModalService"];

function SuspiciousNumberDetailsController($scope, CaseManagementAPIService, NormalCDRAPIService, NumberProfileAPIService, StrategyAPIService, UsersAPIService, SuspicionLevelEnum, CaseStatusEnum, CallTypeEnum, LabelColorsEnum, UtilsService, VRNavigationService, VRNotificationService, VRModalService) {
    var gridAPI_Occurances = undefined;
    var occurancesLoaded = false;

    var gridAPI_NormalCDRs = undefined;
    var normalCDRsLoaded = false;

    var gridAPI_NumberProfiles = undefined;
    var numberProfilesLoaded = false;

    var gridAPI_CaseHistory = undefined;
    var casesLoaded = false;

    var gridAPI_RelatedNumbers = undefined;
    var sequencedNumbers = [];
    var IMEIs = [];

    var modalLevel = undefined;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            $scope.accountNumber = parameters.AccountNumber;
            $scope.fromDate = parameters.FromDate;
            $scope.toDate = parameters.ToDate;

            modalLevel = parameters.ModalLevel;
            $scope.showRelatedNumbers = (modalLevel == 1);
        }
    }

    function defineScope() {
        $scope.users = []; // the users array must be defined on the scope so that it can be passed to the case logs subgrid via viewScope
        $scope.selectedTabIndex = 0;

        // Current Occurances
        $scope.occurances = [];
        $scope.showOccurancesGrid = true;
        $scope.message = undefined;

        // Normal CDRs
        $scope.fromDate_NormalCDRs = $scope.fromDate;
        $scope.toDate_NormalCDRs = $scope.toDate;
        $scope.normalCDRs = [];

        // Number Profiles
        $scope.profileSources = [
            { value: 0, description: "From Strategy Execution" },
            { value: 1, description: "From Profiling" }
        ];
        $scope.selectedProfileSource = $scope.profileSources[0];

        $scope.fromDate_NumberProfiles = $scope.fromDate;
        $scope.toDate_NumberProfiles = $scope.toDate;

        $scope.aggregateDefinitions = []; // column names
        $scope.detailAggregateValues = [];
        $scope.numberProfile = [];
        
        $scope.showProfileOptions = true;
        $scope.showDate = false;
        
        // Case History
        $scope.cases = [];

        // Related Numbers
        $scope.reasons = [
            { value: 0, description: "Sequence" },
            { value: 1, description: "IMEIs" }
        ];
        $scope.selectedReason = $scope.reasons[0];
        $scope.relatedNumbers = [];

        // Update Case
        $scope.caseStatuses = UtilsService.getArrayEnum(CaseStatusEnum);
        $scope.selectedCaseStatus = undefined;
        $scope.updateReason = undefined;
        $scope.validTill = undefined;
        $scope.whiteListSelected = false;

        $scope.onGridReady_Occurances = function (api) {
            gridAPI_Occurances = api;
            return retrieveData_Occurances();
        }

        $scope.onGridReady_NormalCDRs = function (api) {
            gridAPI_NormalCDRs = api;
        }

        $scope.onGridReady_NumberProfiles = function (api) {
            gridAPI_NumberProfiles = api;
        }

        $scope.onGridReady_CaseHistory = function (api) {
            gridAPI_CaseHistory = api;
        }

        $scope.onGridReady_RelatedNumbers = function (api) {
            gridAPI_RelatedNumbers = api;

            if (sequencedNumbers.length == 0) {
                setSequencedNumbers();

                angular.forEach(sequencedNumbers, function (item) {
                    $scope.relatedNumbers.push(item);
                });
            }
        }

        $scope.relatedNumberClicked = function (dataItem) {
            openRelatedNumber(dataItem.RelatedNumber);
        };

        $scope.dataRetrievalFunction_Occurances = function (dataRetrievalInput, onResponseReady) {

            return CaseManagementAPIService.GetFilteredAccountSuspicionDetails(dataRetrievalInput)
            .then(function (response) {

                occurancesLoaded = true;
                
                if (response.Data != undefined) { // else, the export button was clicked

                    if (response.Data.length > 0)
                        $scope.detailAggregateValues.push(response.Data[0]);
                    else {
                        $scope.showOccurancesGrid = false;
                        $scope.message = "No open occurances were found for the current account number";
                        $scope.showProfileOptions = false;
                        $scope.showDate = true;
                    }

                    angular.forEach(response.Data, function (item) {
                        var suspicionLevel = UtilsService.getEnum(SuspicionLevelEnum, "value", item.SuspicionLevelID);
                        item.SuspicionLevelDescription = suspicionLevel.description;
                    });
                }

                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        $scope.dataRetrievalFunction_NormalCDRs = function (dataRetrievalInput, onResponseReady) {

            return NormalCDRAPIService.GetNormalCDRs(dataRetrievalInput)
            .then(function (response) {
                normalCDRsLoaded = true;

                angular.forEach(response.Data, function (item) {
                    var callType = UtilsService.getEnum(CallTypeEnum, "value", item.CallType);
                    item.CallTypeDescription = callType.description;
                });

                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        $scope.dataRetrievalFunction_NumberProfiles = function (dataRetrievalInput, onResponseReady) {

            return NumberProfileAPIService.GetNumberProfiles(dataRetrievalInput)
            .then(function (response) {
                numberProfilesLoaded = true;
                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        $scope.dataRetrievalFunction_CaseHistory = function (dataRetrievalInput, onResponseReady) {

            return CaseManagementAPIService.GetFilteredCasesByAccountNumber(dataRetrievalInput)
            .then(function (response) {
                casesLoaded = true;

                angular.forEach(response.Data, function (item) {
                    var caseStatus = UtilsService.getEnum(CaseStatusEnum, "value", item.StatusID);
                    item.CaseStatusDescription = caseStatus.description;

                    var user = UtilsService.getItemByVal($scope.users, item.UserID, "UserId");
                    item.UserName = (user != null) ? user.Name : "System";
                });

                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        $scope.updateAccountCase = function () {

            return CaseManagementAPIService.UpdateAccountCase({
                    accountNumber: $scope.accountNumber,
                    caseStatus: $scope.selectedCaseStatus.value,
                    validTill: $scope.validTill,
                    FromDate: $scope.fromDate,
                    ToDate: $scope.toDate,
                    Reason: $scope.updateReason
                })
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Account Case", response)) {
                        if ($scope.onAccountCaseUpdated != undefined) {

                            if (response.UpdatedObject.SuspicionLevelID != 0) {
                                var suspicionLevel = UtilsService.getEnum(SuspicionLevelEnum, "value", response.UpdatedObject.SuspicionLevelID);
                                response.UpdatedObject.SuspicionLevelDescription = suspicionLevel.description;
                            }

                            var accountStatus = UtilsService.getEnum(CaseStatusEnum, "value", response.UpdatedObject.AccountStatusID);
                            response.UpdatedObject.AccountStatusDescription = accountStatus.description;

                            $scope.onAccountCaseUpdated(response.UpdatedObject);
                        }
                        
                        $scope.modalContext.closeModal();
                    }

                    $scope.modalContext.closeModal();
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        }

        $scope.filterData = function () {

            if ($scope.selectedTabIndex == 1)
                normalCDRsLoaded = false; // re-load the normal cdrs

            else if ($scope.selectedTabIndex == 2)
                numberProfilesLoaded = false; // re-load the number profiles

            return retrieveData();
        }

        $scope.onSelectedTabChanged = function () {
            return retrieveData();
        }

        $scope.onProfileSourceChanged = function () {

            if ($scope.selectedProfileSource != null)
            {
                if ($scope.selectedProfileSource.value == 1) {
                    $scope.showDate = true;
                    retrieveData_NumberProfiles();
                }
                else
                    $scope.showDate = false;
            }
        }

        $scope.toggleValidTill = function (selectedStatus) {
            $scope.whiteListSelected = (selectedStatus != undefined && selectedStatus.value == CaseStatusEnum.ClosedWhitelist.value);
        }
        
        $scope.onReasonChanged = function () {

            if ($scope.selectedReason != undefined) {
                if ($scope.selectedReason.value == 0) {

                    if (sequencedNumbers.length == 0)
                        setSequencedNumbers();

                    angular.forEach(sequencedNumbers, function (item) {
                        $scope.relatedNumbers.push(item);
                    });
                }
                else {

                    if (IMEIs.length == 0)
                        loadRelatedNumbers();
                    else {
                        $scope.relatedNumbers = [];

                        angular.forEach(IMEIs, function (item) {
                            $scope.relatedNumbers.push(item);
                        });
                    }
                }
            }
        }

        $scope.getSuspicionLevelColor = function (dataItem) {

            if (dataItem.SuspicionLevelID == SuspicionLevelEnum.Suspicious.value) return LabelColorsEnum.WarningLevel1.color;
            else if (dataItem.SuspicionLevelID == SuspicionLevelEnum.HighlySuspicious.value) return LabelColorsEnum.WarningLevel2.color;
            else if (dataItem.SuspicionLevelID == SuspicionLevelEnum.Fraud.value) return LabelColorsEnum.Error.color;
        }

        $scope.getCaseStatusColor = function (dataItem) {

            if (dataItem.StatusID == CaseStatusEnum.Open.value) return LabelColorsEnum.New.color;
            else if (dataItem.StatusID == CaseStatusEnum.Pending.value) return LabelColorsEnum.Processing.color;
            else if (dataItem.StatusID == CaseStatusEnum.ClosedFraud.value) return LabelColorsEnum.Error.color;
            else if (dataItem.StatusID == CaseStatusEnum.ClosedWhitelist.value) return LabelColorsEnum.Success.color;
        }
    }

    function load() {
        $scope.isInitializing = true;
        
        return UtilsService.waitMultipleAsyncOperations([loadAggregateDefinitions, loadUsers, loadAccountStatus])
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isInitializing = false;
            });
    }

    // retrieveData is invoked when the selected tab changes or when the date fields are changed
    function retrieveData() {

        if (gridAPI_Occurances != undefined && $scope.selectedTabIndex == 0 && !occurancesLoaded)
            return retrieveData_Occurances();

        else if (gridAPI_NormalCDRs != undefined && $scope.selectedTabIndex == 1 && !normalCDRsLoaded)
            return retrieveData_NormalCDRs();

        else if (gridAPI_NumberProfiles != undefined && $scope.selectedTabIndex == 2 && !numberProfilesLoaded)
            return retrieveData_NumberProfiles();

        else if (gridAPI_CaseHistory != undefined && $scope.selectedTabIndex == 3 && !casesLoaded)
            return retrieveData_CaseHistory();
        
    }

    function retrieveData_Occurances() {

        var query = {
            AccountNumber: $scope.accountNumber,
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate
        };

        return gridAPI_Occurances.retrieveData(query);
    }

    function retrieveData_NormalCDRs() {

        var query = {
            MSISDN: $scope.accountNumber,
            FromDate: $scope.fromDate_NormalCDRs,
            ToDate: $scope.toDate_NormalCDRs,
        };

        return gridAPI_NormalCDRs.retrieveData(query);
    }

    function retrieveData_NumberProfiles() {

        var query = {
            AccountNumber: $scope.accountNumber,
            FromDate: $scope.fromDate_NumberProfiles,
            ToDate: $scope.toDate_NumberProfiles
        };

        return gridAPI_NumberProfiles.retrieveData(query);
    }

    function retrieveData_CaseHistory() {

        var query = {
            AccountNumber: $scope.accountNumber
        };

        return gridAPI_CaseHistory.retrieveData(query);
    }

    function loadAccountStatus() {
        return CaseManagementAPIService.GetAccountStatus($scope.accountNumber)
            .then(function (response) {

                if (response != null) {
                    // open, pending, fraud, whitelist

                    $scope.caseStatuses.splice(0, 1); // remove the open option

                    if (response == CaseStatusEnum.Pending.value)
                        $scope.caseStatuses.splice(0, 1); // remove the pending option

                    else if (response == CaseStatusEnum.ClosedFraud.value)
                        $scope.caseStatuses.splice(1, 1); // remove the closed: fruad option

                    else if (response == CaseStatusEnum.ClosedWhitelist.value)
                        $scope.caseStatuses.splice(2, 1); // remove the closed: white list option

                    var accountStatus = UtilsService.getEnum(CaseStatusEnum, "value", response);
                    $scope.accountStatusID = response;
                    $scope.accountStatusDescription = accountStatus.description;
                }
            });
    }

    function loadAggregateDefinitions() {
        return StrategyAPIService.GetAggregates()
            .then(function (response) {

                angular.forEach(response, function (item) {
                    $scope.aggregateDefinitions.push({ name: item.Name , numberPrecision: item.NumberPrecision });
                });
            })
    }

    function loadUsers() {
        return UsersAPIService.GetUsers()
            .then(function (response) {

                angular.forEach(response, function (item) {
                    $scope.users.push(item);
                });
            });
    }

    function loadRelatedNumbers() {
        $scope.relatedNumbers = [];

        return CaseManagementAPIService.GetRelatedNumbersByAccountNumber($scope.accountNumber)
            .then(function (response) {

                angular.forEach(response, function (item) {
                    $scope.relatedNumbers.push({ RelatedNumber: item.AccountNumber });
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function setSequencedNumbers() {

        for (var i = 10; i >= 1; i--) {
            sequencedNumbers.push({ RelatedNumber: ($scope.accountNumber - i) });
        }

        for (var i = 1; i <= 10; i++) {
            sequencedNumbers.push({ RelatedNumber: (Number($scope.accountNumber) + i) });
        }
    }

    function openRelatedNumber(relatedNumber) {
        var modalSettings = {};

        var parameters = {
            AccountNumber: relatedNumber,
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            ModalLevel: modalLevel + 1
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Suspicious Number Details";
            modalScope.onAccountCaseUpdated = function (accountSuspicionSummary) {
                $scope.onAccountCaseUpdated(accountSuspicionSummary);
            }
        };

        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/SuspiciousAnalysis/SuspiciousNumberDetails.html", parameters, modalSettings);
    }
}

appControllers.controller("FraudAnalysis_SuspiciousNumberDetailsController", SuspiciousNumberDetailsController);
