var FindRelatedNumbersProcessInputController = function ($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {
    var pageLoaded = false;

    defineScope();

    function defineScope() {

        $scope.createProcessInputObjects = [];

        $scope.createProcessInput.getData = function () {

            $scope.createProcessInputObjects.length = 0;

            $scope.createProcessInputObjects.push({
                InputArguments: {
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.FindRelatedNumbersProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                    FromDate: new Date($scope.fromDate),
                    ToDate: new Date($scope.toDate)
                }
            });

            return $scope.createProcessInputObjects;

        };


    }


}

FindRelatedNumbersProcessInputController.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
appControllers.controller('FraudAnalysis_FindRelatedNumbersProcessInputController', FindRelatedNumbersProcessInputController)



