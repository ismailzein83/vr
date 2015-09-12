var FindRelatedNumbersProcessInputController = function ($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {
    var pageLoaded = false;

    defineScope();

    function defineScope() {

        $scope.createProcessInputObjects = [];

        $scope.createProcessInput.getData = function () {

            $scope.createProcessInputObjects.length = 0;

            $scope.createProcessInputObjects.push({
                InputArguments: {
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.AssignStrategyCasesProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments"
                }
            });

            return $scope.createProcessInputObjects;

        };


    }


}

FindRelatedNumbersProcessInputController.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
appControllers.controller('FraudAnalysis_FindRelatedNumbersProcessInputController', FindRelatedNumbersProcessInputController)



