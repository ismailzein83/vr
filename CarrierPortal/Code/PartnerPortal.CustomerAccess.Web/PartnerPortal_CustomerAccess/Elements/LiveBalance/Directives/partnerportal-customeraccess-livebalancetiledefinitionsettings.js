"use strict";
app.directive("partnerportalCustomeraccessLivebalancetiledefinitionsettings", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new LiveBalanceTileDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_CustomerAccess/Elements/LiveBalance/Directives/Templates/LiveBalanceTileDefinitionSettings.html"
        };
        function LiveBalanceTileDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionSelectorApi;
            var connectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
           
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorApi = api;
                    connectionSelectorPromiseDeferred.resolve();
                };
              
                UtilsService.waitMultiplePromises([connectionSelectorPromiseDeferred.promise]).then(function(){
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var tileExtendedSettings;
                    var invoiceViewerTypeIds;
                    if (payload != undefined) {
                        tileExtendedSettings = payload.tileExtendedSettings;
                        if (tileExtendedSettings != undefined)
                            $scope.scopeModel.accountTypeId = tileExtendedSettings.AccountTypeId;
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
                        $type: "PartnerPortal.CustomerAccess.Business.LiveBalanceTileDefinitionSettings, PartnerPortal.CustomerAccess.Business",
                        AccountTypeId: $scope.scopeModel.accountTypeId,
                        VRConnectionId: connectionSelectorApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }
]);