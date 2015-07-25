﻿OrgChartAssignmentEditorController.$inject = ['$scope', 'AccountManagerAPIService', 'OrgChartAPIService', 'UtilsService', 'VRModalService', 'VRNavigationService', 'VRNotificationService'];

function OrgChartAssignmentEditorController($scope, AccountManagerAPIService, OrgChartAPIService, UtilsService, VRModalService, VRNavigationService, VRNotificationService) {

    var passedOrgChartId = undefined;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null)
            passedOrgChartId = parameters.assignedOrgChartId;
    }

    function defineScope() {
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
        $scope.isGettingData = true;

        getOrgCharts().finally(function () {
            $scope.isGettingData = false;
            $scope.assignedOrgChart = UtilsService.getItemByVal($scope.orgCharts, passedOrgChartId, 'Id');
        });
    }

    function getOrgCharts() {
        return OrgChartAPIService.GetOrgCharts()
            .then(function (response) {
                $scope.orgCharts = response;
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }
}

appControllers.controller('BusinessEntity_OrgChartAssignmentEditorController', OrgChartAssignmentEditorController);
