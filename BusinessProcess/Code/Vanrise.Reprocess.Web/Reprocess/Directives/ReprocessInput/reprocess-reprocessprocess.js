﻿"use strict";

app.directive("reprocessReprocessprocess", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'ReprocessChunkTimeEnum',
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
            templateUrl: '/Client/Modules/Reprocess/Directives/ReprocessInput/Templates/ReprocessProcessTemplate.html'
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var reprocessDefinitionSelectorAPI;
            var reprocessDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var chunkTimeSelectorAPI;
            var chunkTimeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onReprocessDefinitionSelectorReady = function (api) {
                    reprocessDefinitionSelectorAPI = api;
                    reprocessDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.onChunkTimeSelectorReady = function (api) {
                    chunkTimeSelectorAPI = api;
                    chunkTimeSelectorReadyDeferred.resolve();
                };

                $scope.validateTimeRange = function () {
                    return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var reprocessDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    reprocessDefinitionSelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(reprocessDefinitionSelectorAPI, undefined, reprocessDefinitionSelectorLoadDeferred);
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
                            FromTime: $scope.fromDate,
                            ToTime: new Date($scope.toDate.setDate($scope.toDate.getDate() + 1)),
                            ChunkTime: chunkTimeSelectorAPI.getSelectedIds(),
                            UseTempStorage: $scope.useTempStorage
                        }
                    };
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
