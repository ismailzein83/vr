OrgChartAssignmentEditorController.$inject = ['$scope', 'OrgChartAPIService', 'UtilsService', 'VRModalService', 'VRNavigationService'];

function OrgChartAssignmentEditorController($scope, OrgChartAPIService, UtilsService, VRModalService, VRNavigationService) {
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        // make sure that $scope.orgCharts is set before setting $scope.assignedOrgChart
        OrgChartAPIService.GetOrgCharts().then(function (response) {
            $scope.orgCharts = response;

            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                // getItemByVal assumes that $scope.orgCharts is set
                $scope.assignedOrgChart = UtilsService.getItemByVal($scope.orgCharts, parameters.assignedOrgChartId, 'Id');
            }
        });
    }

    function defineScope() {
        $scope.orgChartName = '';
        $scope.orgCharts = [];

        $scope.assignOrgChart = function () {
            $scope.onOrgChartAssigned($scope.assignedOrgChart.Id);
            $scope.modalContext.closeModal();
        }
        $scope.closeModal = function () {
            $scope.modalContext.closeModal();
        }
    }

    function load() {
        
    }
}

appControllers.controller('BusinessEntity_OrgChartAssignmentEditorController', OrgChartAssignmentEditorController);
