"use strict";
app.directive("cpMultinetAccountadditionalinfotiledefinitionsettings", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountAdditionalInfoTileDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/CP_MultiNet/Elements/AccountAdditionalInfo/Directives/Templates/AccountAdditionalInfoTileDefinitionSettings.html"
        };
        function AccountAdditionalInfoTileDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionSelectorApi;
            var connectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorApi = api;
                    connectionSelectorPromiseDeferred.resolve();
                };
            
                UtilsService.waitMultiplePromises([connectionSelectorPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var tileExtendedSettings;
                    if (payload != undefined) {
                        tileExtendedSettings = payload.tileExtendedSettings;
                    }

                    function loadConnectionSelector()
                    {
                        var payloadConnectionSelector = {
                            filter: { Filters: [] }
                        };
                        payloadConnectionSelector.filter.Filters.push({
                            $type: "Vanrise.Common.Business.VRInterAppRestConnectionFilter, Vanrise.Common.Business"
                        });
                        if (tileExtendedSettings != undefined) {
                            payloadConnectionSelector.selectedIds = tileExtendedSettings.VRConnectionId;
                        };
                        return connectionSelectorApi.load(payloadConnectionSelector);

                    }
                 
                    promises.push(loadConnectionSelector());


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "CP.MultiNet.Business.AccountAdditionalInfoTileDefinitionSettings, CP.MultiNet.Business",
                        VRConnectionId: connectionSelectorApi.getSelectedIds(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }
]);