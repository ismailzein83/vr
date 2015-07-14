OrgChartAssignmentEditorController.$inject = ['$scope', 'OrgChartAPIService', 'VRModalService', 'VRNavigationService'];

function OrgChartAssignmentEditorController($scope, OrgChartAPIService, VRModalService, VRNavigationService) {
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        
        if (parameters != undefined && parameters != null) {
            $scope.assignedOrgChart = parameters.assignedOrgChart;
        }
    }

    function defineScope() {
        $scope.orgChartName = '';
        $scope.orgCharts = [];

        $scope.assignOrgChart = function () {
            $scope.onOrgChartAssigned($scope.assignedOrgChart);
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

appControllers.controller('BusinessEntity_OrgChartAssignmentEditorController', OrgChartAssignmentEditorController);
