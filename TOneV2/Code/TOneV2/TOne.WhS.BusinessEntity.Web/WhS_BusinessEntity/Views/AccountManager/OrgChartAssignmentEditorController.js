(function (appControllers) {

    'use strict';

    OrgChartAssignmentEditorController.$inject = ['$scope', 'WhS_BE_AccountManagerAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function OrgChartAssignmentEditorController($scope, WhS_BE_AccountManagerAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var passedOrgChartId;

        var orgChartSelectorAPI;
        var orgChartSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                passedOrgChartId = parameters.assignedOrgChartId;
            }
        }

        function defineScope() {

            $scope.hasUpdateLinkedOrgChartPermission = function () {
                return WhS_BE_AccountManagerAPIService.HasUpdateLinkedOrgChartPermission();
            };

            $scope.onOrgChartSelectorReady = function (api) {
                orgChartSelectorAPI = api;
                orgChartSelectorReadyDeferred.resolve();
            };

            $scope.assignOrgChart = function () {
                var selectedOrgChartId = orgChartSelectorAPI.getSelectedIds();
                
                return WhS_BE_AccountManagerAPIService.UpdateLinkedOrgChart(selectedOrgChartId).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated('Org Chart', response)) {
                        if ($scope.onOrgChartAssigned && typeof $scope.onOrgChartAssigned == 'function') {
                            $scope.onOrgChartAssigned(selectedOrgChartId);
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadOrgChartSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = 'Assign Org Chart';
            }

            function loadOrgChartSelector() {
                var loadOrgChartSelectorDeferred = UtilsService.createPromiseDeferred();

                orgChartSelectorReadyDeferred.promise.then(function () {
                    var orgChartSelectorPayload = {
                        selectedIds: passedOrgChartId
                    };
                    VRUIUtilsService.callDirectiveLoad(orgChartSelectorAPI, orgChartSelectorPayload, loadOrgChartSelectorDeferred);
                });

                return loadOrgChartSelectorDeferred.promise;
            }
        }
    }

    appControllers.controller('WhS_BE_OrgChartAssignmentEditorController', OrgChartAssignmentEditorController);

})(appControllers);
