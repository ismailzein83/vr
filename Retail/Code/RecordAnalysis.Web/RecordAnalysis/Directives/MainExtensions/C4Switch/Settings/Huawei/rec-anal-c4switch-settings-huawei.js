(function (app) {

    'use strict';

    RecordAnalysisC4SwitchSettingsHuaweiDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function RecordAnalysisC4SwitchSettingsHuaweiDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SwitchCommunicationSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "huaweiSettingsCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/RecordAnalysis/Directives/MainExtensions/C4Switch/Settings/Huawei/Templates/HuaweiC4SwitchSettingsTemplate.html"
        };

        function SwitchCommunicationSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var sshCommunicatorAPI;;
            var sshCommunicatorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
               
                $scope.scopeModel = {};

                $scope.scopeModel.onSSHCommunicatorReady = function (api) {
                    sshCommunicatorAPI = api;
                    sshCommunicatorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined && payload.settings != undefined) {
                        var sshCommunicationList = payload.settings.HuaweiSSHCommunicationList;

                        var sshCommunicationLoadPromise = getSSHCommunicationLoadPromise();
                        promises.push(sshCommunicationLoadPromise);

                        function getSSHCommunicationLoadPromise() {
                            var sshCommunicatorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                            sshCommunicatorReadyDeferred.promise.then(function () {
                                
                                var sshCommunicatorPayload = { sshCommunicationList: sshCommunicationList };
                                VRUIUtilsService.callDirectiveLoad(sshCommunicatorAPI, sshCommunicatorPayload, sshCommunicatorLoadPromiseDeferred);
                            });

                            return sshCommunicatorLoadPromiseDeferred.promise;
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'RecordAnalysis.MainExtensions.C4Switch.HuaweiC4SwitchSettings, RecordAnalysis.MainExtensions',
                        HuaweiSSHCommunicationList: sshCommunicatorAPI.getData()
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('recAnalC4switchSettingsHuawei', RecordAnalysisC4SwitchSettingsHuaweiDirective);
})(app);