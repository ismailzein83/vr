
function MainPageCtrl($scope, $location, $rootScope , $http) {
  $http.get("/BusinessProcess/api/BusinessProcess/GetDefinitions")
   .success(function (response) {
       $scope.defnitions = response;
   });
    $scope.myData = [];
    $scope.gridOptions = {
        data: 'myData',
        rowTemplate: '<div ng-click="onDblClickRow(row)" ng-style="{ \'cursor\': row.cursor }" ng-repeat="col in renderedColumns" ng-class="col.colIndex()" class="ngCell {{col.cellClass}}"><div class="ngVerticalBar" ng-style="{height: rowHeight}" ng-class="{ ngVerticalBarVisible: !$last }">&nbsp;</div><div ng-cell></div></div>',
        columnDefs: [
            { field: 'Title', displayName: 'Title' },
            { field: 'Status', displayName: 'Status' },
            { field: 'LastMessage', displayName: 'Error / Warning' },
            { field: 'CreatedTime', displayName: 'Created Time' }
        ],
        multiSelect: false,
    };
    $scope.onDblClickRow = function (rowItem) {
      
        $http.get("/BusinessProcess/api/BusinessProcess/GetTrackingsByInstanceId",
             {
                 params: {
                     ProcessInstanceID: rowItem.entity.ProcessInstanceID
                 }
             })
         .success(function (response) {
             //$scope.Instances = response;
             //$scope.myData = response;
             console.log(response);
         });
    };
 $scope.filter = {};
 $scope.onlickSearch = function () {        
     $http.get("/BusinessProcess/api/BusinessProcess/GetFilteredInstances",
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