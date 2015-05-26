appControllers.controller('SwitchManagmentController',
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



            $scope.GridPagerSettings = {
                currentPage: 1,
                pageSize: 10,
                totalDataCount: 0,
                pageChanged: function () {
                    alert(pageChanged);
                    $scope.GetSwitchsDataSource();
                }
            };


            $scope.switchName = '';
            $scope.rowFrom = 0;
            $scope.rowTo = 20;
            $scope.SwitchsDataSource = [];



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
                        zoneId: $scope.zoneId,
                        zoneName: $scope.zoneName
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
            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/SwitchEditor.html', parameters, modalSettings);
        }


        function resetSorting() {
            sortColumn = 'Name';
            sortDescending = true;
        }

        function resetLoadingData() {
            current = 0;
            allDatawasRetreived = false;
            $scope.GridPagerSettings.currentPage = 1;
        }

        function IncrementScrollrowFrom() {
            current = current + $scope.GridPagerSettings.pageSize;
            $scope.GridPagerSettings.currentPage++;
        }

        function resetSwitchsDataSource() {
            $scope.SwitchsDataSource.length = 0;
        }

        function SetSearchBoundaries() {
            $scope.rowFrom = current + 1;
            $scope.rowTo = $scope.rowFrom + $scope.GridPagerSettings.pageSize;
        }
        function SetActionGettingData(isGetting) {
            $scope.isGettingData = isGetting;
        }


        $scope.getObjectsFromService = function (asyncHandle) {//
            SwitchManagmentAPIService.GetFilteredSwitches($scope.switchName, $scope.rowFrom, $scope.rowTo).then(function (response) {
                response.length
                angular.forEach(response, function (itm) {
                    $scope.SwitchsDataSource.push(itm);
                });
                if (response.length < $scope.GridPagerSettings.pageSize)
                    allDatawasRetreived = true;
            }).finally(function () {
                if (asyncHandle)
                    asyncHandle.operationDone();
                SetActionGettingData(false);
            });
        };


        $scope.GetSwitchsDataSource = function (asyncHandle) {//

            resetLoadingData();
            SetSearchBoundaries();
            IncrementScrollrowFrom();
            resetSwitchsDataSource();

            SetActionGettingData(true);

            $scope.getObjectsFromService(asyncHandle);//
        }

        $scope.loadDataonScrolling = function (asyncHandle) {//
            if (allDatawasRetreived) {
                if (asyncHandle)
                    asyncHandle.operationDone();
                return;
            }

            SetSearchBoundaries();
            IncrementScrollrowFrom();
            SetActionGettingData(true);
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
            $scope.GridPagerSettings.currentPage = 1;
            resetSorting();
            $scope.GetSwitchsDataSource(asyncHandle);
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

    });