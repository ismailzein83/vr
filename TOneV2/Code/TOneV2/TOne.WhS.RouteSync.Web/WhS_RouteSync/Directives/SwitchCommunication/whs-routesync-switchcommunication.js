(function (app) {

    'use strict';

    whsRoutesyncSwitchcommunication.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_RouteSync_SwitchCommunicationAPIService', 'VRNotificationService'];

    function whsRoutesyncSwitchcommunication(UtilsService, VRUIUtilsService, WhS_RouteSync_SwitchCommunicationAPIService, VRNotificationService) {
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
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/SwitchCommunication/Templates/SwitchCommunicationTemplate.html"

        };
        function SwitchCommunicationCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.switchCommunications = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyDeferred.resolve();
                };

                $scope.scopeModel.validate = function () {
                    var oneItemIsSelectedAndActive = false;
                    for (var x = 0; x < $scope.scopeModel.switchCommunications.length; x++) {
                        var currentItem = $scope.scopeModel.switchCommunications[x];
                        if (currentItem.isSelected && currentItem.isActive) {
                            oneItemIsSelectedAndActive = true;
                            break;
                        }
                    }
                    if (!oneItemIsSelectedAndActive)
                        return 'At least one item should be activated';

                    return;
                };

                gridReadyDeferred.promise.then(function () {
                    defineAPI();
                });

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var switchCommunicationList;

                    if (payload != undefined) {
                        switchCommunicationList = payload.switchCommunicationList;
                    }

                    var routeOptionRuleSettingsTemplatesLoadPromise = getRouteOptionRuleSettingsTemplatesLoadPromise();
                    promises.push(routeOptionRuleSettingsTemplatesLoadPromise);

                    function getRouteOptionRuleSettingsTemplatesLoadPromise() {
                        var routeOptionRuleSettingsTemplatesLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        WhS_RouteSync_SwitchCommunicationAPIService.GetSwitchCommunicationTemplates().then(function (response) {
                            var internalPromises = [];

                            for (var x = 0; x < response.length; x++) {
                                var currentItem = response[x];
                                var obj = { entity: currentItem, isSelected: false, readyDeferred: UtilsService.createPromiseDeferred(), loadDeferred: undefined, isActive: false };

                                var existingSwitchCommunication = getExistingSwitchCommunication(obj);
                                if (existingSwitchCommunication != null) {
                                    obj.isSelected = true;
                                    obj.isActive = existingSwitchCommunication.IsActive;
                                }

                                if (obj.isSelected) {
                                    obj.loadDeferred = UtilsService.createPromiseDeferred();
                                    internalPromises.push(obj.loadDeferred.promise);

                                    obj.loadDeferred.promise.then(function () {
                                        obj.loadDeferred = undefined;
                                    });
                                }

                                extendObject(obj, existingSwitchCommunication);
                                $scope.scopeModel.switchCommunications.push(obj);
                            }
                            UtilsService.waitMultiplePromises(internalPromises).then(function () {
                                routeOptionRuleSettingsTemplatesLoadPromiseDeferred.resolve();
                            });
                        });

                        return routeOptionRuleSettingsTemplatesLoadPromiseDeferred.promise;
                    }

                    function getExistingSwitchCommunication(switchCommunication) {
                        if (switchCommunicationList != undefined) {
                            for (var y = 0; y < switchCommunicationList.length; y++) {
                                var currentItem = switchCommunicationList[y];
                                if (currentItem.ConfigId == switchCommunication.entity.ExtensionConfigurationId) {
                                    return currentItem;
                                }
                            }
                        }
                        return null;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var result = [];
                    for (var x = 0; x < $scope.scopeModel.switchCommunications.length; x++) {
                        var currentItem = $scope.scopeModel.switchCommunications[x];
                        if (currentItem.isSelected) {
                            var objResult = currentItem.api.getData();
                            objResult.IsActive = currentItem.isActive;
                            result.push(objResult);
                        }
                    }
                    return result;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function extendObject(switchCommunication, existingSwitchCommunication) {
                switchCommunication.onReady = function (api) {
                    switchCommunication.api = api;
                    switchCommunication.readyDeferred.resolve();

                    if (switchCommunication.loadDeferred == undefined) {
                        switchCommunication.isActive = true;
                        var setLoader = function (value) { switchCommunication.isLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, switchCommunication.api, undefined, setLoader);
                    }
                };

                switchCommunication.readyDeferred.promise.then(function () {
                    if (switchCommunication.loadDeferred != undefined) {
                        var switchCommunicationPayload = undefined;
                        if (existingSwitchCommunication != null) {
                            switchCommunicationPayload = { communicatorSettings: existingSwitchCommunication };
                        }
                        VRUIUtilsService.callDirectiveLoad(switchCommunication.api, switchCommunicationPayload, switchCommunication.loadDeferred);
                    }
                });
            }
        }
    }

    app.directive('whsRoutesyncSwitchcommunication', whsRoutesyncSwitchcommunication);

})(app);