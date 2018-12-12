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
            templateUrl: "/Client/Modules/Retail_RA/Directives/OperatorDeclaration/MainExtensions/Templates/OperatorDeclarationServiceVoiceTemplate.html"
        };

        function OperatorDeclarationServiceVoiceCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var trafficDirectionSelectorAPI;
            var trafficDirectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

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
                        $scope.scopeModel.amount = voiceEntity.Amount;
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

                    //var trafficTypeSelectorLoadPromise = loadTrafficTypeSelector();
                    var trafficDirectionSelectorLoadPromise = loadTrafficDirectionSelector();

                    //promises.push(trafficTypeSelectorLoadPromise);
                    promises.push(trafficDirectionSelectorLoadPromise);

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.Voice,Retail.RA.Business",
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

    app.directive('retailRaOperatordeclarationserviceVoice', OperatorDeclarationServiceVoice);

})(app);