"use strict";

app.directive("vrAnalyticDaprofcalcProcessinput", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'DAProfCalcChunkTimeEnum',
    function (UtilsService, VRUIUtilsService, VRValidationService, DAProfCalcChunkTimeEnum) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/ProcessInput/Templates/DAProfCalcProcessInputTemplate.html'
        };

        function DirectiveConstructor($scope, ctrl) {

            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }

            $scope.scopeModel = {};
            var dataAnalysisDefinitionSelectorAPI;
            var dataAnalysisDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var sourceDataRecordStorageSelectorAPI;
            var sourceDataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            //var destinationDataRecordStorageSelectorAPI;
            //var destinationDataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            $scope.onDataAnalysisDefinitionSelectorReady = function (api) {
                dataAnalysisDefinitionSelectorAPI = api;
                dataAnalysisDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.onSourceDataRecordStorageSelectorReady = function (api) {
                sourceDataRecordStorageSelectorAPI = api;
                sourceDataRecordStorageSelectorReadyDeferred.resolve();
            };

            //$scope.onDestinationDataRecordStorageSelectorReady = function (api) {
            //    destinationDataRecordStorageSelectorAPI = api;
            //    destinationDataRecordStorageSelectorReadyDeferred.resolve();
            //};


            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            };

            $scope.onDataAnalysisDefinitionSelectionChanged = function (dataItem) {
                if (dataItem != undefined) {
                    var filters = [];
                    var daProfCalcDataRecordStorageFilter = {
                        $type: 'Vanrise.Analytic.MainExtensions.DataAnalysis.DAProfCalcDataRecordStorageFilter,Vanrise.Analytic.MainExtensions',
                        DataAnalysisDefinitionId: dataItem.DataAnalysisDefinitionId
                    };
                    filters.push(daProfCalcDataRecordStorageFilter);

                    var setSourceLoader = function (value) { $scope.scopeModel.isLoadingSourceDataRecordStorage = value };
                    var payload = {
                        filters: filters
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceDataRecordStorageSelectorAPI, payload, setSourceLoader, sourceDataRecordStorageSelectorReadyDeferred);

                    //var setDestinationLoader = function (value) { $scope.scopeModel.isLoadingDestinationDataRecordStorage = value };
                    //var payload = {
                    //    filters: filters
                    //};
                    //VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, destinationDataRecordStorageSelectorAPI, payload, setDestinationLoader,destinationDataRecordStorageSelectorReadyDeferred);
                }
            };

            function defineAPI() {

                var api = {};
                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "Vanrise.Analytic.BP.Arguments.DAProfCalcProcessInput, Vanrise.Analytic.BP.Arguments",
                            DAProfCalcDefinitionId: dataAnalysisDefinitionSelectorAPI.getSelectedIds(),
                            InRecordStorageIds: sourceDataRecordStorageSelectorAPI.getSelectedIds(),
                            FromTime: $scope.fromDate,
                            ToTime: new Date($scope.toDate.setDate($scope.toDate.getDate() + 1)),
                            ChunkTime: $scope.selectedChunkTime.value
                        }
                    };
                };

                api.load = function (payload) {
                    $scope.chunkTimes = UtilsService.getArrayEnum(DAProfCalcChunkTimeEnum);

                    var promises = [];

                    var dataAnalysisDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    dataAnalysisDefinitionSelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataAnalysisDefinitionSelectorAPI, undefined, dataAnalysisDefinitionSelectorLoadDeferred);
                    });

                    promises.push(dataAnalysisDefinitionSelectorLoadDeferred.promise);
                    promises.push(sourceDataRecordStorageSelectorReadyDeferred.promise);
                    //promises.push(destinationDataRecordStorageSelectorReadyDeferred.promise);

                    sourceDataRecordStorageSelectorReadyDeferred.promise.then(function () {
                        sourceDataRecordStorageSelectorReadyDeferred = undefined;
                    });
                    //destinationDataRecordStorageSelectorReadyDeferred.promise.then(function () {
                    //    destinationDataRecordStorageSelectorReadyDeferred = undefined;
                    //});

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);