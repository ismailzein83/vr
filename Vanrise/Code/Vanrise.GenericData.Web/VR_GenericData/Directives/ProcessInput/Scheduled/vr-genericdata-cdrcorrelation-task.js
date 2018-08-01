"use strict";

app.directive("vrGenericdataCdrcorrelationTask", ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CDRCorrelationTaskDirectiveCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/ProcessInput/Scheduled/Templates/GenericDataCDRCorrelationTaskTemplate.html"
        };

        function CDRCorrelationTaskDirectiveCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var cdrCorrelationDefinitionSelectorAPI;
            var cdrCorrelationDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCDRCorrelationDefinitionSelectorReady = function (api) {
                    cdrCorrelationDefinitionSelectorAPI = api;
                    cdrCorrelationDefinitionSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.dateTimeMargin = payload.data.DateTimeMargin;
                        $scope.scopeModel.durationMargin = payload.data.DurationMargin;
                        $scope.scopeModel.batchIntervalTime = payload.data.BatchIntervalTime;
                    } else {
                        $scope.scopeModel.dateTimeMargin = "00:00:05";
                        $scope.scopeModel.durationMargin = "00:00:05";
                        $scope.scopeModel.batchIntervalTime = "01:00:00";
                    }

                    var loadCDRCorrelationDefinitionSelectorPromise = loadCDRCorrelationDefinitionSelector();
                    promises.push(loadCDRCorrelationDefinitionSelectorPromise);

                    function loadCDRCorrelationDefinitionSelector() {

                        var cdrCorrelationDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        cdrCorrelationDefinitionSelectorReadyDeferred.promise.then(function () {
                            var cdrCorrelationDefinitionSelectorPayload;
                            if (payload != undefined && payload.data != undefined) {
                                cdrCorrelationDefinitionSelectorPayload = { selectedIds: payload.data.CDRCorrelationDefinitionId };
                            }

                            VRUIUtilsService.callDirectiveLoad(cdrCorrelationDefinitionSelectorAPI, cdrCorrelationDefinitionSelectorPayload, cdrCorrelationDefinitionSelectorLoadDeferred);
                        });

                        return cdrCorrelationDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.BP.Arguments.CDRCorrelationProcessInput,Vanrise.GenericData.BP.Arguments",
                        CDRCorrelationDefinitionId: cdrCorrelationDefinitionSelectorAPI.getSelectedIds(),
                        DateTimeMargin: $scope.scopeModel.dateTimeMargin,
                        DurationMargin: $scope.scopeModel.durationMargin,
                        BatchIntervalTime: $scope.scopeModel.batchIntervalTime
                    };
                };

                api.getExpressionsData = function () {
                    return { "ScheduleTime": "ScheduleTime" };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;
    }]);
