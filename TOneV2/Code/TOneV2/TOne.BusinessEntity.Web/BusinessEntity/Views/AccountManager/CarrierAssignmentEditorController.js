CarrierAssignmentEditorController.$inject = ['$scope', 'CarrierAPIService', 'VRModalService', 'VRNavigationService'];

function CarrierAssignmentEditorController($scope, CarrierAPIService, VRModalService, VRNavigationService) {
    var gridApi;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            $scope.accountManager = parameters.accountManager;
        }
    }

    function defineScope() {
        $scope.accountManager = {};
        $scope.carriers = [];

        $scope.onGridReady = function (api) {
            gridApi = api;
            getData();
        }
        $scope.loadMoreData = function () {
            return getData();
        }
        function getData() {
            var pageInfo = gridApi.getPageInfo();

            return CarrierAPIService.GetForAccountManager(pageInfo.fromRow, pageInfo.toRow).then(function (data) {
                angular.forEach(data, function (item) {
                    $scope.carriers.push(item);
                });
            });
        }
        //$scope.onCellClick = function (dataItem) {
        //    console.log(dataItem);
        //    console.log('clicked');
        //}
        //$scope.onCellClickedAgain = function (dataItem) {
        //    console.log(dataItem);
        //    console.log('clicked again');
        //}
        //$scope.onSwitchToggle = function () {
        //    console.log('dirty');
        //    console.log($scope.carriers);
        //}
        $scope.assignCarriers = function () {
            var dirtyCarriers = $scope.carriers.filter(isDirty);

            $scope.modalContext.closeModal();
        }
        $scope.closeModal = function () {
            $scope.modalContext.closeModal();
        }
    }

    function load() {
    }

    function isDirty(item) {
        if (item.IsDirty != undefined && item.IsDirty != null && item.isDirty == true)
            return true;
        return false;
    }
}

appControllers.controller('BusinessEntity_CarrierAssignmentEditorController', CarrierAssignmentEditorController);
