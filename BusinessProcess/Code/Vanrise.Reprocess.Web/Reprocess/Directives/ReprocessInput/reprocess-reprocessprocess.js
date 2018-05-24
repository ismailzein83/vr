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

            var reprocessDefinitionSelectorAPI;
            var reprocessDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var chunkTimeSelectorAPI;
            var chunkTimeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var filterDefinitionDirectiveAPI;
            var filterDefinitionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

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

                $scope.scopeModel.validateTimeRange = function () {
                    return VRValidationService.validateTimeRange($scope.scopeModel.fromDate, $scope.scopeModel.toDate);
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
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterDefinitionDirectiveAPI, payload, setLoader, undefined);
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

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
                    promises.push(reprocessDefinitionSelectorLoadDeferred.promise);

                    var chunkTimeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    chunkTimeSelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(chunkTimeSelectorAPI, undefined, chunkTimeSelectorLoadDeferred);
                    });
                    promises.push(chunkTimeSelectorLoadDeferred.promise);

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
