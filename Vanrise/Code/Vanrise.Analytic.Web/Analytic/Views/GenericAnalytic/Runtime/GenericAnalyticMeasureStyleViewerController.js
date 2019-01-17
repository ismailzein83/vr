(function (appControllers) {
    'use strict';

    genericAnalyticMeasureStyleViewerController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function genericAnalyticMeasureStyleViewerController($scope, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService) {
        var analyticTableId;
        var measureStyleViewerAPI;
        var measureStyleViewerReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var highlightedId;
        var statusBeDefinitionId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                analyticTableId = parameters.analyticTableId;
                highlightedId = parameters.highlightedId;
                statusBeDefinitionId = parameters.statusBeDefinitionId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onMeasureStyleViewerReady = function (api) {
                measureStyleViewerAPI = api;
                measureStyleViewerReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
                loadAllControls();
           
            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadMeasureStyleViewer]).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }

        }

        function loadMeasureStyleViewer() {
            var loadMeasureStyleViewerPromiseDeferred = UtilsService.createPromiseDeferred();
            measureStyleViewerReadyPromiseDeferred.promise.then(function () {
                var payLoad = {
                    analyticTableId: analyticTableId,
                    statusBeDefinitionId: statusBeDefinitionId,
                    highlightedId: highlightedId
                };
                VRUIUtilsService.callDirectiveLoad(measureStyleViewerAPI, payLoad, loadMeasureStyleViewerPromiseDeferred);
            });
            return loadMeasureStyleViewerPromiseDeferred.promise;
        }
    }

    appControllers.controller('VR_Analytic_GenericAnalyticMeasureStyleViewerController', genericAnalyticMeasureStyleViewerController);

})(appControllers);
