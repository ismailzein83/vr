SuspiciousCaseManagementController.$inject = ["$scope", "SuspiciousCaseAPIService", "SuspicionLevelEnum2", "CaseStatusEnum2"];

function SuspiciousCaseManagementController($scope, SuspiciousCaseAPIService, SuspicionLevelEnum2, CaseStatusEnum2) {

    defineScope();
    load();

    function defineScope() {
        $scope.accountNumber = undefined;

        $scope.from = Date.now();
        $scope.to = Date.now();

        $scope.strategies = [];
        $scope.selectedStrategies = [];

        $scope.suspicionLevels = [];
        $scope.selectedSuspicionLevels = [];

        $scope.caseStatuses = [];
        $scope.selectedCaseStatuses = [];
    }

    function load() {
        $scope.isLoadingFilters = true;

        loadSuspicionLevels();
        loadCaseStatuses();

        SuspiciousCaseAPIService.GetAllStrategies()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.strategies.push(item);
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isLoadingFilters = false;
            });
    }

    function loadSuspicionLevels() {
        for (var property in SuspicionLevelEnum2)
            $scope.suspicionLevels.push(SuspicionLevelEnum2[property]);
    }

    function loadCaseStatuses() {
        for (var property in CaseStatusEnum2)
            $scope.caseStatuses.push(CaseStatusEnum2[property]);
    }
}

appControllers.controller("FraudAnalysis_SuspiciousCaseManagementController", SuspiciousCaseManagementController);
