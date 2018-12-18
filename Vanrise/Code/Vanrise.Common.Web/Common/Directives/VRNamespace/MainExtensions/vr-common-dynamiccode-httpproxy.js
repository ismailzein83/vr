'use strict';

app.directive('vrCommonDynamiccodeHttpproxy', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService','VRCommon_HttpProxyMethodService',
    function (UtilsService, VRUIUtilsService, VRNotificationService, VRCommon_HttpProxyMethodService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRHttpProxyDymanicCode($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRNamespace/MainExtensions/Templates/VRHttpProxyDynamicCodeTemplate.html'
        };

        function VRHttpProxyDymanicCode($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionSelectorAPI;
            var connectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var context;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.httpProxyMethods = [];
                $scope.scopeModel.menuActions = [];
                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorAPI = api;
                    connectionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.addHttpProxyMethod = function () {
                    var onHttpProxyMethodAdded = function (httpProxyMethodAdded) {
                        $scope.scopeModel.httpProxyMethods.push({ Entity: httpProxyMethodAdded });
                    };

                    VRCommon_HttpProxyMethodService.addHttpProxyMethod(onHttpProxyMethodAdded, getContext());
                };

                $scope.scopeModel.validateHttpProxyMethods = function () {
                    if ($scope.scopeModel.httpProxyMethods.length == 0) {
                        return 'At least one Http proxy method should be added';
                    }
                    var names = [];
                    for (var i = 0; i < $scope.scopeModel.httpProxyMethods.length; i++) {
                        var entity = $scope.scopeModel.httpProxyMethods[i].Entity;
                        if (entity != undefined) {
                            names.push(entity.MethodName);
                        }
                    }
                    while (names.length > 0) {
                        var nameToValidate = names[0];
                        names.splice(0, 1);
                        if (!validateName(nameToValidate, names)) {
                            return 'Two or more methods have the same name.';
                        }
                    }
                    return null;
                    function validateName(name, array) {
                        for (var j = 0; j < array.length; j++) {
                            if (array[j] == name) {
                                return false;
                            }
                        }
                        return true;
                    }
                };

                $scope.scopeModel.onDeleteHttpMethod = function (httpProxyMethod) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.httpProxyMethods, httpProxyMethod.Entity.MethodName, 'Entity.MethodName');
                    if (index > -1) {
                        $scope.scopeModel.httpProxyMethods.splice(index, 1);
                    }
                };
                defineAPI();
                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined && payload.vrCustomCodeSettingsEntity != undefined) {
                        $scope.scopeModel.className = payload.vrCustomCodeSettingsEntity.ClassName;
                        $scope.scopeModel.namespaceMembers = payload.vrCustomCodeSettingsEntity.NamespaceMembers;
                        if (payload.vrCustomCodeSettingsEntity.Methods != undefined && payload.vrCustomCodeSettingsEntity.Methods.length > 0) {
                            for (var i = 0; i < payload.vrCustomCodeSettingsEntity.Methods.length; i++) {
                                var method = payload.vrCustomCodeSettingsEntity.Methods[i];
                                $scope.scopeModel.httpProxyMethods.push({ Entity: method });
                            }
                        }
                    }

                    promises.push(loadConnectionSelector());

                    function loadConnectionSelector() {
                        var connectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        connectionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Common.Business.VRHttpConnectionFilter, Vanrise.Common.Business"
                                    }]
                                },
                                selectedIds: payload!=undefined && payload.vrCustomCodeSettingsEntity != undefined ? payload.vrCustomCodeSettingsEntity.ConnectionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(connectionSelectorAPI, selectorPayload, connectionSelectorLoadDeferred);
                        });
                        return connectionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    function getMethods() {
                        var methods = [];
                        if ($scope.scopeModel.httpProxyMethods.length > 0) {
                            for (var i = 0; i < $scope.scopeModel.httpProxyMethods.length; i++) {
                                var httpProxyMethod = $scope.scopeModel.httpProxyMethods[i];
                                if (httpProxyMethod.Entity != undefined) {
                                    methods.push({
                                        MethodName: httpProxyMethod.Entity.MethodName,
                                        MethodType: httpProxyMethod.Entity.MethodType,
                                        MessageFormat: httpProxyMethod.Entity.MessageFormat,
                                        ActionPath: httpProxyMethod.Entity.ActionPath,
                                        ReturnType: httpProxyMethod.Entity.ReturnType,
                                        Body: httpProxyMethod.Entity.Body,
                                        Headers: httpProxyMethod.Entity.Headers,
                                        Parameters: httpProxyMethod.Entity.Parameters,
                                        URLParameters: httpProxyMethod.Entity.URLParameters,
                                        ResponseLogic: httpProxyMethod.Entity.ResponseLogic,
                                        ClassMembers: httpProxyMethod.Entity.ClassMembers,
                                        NamespaceMembers: httpProxyMethod.Entity.NamespaceMembers
                                    });
                                }
                            }
                        }
                        return methods;
                    }
                    return {
                        $type: "Vanrise.Common.MainExtensions.VRDynamicCode.HttpProxyDynamicCodeSettings,Vanrise.Common.MainExtensions",
                        ConnectionId: connectionSelectorAPI.getSelectedIds(),
                        ClassName: $scope.scopeModel.className,
                        Methods: getMethods(),
                        NamespaceMembers: $scope.scopeModel.namespaceMembers
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: "Edit",
                    clicked: editHttpProxyMethod
                }];
            }

            function editHttpProxyMethod(httpProxyMethod) {

                var onHttpProxyMethodUpdated = function (httpProxyMethodUpdated) {
                    var index = $scope.scopeModel.httpProxyMethods.indexOf(httpProxyMethod);
                    $scope.scopeModel.httpProxyMethods[index] = { Entity: httpProxyMethodUpdated };
                };

                VRCommon_HttpProxyMethodService.editHttpProxyMethod(onHttpProxyMethodUpdated, httpProxyMethod.Entity, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

        }
    }]);
