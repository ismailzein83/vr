SuspiciousNumberDetails2Controller.$inject = ["$scope", "SuspicionAnalysisAPIService", "VRNavigationService"];

function SuspiciousNumberDetails2Controller($scope, SuspicionAnalysisAPIService, VRNavigationService) {

    var gridAPI = undefined;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            $scope.accountNumber = parameters.AccountNumber;
            $scope.suspicionLevelDescription = parameters.SuspicionLevelDescription;
            $scope.from = parameters.From;
            $scope.to = parameters.To;
        }
    }

    function defineScope() {

        $scope.logs = [];

        $scope.onGridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return SuspicionAnalysisAPIService.GetFilteredAccountSuspicionDetails(dataRetrievalInput)
                .then(function (response) {
                    console.log(response);
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        }
    }

    function load() { }

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
