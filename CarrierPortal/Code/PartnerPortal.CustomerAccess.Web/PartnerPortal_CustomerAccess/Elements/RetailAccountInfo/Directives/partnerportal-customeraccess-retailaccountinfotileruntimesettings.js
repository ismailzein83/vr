"use strict";
app.directive("partnerportalCustomeraccessRetailaccountinfotileruntimesettings", ["UtilsService", "VRUIUtilsService", "PartnerPortal_CustomerAccess_RetailAccountInfoAPIService",
    function (UtilsService, VRUIUtilsService, PartnerPortal_CustomerAccess_RetailAccountInfoAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "=",
                title: '=',
                index:'='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailAccountInfoTileRuntimeSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_CustomerAccess/Elements/RetailAccountInfo/Directives/Templates/RetailAccountInfoTileRuntimeSettings.html"
        };
        function RetailAccountInfoTileRuntimeSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
         
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.tileTitle = ctrl.title;
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var definitionSettings;
                    if (payload != undefined)
                    {
                        definitionSettings = payload.definitionSettings;
                    }
                    if(definitionSettings != undefined)
                    {
                        promises.push(loadFilteredDIDs());
                    }
                    function loadFilteredDIDs()
                    {
                        return PartnerPortal_CustomerAccess_RetailAccountInfoAPIService.GetClientProfileAccountInfo(definitionSettings.VRConnectionId).then(function (response) {
                            if (response != undefined)
                            {
                                $scope.scopeModel.accountInfo = response;
                            }
                        });
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }
]);