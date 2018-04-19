(function (app) {

    'use strict';

    whsRoutesyncSwitchcommunicationFtp.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_BE_CarrierAccountAPIService', 'VRNotificationService'];

    function whsRoutesyncSwitchcommunicationFtp(UtilsService, VRUIUtilsService, WhS_BE_CarrierAccountAPIService, VRNotificationService) {
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
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/SwitchCommunication/Templates/FTPSwitchCommunicationTemplate.html"

        };
        function FTPSwitchCommunicationCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var ftpCommunicatorSettingsReadyAPI;
            var ftpCommunicatorSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onFTPCommunicatorSettingsReady = function (api) {
                    ftpCommunicatorSettingsReadyAPI = api;
                    ftpCommunicatorSettingsReadyDeferred.resolve();
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var ftpCommunicatorSettings;

                    if (payload != undefined && payload.communicatorSettings != undefined) {
                        ftpCommunicatorSettings = payload.communicatorSettings.FTPCommunicatorSettings;
                    }

                    var ftpCommunicatorSettingsLoadPromise = getFTPCommunicatorSettingsLoadPromise();
                    promises.push(ftpCommunicatorSettingsLoadPromise);

                    function getFTPCommunicatorSettingsLoadPromise() {
                        var ftpCommunicatorSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        ftpCommunicatorSettingsReadyDeferred.promise.then(function () {
                            var ftpCommunicatorSettingsPayload = undefined;
                            if (ftpCommunicatorSettings != undefined)
                                ftpCommunicatorSettingsPayload = { ftpCommunicatorSettings: ftpCommunicatorSettings };

                            VRUIUtilsService.callDirectiveLoad(ftpCommunicatorSettingsReadyAPI, ftpCommunicatorSettingsPayload, ftpCommunicatorSettingsLoadPromiseDeferred);
                        });

                        return ftpCommunicatorSettingsLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.RouteSync.MainExtensions.Switch.SwitchFTPCommunication, TOne.WhS.RouteSync.MainExtensions",
                        FTPCommunicatorSettings: ftpCommunicatorSettingsReadyAPI.getData()
                    };
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncSwitchcommunicationFtp', whsRoutesyncSwitchcommunicationFtp);

})(app);