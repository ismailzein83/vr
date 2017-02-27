'use strict';

app.directive('vrBebridgeSourcebereadersSqlDirective', ['VRNotificationService','UtilsService','VRUIUtilsService',
    function (vrNotificationService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var sqlSourceReader = new filepSourceReader($scope, ctrl, $attrs);
                sqlSourceReader.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_BEBridge/Directives/BEReceiveDefinition/SourceBEReaders/Templates/BEReceiveDefinitionSqlSourceReadersTemplate.html'
        };

        function filepSourceReader($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionSelectorAPI;
            var connectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorAPI = api;
                    connectionSelectorReadyDeferred.resolve();
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.query = payload.Setting.Query;
                        $scope.scopeModel.timeoutInSec = payload.Setting.CommandTimeout;
                        $scope.scopeModel.basedOnId = payload.Setting.BasedOnId;
                        $scope.scopeModel.idField = payload.Setting.IdField;
                    }
                    var connectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    connectionSelectorReadyDeferred.promise.then(function () {
                        var selectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Vanrise.Common.Business.SQLConnectionFilter ,Vanrise.Common.Business"
                                }]
                            },
                            selectedIds: payload != undefined && payload.Setting != undefined && payload.Setting.VRConnectionId || undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(connectionSelectorAPI, selectorPayload, connectionSelectorLoadDeferred);
                    });

                    return connectionSelectorLoadDeferred.promise;
                };
                api.getData = function () {
                    var setting =
                    {
                        VRConnectionId: connectionSelectorAPI.getSelectedIds(),
                        Query: $scope.scopeModel.query,
                        CommandTimeout: $scope.scopeModel.timeoutInSec,
                        BasedOnId: $scope.scopeModel.basedOnId,
                        IdField: $scope.scopeModel.idField,
                    };
                    return {
                        $type: "Vanrise.BEBridge.MainExtensions.SourceBEReaders.SqlSourceReader, Vanrise.BEBridge.MainExtensions",
                        Setting: setting
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
