OrgChartAssignmentEditorController.$inject = ['$scope', 'AccountManagerAPIService', 'OrgChartAPIService', 'UtilsService', 'VRModalService', 'VRNavigationService', 'VRNotificationService'];

function OrgChartAssignmentEditorController($scope, AccountManagerAPIService, OrgChartAPIService, UtilsService, VRModalService, VRNavigationService, VRNotificationService) {
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

            AccountManagerAPIService.UpdateLinkedOrgChart($scope.assignedOrgChart.Id).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Org Chart", response)) {
                    if ($scope.onOrgChartAssigned != undefined)
                        $scope.onOrgChartAssigned($scope.assignedOrgChart.Id);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        $scope.closeModal = function () {
            $scope.modalContext.closeModal();
        }
    }

    function load() {
        
    }
}

appControllers.controller('BusinessEntity_OrgChartAssignmentEditorController', OrgChartAssignmentEditorController);
