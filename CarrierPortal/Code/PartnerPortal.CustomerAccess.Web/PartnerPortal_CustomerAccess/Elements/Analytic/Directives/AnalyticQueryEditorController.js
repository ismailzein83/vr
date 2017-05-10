(function (appControllers) {

    "use strict";

    AnalyticQueryEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function AnalyticQueryEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var analyticQueryEntity;

        var timePeriodDirectiveApi;
        var timePeriodReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var connectionDirectiveApi;
        var connectionReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var selectedConnectionPromiseDeferred;

        var analyticTableDirectiveApi;
        var analyticTableReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var selectedAnalyticTablePromiseDeferred;

        var analyticMeasureDirectiveApi;
        var analyticMeasureReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                analyticQueryEntity = parameters.analyticQueryEntity;
            }
            isEditMode = (analyticQueryEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.selectedMeasures = [];

            $scope.scopeModel.gridMeasures = [];

            $scope.scopeModel.isValid = function () {
                if ($scope.scopeModel.gridMeasures == undefined || $scope.scopeModel.gridMeasures.length == 0)
                    return "You Should Select at least one Measure ";
                return null;
            };

            $scope.scopeModel.onSelectMeasure = function (selectedMeasure) {
                var dataItem = {
                    Entity: {
                        MeasureItemId: UtilsService.guid(),
                        MeasureName: selectedMeasure.Name,
                        MeasureTitle: selectedMeasure.Title
                    }
                };
                $scope.scopeModel.gridMeasures.push(dataItem);
            };

            $scope.scopeModel.onDeselectMeasure = function (dataItem) {
                var datasourceIndex = $scope.scopeModel.gridMeasures.indexOf(dataItem);
                $scope.scopeModel.gridMeasures.splice(datasourceIndex, 1);
            };

            $scope.scopeModel.removeMeasure = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedMeasures, dataItem.Entity.MeasureName, 'MeasureName');
                $scope.scopeModel.selectedMeasures.splice(index, 1);
                var datasourceIndex = $scope.scopeModel.gridMeasures.indexOf(dataItem);
                $scope.scopeModel.gridMeasures.splice(datasourceIndex, 1);
            };

            $scope.scopeModel.onTimePeriodDirectiveReady = function (api) {
                timePeriodDirectiveApi = api;
                timePeriodReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onConnectionSelectorReady = function (api) {
                connectionDirectiveApi = api;
                connectionReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onAnalyticTableSelectorReady = function (api) {
                analyticTableDirectiveApi = api;
                analyticTableReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onAnalyticMeasureSelectorReady = function (api) {
                analyticMeasureDirectiveApi = api;
                analyticMeasureReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onConenctionSelectionChanged = function (value) {
                if (value != undefined) {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directiveTablePayload = {
                        connectionId: connectionDirectiveApi.getSelectedIds()
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, analyticTableDirectiveApi, directiveTablePayload, setLoader, selectedConnectionPromiseDeferred);
                }
            };

            $scope.scopeModel.onAnalyticTableSelectionChanged = function (tableValue) {
                if (tableValue != undefined)
                {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivePayload = {
                        connectionId: connectionDirectiveApi.getSelectedIds(),
                        filter: { TableIds: [analyticTableDirectiveApi.getSelectedIds()] }
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, analyticMeasureDirectiveApi, directivePayload, setLoader, selectedAnalyticTablePromiseDeferred);
                }
            };

            $scope.scopeModel.saveAnalyticQuery = function () {
                if (isEditMode)
                    return updateAnalyticQuery();
                else
                    return insertAnalyticQuery();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.isLoading = true;
            loadAllControls()
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && analyticQueryEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(analyticQueryEntity.Name, "Analytic Query");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Analytic Query");
            }

            function loadStaticData() {
                if (analyticQueryEntity == undefined)
                    return;
                $scope.scopeModel.queryName = analyticQueryEntity.Name;
                $scope.scopeModel.userDimensionName = analyticQueryEntity.UserDimensionName;
       
                selectedConnectionPromiseDeferred = UtilsService.createPromiseDeferred();
                selectedAnalyticTablePromiseDeferred =  UtilsService.createPromiseDeferred();
            }

            function loadTimePeriodDirective() {
                var timePeriodLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                timePeriodReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload = {
                            timePeriod: analyticQueryEntity != undefined ? analyticQueryEntity.TimePeriod : undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(timePeriodDirectiveApi, directivePayload, timePeriodLoadPromiseDeferred);
                    });
                return timePeriodLoadPromiseDeferred.promise;
            }

            function loadConnectionDirective() {
                var connectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                connectionReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload = {
                            selectedIds: analyticQueryEntity != undefined ? analyticQueryEntity.VRConnectionId : undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(connectionDirectiveApi, directivePayload, connectionLoadPromiseDeferred);
                    });
                return connectionLoadPromiseDeferred.promise;
            }

            function loadAnalyticTableDirective() {
                if (analyticQueryEntity != undefined)
                {
                    var analyticTableReadyLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises([selectedConnectionPromiseDeferred.promise, analyticTableReadyPromiseDeferred.promise])
                        .then(function () {
                            selectedConnectionPromiseDeferred = undefined;
                            var directivePayload = {
                                selectedIds: analyticQueryEntity.TableId,
                                connectionId: analyticQueryEntity.VRConnectionId
                            };
                            VRUIUtilsService.callDirectiveLoad(analyticTableDirectiveApi, directivePayload, analyticTableReadyLoadPromiseDeferred);
                        });
                    return analyticTableReadyLoadPromiseDeferred.promise;
                }
            }

            function loadAnalyticMeasureDirective() {
                if (analyticQueryEntity != undefined) {
                    var analyticMeasureReadyLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises([selectedAnalyticTablePromiseDeferred.promise, analyticMeasureReadyPromiseDeferred.promise])
                        .then(function () {
                            selectedAnalyticTablePromiseDeferred = undefined;

                            var measureNames = [];
                            if (analyticQueryEntity.Measures != undefined) {
                                for (var i = 0, length = analyticQueryEntity.Measures.length ; i < length; i++) {
                                    var measure = analyticQueryEntity.Measures[i];
                                    measureNames.push(measure.MeasureName);
                                }
                            }

                            var directivePayload = {
                                selectedIds: measureNames,
                                connectionId: connectionDirectiveApi.getSelectedIds(),
                                filter: { TableIds: [analyticQueryEntity.TableId] }
                            };
                            VRUIUtilsService.callDirectiveLoad(analyticMeasureDirectiveApi, directivePayload, analyticMeasureReadyLoadPromiseDeferred);
                        });
                    return analyticMeasureReadyLoadPromiseDeferred.promise;
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTimePeriodDirective, loadConnectionDirective, loadAnalyticTableDirective, loadAnalyticMeasureDirective]).then(function () {
                if(analyticQueryEntity != undefined && analyticQueryEntity.Measures != undefined)
                {
                    for (var i = 0, length = analyticQueryEntity.Measures.length ; i < length; i++) {
                        var measure = analyticQueryEntity.Measures[i];
                        $scope.scopeModel.gridMeasures.push({ Entity: measure });
                    }
                }
            }).catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function buildAnalyticQueryObjFromScope() {

            var measures = [];
            for (var i = 0, length = $scope.scopeModel.gridMeasures.length ; i < length; i++) {
                var gridMeasure = $scope.scopeModel.gridMeasures[i];
                measures.push(gridMeasure.Entity);
            }
            var obj = {
                Name: $scope.scopeModel.queryName,
                UserDimensionName: $scope.scopeModel.userDimensionName,
                VRConnectionId: connectionDirectiveApi.getSelectedIds(),
                TimePeriod: timePeriodDirectiveApi.getData(),
                TableId: analyticTableDirectiveApi.getSelectedIds(),
                Measures: measures
            };
            return obj;
        }

        function insertAnalyticQuery() {
            var vrAnalyticQueryObject = buildAnalyticQueryObjFromScope();
            if ($scope.onAnalyticQueryAdded != undefined)
                $scope.onAnalyticQueryAdded(vrAnalyticQueryObject);
            $scope.modalContext.closeModal();
        }

        function updateAnalyticQuery() {
            var vrAnalyticQueryObject = buildAnalyticQueryObjFromScope();
            if ($scope.onAnalyticQueryUpdated != undefined)
                $scope.onAnalyticQueryUpdated(vrAnalyticQueryObject);
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('PartnerPortal_CustomerAccess_AnalyticQueryEditorController', AnalyticQueryEditorController);
})(appControllers);
