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

            var sshGridAPI;
            var sshGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.switchSSHCommunications = [];
                $scope.scopeModel.switchLoggers = [];

                $scope.scopeModel.onSSHGridReady = function (api) {
                    sshGridAPI = api;
                    sshGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onLoggerGridReady = function (api) {
                    loggerGridAPI = api;
                    loggerGridReadyDeferred.resolve();
                };

                $scope.scopeModel.validateSSHCommunicationList = function () {
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
                initPromises.push(sshGridReadyDeferred.promise);
                initPromises.push(loggerGridReadyDeferred.promise);

                var loadSwitchSSHCommunicationsPromise = UtilsService.createPromiseDeferred();
                WhS_RouteSync_EricssonSwitchLoggerAPIService.GetSwitchLoggerTemplates().then(function (response) {
                    $scope.scopeModel.loggerTypes = response;
                    loadSwitchSSHCommunicationsPromise.resolve();
                });

                initPromises.push(loadSwitchSSHCommunicationsPromise.promise);

                UtilsService.waitMultiplePromises(initPromises).then(function () {
                    defineAPI();
                });

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var sshCommunication;
                    var logger;

                    if (payload != undefined) {
                        var sshCommunicationList = payload.sshCommunicationList;
                        if (sshCommunicationList != undefined && sshCommunicationList.length > 0)
                            sshCommunication = sshCommunicationList[0];

                        var loggerList = payload.switchLoggerList;
                        if (loggerList != undefined && loggerList.length > 0)
                            logger = loggerList[0];
                    }

                    var sshCommnunicationItem = {
                        isSelected: false,
                        isActive: false,
                        api: undefined
                    };

                    extendSSHCommunicationItem(sshCommnunicationItem, sshCommunication);
                    $scope.scopeModel.switchSSHCommunications.push(sshCommnunicationItem);


                    var loggerItem = {
                        isActive: false,
                        api: undefined
                    };
                    extendLoggerItem(loggerItem, logger);
                    $scope.scopeModel.switchLoggers.push(loggerItem);

                    function extendSSHCommunicationItem(sshCommnunicationItem, sshCommunication) {
                        sshCommnunicationItem.onSSHCommunicatorSettingsReady = function (api) {
                            sshCommnunicationItem.api = api;

                            if (sshCommnunicationItem.sshCommunicatorSettingsReadyDeferred != undefined) {
                                sshCommnunicationItem.sshCommunicatorSettingsReadyDeferred.resolve();
                            }
                            else {
                                sshCommnunicationItem.isActive = true;
                                var setLoader = function (value) {
                                    setTimeout(function () { sshCommnunicationItem.isSwitchSSHCommunicationLoading = value; UtilsService.safeApply($scope); });
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sshCommnunicationItem.api, undefined, setLoader);
                            }
                        };

                        if (sshCommunication != undefined) {
                            sshCommnunicationItem.sshCommunicatorSettingsReadyDeferred = UtilsService.createPromiseDeferred();
                            sshCommnunicationItem.isSelected = true;
                            sshCommnunicationItem.isActive = sshCommunication.IsActive;

                            sshCommnunicationItem.sshCommunicationSettingsLoadPromise = getSSHCommunicationSettingsLoadPromise();
                            promises.push(sshCommnunicationItem.sshCommunicationSettingsLoadPromise);

                            function getSSHCommunicationSettingsLoadPromise() {
                                var sshCommunicatorSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                                sshCommnunicationItem.sshCommunicatorSettingsReadyDeferred.promise.then(function () {
                                    sshCommnunicationItem.sshCommunicatorSettingsReadyDeferred = undefined;
                                    var sshCommunicatorSettingsPayload = { sshCommunicatorSettings: sshCommunication.SSHCommunicatorSettings };
                                    VRUIUtilsService.callDirectiveLoad(sshCommnunicationItem.api, sshCommunicatorSettingsPayload, sshCommunicatorSettingsLoadPromiseDeferred);
                                });

                                return sshCommunicatorSettingsLoadPromiseDeferred.promise;
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
                    var switchSSHCommunicationItem = $scope.scopeModel.switchSSHCommunications[0];
                    var sshCommunicatorSettings = switchSSHCommunicationItem.isSelected && switchSSHCommunicationItem.api != undefined ? switchSSHCommunicationItem.api.getData() : undefined;

                    var loggerItem = $scope.scopeModel.switchLoggers[0];
                    var loggerSettings = loggerItem.api != undefined ? loggerItem.api.getData() : undefined;
                    loggerSettings.IsActive = loggerItem.isActive;

                    var result = {
                        sshCommunicationList: sshCommunicatorSettings != undefined ? [{ SSHCommunicatorSettings: sshCommunicatorSettings, IsActive: switchSSHCommunicationItem.isActive }] : undefined,
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