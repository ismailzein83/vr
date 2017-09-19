"use strict";
app.directive("partnerportalCustomeraccessRetailsubaccountsinfotileruntimesettings", ["UtilsService", "VRUIUtilsService", "PartnerPortal_CustomerAccess_RetailAccountInfoAPIService",
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
            templateUrl: "/Client/Modules/PartnerPortal_CustomerAccess/Elements/RetailAccountInfo/Directives/Templates/RetailSubAccountsInfoTileRuntimeSettings.html"
        };
        function RetailAccountInfoTileRuntimeSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var gridPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.tileTitle = ctrl.title;
                $scope.scopeModel.onSubAccountsGridReady = function (api) {
                    gridAPI = api;
                    gridPromiseDeferred.resolve();
                };
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
                        loadGrid();
                    }
                    function loadGrid()
                    {
                        gridPromiseDeferred.promise.then(function () {
                            gridAPI.load(getGridQuery());
                        });
                    }
                    function getGridQuery() {
                        return {
                            accountGridFields: definitionSettings != undefined ? definitionSettings.AccountGridFields : undefined,
                            vrConnectionId: definitionSettings != undefined ? definitionSettings.VRConnectionId : undefined,
                        };
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