'use strict';

app.directive('retailTelesAccountactiondefinitionsettingsChangeuserroutinggroup', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ChangeUserRoutingGroupActionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Teles/Directives/AccountAction/MappingTelesAccountAction/Templates/ChangeUserRoutingGroupActionSettingsTemplate.html'
        };

        function ChangeUserRoutingGroupActionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var conectionTypeAPI;
            var conectionTypeReadyDeferred = UtilsService.createPromiseDeferred();
            var executePermissionAPI;
            var executePermissionReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onConectionTypeReady = function (api) {
                    conectionTypeAPI = api;
                    conectionTypeReadyDeferred.resolve();
                };
                $scope.scopeModel.onExecuteRequiredPermissionReady = function (api) {
                    executePermissionAPI = api;
                    executePermissionReadyDeferred.resolve();
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if(payload != undefined && payload.accountActionDefinitionSettings != undefined)
                    {
                    }
                    var promises = [];

                    promises.push(loadConectionTypes());
                    promises.push(loadExecuteRequiredPermission());

                    function loadConectionTypes() {
                        var conectionTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        conectionTypeReadyDeferred.promise.then(function () {
                            var conectionTypePayload;
                            if (payload != undefined && payload.accountActionDefinitionSettings != undefined) {
                                conectionTypePayload = { selectedIds: payload.accountActionDefinitionSettings.VRConnectionId };
                            }
                            VRUIUtilsService.callDirectiveLoad(conectionTypeAPI, conectionTypePayload, conectionTypeLoadDeferred);
                        });
                        return conectionTypeLoadDeferred.promise
                    }
                    function loadExecuteRequiredPermission() {
                        var executePermissionLoadDeferred = UtilsService.createPromiseDeferred();

                        executePermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: payload != undefined && payload.accountActionDefinitionSettings && payload.accountActionDefinitionSettings.Security != undefined && payload.accountActionDefinitionSettings.Security.ExecutePermission || undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(executePermissionAPI, dataPayload, executePermissionLoadDeferred);
                        });
                        return executePermissionLoadDeferred.promise
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: 'Retail.Teles.Business.AccountBEActionTypes.ChangeUserRoutingGroupActionSettings, Retail.Teles.Business',
                        VRConnectionId: conectionTypeAPI.getSelectedIds(),
                        Security:{
                            ExecutePermission: executePermissionAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);