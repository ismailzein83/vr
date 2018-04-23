(function (app) {

    'use strict';

    whsRoutesyncSwitchcommunicationSsh.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_BE_CarrierAccountAPIService', 'VRNotificationService'];

    function whsRoutesyncSwitchcommunicationSsh(UtilsService, VRUIUtilsService, WhS_BE_CarrierAccountAPIService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FTPSwitchCommunicationCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/SwitchCommunication/Templates/SSHSwitchCommunicationTemplate.html"

        };
        function FTPSwitchCommunicationCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var sshCommunicatorSettingsReadyAPI;
            var sshCommunicatorSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSSHCommunicatorSettingsReady = function (api) {
                    sshCommunicatorSettingsReadyAPI = api;
                    sshCommunicatorSettingsReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var sshCommunicatorSettings;

                    if (payload != undefined && payload.communicatorSettings != undefined) {
                        sshCommunicatorSettings = payload.communicatorSettings.SSHCommunicatorSettings;
                    }
                    
                    var sshCommunicatorSettingsLoadPromise = getSSHCommunicatorSettingsLoadPromise();
                    promises.push(sshCommunicatorSettingsLoadPromise);

                    function getSSHCommunicatorSettingsLoadPromise() {
                        var sshCommunicatorSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        sshCommunicatorSettingsReadyDeferred.promise.then(function () {
                            var sshCommunicatorSettingsPayload = undefined;
                            if (sshCommunicatorSettings != undefined)
                                sshCommunicatorSettingsPayload = { sshCommunicatorSettings: sshCommunicatorSettings };

                            VRUIUtilsService.callDirectiveLoad(sshCommunicatorSettingsReadyAPI, sshCommunicatorSettingsPayload, sshCommunicatorSettingsLoadPromiseDeferred);
                        });

                        return sshCommunicatorSettingsLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.RouteSync.MainExtensions.Switch.SwitchSSHCommunication, TOne.WhS.RouteSync.MainExtensions",
                        SSHCommunicatorSettings: sshCommunicatorSettingsReadyAPI.getData()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncSwitchcommunicationSsh', whsRoutesyncSwitchcommunicationSsh);

})(app);