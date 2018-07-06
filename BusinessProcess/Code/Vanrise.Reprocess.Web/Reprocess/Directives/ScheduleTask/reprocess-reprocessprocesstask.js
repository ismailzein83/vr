"use strict";

app.directive("reprocessReprocessprocesstask", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'ReprocessChunkTimeEnum', 'Reprocess_ReprocessDefinitionAPIService', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, VRValidationService, ReprocessChunkTimeEnum, Reprocess_ReprocessDefinitionAPIService, VRNotificationService) {
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
            templateUrl: '/Client/Modules/Reprocess/Directives/ScheduleTask/Templates/ReprocessProcessTaskTemplate.html'
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var reprocessDefinitionSettings;

            var reprocessDefinitionSelectorAPI;
            var reprocessDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var reprocessDefinitionSelectionChangedDeferred;

            var chunkTimeSelectorAPI;
            var chunkTimeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var filterDefinitionDirectiveAPI;
            var filterDefinitionDirectiveReadyDeferred;


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onReprocessDefinitionSelectorReady = function (api) {
                    reprocessDefinitionSelectorAPI = api;
                    reprocessDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onChunkTimeSelectorReady = function (api) {
                    chunkTimeSelectorAPI = api;
                    chunkTimeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onFilterDefinitionDirectiveReady = function (api) {
                    filterDefinitionDirectiveAPI = api;

                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload = {
                        filterDefinition: reprocessDefinitionSettings != undefined ? reprocessDefinitionSettings.FilterDefinition : undefined
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterDefinitionDirectiveAPI, payload, setLoader, filterDefinitionDirectiveReadyDeferred);
                };

                $scope.scopeModel.onReprocessDefinitionSelectionChanged = function (selectedValue) {
                    if (selectedValue != undefined) {
                        $scope.scopeModel.isLoadingDirective = true;

                        Reprocess_ReprocessDefinitionAPIService.GetReprocessDefinition(selectedValue.ReprocessDefinitionId).then(function (response) {
                            reprocessDefinitionSettings = response.Settings;
                            $scope.scopeModel.filterDefinitionEditor = reprocessDefinitionSettings.FilterDefinition != undefined ? reprocessDefinitionSettings.FilterDefinition.RuntimeEditor : undefined;

                            if (reprocessDefinitionSelectionChangedDeferred == undefined) {
                                $scope.scopeModel.useTempStorage = false;

                                var chunkTimeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                                var chunkTimeSelectorPayload = {
                                    includedChunkTimes: reprocessDefinitionSettings.IncludedChunkTimes,
                                    selectIfSingleItem: true
                                };
                                VRUIUtilsService.callDirectiveLoad(chunkTimeSelectorAPI, chunkTimeSelectorPayload, chunkTimeSelectorLoadDeferred);

                                chunkTimeSelectorLoadDeferred.promise.then(function () {
                                    $scope.scopeModel.isLoadingDirective = false;
                                });
                            } else {
                                reprocessDefinitionSelectionChangedDeferred.resolve();
                                $scope.scopeModel.isLoadingDirective = false;
                            }
                        }).catch(function (error) {
                            $scope.scopeModel.isLoadingDirective = false;
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                    else {
                        reprocessDefinitionSettings = undefined;
                        $scope.scopeModel.filterDefinitionEditor = undefined;
                    }
                };

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.daysBack != undefined && $scope.scopeModel.numberOfDays != undefined) {
                        if (parseFloat($scope.scopeModel.numberOfDays) > (parseFloat($scope.scopeModel.daysBack) + 1)) {
                            return 'Reprocess End Date should not exceed Today Date';
                        }
                    }
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    var filter;
                    var chunkTime;

                    if (payload != undefined) {
                        if (payload.rawExpressions != undefined) {
                            $scope.scopeModel.daysBack = payload.rawExpressions.DaysBack;
                            $scope.scopeModel.numberOfDays = payload.rawExpressions.NumberOfDays;
                        }

                        if (payload.data != undefined) {
                            $scope.scopeModel.useTempStorage = payload.data.UseTempStorage;
                            filter = payload.data.Filter;
                            chunkTime = payload.data.ChunkTime;
                        }
                    }

                    var promises = [];

                    var reprocessDefinitionSelectorLoadPromise = getReprocessDefinitionSelectorLoadPromise();
                    promises.push(reprocessDefinitionSelectorLoadPromise);


                    if (filter != undefined) {
                        filterDefinitionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var filterLoadPromise = getFilterLoadPromise();
                        promises.push(filterLoadPromise);
                    }

                    if (chunkTime != undefined) {
                        reprocessDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var chunkTimeSelectorLoadPromise = getChunkTimeSelectorLoadPromise();
                        promises.push(chunkTimeSelectorLoadPromise);
                    }

                    function getReprocessDefinitionSelectorLoadPromise() {
                        var reprocessDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        reprocessDefinitionSelectorReadyDeferred.promise.then(function () {
                            var reprocessPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Reprocess.Business.ManualTriggerReprocessDefinitionFilter, Vanrise.Reprocess.Business"
                                    }]
                                }
                            };

                            if (payload != undefined && payload.data != undefined) {
                                reprocessPayload.selectedIds = payload.data.ReprocessDefinitionId;
                            }

                            VRUIUtilsService.callDirectiveLoad(reprocessDefinitionSelectorAPI, reprocessPayload, reprocessDefinitionSelectorLoadDeferred);
                        });

                        return reprocessDefinitionSelectorLoadDeferred.promise;
                    }
                    function getChunkTimeSelectorLoadPromise() {
                        var chunkTimeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([chunkTimeSelectorReadyDeferred.promise, reprocessDefinitionSelectionChangedDeferred.promise]).then(function () {
                            reprocessDefinitionSelectionChangedDeferred = undefined;

                            var chunkTimeSelectorPayload = {
                                includedChunkTimes: reprocessDefinitionSettings != undefined ? reprocessDefinitionSettings.IncludedChunkTimes : undefined,
                                selectIfSingleItem: true
                            };
                            if (payload != undefined && payload.data != undefined) {
                                chunkTimeSelectorPayload.selectedIds = payload.data.ChunkTime;
                            }
                            VRUIUtilsService.callDirectiveLoad(chunkTimeSelectorAPI, chunkTimeSelectorPayload, chunkTimeSelectorLoadDeferred);
                        });

                        return chunkTimeSelectorLoadDeferred.promise;
                    }
                    function getFilterLoadPromise() {
                        var loadFilterPromiseDeferred = UtilsService.createPromiseDeferred();

                        filterDefinitionDirectiveReadyDeferred.promise.then(function () {
                            filterDefinitionDirectiveReadyDeferred = undefined;

                            var payload = {
                                filterDefinition: reprocessDefinitionSettings != undefined ? reprocessDefinitionSettings.FilterDefinition : undefined,
                                filter: filter
                            };
                            VRUIUtilsService.callDirectiveLoad(filterDefinitionDirectiveAPI, payload, loadFilterPromiseDeferred);
                        });

                        return loadFilterPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getExpressionsData = function () {
                    return { "ScheduleTime": "ScheduleTime", "DaysBack": $scope.scopeModel.daysBack, "NumberOfDays": $scope.scopeModel.numberOfDays };
                };

                api.getData = function () {

                    return {
                        $type: "Vanrise.Reprocess.BP.Arguments.ReProcessingProcessInput, Vanrise.Reprocess.BP.Arguments",
                        ReprocessDefinitionId: reprocessDefinitionSelectorAPI.getSelectedIds(),
                        ChunkTime: chunkTimeSelectorAPI.getSelectedIds(),
                        UseTempStorage: $scope.scopeModel.selectedReprocessDefinition.ForceUseTempStorage ? true : $scope.scopeModel.useTempStorage,
                        Filter: filterDefinitionDirectiveAPI != undefined ? filterDefinitionDirectiveAPI.getData() : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
