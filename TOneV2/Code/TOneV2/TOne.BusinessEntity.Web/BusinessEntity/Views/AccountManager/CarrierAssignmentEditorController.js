CarrierAssignmentEditorController.$inject = ['$scope', 'OrgChartAPIService', 'VRModalService', 'VRNavigationService'];

function CarrierAssignmentEditorController($scope, OrgChartAPIService, VRModalService, VRNavigationService) {
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

            //return CarriersAPIService.GetRelevantCarriers(pageInfo.fromRow, pageInfo.toRow).then(function (data) {
            //    angular.forEach(data, function (item) {
            //        $scope.carriers.push(item);
            //    });
            //});
        }
        $scope.assignCarriers = function () {
            $scope.modalContext.closeModal();
        }
        $scope.closeModal = function () {
            $scope.modalContext.closeModal();
        }
    }

    function load() {
        OrgChartAPIService.GetOrgCharts().then(function (data) {
            $scope.orgCharts = data;
        });
    }
}

appControllers.controller('BusinessEntity_CarrierAssignmentEditorController', CarrierAssignmentEditorController);
