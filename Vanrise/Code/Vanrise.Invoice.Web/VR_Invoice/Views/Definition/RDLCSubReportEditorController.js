(function (appControllers) {

    'use strict';

    rdlcsubReportEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function rdlcsubReportEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var subReports = [];
        var subReportEntity;

        var isEditMode;

        var subReportDataSourcesAPI;
        var subReportDataSourcesReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                subReports = parameters.subReports;
                subReportEntity = parameters.subReportEntity;
            }
            isEditMode = (subReportEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onSubReportDataSourcesReady = function (api) {
                subReportDataSourcesAPI = api;
                subReportDataSourcesReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSubReport() : addSubReport();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSubReportDataSourcesDirective]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
        }
        function setTitle() {
            if (isEditMode && subReportEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(subReportEntity.SubReportName, 'Sub Report');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Sub Report');
        }
        function loadStaticData() {
            if (subReportEntity != undefined) {
                $scope.scopeModel.subReportName = subReportEntity.SubReportName;
            }
        }
        function loadSubReportDataSourcesDirective() {
            var subReportDataSourcesLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            subReportDataSourcesReadyPromiseDeferred.promise.then(function () {
                var subReportPayload = subReportEntity != undefined ? { dataSources: subReportEntity.SubReportDataSources } : undefined;
                VRUIUtilsService.callDirectiveLoad(subReportDataSourcesAPI, subReportPayload, subReportDataSourcesLoadPromiseDeferred);
            });
            return subReportDataSourcesLoadPromiseDeferred.promise;
        }

        function builSubReportObjFromScope() {
            return {
                SubReportName: $scope.scopeModel.subReportName,
                SubReportDataSources: subReportDataSourcesAPI.getData()
            };
        }

        function addSubReport() {
            var subReportObj = builSubReportObjFromScope();
            if ($scope.onSubReportAdded != undefined) {
                $scope.onSubReportAdded(subReportObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateSubReport() {
            var subReportObj = builSubReportObjFromScope();
            if ($scope.onSubReportUpdated != undefined) {
                $scope.onSubReportUpdated(subReportObj);
            }
            $scope.modalContext.closeModal();
        }

    }
    appControllers.controller('VR_Invoice_RDLCSubReportEditorController', rdlcsubReportEditorController);

})(appControllers);