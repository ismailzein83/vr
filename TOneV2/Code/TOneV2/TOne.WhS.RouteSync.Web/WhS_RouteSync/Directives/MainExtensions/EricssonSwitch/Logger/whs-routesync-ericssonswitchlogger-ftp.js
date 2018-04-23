(function (app) {

    'use strict';

    whsRoutesyncEricssonswitchloggerFtp.$inject = ["UtilsService", 'VRUIUtilsService'];

    function whsRoutesyncEricssonswitchloggerFtp(UtilsService, VRUIUtilsService) {
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
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSwitch/Logger/Templates/EricssonSwitchFTPLoggerTemplate.html"

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
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var ftpCommunicatorSettings;

                    if (payload != undefined && payload.logger != undefined) {
                        ftpCommunicatorSettings = payload.logger.FTPCommunicatorSettings;
                    }

                    var ftpCommunicatorSettingsLoadPromise = getFTPCommunicatorSettingsLoadPromise();
                    promises.push(ftpCommunicatorSettingsLoadPromise);

                    function getFTPCommunicatorSettingsLoadPromise() {
                        var ftpCommunicatorSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        ftpCommunicatorSettingsReadyDeferred.promise.then(function () {
                            var ftpCommunicatorSettingsPayload;
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
                        $type: "TOne.WhS.RouteSync.Ericsson.Business.FTPLogger, TOne.WhS.RouteSync.Ericsson",
                        FTPCommunicatorSettings: ftpCommunicatorSettingsReadyAPI.getData()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncEricssonswitchloggerFtp', whsRoutesyncEricssonswitchloggerFtp);

})(app);