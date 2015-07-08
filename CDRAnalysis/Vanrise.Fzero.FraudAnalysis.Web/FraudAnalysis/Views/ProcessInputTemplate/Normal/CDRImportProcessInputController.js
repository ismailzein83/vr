var CDRImportProcessInputController = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {
    defineScope();
   
    function defineScope() {
        $scope.createProcessInput.getData = function () {

            var createProcessInputObject = {
                InputArguments: {
                    $type: "Vanrise.Fzero.CDRImport.BP.Arguments.CDRImportProcessInput, Vanrise.Fzero.CDRImport.BP.Arguments"
                }
            };

            return createProcessInputObject;
        };
    }

}

CDRImportProcessInputController.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
appControllers.controller('FraudAnalysis_CDRImportProcessInputController', CDRImportProcessInputController)



