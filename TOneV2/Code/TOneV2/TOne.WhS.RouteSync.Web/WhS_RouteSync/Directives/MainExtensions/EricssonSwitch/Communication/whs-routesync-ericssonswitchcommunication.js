(function (app) {

    'use strict';

    whsRoutesyncEricssonswitchcommunication.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_RouteSync_EricssonSwitchLoggerAPIService'];

    function whsRoutesyncEricssonswitchcommunication(UtilsService, VRUIUtilsService, WhS_RouteSync_EricssonSwitchLoggerAPIService) {
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
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSwitch/Communication/Templates/EricssonSwitchCommunicationTemplate.html"

        };
        function SwitchCommunicationCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var loggerGridAPI;
            var loggerGridReadyDeferred = UtilsService.createPromiseDeferred();

            var remoteGridAPI;
            var remoteGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.switchRemoteCommunications = [];
                $scope.scopeModel.switchLoggers = [];

                $scope.scopeModel.onRemoteGridReady = function (api) {
                    remoteGridAPI = api;
                    remoteGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onLoggerGridReady = function (api) {
                    loggerGridAPI = api;
                    loggerGridReadyDeferred.resolve();
                };

                $scope.scopeModel.validateRemoteCommunicationList = function () {
                    return;
                };

                $scope.scopeModel.validateLoggers = function () {
                    if ($scope.scopeModel.switchLoggers == undefined || $scope.scopeModel.switchLoggers.length == 0)
                        return 'At least one logger should be added';

                    var oneLoggerIsActive = false;

                    for (var x = 0; x < $scope.scopeModel.switchLoggers.length; x++) {
                        var currentItem = $scope.scopeModel.switchLoggers[x];
                        if (currentItem.isActive) {
                            oneLoggerIsActive = true;
                            break;
                        }
                    }

                    if (!oneLoggerIsActive)
                        return 'At least one logger should be activated';

                    return;
                };

                var initPromises = [];
                initPromises.push(remoteGridReadyDeferred.promise);
                initPromises.push(loggerGridReadyDeferred.promise);

                var loadSwitchRemoteCommunicationsPromise = UtilsService.createPromiseDeferred();
                WhS_RouteSync_EricssonSwitchLoggerAPIService.GetSwitchLoggerTemplates().then(function (response) {
                    $scope.scopeModel.loggerTypes = response;
                    loadSwitchRemoteCommunicationsPromise.resolve();
                });

                initPromises.push(loadSwitchRemoteCommunicationsPromise.promise);

                UtilsService.waitMultiplePromises(initPromises).then(function () {
                    defineAPI();
                });

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var remoteCommunication;
                    var logger;

                    if (payload != undefined) {
                        var remoteCommunicationList = payload.remoteCommunicationList;
                        if (remoteCommunicationList != undefined && remoteCommunicationList.length > 0)
                            remoteCommunication = remoteCommunicationList[0];

                        var loggerList = payload.switchLoggerList;
                        if (loggerList != undefined && loggerList.length > 0)
                            logger = loggerList[0];
                    }

                    var remoteCommnunicationItem = {
                        isSelected: false,
                        isActive: false,
                        api: undefined
                    };

                    extendRemoteCommunicationItem(remoteCommnunicationItem, remoteCommunication);
                    $scope.scopeModel.switchRemoteCommunications.push(remoteCommnunicationItem);


                    var loggerItem = {
                        isActive: false,
                        api: undefined
                    };
                    extendLoggerItem(loggerItem, logger);
                    $scope.scopeModel.switchLoggers.push(loggerItem);

                    function extendRemoteCommunicationItem(RemoteCommnunicationItem, RemoteCommunication) {
                        RemoteCommnunicationItem.onRemoteCommunicatorSettingsReady = function (api) {
                            RemoteCommnunicationItem.api = api;

                            if (RemoteCommnunicationItem.remoteCommunicatorSettingsReadyDeferred != undefined) {
                                RemoteCommnunicationItem.remoteCommunicatorSettingsReadyDeferred.resolve();
                            }
                            else {
                                RemoteCommnunicationItem.isActive = true;
                                var setLoader = function (value) {
                                    setTimeout(function () { RemoteCommnunicationItem.isSwitchRemoteCommunicationLoading = value; UtilsService.safeApply($scope); });
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, RemoteCommnunicationItem.api, undefined, setLoader);
                            }
                        };

                        if (RemoteCommunication != undefined) {
                            RemoteCommnunicationItem.remoteCommunicatorSettingsReadyDeferred = UtilsService.createPromiseDeferred();
                            RemoteCommnunicationItem.isSelected = true;
                            RemoteCommnunicationItem.isActive = RemoteCommunication.IsActive;

                            RemoteCommnunicationItem.remoteCommunicationSettingsLoadPromise = getRemoteCommunicationSettingsLoadPromise();
                            promises.push(RemoteCommnunicationItem.remoteCommunicationSettingsLoadPromise);

                            function getRemoteCommunicationSettingsLoadPromise() {
                                var remoteCommunicatorSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                                RemoteCommnunicationItem.remoteCommunicatorSettingsReadyDeferred.promise.then(function () {
                                    RemoteCommnunicationItem.remoteCommunicatorSettingsReadyDeferred = undefined;
                                    var remoteCommunicatorSettingsPayload = { remoteCommunicatorSettings: RemoteCommunication.RemoteCommunicatorSettings };
                                    VRUIUtilsService.callDirectiveLoad(RemoteCommnunicationItem.api, remoteCommunicatorSettingsPayload, remoteCommunicatorSettingsLoadPromiseDeferred);
                                });

                                return remoteCommunicatorSettingsLoadPromiseDeferred.promise;
                            }
                        }
                    }

                    function extendLoggerItem(loggerItem, logger) {
                        loggerItem.onReady = function (api) {
                            loggerItem.api = api;

                            if (loggerItem.loggerSettingsReadyDeferred != undefined) {
                                loggerItem.loggerSettingsReadyDeferred.resolve();
                            }
                            else {
                                loggerItem.isActive = true;
                                var setLoader = function (value) {
                                    setTimeout(function () { loggerItem.isSwitchLoggerLoading = value; UtilsService.safeApply($scope); });
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, loggerItem.api, undefined, setLoader);
                            }
                        };

                        if (logger != undefined) {
                            loggerItem.selectedLoggerType = UtilsService.getItemByVal($scope.scopeModel.loggerTypes, logger.ConfigId, "ExtensionConfigurationId");
                            loggerItem.loggerSettingsReadyDeferred = UtilsService.createPromiseDeferred();
                            loggerItem.isActive = logger.IsActive;

                            loggerItem.loggerSettingsLoadPromise = getLoggerSettingsLoadPromise();
                            promises.push(loggerItem.loggerSettingsLoadPromise);

                            function getLoggerSettingsLoadPromise() {
                                var loggerSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                                loggerItem.loggerSettingsReadyDeferred.promise.then(function () {
                                    loggerItem.loggerSettingsReadyDeferred = undefined;
                                    var loggerSettingsPayload = { logger: logger };
                                    VRUIUtilsService.callDirectiveLoad(loggerItem.api, loggerSettingsPayload, loggerSettingsLoadPromiseDeferred);
                                });

                                return loggerSettingsLoadPromiseDeferred.promise;
                            }
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var switchRemoteCommunicationItem = $scope.scopeModel.switchRemoteCommunications[0];
                    var remoteCommunicatorSettings = switchRemoteCommunicationItem.isSelected && switchRemoteCommunicationItem.api != undefined ? switchRemoteCommunicationItem.api.getData() : undefined;

                    var loggerItem = $scope.scopeModel.switchLoggers[0];
                    var loggerSettings = loggerItem.api != undefined ? loggerItem.api.getData() : undefined;
                    loggerSettings.IsActive = loggerItem.isActive;

                    var result = {
                        switchCommunicationList: remoteCommunicatorSettings != undefined ? [{ RemoteCommunicatorSettings: remoteCommunicatorSettings, IsActive: switchRemoteCommunicationItem.isActive }] : undefined,
                        switchLoggerList: [loggerSettings]
                    };

                    return result;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncEricssonswitchcommunication', whsRoutesyncEricssonswitchcommunication);

})(app);