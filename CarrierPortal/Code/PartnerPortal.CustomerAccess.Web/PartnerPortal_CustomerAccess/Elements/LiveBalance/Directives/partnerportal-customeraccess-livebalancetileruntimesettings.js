"use strict";
app.directive("partnerportalCustomeraccessLivebalancetileruntimesettings", ["UtilsService", "VRUIUtilsService", "PartnerPortal_CustomerAccess_LiveBalanceAPIService",
    function (UtilsService, VRUIUtilsService, PartnerPortal_CustomerAccess_LiveBalanceAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "=",
                title:'='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new LiveBalanceTileRuntimeSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_CustomerAccess/Elements/LiveBalance/Directives/Templates/LiveBalanceTileRuntimeSettings.html"
        };
        function LiveBalanceTileRuntimeSettingsCtor($scope, ctrl, $attrs) {
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
                        promises.push(loadLiveBalance());
                    }
                    function loadLiveBalance()
                    {
                        return PartnerPortal_CustomerAccess_LiveBalanceAPIService.GetCurrentAccountBalance(definitionSettings.VRConnectionId, definitionSettings.AccountTypeId).then(function (response) {
                            $scope.scopeModel.currentBalance = response.CurrentBalance;
                            $scope.scopeModel.currencyDescription = response.CurrencyDescription;
                            $scope.scopeModel.balanceFlagDescription = response.BalanceFlagDescription;
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