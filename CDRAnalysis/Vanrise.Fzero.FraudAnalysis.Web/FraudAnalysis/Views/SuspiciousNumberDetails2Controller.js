SuspiciousNumberDetails2Controller.$inject = ["$scope", "SuspicionAnalysisAPIService", "SuspicionLevelEnum", "SuspicionOccuranceStatusEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

function SuspiciousNumberDetails2Controller($scope, SuspicionAnalysisAPIService, SuspicionLevelEnum, SuspicionOccuranceStatusEnum, UtilsService, VRNavigationService, VRNotificationService) {

    var gridAPI = undefined;

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
        $scope.to = "01/01/2014 00:00";

        $scope.logs = [];
        $scope.caseStatuses = [];
        $scope.selectedCaseStatus = undefined;
        $scope.whiteListSelected = false;

        $scope.onGridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return SuspicionAnalysisAPIService.GetFilteredAccountSuspicionDetails(dataRetrievalInput)
                .then(function (response) {
                    console.log(response);

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

        $scope.updateAccountCase = function () {
            return SuspicionAnalysisAPIService.UpdateAccountCase($scope.accountNumber, $scope.selectedCaseStatus, $scope.validTill)
                .then(function (response) {
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
            if (gridAPI != undefined)
                return retrieveData();
        }

        $scope.toggleValidTill = function (selectedStatus) {
            $scope.whiteListSelected = (selectedStatus != undefined && selectedStatus.value == SuspicionOccuranceStatusEnum.ClosedWhitelist.value) ? true : false;
        }
    }

    function load() {
        $scope.caseStatuses = UtilsService.getArrayEnum(SuspicionOccuranceStatusEnum);
    }

    function retrieveData() {
        var query = {
            AccountNumber: $scope.accountNumber,
            From: $scope.from,
            To: $scope.to
        };

        console.log(query);
        return gridAPI.retrieveData(query);
    }
}

appControllers.controller("FraudAnalysis_SuspiciousNumberDetails2Controller", SuspiciousNumberDetails2Controller);
