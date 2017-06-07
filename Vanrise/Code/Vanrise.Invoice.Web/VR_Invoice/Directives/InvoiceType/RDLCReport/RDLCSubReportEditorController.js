(function (appControllers) {

    'use strict';

    rdlcsubReportEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function rdlcsubReportEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var subReportEntity;

        var isEditMode;

        var subReportDataSourcesAPI;
        var subReportDataSourcesReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var recordFilterAPI;
        var recordFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var subReportsAPI;
        var subReportsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                subReportEntity = parameters.subReportEntity;
            }
            isEditMode = (subReportEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.datasources = [];
            $scope.scopeModel.selectedDataSource;

            $scope.scopeModel.onRecordFilterReady = function (api) {
                recordFilterAPI = api;
                recordFilterReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onSubreportDirectiveReady = function (api) {
                subReportsAPI = api;
                subReportsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onSubReportDataSourcesReady = function (api) {
                subReportDataSourcesAPI = api;
                subReportDataSourcesReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSubReport() : addSubReport();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builSubReportObjFromScope() {
                var filter = recordFilterAPI.getData();
                return {
                    SubReportName: $scope.scopeModel.subReportName,
                    SubReportDataSources: subReportDataSourcesAPI.getData(),
                    FilterGroup: filter != undefined ? filter.filterObj : undefined,
                    RepeatedSubReport: $scope.scopeModel.repeatedSubReport,
                    ParentDataSourceName: $scope.scopeModel.repeatedSubReport ? $scope.scopeModel.selectedDataSource.DataSourceName : undefined,
                    SubReports: subReportsAPI.getData()
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

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {
                function setTitle() {
                    if (isEditMode && subReportEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(subReportEntity.SubReportName, 'Sub Report');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Sub Report');
                }
                function loadStaticData() {
                    if (context != undefined) {
                        var dataSources = context.getDataSourcesInfo();
                        if (dataSources != undefined)
                            $scope.scopeModel.datasources = dataSources;
                    }
                    if (subReportEntity != undefined) {
                        $scope.scopeModel.subReportName = subReportEntity.SubReportName;
                        $scope.scopeModel.repeatedSubReport = subReportEntity.RepeatedSubReport;

                        if (subReportEntity.ParentDataSourceName != undefined)
                            $scope.scopeModel.selectedDataSource = UtilsService.getItemByVal($scope.scopeModel.datasources, subReportEntity.ParentDataSourceName, "DataSourceName");
                    }
                }
                function loadSubReportDataSourcesDirective() {
                    var subReportDataSourcesLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    subReportDataSourcesReadyPromiseDeferred.promise.then(function () {
                        var subReportPayload = { context: getContext() };
                        if (subReportEntity != undefined)
                            subReportPayload.dataSources = subReportEntity.SubReportDataSources;
                        VRUIUtilsService.callDirectiveLoad(subReportDataSourcesAPI, subReportPayload, subReportDataSourcesLoadPromiseDeferred);
                    });
                    return subReportDataSourcesLoadPromiseDeferred.promise;
                }
                function loadRecordFilterDirective() {
                    var recordFilterLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    recordFilterReadyPromiseDeferred.promise.then(function () {
                        var recordFilterPayload = { context: getContext() };
                        if (subReportEntity != undefined) {
                            recordFilterPayload.FilterGroup = subReportEntity.FilterGroup;
                        }
                        VRUIUtilsService.callDirectiveLoad(recordFilterAPI, recordFilterPayload, recordFilterLoadPromiseDeferred);
                    });
                    return recordFilterLoadPromiseDeferred.promise;
                }
                function loadSubReportsDirective() {
                    var subReportsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    subReportsReadyPromiseDeferred.promise.then(function () {
                        var subReportsPayload = { context: getContext() };
                        if (subReportEntity != undefined) {
                            subReportsPayload.subReports = subReportEntity.SubReports;
                        }
                        VRUIUtilsService.callDirectiveLoad(subReportsAPI, subReportsPayload, subReportsLoadPromiseDeferred);
                    });
                    return subReportsLoadPromiseDeferred.promise;
                }
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSubReportDataSourcesDirective, loadRecordFilterDirective,loadSubReportsDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }

        }

        function getContext()
        {
            var currentContext = UtilsService.cloneObject(context);
          
            if (currentContext == undefined)
                currentContext = {};
            currentContext.showItemsFilter = function () {
                return $scope.scopeModel.repeatedSubReport;
            };
            currentContext.getDataSourcesInfo = function()
            {
                var dataSources = [];
                var reportDataSources = subReportDataSourcesAPI.getData();
                if (reportDataSources != undefined) {
                    for (var i = 0; i < reportDataSources.length; i++) {
                        var reportDataSource = reportDataSources[i];
                        dataSources.push({ DataSourceName: reportDataSource.DataSourceName })
                    }
                }
                return dataSources;
            }
            return currentContext;
        }
       

    }
    appControllers.controller('VR_Invoice_RDLCSubReportEditorController', rdlcsubReportEditorController);

})(appControllers);