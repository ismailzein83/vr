"use strict";

app.directive("vrGenericdataGenericbusinessentityVieweditor", ["UtilsService", "VRNotificationService","VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBusinessEntityViewEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericBusinessEntityViewEditor.html"
        };
        function GenericBusinessEntityViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
              
                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    promises.push(loadBusinessEntityDefinitionSelector());

                    function loadBusinessEntityDefinitionSelector() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                selectedIds: payload != undefined ? getGenericBEDefinitionIdsFromSettings(payload.Settings) : undefined,
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.GenericData.Business.GenericBusinessEntityDefinitionFilter, Vanrise.GenericData.Business"
                                    }]
                                }
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
                        });
                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Business.GenericBEViewSettings, Vanrise.GenericData.Business",
                        Settings: buildGenericBEDefinitionViewSettings()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function buildGenericBEDefinitionViewSettings() {
                var settings = [];
                var selectedGenericBEDefinitions = beDefinitionSelectorApi.getSelectedIds();
                if (selectedGenericBEDefinitions != undefined) {
                    for (var i = 0 ; i < selectedGenericBEDefinitions.length; i++) {
                        settings.push({ BusinessEntityDefinitionId: selectedGenericBEDefinitions[i] });
                    }
                }
                return settings;
            }
            function getGenericBEDefinitionIdsFromSettings(items) {
                var settings = [];
                for (var i = 0 ; i < items.length; i++) {
                    settings.push(items[i].BusinessEntityDefinitionId);
                }
                return settings;
            }
        }

        return directiveDefinitionObject;
    }
]);