"use strict";

app.directive("vrGenericdataRestapibedefinitionsEditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:{
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var genericBusinessEntityDefinitionEditor = new VRRestAPIBEDefinitionEditor($scope, ctrl, $attrs);
                genericBusinessEntityDefinitionEditor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/VRRestAPIBEDefinitionSettings/Templates/VRRestAPIBEDefinitionsEditorTemplate.html"

        };

        function VRRestAPIBEDefinitionEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionSelectorAPI;
            var connectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorAPI = api;
                    connectionSelectorPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var connectionId;

                    if (payload != undefined && payload.data != undefined) {
                        var beDefinitionSettings = payload.businessEntityDefinitionSettings;
                        connectionId = beDefinitionSettings.ConnectionId;
                    }

                    //Loading ConnectionId selector
                    var connectionSelectorLoadPromise = getConnectionSelectorLoadPromise();
                    promises.push(connectionSelectorLoadPromise);

                    function getConnectionSelectorLoadPromise() {
                        var connectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        connectionSelectorPromiseDeferred.promise.then(function () {

                            var connectionSelectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Common.Business.VRInterAppRestConnectionFilter, Vanrise.Common.Business"
                                    }]
                                }
                            };
                            if (connectionId != undefined) {
                                connectionSelectorPayload.selectedIds = connectionId;
                            };
                            VRUIUtilsService.callDirectiveLoad(connectionSelectorAPI, connectionSelectorPayload, connectionSelectorLoadDeferred);
                        });

                        return connectionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Entities",
                        ConnectionId: connectionSelectorAPI.getSelectedIds(),
                        RemoteBEDefinitionId: undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);