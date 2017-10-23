﻿"use strict";

app.directive("reprocessReprocessprocesstask", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'ReprocessChunkTimeEnum',
    function (UtilsService, VRUIUtilsService, VRValidationService, ReprocessChunkTimeEnum) {
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

            var chunkTimeSelectorAPI;
            var chunkTimeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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
                        if (parseFloat($scope.scopeModel.numberOfDays) > parseFloat($scope.scopeModel.daysBack)) {
                            return 'Number Of Days should be less than or equal to Days Back.';
                        }
                    }
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        if (payload.rawExpressions != undefined) {
                            $scope.scopeModel.daysBack = payload.rawExpressions.DaysBack;
                            $scope.scopeModel.numberOfDays = payload.rawExpressions.NumberOfDays;
                        }

                        if (payload.data != undefined) {
                            $scope.scopeModel.useTempStorage = payload.data.UseTempStorage;
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
                        UseTempStorage: $scope.scopeModel.selectedReprocessDefinition.ForceUseTempStorage ? true : $scope.scopeModel.useTempStorage
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
