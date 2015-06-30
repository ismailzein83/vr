var SaveCDRToDBProcessInputController = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {
    defineScope();
   
    function defineScope() {
        $scope.createProcessInput.getData = function () {

            var createProcessInputObject = {
                InputArguments: {
                    $type: "Vanrise.Fzero.CDRImport.BP.Arguments.SaveCDRToDBProcessInput, Vanrise.Fzero.CDRImport.BP.Arguments"
                }
            };

            return createProcessInputObject;
        };
    }

}

SaveCDRToDBProcessInputController.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
appControllers.controller('FraudAnalysis_SaveCDRToDBProcessInputController', SaveCDRToDBProcessInputController)



