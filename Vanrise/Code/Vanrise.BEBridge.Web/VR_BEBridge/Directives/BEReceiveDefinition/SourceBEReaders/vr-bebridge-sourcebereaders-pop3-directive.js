'use strict';

app.directive('vrBebridgeSourcebereadersPop3Directive', ['VRNotificationService', 'UtilsService', 'VRUIUtilsService',
    function (vrNotificationService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var Pop3SourceReader = new pop3SourceReader($scope, ctrl, $attrs);
                Pop3SourceReader.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_BEBridge/Directives/BEReceiveDefinition/SourceBEReaders/Templates/BEReceiveDefinitionPop3SourceReaderTemplate.html'
        };

        function pop3SourceReader($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionSelectorAPI;
            var connectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var pop3FilterSelectorAPI;
            var pop3FilterSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorAPI = api;
                    connectionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onPop3FilterSelectorReady = function (api) {
                    pop3FilterSelectorAPI = api;
                    pop3FilterSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != null && payload.Setting != null) {
                        $scope.scopeModel.batchSize = payload.Setting.BatchSize;
                    }

                    var promises = [];
                    var connectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var pop3FilterSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(connectionSelectorLoadDeferred.promise);
                    promises.push(pop3FilterSelectorLoadDeferred.promise);

                    connectionSelectorReadyDeferred.promise.then(function () {
                        var selectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Vanrise.Common.Business.Pop3ConnectionFilter ,Vanrise.Common.Business"
                                }]
                            },
                            selectedIds: payload != undefined && payload.Setting != undefined && payload.Setting.VRConnectionId || undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(connectionSelectorAPI, selectorPayload, connectionSelectorLoadDeferred);
                    });

                    pop3FilterSelectorReadyDeferred.promise.then(function () {
                        var selectorPayload = {
                            Pop3MessageFilter: (payload != undefined && payload.Setting != undefined) ? payload.Setting.Pop3MessageFilter : undefined,
                        };
                        VRUIUtilsService.callDirectiveLoad(pop3FilterSelectorAPI, selectorPayload, pop3FilterSelectorLoadDeferred);
                    });

                    return connectionSelectorLoadDeferred.promise;
                };
                api.getData = function () {
                    var setting =
                    {
                        VRConnectionId: connectionSelectorAPI.getSelectedIds(),
                        Pop3MessageFilter: pop3FilterSelectorAPI.getData(),
                        BatchSize: $scope.scopeModel.batchSize,
                    };
                    return {
                        $type: "Vanrise.BEBridge.MainExtensions.SourceBEReaders.Pop3SourceReader, Vanrise.BEBridge.MainExtensions",
                        Setting: setting
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
