"use strict";

app.directive("retailBeAccountPortalaccountGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_BE_PortalAccountService", "Retail_BE_PortalAccountAPIService", "VR_Sec_UserActivationStatusEnum",
function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_PortalAccountService, Retail_BE_PortalAccountAPIService, VR_Sec_UserActivationStatusEnum) {

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
        var name;
        var email;
        var primaryAccountName;
        var primaryAccountEmail;
        var context;
        var isPrimaryPortalAccount;
        var accountViewDefinitionId;

        function initializeController() {
            $scope.portalAccounts = [];
            $scope.addNewPortalAccount = function () {
                if ($scope.portalAccounts.length != 0)
                    isPrimaryPortalAccount = false;
                var onPortalAccountAdded = function (portalAccount) {
                    $scope.portalAccounts.push(portalAccount);
                    gridAPI.itemAdded(portalAccount);
                };

                Retail_BE_PortalAccountService.addPortalAccount(onPortalAccountAdded, accountBEDefinitionId, parentAccountId, accountViewDefinition, getContext());
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

                            context = payload.context;
                            accountViewDefinition = payload.accountViewDefinition;
                            accountBEDefinitionId = payload.accountBEDefinitionId;
                            parentAccountId = payload.parentAccountId;
                            name = payload.name;
                            email = payload.email;

                            if (payload.accountViewDefinition != undefined)
                                accountViewDefinitionId = payload.accountViewDefinition.AccountViewDefinitionId

                            return Retail_BE_PortalAccountAPIService.GetPortalAccountDetails(accountBEDefinitionId, parentAccountId, accountViewDefinitionId).then(function (response) {
                                $scope.portalAccounts = response;

                                if ($scope.portalAccounts.length == 0)
                                    isPrimaryPortalAccount = true;
                                else
                                    isPrimaryPortalAccount = false;

                            });
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
            $scope.gridMenuActions = function (dataItem) {
                var menuActions = [];
                menuActions.push({

                    name: "Reset Password",
                    clicked: resetAdditionalPassword,
                    haspermission: function () {
                        return Retail_BE_PortalAccountAPIService.DosesUserHaveResetPasswordAccess(accountBEDefinitionId, accountViewDefinition.AccountViewDefinitionId);
                    },

                });
                menuActions.push({
                    name: "Edit",
                    clicked: editPortalAccount
                });
                if (dataItem.UserStatus == VR_Sec_UserActivationStatusEnum.Active.value) {
                    menuActions.push({
                        name: "Disable",
                        clicked: disablePortalAccount
                    });
                }
                if (dataItem.UserStatus == VR_Sec_UserActivationStatusEnum.Inactive.value) {
                    menuActions.push({
                        name: "Enable",
                        clicked: enablePortalAccount
                    });
                }
                if (dataItem.UserStatus == VR_Sec_UserActivationStatusEnum.Locked.value) {
                    menuActions.push({
                        name: "Unlock",
                        clicked: unlockPortalAccount
                    });
                }
                return menuActions;
            }
        }

        function unlockPortalAccount(dataItem) {
            $scope.isLoading = true;
            var userId;
            if (dataItem != undefined)
                userId = dataItem.UserId;
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    Retail_BE_PortalAccountAPIService.UnlockPortalAccount(accountBEDefinitionId, accountViewDefinitionId, parentAccountId, userId).then(function (response) {
                        if (VRNotificationService.notifyOnItemUpdated("Portal Account", response, "Email")) {
                            gridAPI.itemUpdated(response.UpdatedObject);
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.isLoading = false;
                    });
                }
                else $scope.isLoading = false;
            });
        }
        function resetAdditionalPassword(dataItem) {
            var userId;
            if (dataItem != undefined)
                userId = dataItem.UserId;
            Retail_BE_PortalAccountService.resetPassword(accountBEDefinitionId, parentAccountId, getContext(), userId);
        }
        function editPortalAccount(portalAccountObj) {
            var userId;
            if (portalAccountObj != undefined)
                userId = portalAccountObj.UserId;
            var onPortalAccountUpdated = function (portalAccount) {
                var index = $scope.portalAccounts.indexOf(portalAccountObj);
                $scope.portalAccounts[index] = portalAccount;
                gridAPI.itemUpdated(portalAccount);
            };
            Retail_BE_PortalAccountService.editPortalAccount(accountBEDefinitionId, parentAccountId, accountViewDefinitionId, userId, getContext(), onPortalAccountUpdated, $scope.portalAccounts);
        }
        function enablePortalAccount(dataItem) {
            $scope.isLoading = true;
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    return Retail_BE_PortalAccountAPIService.EnablePortalAccount(accountBEDefinitionId, accountViewDefinitionId, parentAccountId, dataItem.UserId).then(function (response) {
                        if (VRNotificationService.notifyOnItemUpdated("Portal Account", response, "Email")) {
                            gridAPI.itemUpdated(response.UpdatedObject);
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.isLoading = false;
                    });
                }
                else
                    $scope.isLoading = false;
            });
        }
        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            currentContext.isPrimaryAccount = function () {
                return isPrimaryPortalAccount;
            };
            return currentContext;
        }
        function disablePortalAccount(dataItem) {
            $scope.isLoading = true;
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    return Retail_BE_PortalAccountAPIService.DisablePortalAccount(accountBEDefinitionId, accountViewDefinitionId, parentAccountId, dataItem.UserId).then(function (response) {
                        if (VRNotificationService.notifyOnItemUpdated("Portal Account", response, "Email")) {
                            gridAPI.itemUpdated(response.UpdatedObject);
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.isLoading =false;
                    });
                }
                else 
                    $scope.isLoading = false;
            });
        }
    }
    return directiveDefinitionObject;
}]);