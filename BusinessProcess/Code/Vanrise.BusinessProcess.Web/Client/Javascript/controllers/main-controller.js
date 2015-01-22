
function MainPageCtrl($scope, $location, $rootScope , $http) {
  $http.get("/Vanrise.BusinessProcess.Web/api/BusinessProcess/GetDefinitions")
   .success(function (response) {
       $scope.defnitions = response;
   });
    $scope.myData = [];
    $scope.gridOptions = { data: 'myData' };

 $scope.filter = {};
 $scope.onlickSearch = function () {        
     $http.get("/Vanrise.BusinessProcess.Web/api/BusinessProcess/GetFilteredInstances",
            {
                params: {
                    definitionID: $scope.filter.definitionID.BPDefinitionID,
                    datefrom: dateToStringWithHMS($scope.filter.FromDate),
                    dateto: dateToStringWithHMS($scope.filter.ToDate)
                }
            })
        .success(function (response) {
            //$scope.Instances = response;
            $scope.myData = response;
        });
    };
}