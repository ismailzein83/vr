SuspiciousNumberDetails2Controller.$inject = ["$scope", "CaseManagementAPIService", "NormalCDRAPIService", "NumberProfileAPIService", "SuspicionLevelEnum", "CaseStatusEnum2", "SuspicionOccuranceStatusEnum", "CallTypeEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

function SuspiciousNumberDetails2Controller($scope, CaseManagementAPIService, NormalCDRAPIService, NumberProfileAPIService, SuspicionLevelEnum, CaseStatusEnum2, SuspicionOccuranceStatusEnum, CallTypeEnum, UtilsService, VRNavigationService, VRNotificationService) {

    var gridAPI_Occurances = undefined;
    var occurancesLoaded = false;

    var gridAPI_NormalCDRs = undefined;
    var normalCDRsLoaded = false;

    var gridAPI_NumberProfiles = undefined;
    var numberProfilesLoaded = false;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            $scope.accountNumber = parameters.AccountNumber;
            $scope.from = parameters.From;
            $scope.to = parameters.To;
        }
    }

    function defineScope() {

        $scope.from = "01/01/2014 00:00";
        $scope.to = "02/01/2014 00:00";

        $scope.selectedTabIndex = 0;

        $scope.occurances = [];
        $scope.normalCDRs = [];
        $scope.numberProfiles = [];

        $scope.caseStatuses = [];
        $scope.selectedCaseStatus = undefined;
        $scope.whiteListSelected = false;

        $scope.onGridReady_Occurances = function (api) {
            gridAPI_Occurances = api;

            if ($scope.occurancesSelected)
                return retrieveData_Occurances();
        }

        $scope.onGridReady_NormalCDRs = function (api) {
            gridAPI_NormalCDRs = api;

            if ($scope.normalCDRsSelected)
                return retrieveData_NormalCDRs();
        }

        $scope.onGridReady_NumberProfiles = function (api) {
            gridAPI_NumberProfiles = api;

            if ($scope.numberProfilesSelected)
                return retrieveData_NumberProfiles();
        }

        $scope.dataRetrievalFunction_Occurances = function (dataRetrievalInput, onResponseReady) {

            return CaseManagementAPIService.GetFilteredAccountSuspicionDetails(dataRetrievalInput)
            .then(function (response) {
                occurancesLoaded = true;

                angular.forEach(response.Data, function (item) {
                    var suspicionLevel = UtilsService.getEnum(SuspicionLevelEnum, "value", item.SuspicionLevelID);
                    item.SuspicionLevelDescription = suspicionLevel.description;

                    var accountStatus = UtilsService.getEnum(SuspicionOccuranceStatusEnum, "value", item.AccountStatusID);
                    item.AccountStatusDescription = accountStatus.description;
                });

                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
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
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        $scope.dataRetrievalFunction_NumberProfiles = function (dataRetrievalInput, onResponseReady) {

            return NumberProfileAPIService.GetNumberProfiles(dataRetrievalInput)
            .then(function (response) {
                numberProfilesLoaded = true;
                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        $scope.updateAccountCase = function () {

            return CaseManagementAPIService.UpdateAccountCase({
                    accountNumber: $scope.accountNumber,
                    caseStatus: $scope.selectedCaseStatus.value,
                    validTill: $scope.validTill,
                    from: $scope.from,
                    to: $scope.to
                })
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Account Case", response)) {
                        if ($scope.onAccountCaseUpdated != undefined) {

                            var suspicionLevel = UtilsService.getEnum(SuspicionLevelEnum, "value", response.UpdatedObject.SuspicionLevelID);
                            response.UpdatedObject.SuspicionLevelDescription = suspicionLevel.description;

                            var accountStatus = UtilsService.getEnum(CaseStatusEnum2, "value", response.UpdatedObject.AccountStatusID);
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

        $scope.filterDetails = function () {

            if ($scope.selectedTabIndex == 0)
                occurancesLoaded = false; // re-load the occurances

            else if ($scope.selectedTabIndex == 1)
                normalCDRsLoaded = false; // re-load the normal cdrs

            else if ($scope.selectedTabIndex == 2)
                numberProfilesLoaded = false; // re-load the number profiles

            return retrieveData();
        }

        $scope.onSelectedTabChanged = function () {
            return retrieveData();
        }

        $scope.toggleValidTill = function (selectedStatus) {
            $scope.whiteListSelected = (selectedStatus != undefined && selectedStatus.value == CaseStatusEnum2.ClosedWhitelist.value) ? true : false;
        }
    }

    function load() {
        $scope.caseStatuses = UtilsService.getArrayEnum(CaseStatusEnum2);
    }

    function retrieveData() {

        if (gridAPI_Occurances != undefined && $scope.selectedTabIndex == 0 && !occurancesLoaded)
            return retrieveData_Occurances();

        else if (gridAPI_NormalCDRs != undefined && $scope.selectedTabIndex == 1 && !normalCDRsLoaded)
            return retrieveData_NormalCDRs();

        else if (gridAPI_NumberProfiles != undefined && $scope.selectedTabIndex == 2 && !numberProfilesLoaded)
            return retrieveData_NumberProfiles();
    }

    function retrieveData_Occurances() {

        var query = {
            AccountNumber: $scope.accountNumber,
            From: $scope.from,
            To: $scope.to
        };

        return gridAPI_Occurances.retrieveData(query);
    }

    function retrieveData_NormalCDRs() {

        var query = {
            FromDate: $scope.from,
            ToDate: $scope.to,
            MSISDN: $scope.accountNumber
        };

        return gridAPI_NormalCDRs.retrieveData(query);
    }

    function retrieveData_NumberProfiles() {

        var query = {
            FromDate: $scope.from,
            ToDate: $scope.to,
            AccountNumber: $scope.accountNumber
        };

        return gridAPI_NumberProfiles.retrieveData(query);
    }
}

appControllers.controller("FraudAnalysis_SuspiciousNumberDetails2Controller", SuspiciousNumberDetails2Controller);
