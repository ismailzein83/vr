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

            var reprocessDefinitionSelectorAPI;
            var reprocessDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            //var onReprocessDefinitionSelectionChangedDeferred;

            var chunkTimeSelectorAPI;
            var chunkTimeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var filterDefinitionDirectiveAPI;
            var filterDefinitionDirectiveReadyDeferred;

            var filterDefinition;

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

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.daysBack != undefined && $scope.scopeModel.numberOfDays != undefined) {
                        if (parseFloat($scope.scopeModel.numberOfDays) > (parseFloat($scope.scopeModel.daysBack) + 1)) {
                            return 'Reprocess End Date should not exceed Today Date';
                        }
                    }
                    return null;
                };

                $scope.scopeModel.onReprocessDefinitionSelectionChanged = function (selectedValue) {
                    if (selectedValue != undefined) {
                        $scope.scopeModel.isLoadingDirective = true;

                        Reprocess_ReprocessDefinitionAPIService.GetReprocessDefinition(selectedValue.ReprocessDefinitionId).then(function (response) {
                            filterDefinition = response.Settings.FilterDefinition;
                            $scope.scopeModel.filterDefinitionEditor = filterDefinition != undefined ? filterDefinition.RuntimeEditor : undefined;
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            $scope.scopeModel.isLoadingDirective = false;
                        });
                    }
                    else {
                        filterDefinition = undefined;
                        $scope.scopeModel.filterDefinitionEditor = undefined;
                    }
                };

                $scope.scopeModel.onFilterDefinitionDirectiveReady = function (api) {
                    filterDefinitionDirectiveAPI = api;

                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload = { filterDefinition: filterDefinition };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterDefinitionDirectiveAPI, payload, setLoader, filterDefinitionDirectiveReadyDeferred);
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var filter;

                    if (payload != undefined) {
                        if (payload.rawExpressions != undefined) {
                            $scope.scopeModel.daysBack = payload.rawExpressions.DaysBack;
                            $scope.scopeModel.numberOfDays = payload.rawExpressions.NumberOfDays;
                        }

                        if (payload.data != undefined) {
                            $scope.scopeModel.useTempStorage = payload.data.UseTempStorage;
                            filter = payload.data.Filter;
                        }
                    }

                    var promises = [];

                    var reprocessDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    reprocessDefinitionSelectorReadyDeferred.promise.then(function () {
                        var reprocessPayload;
                        if (payload != undefined && payload.data != undefined) {
                            reprocessPayload = {
                                selectedIds: payload.data.ReprocessDefinitionId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(reprocessDefinitionSelectorAPI, reprocessPayload, reprocessDefinitionSelectorLoadDeferred);
                    });
                    promises.push(reprocessDefinitionSelectorLoadDeferred.promise);

                    var chunkTimeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    chunkTimeSelectorReadyDeferred.promise.then(function () {
                        var chunkTimeSelectorPayload;
                        if (payload != undefined && payload.data != undefined) {
                            chunkTimeSelectorPayload = {
                                selectedIds: payload.data.ChunkTime
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(chunkTimeSelectorAPI, chunkTimeSelectorPayload, chunkTimeSelectorLoadDeferred);
                    });
                    promises.push(chunkTimeSelectorLoadDeferred.promise);


                    if (filter != undefined) {
                        filterDefinitionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var filterLoadPromise = getFilterLoadPromise();
                        promises.push(filterLoadPromise);
                    }


                    function getFilterLoadPromise() {
                        var loadFilterPromiseDeferred = UtilsService.createPromiseDeferred();

                        filterDefinitionDirectiveReadyDeferred.promise.then(function () {
                            filterDefinitionDirectiveReadyDeferred = undefined;

                            var payload = { filterDefinition: filterDefinition, filter: filter };
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
