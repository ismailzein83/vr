SwitchManagmentController.$inject = ['$scope', '$q', 'SwitchManagmentAPIService', '$location', '$timeout', '$modal', 'VRModalService', 'VRNotificationService'];


function SwitchManagmentController($scope, $q, SwitchManagmentAPIService, $location, $timeout, $modal, VRModalService, VRNotificationService) {


    var sortColumn;
    var sortDescending = true;
    var current = 0;
    var allDatawasRetreived = false;
    var gridApi;


    defineScopeObjects();

    function defineScopeObjects() {

        $scope.sortType = 'Name';
        $scope.sortReverse = false;
        $scope.sortData = function (name) {
            $scope.sortType = name;
            $scope.sortReverse = !$scope.sortReverse;
        }
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

                };
                var parameters = {
                    switchId: dataItem.SwitchId,
                    Symbol: dataItem.Symbol,
                    Name: dataItem.Name
                };
                modalSettings.onScopeReady = function (modalScope) {
                    alert('Ready');
                    modalScope.title = "Switch Info(" + dataItem.Symbol + ")";
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



    $scope.AddNewSwitch = function () {

        //var scopeDetails = $scope.$root.$new();
        //scopeDetails.title = "New Switch Info";
        //scopeDetails.switchId = 'undefined';
        //var addModal = $modal({ scope: scopeDetails, template: '/Client/Modules/BusinessEntity/Views/SwitchEditor.html', show: true, animation: "am-fade-and-scale" });


        var modalSettings = {
            //useModalTemplate: true,
            //width: "80%",
            //maxHeight: "800px"
        };
        var parameters = {
            switchId: 'undefined',
            Symbol: '',
            Name: ''
        };
        modalSettings.onScopeReady = function (modalScope) {
            alert('Ready');
            modalScope.title = "New Switch Info";
        };
        VRModalService.showModal('/Client/Modules/BusinessEntity/Views/SwitchEditor.html', null, modalSettings);
    }


    function resetSorting() {
        sortColumn = 'Name';
        sortDescending = true;
    }

    function resetLoadingData() {
        current = 0;
        allDatawasRetreived = false;
        $scope.gridPagerSettings.currentPage = 1;
    }

    function incrementScrollrowFrom() {
        current = current + $scope.gridPagerSettings.pageSize;
        $scope.gridPagerSettings.currentPage++;
    }

    function resetSwitchsDataSource() {
        $scope.switchsDataSource.length = 0;
    }

    function setSearchBoundaries() {
        $scope.rowFrom = current + 1;
        $scope.rowTo = $scope.rowFrom + $scope.gridPagerSettings.pageSize;
    }
    function setActionGettingData(isGetting) {
        $scope.isGettingData = isGetting;
    }


    $scope.getObjectsFromService = function (asyncHandle) {//
        SwitchManagmentAPIService.getFilteredSwitches($scope.switchName, $scope.rowFrom, $scope.rowTo).then(function (response) {
            response.length
            angular.forEach(response, function (itm) {
                $scope.switchsDataSource.push(itm);
            });
            if (response.length < $scope.gridPagerSettings.pageSize)
                allDatawasRetreived = true;
        }).finally(function () {
            if (asyncHandle)
                asyncHandle.operationDone();
            setActionGettingData(false);
        });
    };


    $scope.getSwitchsDataSource = function (asyncHandle) {//

        resetLoadingData();
        setSearchBoundaries();
        incrementScrollrowFrom();
        resetSwitchsDataSource();

        setActionGettingData(true);

        $scope.getObjectsFromService(asyncHandle);//
    }

    $scope.loadDataonScrolling = function (asyncHandle) {//
        if (allDatawasRetreived) {
            if (asyncHandle)
                asyncHandle.operationDone();
            return;
        }

        setSearchBoundaries();
        incrementScrollrowFrom();
        setActionGettingData(true);
        return $scope.getObjectsFromService();//
    }


    $scope.resetSearchForm = function () {
        resetLoadingData();
        resetSorting();
        resetSwitchsDataSource();
        $scope.switchName = '';
        $scope.gridApi.infiniteScroll.resetScroll(false, true);
    }






    /************************************************************* Sorting Function ********************************************/
    $scope.getData = function (asyncHandle) {
        $scope.gridPagerSettings.currentPage = 1;
        resetSorting();
        $scope.getSwitchsDataSource(asyncHandle);
    };


    $scope.onGridSortChanged = function (colDef, sortDirection, handle) {
        sortColumn = colDef.tag;
        sortDescending = (sortDirection == "DESC");
        $scope.getData(handle);
    }


    $scope.gridReady = function (api) {
        gridApi = api;
    };
    /*********************************************************END Sorting Function************************************************/

}

appControllers.controller('SwitchManagmentController', SwitchManagmentController);