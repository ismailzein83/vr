(function (app) {

    'use strict';

    OperatorDeclarationServiceVoice.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OperatorDeclarationServiceVoice(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new OperatorDeclarationServiceVoiceCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/OperatorDeclarationServices/MainExtensions/Templates/OperatorDeclarationServiceVoiceTemplate.html"
        };

        function OperatorDeclarationServiceVoiceCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var trafficTypeSelectorAPI;
            var trafficTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var trafficDirectionSelectorAPI;
            var trafficDirectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onTrafficTypeSelectorReady = function (api) {
                    trafficTypeSelectorAPI = api;
                    trafficTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onTrafficDirectionSelectorReady = function (api) {
                    trafficDirectionSelectorAPI = api;
                    trafficDirectionSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var voiceEntity;

                    if (payload != undefined) {

                        voiceEntity = payload.settings;
                        $scope.scopeModel.numberOfCalls = voiceEntity.NumberOfCalls;
                        $scope.scopeModel.duration = voiceEntity.Duration;
                        $scope.scopeModel.amount = voiceEntity.TotalChargedDuration;
                    }

                    function loadTrafficTypeSelector() {
                        var trafficTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        trafficTypeSelectorReadyDeferred.promise.then(function () {
                            var payload;
                            if (voiceEntity != undefined)
                                payload = {
                                    selectedIds: voiceEntity.TrafficType
                                };
                            VRUIUtilsService.callDirectiveLoad(trafficTypeSelectorAPI, payload, trafficTypeSelectorLoadDeferred);
                        });
                        return trafficTypeSelectorLoadDeferred.promise;
                    }

                    function loadTrafficDirectionSelector() {

                        var trafficDirectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        trafficDirectionSelectorReadyDeferred.promise.then(function () {
                            var payload;

                            if (voiceEntity != undefined)
                                payload = {
                                    selectedIds: voiceEntity.TrafficDirection
                                };
                            VRUIUtilsService.callDirectiveLoad(trafficDirectionSelectorAPI, payload, trafficDirectionSelectorLoadDeferred);
                        });
                        return trafficDirectionSelectorLoadDeferred.promise;
                    }

                    var trafficTypeSelectorLoadPromise = loadTrafficTypeSelector();
                    var trafficDirectionSelectorLoadPromise = loadTrafficDirectionSelector();

                    promises.push(trafficTypeSelectorLoadPromise);
                    promises.push(trafficDirectionSelectorLoadPromise);

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.OperatorDeclarationServices.Voice,Retail.BusinessEntity.MainExtensions",
                        TrafficType: trafficTypeSelectorAPI.getSelectedIds(),
                        TrafficDirection: trafficDirectionSelectorAPI.getSelectedIds(),
                        NumberOfCalls: $scope.scopeModel.numberOfCalls,
                        Duration: $scope.scopeModel.duration,
                        Amount: $scope.scopeModel.amount
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeOperatordeclarationserviceVoice', OperatorDeclarationServiceVoice);

})(app);