appControllers.controller('SwitchsController', ['$scope', '$http', function ($scope, $http) {
    $scope.SwitchItems = [];
    $scope.allData = [];
    $scope.sortType = 'SwitchId';
    $scope.sortReverse = false;
    $scope.searchSwitchs = '';
    $scope.sortData = function (name) {
        $scope.sortType = name;
        $scope.sortReverse = !$scope.sortReverse;
    }

    $scope.refresh = function () {

        $http({
            url: "/api/BusinessEntity/GetSwitches",
            method: "GET"
        }).success(function (data, status, headers, config) {
            $scope.totalServerItems = data.length;
            $scope.SwitchItems = data;
            $scope.gridOptions.data = data;


        }).error(function (data, status, headers, config) {
            alert(JSON.stringify(data));
        });

    };
    //$scope.refresh();


    /****************************************************/
    $scope.getObjectsFromApi = function () {
        return $http({
            url: "/api/BusinessEntity/GetSwitches",
            method: "GET"
        });
    }
    $scope.getObjectsFromService = function () {
        $scope.getObjectsFromApi()
        .success(function (data, status, headers, config) {
            $scope.totalServerItems = data.length;
            $scope.SwitchItems = data;
            $scope.gridOptions.data = data;
        })
        .error(function (data, status, headers, config) {
            alert(JSON.stringify(data));
        });
    };

    /***************************************************/



    $scope.BindSwitchs = function () {

        var data = $scope.getObjectsFromService();
        alert(data);
        $scope.totalServerItems = data.length;
        $scope.SwitchItems = data;
        $scope.gridOptions.data = data;
    };

    $scope.getObjectsFromService();

    $scope.addSwitch = function () {
        $scope.SwitchItems.push({ 'SwitchId': $scope.switchIDInput, 'Name': $scope.switchNameInput });
        $scope.SwitchId = '';
        $scope.Name = '';
    };
    $scope.removeSwitch = function (SwitchId) {
        var index = -1;
        var comArr = eval($scope.SwitchItems);
        for (var i = 0; i < comArr.length; i++) {
            if (comArr[i].SwitchId === SwitchId) {
                index = i;
                break;
            }
        }
        if (index === -1) {
            alert("Something gone wrong");
        }
        $scope.SwitchItems.splice(index, 1);
    };



    /***********************************************************Paging***************************************************/


    $scope.filterOptions = {
        filterText: "",
        useExternalFilter: true
    };
    $scope.totalServerItems = 0;
    $scope.pagingOptions = {
        pageSizes: [10, 15, 20],
        pageSize: 20,//$scope.PageSideNeeded
        currentPage: 1 //$scope.PageIndexNeeded
    };
    $scope.setPagingData = function () {
        $scope.refresh();
        var data = $scope.SwitchItems;
        var page = $scope.PageIndexNeeded;
        var pageSize = $scope.PageSideNeeded;
        alert('page' + page + 'pageSize' + pageSize + 'data' + data.length);
        var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
        $scope.SwitchItems = pagedData;
        $scope.totalServerItems = pagedData.length;
        $scope.gridOptions.data = pagedData;
        alert(pagedData.length);
    };

    $scope.$watch('pagingOptions', function (newVal, oldVal) {
        if (newVal !== oldVal && newVal.currentPage !== oldVal.currentPage) {
            $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage, $scope.filterOptions.filterText);
        }
    }, true);
    $scope.$watch('filterOptions', function (newVal, oldVal) {
        if (newVal !== oldVal) {
            $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage, $scope.filterOptions.filterText);
        }
    }, true);

    $scope.gridOptions = {
        data: 'SwitchItems',
        enablePaging: true,
        showFooter: true,
        totalServerItems: 'totalServerItems',
        pagingOptions: $scope.pagingOptions,
        filterOptions: $scope.filterOptions
    };


    /*********************************************************END PAGING*************************************************/
    //$scope.filterOptions = {
    //    filterText: "",
    //    useExternalFilter: true
    //};
    //$scope.totalServerItems = 0;
    //$scope.pagingOptions = {
    //    pageSizes: [250, 500, 1000],
    //    pageSize: 250,
    //    currentPage: 1
    //};
    //$scope.setPagingData = function (data, page, pageSize) {
    //    var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
    //    $scope.myData = pagedData;
    //    $scope.totalServerItems = data.length;
    //    if (!$scope.$$phase) {
    //        $scope.$apply();
    //    }
    //};
    //$scope.getPagedDataAsync = function (pageSize, page, searchText) {
    //    setTimeout(function () {
    //        var data;
    //        if (searchText) {
    //            var ft = searchText.toLowerCase();
    //            $http.get('jsonFiles/largeLoad.json').success(function (largeLoad) {
    //                data = largeLoad.filter(function (item) {
    //                    return JSON.stringify(item).toLowerCase().indexOf(ft) != -1;
    //                });
    //                $scope.setPagingData(data, page, pageSize);
    //            });
    //        } else {
    //            $http.get('jsonFiles/largeLoad.json').success(function (largeLoad) {
    //                $scope.setPagingData(largeLoad, page, pageSize);
    //            });
    //        }
    //    }, 100);
    //};

    //$scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);

    //$scope.$watch('pagingOptions', function (newVal, oldVal) {
    //    if (newVal !== oldVal && newVal.currentPage !== oldVal.currentPage) {
    //        $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage, $scope.filterOptions.filterText);
    //    }
    //}, true);
    //$scope.$watch('filterOptions', function (newVal, oldVal) {
    //    if (newVal !== oldVal) {
    //        $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage, $scope.filterOptions.filterText);
    //    }
    //}, true);

    //$scope.gridOptions = {
    //    data: 'myData',
    //    enablePaging: true,
    //    showFooter: true,
    //    totalServerItems: 'totalServerItems',
    //    pagingOptions: $scope.pagingOptions,
    //    filterOptions: $scope.filterOptions
    //};


}]);