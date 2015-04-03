function MainPageCtrl($scope, $location, $rootScope, $http) {
   // $('.collapse').collapse();
    $http.get(baseurl + "/api/BusinessProcess/GetDefinitions")
     .success(function (response) {
        $scope.defnitions = response;
    });
    $http.get(baseurl + "/api/BusinessProcess/GetStatusList")
      .success(function (response) {
        $scope.bpInstanceStatusList = response;
    });
    $scope.myData = [];
    $scope.gridOptions = {
        data: 'myData',
        enableHorizontalScrollbar: 0,
        enableVerticalScrollbar: 0,
        columnDefs: [
          { name: 'Title', field: 'Title' ,height:40},
          { name: 'Status', field: 'StatusDescription', height: 40, width: 100 },
          { name: 'Last Message', field: 'LastMessage', height: 40, width:250 },
          { name: 'CreatedTime', field: 'CreatedTime', type: 'date', cellFilter: 'date:"yyyy-MM-dd HH:mm:ss"', height: 40, width: 150 }
        ],
        paginationPageSizes: [10, 25, 50, 75],
        paginationPageSize: 10,
        appScopeProvider: {
            onDblClick: function (row) {
                $location.path("/ListTracking/" + row.entity.ProcessInstanceID).replace();
            },
            onDblClick: function (row) {
                $location.path("/ListTracking/" + row.entity.ProcessInstanceID).replace();
            }
        },
        rowTemplate: '<div ng-dblclick=\"grid.appScope.onDblClick(row)\"   ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell hand" ui-grid-cell></div>'

    };
    var d = new Date();
    if ($rootScope.filter.FromDate == null && $rootScope.filter.definitionID == null) {
        $rootScope.filter.FromDate = d;
        $rootScope.filter.definitionID = { BPDefinitionID: 1 };
    }
    $scope.onclickClear = function () {
        $rootScope.filter = {};
    }
    $scope.isloadingdata = false;
    $scope.pageNumber = 1;
    $scope.pageSize = 10;
    $scope.onclickSearch = function () {
        $scope.isloadingdata = true;
        $http.get(baseurl + "/api/BusinessProcess/GetFilteredInstances",
            {
                params: {
                    definitionID: $rootScope.filter.definitionID.BPDefinitionID,
                    datefrom: dateToStringWithHMS($rootScope.filter.FromDate),
                    dateto: dateToStringWithHMS($rootScope.filter.EndDate),
                    pageNumber: $scope.pageNumber,
                    pageSize:$scope.pageSize
                }
            })
        .success(function (response) {
           // $scope.isloadingdata = false;
            $scope.myData = response;
            $scope.last = (response.length < 10) ? false : true;
          
           
        });
    };
    if (typeof ($rootScope.filter.definitionID) != 'undefined') {
        $scope.onclickSearch();
    }
    $scope.goPrevious = function () {
        $scope.pageNumber--;
        $scope.onclickSearch();

    }
    $scope.goNext = function () {
        $scope.pageNumber++;
        $scope.onclickSearch();
    }



}