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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/VRRestAPIBusinessEntityDefinition/Templates/VRRestAPIBEDefinitionsEditorTemplate.html"

        };

        function VRRestAPIBEDefinitionEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionId;
            var remoteBEDefinitionId;

            var connectionSelectorAPI;
            var connectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var connectionSelectionChangedPromiseDeferred;

            var beDefinitionRemoteSelectorAPI;
            var beDefinitionRemoteSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.showBEDefinitionRemoteSelector = false;

                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorAPI = api;
                    connectionSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onBEDefinitionRemoteSelectorReady = function (api) {
                    beDefinitionRemoteSelectorAPI = api;
                    beDefinitionRemoteSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onConnectionSelectionChanged = function (selectedItem) {
                    if (selectedItem != null) {
                        connectionId = selectedItem.VRConnectionId;

                        if (connectionSelectionChangedPromiseDeferred != undefined) {
                            connectionSelectionChangedPromiseDeferred.resolve();
                        }
                        else {

                            loadBEDefinitionRemoteSelector();

                            function loadBEDefinitionRemoteSelector() {
                                var beDefinitionRemoteSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                                beDefinitionRemoteSelectorReadyPromiseDeferred.promise.then(function () {

                                    var beDefinitionRemoteSelectorPayload = {
                                        connectionId: connectionId
                                    };
                                    VRUIUtilsService.callDirectiveLoad(beDefinitionRemoteSelectorAPI, beDefinitionRemoteSelectorPayload, beDefinitionRemoteSelectorLoadDeferred);
                                });

                                return beDefinitionRemoteSelectorLoadDeferred.promise.then(function () {
                                    $scope.scopeModel.showBEDefinitionRemoteSelector = true;
                                });
                            }
                        }
                    }
                }

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var singularTitle;
                    var pluralTitle;

                    if (payload != undefined && payload.businessEntityDefinitionSettings != undefined) {
                        var beDefinitionSettings = payload.businessEntityDefinitionSettings;
                        singularTitle = beDefinitionSettings.SingularTitle;
                        pluralTitle = beDefinitionSettings.PluralTitle;

                        if (beDefinitionSettings != undefined) {
                            connectionId = beDefinitionSettings.ConnectionId;
                            remoteBEDefinitionId = beDefinitionSettings.RemoteBEDefinitionId;
                        }
                    }
                     
                    //Loading ConnectionId Selector
                    var connectionSelectorLoadPromise = getConnectionSelectorLoadPromise();
                    promises.push(connectionSelectorLoadPromise);

                    //Loading BEDefinition RemoteSelector
                    var beDefinitionRemoteSelectorLoadPromise = getBEDefinitionRemoteSelectorLoadPromise();
                    promises.push(beDefinitionRemoteSelectorLoadPromise);

                    //Loading Singular and Plural Title
                    $scope.scopeModel.singularTitle = singularTitle;
                    $scope.scopeModel.pluralTitle = pluralTitle;

                    function getConnectionSelectorLoadPromise() {
                        if (connectionId != undefined)
                            connectionSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();

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
                    function getBEDefinitionRemoteSelectorLoadPromise() {
                        var beDefinitionRemoteSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        if (connectionId == undefined) {
                            beDefinitionRemoteSelectorLoadDeferred.resolve;
                            return beDefinitionRemoteSelectorLoadDeferred.promise;
                        }

                        UtilsService.waitMultiplePromises([connectionSelectionChangedPromiseDeferred.promise, beDefinitionRemoteSelectorReadyPromiseDeferred.promise]).then(function () {
                            connectionSelectionChangedPromiseDeferred = undefined;

                            var beDefinitionRemoteSelectorPayload = {
                                connectionId: connectionId
                            };
                            if (remoteBEDefinitionId != undefined) {
                                beDefinitionRemoteSelectorPayload.selectedIds = remoteBEDefinitionId;
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionRemoteSelectorAPI, beDefinitionRemoteSelectorPayload, beDefinitionRemoteSelectorLoadDeferred);
                        });

                        return beDefinitionRemoteSelectorLoadDeferred.promise.then(function () {
                            $scope.scopeModel.showBEDefinitionRemoteSelector = true;
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.GenericData.Entities.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Entities",
                        ConnectionId: connectionSelectorAPI.getSelectedIds(),
                        RemoteBEDefinitionId: beDefinitionRemoteSelectorAPI.getSelectedIds(),
                        SingularTitle: $scope.scopeModel.singularTitle,
                        PluralTitle: $scope.scopeModel.pluralTitle
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);