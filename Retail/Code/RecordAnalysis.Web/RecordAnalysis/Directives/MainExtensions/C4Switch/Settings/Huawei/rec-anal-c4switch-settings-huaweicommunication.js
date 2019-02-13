(function (app) {

    'use strict';

    RecordAnalysisC4SwitchSettingsHuaweiCommunication.$inject = ["UtilsService", 'VRUIUtilsService'];

    function RecordAnalysisC4SwitchSettingsHuaweiCommunication(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SwitchCommunicationCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "huaweiCommunicationCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/RecordAnalysis/Directives/MainExtensions/C4Switch/Settings/Huawei/Templates/HuaweiC4SwitchCommunicationTemplate.html"
        };

        function SwitchCommunicationCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var sshCommunicatorSettingsAPI;;
            var sshCommunicatorSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSSHCommunicatorSettingsReady = function (api) {
                    sshCommunicatorSettingsAPI = api;
                    sshCommunicatorSettingsReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        var sshCommunicationList = payload.sshCommunicationList;
                        if (sshCommunicationList != undefined && sshCommunicationList.length > 0) {
                            var sshCommunication = sshCommunicationList[0];

                            $scope.scopeModel.sslHost = sshCommunication.SSLSettings.Host;
                            $scope.scopeModel.sslPort = sshCommunication.SSLSettings.Port;
                            $scope.scopeModel.sslUsername = sshCommunication.SSLSettings.Username;
                            $scope.scopeModel.sslPassword = sshCommunication.SSLSettings.Password;
                            $scope.scopeModel.sslInterfaceIP = sshCommunication.SSLSettings.InterfaceIP;

                            var sshCommunicationSettingsLoadPromise = getSSHCommunicationSettingsLoadPromise();
                            promises.push(sshCommunicationSettingsLoadPromise);

                            function getSSHCommunicationSettingsLoadPromise() {
                                var sshCommunicatorSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                                sshCommunicatorSettingsReadyDeferred.promise.then(function () {

                                    var sshCommunicatorSettingsPayload = { sshCommunicatorSettings: sshCommunication.SSHCommunicatorSettings };
                                    VRUIUtilsService.callDirectiveLoad(sshCommunicatorSettingsAPI, sshCommunicatorSettingsPayload, sshCommunicatorSettingsLoadPromiseDeferred);
                                });

                                return sshCommunicatorSettingsLoadPromiseDeferred.promise;
                            }
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var sshCommunicationList = [{
                        IsActive: true,
                        SSHCommunicatorSettings: sshCommunicatorSettingsAPI.getData(),
                        SSLSettings: {
                            Host: $scope.scopeModel.sslHost,
                            Port: $scope.scopeModel.sslPort,
                            Username: $scope.scopeModel.sslUsername,
                            Password: $scope.scopeModel.sslPassword,
                            InterfaceIP: $scope.scopeModel.sslInterfaceIP
                        }
                    }];

                    return sshCommunicationList;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }
    
    app.directive('recAnalC4switchSettingsHuaweicommunication', RecordAnalysisC4SwitchSettingsHuaweiCommunication);
})(app);