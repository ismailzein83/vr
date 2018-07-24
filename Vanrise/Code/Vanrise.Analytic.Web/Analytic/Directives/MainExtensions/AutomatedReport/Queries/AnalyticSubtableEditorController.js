﻿(function (appControllers) {
    "use strict";
    analyticSubtableEditor.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_VRComponentTypeAPIService', 'VR_Analytic_AdvancedExcelFileGeneratorAPIService'];
    function analyticSubtableEditor($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VRCommon_VRComponentTypeAPIService, VR_Analytic_AdvancedExcelFileGeneratorAPIService) {


        var isEditMode;
        var subtableEntity;
        var analyticTableId;
        var context;

        var dimensionsSelectorAPI;
        var dimensionsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var measuresSelectorAPI;
        var measuresSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {

            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                subtableEntity = parameters.subtableEntity;
                analyticTableId = parameters.analyticTableId;
                context = parameters.context;
            }
            isEditMode = (subtableEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};


            $scope.scopeModel.onDimensionsSelectorReady = function (api) {
                dimensionsSelectorAPI = api;
                dimensionsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onMeasureSelectorReady = function (api) {
                measuresSelectorAPI = api;
                measuresSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.saveSubtable = function () {
                if (isEditMode)
                    return updateSubtable();
                else
                    return insertSubtable();
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

            function setTitle() {
                if (isEditMode && subtableEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(subtableEntity.Title, "Subtable");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Subtable");
            }

            function loadStaticData() {

                if (subtableEntity == undefined)
                    return;
            }
            
            function loadDimensionsSelector() {
                var dimensionsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dimensionsSelectorReadyDeferred.promise.then(function () {
                    var dimensionsSelectorPayload = {
                        filter:
                            {
                                TableIds: [analyticTableId]
                            }
                        ,
                        selectedIds: subtableEntity != undefined ? subtableEntity.Dimensions : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(dimensionsSelectorAPI, dimensionsSelectorPayload, dimensionsSelectorLoadDeferred);
                });
                return dimensionsSelectorLoadDeferred.promise;
            }

            function loadMeasuresSelector() {
                var measuresSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                measuresSelectorReadyDeferred.promise.then(function () {
                    var measuresSelectorPayload = {
                        filter:
                            {
                                TableIds: [analyticTableId]
                            }
                        ,
                        selectedIds: subtableEntity != undefined ? subtableEntity.Measures : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(measuresSelectorAPI, measuresSelectorPayload, measuresSelectorLoadDeferred);
                });
                return measuresSelectorLoadDeferred.promise;
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDimensionsSelector, loadMeasuresSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function buildObjFromScope() {
            var obj = {
              Title: getTitle(),
              Dimensions: dimensionsSelectorAPI.getSelectedIds(),
              Measures: measuresSelectorAPI.getSelectedIds()
            };
            return obj;
        }

        function insertSubtable() {
            var object = buildObjFromScope();
            object.SubTableId = UtilsService.guid();
            if ($scope.onAnalyticSubtableAdded != undefined && typeof ($scope.onAnalyticSubtableAdded) == 'function')
                $scope.onAnalyticSubtableAdded(object);
            $scope.modalContext.closeModal();
        }

        function updateSubtable() {
            var object = buildObjFromScope();
            object.SubTableId = subtableEntity != undefined ? subtableEntity.SubTableId : undefined;
            if ($scope.onAnalyticSubtableUpdated != undefined && typeof ($scope.onAnalyticSubtableUpdated) == 'function')
                $scope.onAnalyticSubtableUpdated(object);
            $scope.modalContext.closeModal();
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }

        function getTitle() {
            if (dimensionsSelectorAPI.getSelectedIds() != undefined) {
                var title = [];
                var dimensions = dimensionsSelectorAPI.getSelectedIds();
                for (var i = 0; i < dimensions.length; i++) {
                    title.push(dimensions[i]);
                }
                title.join(", ");
                return title.toString();
            }
        }
    }
    appControllers.controller('VR_Analytic_AnalyticSubtableEditorController', analyticSubtableEditor);
})(appControllers);