var CDRImportProcessInput_Scheduled = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.bpDefinitions = [];

        $scope.schedulerTaskAction.processInputArguments.getData = function () {
            return {
                $type: "Vanrise.Fzero.CDRImport.BP.Arguments.CDRImportProcessInput, Vanrise.Fzero.CDRImport.BP.Arguments"
            };
        };

        $scope.schedulerTaskAction.rawExpressions.getData = function () {
                return undefined;
        };

        loadForm();
    }

    function loadForm() {

        if ($scope.schedulerTaskAction.processInputArguments.data == undefined)
            return;
    }

    function load() {

    }



    

}

CDRImportProcessInput_Scheduled.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];
appControllers.controller('FraudAnalysis_CDRImportProcessInput_Scheduled', CDRImportProcessInput_Scheduled)



