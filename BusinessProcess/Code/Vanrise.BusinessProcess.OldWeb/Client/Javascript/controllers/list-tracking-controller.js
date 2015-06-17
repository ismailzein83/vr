
function ListTrackingCtrl($scope, $location, $rootScope, $http, $routeParams) {
    $scope.listTracking = [];
    $scope.listTrackingOptions = {
        data: 'listTracking',
        enableHorizontalScrollbar: 0,
        columnDefs: [
         { name: 'Id', field: 'ProcessInstanceId' },
         { name: 'Severity', field: 'SeverityDescription' },
         { name: 'Message', field: 'Message' },
         { name: 'EventTime', field: 'EventTime', type: 'date', cellFilter: 'date:"yyyy-MM-dd HH:mm:ss"' }
        ],
    };
   
    $http.get(baseurl + "/api/BusinessProcess/GetTrackingsByInstanceId",
        {
            params: {
                ProcessInstanceID: $routeParams.ProcessInstanceID
            }
        })
    .success(function (response) {
        console.log(response);
        $scope.listTracking = response;
    });
   
}