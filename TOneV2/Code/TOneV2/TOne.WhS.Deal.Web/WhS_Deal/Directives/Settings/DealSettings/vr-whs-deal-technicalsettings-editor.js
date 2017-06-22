'use strict';

app.directive('vrWhsDealTechnicalsettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DealAnalysisTechnicalSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/Settings/DealSettings/Templates/DealTechnicalSettingsEditorTemplate.html'
        };

        function DealAnalysisTechnicalSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var reprocessDefinitionSelectorAPI;
            var reprocessDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();

            var chunkTimeSelectorAPI;
            var chunkTimeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onReprocessDefinitionSelectorReady = function (api) {
                    reprocessDefinitionSelectorAPI = api;
                    reprocessDefinitionPromiseDeferred.resolve();
                };

                $scope.scopeModel.onChunkTimeSelectorReady = function (api) {
                    chunkTimeSelectorAPI = api;
                    chunkTimeSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var dealTechnicalSettingData;

                    if (payload != undefined && payload.data != undefined) {
                        dealTechnicalSettingData = payload.data;
                    }

                    $scope.scopeModel.intervalOffset = dealTechnicalSettingData.IntervalOffset;

                    //Loading ReprocessDefinition selector 
                    var reprocessDefinitionSelectorLoadPromise = getReprocessDefinitionSelectorLoadPromise();
                    promises.push(reprocessDefinitionSelectorLoadPromise);

                    //Loading ChunkTime selector 
                    var chunktimeSelectorLoadPromise = getChunktimeSelectorLoadPromise();
                    promises.push(chunktimeSelectorLoadPromise);


                    function getReprocessDefinitionSelectorLoadPromise() {
                        var reprocessDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        reprocessDefinitionPromiseDeferred.promise.then(function () {
                            var reprocessDefinitionSelectorPayload;
                            if (dealTechnicalSettingData != undefined) {
                                reprocessDefinitionSelectorPayload = {
                                    selectedIds: dealTechnicalSettingData.ReprocessDefinitionId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(reprocessDefinitionSelectorAPI, reprocessDefinitionSelectorPayload, reprocessDefinitionSelectorLoadDeferred);
                        });

                        return reprocessDefinitionSelectorLoadDeferred.promise;
                    }
                    function getChunktimeSelectorLoadPromise() {
                        var chunkTimeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        chunkTimeSelectorReadyDeferred.promise.then(function () {
                            var chunkTimeSelectorPayload;
                            if (payload != undefined && payload.data != undefined) {
                                chunkTimeSelectorPayload = {
                                    selectedIds: dealTechnicalSettingData.ChunkTime
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(chunkTimeSelectorAPI, chunkTimeSelectorPayload, chunkTimeSelectorLoadDeferred);
                        });

                        return chunkTimeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: 'TOne.WhS.Deal.Entities.DealTechnicalSettingData, TOne.WhS.Deal.Entities',
                        ReprocessDefinitionId: reprocessDefinitionSelectorAPI.getSelectedIds(),
                        ChunkTime: chunkTimeSelectorAPI.getSelectedIds(),
                        IntervalOffset: $scope.scopeModel.intervalOffset
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }
    }]);