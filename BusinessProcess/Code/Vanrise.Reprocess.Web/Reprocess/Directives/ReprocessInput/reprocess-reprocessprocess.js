"use strict";

app.directive("reprocessReprocessprocess", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'ReprocessChunkTimeEnum', 'Reprocess_ReprocessDefinitionAPIService', 'VRNotificationService',
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
            templateUrl: '/Client/Modules/Reprocess/Directives/ReprocessInput/Templates/ReprocessProcessTemplate.html'
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var reprocessDefinitionSettings;

            var reprocessDefinitionSelectorAPI;
            var reprocessDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var chunkTimeSelectorAPI;
            var chunkTimeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var filterDefinitionDirectiveAPI;
            var filterDefinitionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.showChunkTimeSelector = false;

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
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterDefinitionDirectiveAPI, payload, setLoader, undefined);
                };

                $scope.scopeModel.onReprocessDefinitionSelectionChanged = function (selectedValue) {
                    if (selectedValue != undefined) {
                        $scope.scopeModel.isLoadingDirective = true;

                        Reprocess_ReprocessDefinitionAPIService.GetReprocessDefinition(selectedValue.ReprocessDefinitionId).then(function (response) {
                            reprocessDefinitionSettings = response.Settings;
                            $scope.scopeModel.filterDefinitionEditor = reprocessDefinitionSettings.FilterDefinition != undefined ? reprocessDefinitionSettings.FilterDefinition.RuntimeEditor : undefined;
                            $scope.scopeModel.useTempStorage = false;

                            var chunkTimeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                            chunkTimeSelectorReadyDeferred.promise.then(function () {
                                var chunkTimeSelectorPayload = {
                                    includedChunkTimes: reprocessDefinitionSettings.IncludedChunkTimes,
                                    selectIfSingleItem: true
                                };
                                VRUIUtilsService.callDirectiveLoad(chunkTimeSelectorAPI, chunkTimeSelectorPayload, chunkTimeSelectorLoadDeferred);
                            });

                            chunkTimeSelectorLoadDeferred.promise.then(function () {
                                if (chunkTimeSelectorAPI.hasSingleItem()) {
                                    $scope.scopeModel.showChunkTimeSelector = false;
                                } else {
                                    $scope.scopeModel.showChunkTimeSelector = true;
                                }

                                $scope.scopeModel.isLoadingDirective = false;
                            });
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                            $scope.scopeModel.isLoadingDirective = false;
                        });
                    }
                    else {
                        reprocessDefinitionSettings = undefined;
                        $scope.scopeModel.filterDefinitionEditor = undefined;
                    }
                };

                $scope.scopeModel.validateTimeRange = function () {
                    return VRValidationService.validateTimeRange($scope.scopeModel.fromDate, $scope.scopeModel.toDate);
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var reprocessDefinitionSelectorLoadPromise = getReprocessDefinitionSelectorLoadPromise();
                    promises.push(reprocessDefinitionSelectorLoadPromise);

                    function getReprocessDefinitionSelectorLoadPromise() {
                        var reprocessDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        reprocessDefinitionSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Reprocess.Business.ManualTriggerReprocessDefinitionFilter, Vanrise.Reprocess.Business"
                                    }]
                                }
                            };
                            VRUIUtilsService.callDirectiveLoad(reprocessDefinitionSelectorAPI, payload, reprocessDefinitionSelectorLoadDeferred);
                        });

                        return reprocessDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "Vanrise.Reprocess.BP.Arguments.ReProcessingProcessInput, Vanrise.Reprocess.BP.Arguments",
                            ReprocessDefinitionId: reprocessDefinitionSelectorAPI.getSelectedIds(),
                            FromTime: $scope.scopeModel.fromDate,
                            ToTime: new Date($scope.scopeModel.toDate.setDate($scope.scopeModel.toDate.getDate() + 1)),
                            ChunkTime: chunkTimeSelectorAPI.getSelectedIds(),
                            UseTempStorage: $scope.scopeModel.selectedReprocessDefinition.ForceUseTempStorage ? true : $scope.scopeModel.useTempStorage,
                            Filter: filterDefinitionDirectiveAPI != undefined ? filterDefinitionDirectiveAPI.getData() : undefined
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);