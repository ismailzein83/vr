"use strict";

app.directive("retailBeAccountPortalaccountGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_BE_PortalAccountService","Retail_BE_PortalAccountAPIService",
function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_PortalAccountService, Retail_BE_PortalAccountAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var portalaccountGrid = new PortalaccountGrid($scope, ctrl, $attrs);
            portalaccountGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Account/MainExtensions/Templates/PortalAccountGridTemplate.html"
    };

    function PortalaccountGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var gridAPI;
        var accountViewDefinition;
        var accountBEDefinitionId;
        var parentAccountId;
        var userId;

        function initializeController() {
            $scope.portalAccounts = [];
            $scope.addNewPortalAccount = function () {
                var onPortalAccountAdded = function (portalAccount) {
                    $scope.portalAccounts.push(portalAccount);
                    gridAPI.itemAdded(portalAccount);
                };
                Retail_BE_PortalAccountService.addAddintionalPortalAccount(onPortalAccountAdded, accountBEDefinitionId, parentAccountId, accountViewDefinition);
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.load = function (payload) {
                        if (payload != undefined) {
                            accountViewDefinition = payload.accountViewDefinition;
                            accountBEDefinitionId = payload.accountBEDefinitionId;
                            parentAccountId = payload.parentAccountId;
                            if (payload.portalAccounts != undefined) {
                                for (var i = 0; i < payload.portalAccounts.length; i++) {
                                    var portalAccount = payload.portalAccounts[i];
                                    $scope.portalAccounts.push(portalAccount);
                                }
                            }
                        }
                    };
                    directiveAPI.getData = function () {
                        var portalAccounts = [];
                        if ($scope.portalAccounts != undefined) {
                            for (var i = 0; i < $scope.portalAccounts.length; i++) {
                                var portalAccount = $scope.portalAccounts[i];
                                portalAccounts.push(portalAccount);
                            }
                        }
                        return portalAccounts;
                    };
                    return directiveAPI;
                };

            };
            defineMenuActions();
        }
        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Reset Password",
                clicked: resetAdditionalPassword,
                haspermission: function () {
                    return Retail_BE_PortalAccountAPIService.DosesUserHaveResetPasswordAccess(accountBEDefinitionId, accountViewDefinition.AccountViewDefinitionId);
                }
            }];
        }
        function resetAdditionalPassword(dataItem) {
            var userId;
            if (dataItem.Entity != undefined)
                userId = dataItem.Entity.UserId;
            Retail_BE_PortalAccountService.resetPassword(accountBEDefinitionId, parentAccountId, undefined, userId, accountViewDefinition.AccountViewDefinitionId);
        }
    }
    return directiveDefinitionObject;
}]);