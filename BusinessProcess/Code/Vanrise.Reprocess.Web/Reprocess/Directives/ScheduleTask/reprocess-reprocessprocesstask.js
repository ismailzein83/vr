"use strict";

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

            function initializeController() {
                defineAPI();
            }
            var reprocessDefinitionSelectorAPI;
            var reprocessDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            $scope.onReprocessDefinitionSelectorReady = function (api) {
                reprocessDefinitionSelectorAPI = api;
                reprocessDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.isValid = function () {
                if ($scope.daysBack != undefined && $scope.numberOfDays != undefined) {
                    if (parseFloat($scope.numberOfDays) > parseFloat($scope.daysBack)) {
                        return 'Number Of Days should be less than or equal to Days Back.';
                    }

                }
                return null;
            };

            function defineAPI() {

                var api = {};
                api.getData = function () {
                    return {
                        $type: "Vanrise.Reprocess.BP.Arguments.ReProcessingProcessInput, Vanrise.Reprocess.BP.Arguments",
                        ReprocessDefinitionId: reprocessDefinitionSelectorAPI.getSelectedIds(),
                        ChunkTime: $scope.selectedChunkTime.value
                    };
                };

                api.getExpressionsData = function () {
                    return { "ScheduleTime": "ScheduleTime", "DaysBack": $scope.daysBack, "NumberOfDays": $scope.numberOfDays };
                };

                api.load = function (payload) {
                    console.log(payload);
                    $scope.chunkTimes = UtilsService.getArrayEnum(ReprocessChunkTimeEnum);

                    if (payload != undefined) {
                        if (payload.rawExpressions != undefined) {
                            $scope.daysBack = payload.rawExpressions.DaysBack;
                            $scope.numberOfDays = payload.rawExpressions.NumberOfDays;
                        }

                        if (payload.data != undefined) {
                            $scope.selectedChunkTime = UtilsService.getEnum(ReprocessChunkTimeEnum, "value", payload.data.ChunkTime);
                        }
                    }


                    var promises = [];

                    var reprocessDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    reprocessDefinitionSelectorReadyDeferred.promise.then(function () {
                        var reprocessPayload;
                        if (payload != undefined && payload.data != undefined) {
                            reprocessPayload = { selectedIds: payload.data.ReprocessDefinitionId };
                        }
                        VRUIUtilsService.callDirectiveLoad(reprocessDefinitionSelectorAPI, reprocessPayload, reprocessDefinitionSelectorLoadDeferred);
                    });

                    promises.push(reprocessDefinitionSelectorLoadDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
