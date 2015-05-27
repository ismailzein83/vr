SwitchManagmentController.$inject = ['$scope', '$q', 'SwitchManagmentAPIService', '$location', '$timeout', '$modal', 'VRModalService', 'VRNotificationService', 'UtilsService'];
function SwitchManagmentController($scope, $q, SwitchManagmentAPIService, $location, $timeout, $modal, VRModalService, VRNotificationService, UtilsService) {

    var current = 0;
    var allDatawasRetreived = false;
    var gridApi;


    defineScope();
    //load();

    function load() {
        UtilsService.waitMultipleAsyncOperations([getSwitchsDataSource, loadSwitchs]).finally(function () {
            $scope.isGettingData = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error);
        });

    }

    function loadSwitchs() {

    }

    function defineScope() {
        $scope.gridPagerSettings = {
            currentPage: 1,
            pageSize: 10,
            totalDataCount: 0,
            pageChanged: function () {
                alert(pageChanged);
                $scope.getSwitchsDataSource();
            }
        };

        $scope.switchName = '';
        $scope.rowFrom = 0;
        $scope.rowTo = 20;
        $scope.switchsDataSource = [];

        $scope.gridMenuActions = [{
            name: "Edit/Delete Switch",
            clicked: function (dataItem) {
                alert(dataItem.SwitchId);
                var modalSettings = {
                    //useModalTemplate: true,
                    //width: "80%",
                    //maxHeight: "800px"
                };
                var parameters = {
                    switchId: dataItem.SwitchId,
                    Symbol: dataItem.Symbol,
                    Name: dataItem.Name
                };
                modalSettings.onScopeReady = function (modalScope) {
                    alert('ScopeReady');
                    modalScope.title = "Switch Info(" + dataItem.Symbol + ")";
                };
                modalSettings.onSwitchUpdated = function (switchUpdated) {
                    alert('SwitchUpdated');
                    gridApi.itemUpdated(switchUpdated);

                };
                VRModalService.showModal('/Client/Modules/BusinessEntity/Views/SwitchEditor.html', parameters, modalSettings);
            }
        }
        , {
            name: 'Switch Details',
            clicked: function () {
                $scope.$hide();
                var parameters = {
                    switchId: $scope.switchId
                };
                VRNavigationService.goto("/Client/Modules/BusinessEntity/Views/SwitchDetails.html", parameters);
            }
        }];
    }

    function getObjectsFromService() {//asyncHandle
        SwitchManagmentAPIService.getFilteredSwitches($scope.switchName, $scope.rowFrom, $scope.rowTo).then(function (response) {
            response.length
            angular.forEach(response, function (itm) {
                $scope.switchsDataSource.push(itm);
            });
            if (response.length < $scope.gridPagerSettings.pageSize)
                allDatawasRetreived = true;
        }).finally(function () {
            //if (asyncHandle)
            //    asyncHandle.operationDone();
            $scope.isGettingData = false;
        });
    };

    $scope.openNewSwitch = function () {

        //var scopeDetails = $scope.$root.$new();
        //scopeDetails.title = "New Switch Info";
        //scopeDetails.switchId = 'undefined';
        //var addModal = $modal({ scope: scopeDetails, template: '/Client/Modules/BusinessEntity/Views/SwitchEditor.html', show: true, animation: "am-fade-and-scale" });
        var modalSettings = {
            //useModalTemplate: true,
            //width: "80%",
            //maxHeight: "800px"
        };
        modalSettings.onScopeReady = function (modalScope) {
            alert('Ready');
            modalScope.title = "New Switch Info";
        };
        modalSettings.onSwitchAdded = function (switchAdded) {
            alert('SwitchAdded');
            gridApi.itemAdded(switchAdded);

        };
        VRModalService.showModal('/Client/Modules/BusinessEntity/Views/SwitchEditor.html', null, modalSettings);
    }

    $scope.getSwitchsDataSource = function () {//asyncHandle

        current = 0;
        allDatawasRetreived = false;
        $scope.gridPagerSettings.currentPage = 1;

        $scope.rowFrom = current + 1;
        $scope.rowTo = $scope.rowFrom + $scope.gridPagerSettings.pageSize;
        current = current + $scope.gridPagerSettings.pageSize;
        $scope.gridPagerSettings.currentPage++;

        $scope.switchsDataSource.length = 0;
        $scope.isGettingData = true;

        return getObjectsFromService();//asyncHandle
    }

    $scope.loadDataonScrolling = function () {//asyncHandle
        if (allDatawasRetreived) {
            //if (asyncHandle)
            //    asyncHandle.operationDone();
            return;
        }

        $scope.rowFrom = current + 1;
        $scope.rowTo = $scope.rowFrom + $scope.gridPagerSettings.pageSize;
        current = current + $scope.gridPagerSettings.pageSize;
        $scope.gridPagerSettings.currentPage++;

        $scope.isGettingData = true;
        return getObjectsFromService();//asyncHandle
    }

    $scope.resetSearchForm = function () {
        current = 0;
        allDatawasRetreived = false;
        $scope.gridPagerSettings.currentPage = 1;
        $scope.switchsDataSource.length = 0;
        $scope.switchName = '';
        $scope.gridApi.infiniteScroll.resetScroll(false, true);
    }

    $scope.gridReady = function (api) {
        gridApi = api;
    };


    ///************************************************************* Sorting Function ********************************************/

    //$scope.sortType = 'Name';
    //$scope.sortReverse = false;
    //$scope.sortData = function (name) {
    // $scope.sortType = name;
    // $scope.sortReverse = !$scope.sortReverse;
    // }
    //var sortColumn;
    //var sortDescending = true;
    //function resetSorting() {
    //    sortColumn = 'Name';
    //    sortDescending = true;
    //}
    //$scope.getData = function (asyncHandle) {
    //    $scope.gridPagerSettings.currentPage = 1;
    //    resetSorting();
    //    $scope.getSwitchsDataSource(asyncHandle);
    //};


    //$scope.onGridSortChanged = function (colDef, sortDirection, handle) {
    //    sortColumn = colDef.tag;
    //    sortDescending = (sortDirection == "DESC");
    //    $scope.getData(handle);
    //}
    ///*********************************************************END Sorting Function************************************************/

}
appControllers.controller('SwitchManagmentController', SwitchManagmentController);