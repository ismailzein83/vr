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
            templateUrl: "/Client/Modules/Retail_RA/Directives/OperatorDeclaration/MainExtensions/Templates/IcxOperatorDeclarationServiceVoiceTemplate.html"
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
                        $scope.scopeModel.numberOfCalls = voiceEntity.DeclaredNumberOfCalls;
                        $scope.scopeModel.duration = voiceEntity.DeclaredDuration;
                        $scope.scopeModel.revenue = voiceEntity.DeclaredRevenue;
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

                    var trafficDirectionSelectorLoadPromise = loadTrafficDirectionSelector();
                    promises.push(trafficDirectionSelectorLoadPromise);
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.IcxVoice,Retail.RA.Business",
                        TrafficDirection: trafficDirectionSelectorAPI.getSelectedIds(),
                        DeclaredNumberOfCalls: $scope.scopeModel.numberOfCalls,
                        DeclaredDuration: $scope.scopeModel.duration,
                        DeclaredRevenue: $scope.scopeModel.revenue
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailRaIcxoperatordeclarationserviceVoice', OperatorDeclarationServiceVoice);

})(app);