"use strict";

app.directive("vrWhsBePortalaccountGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "WhS_BE_PortalAccountAPIService", "WhS_BE_PortalAccountService","VR_Sec_UserActivationStatusEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_PortalAccountAPIService, WhS_BE_PortalAccountService, VR_Sec_UserActivationStatusEnum) {

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
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PortalAccount/Templates/PortalAccountGridTemplate.html"
        };

        function PortalaccountGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            var context; 
            var carrierProfileId;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.portalAccounts = [];

                $scope.scopeModel.addNewPortalAccount = function () {
                    var onPortalAccountAdded = function (portalAccount) {
                        $scope.scopeModel.portalAccounts.push(portalAccount);
                        gridAPI.itemAdded(portalAccount);
                    };

                    WhS_BE_PortalAccountService.addPortalAccount(onPortalAccountAdded, carrierProfileId, getContext());
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
                                carrierProfileId = payload.ObjectId;
                                return WhS_BE_PortalAccountAPIService.GetCarrierProfilePortalAccounts(carrierProfileId).then(function (response) {
                                    if (response != undefined)
                                        $scope.scopeModel.portalAccounts = response;
                                });
                            }
                        };
                        directiveAPI.getData = function () {
                            var portalAccounts = [];
                            if ($scope.scopeModel.portalAccounts != undefined) {
                                for (var i = 0; i < $scope.scopeModel.portalAccounts.length; i++) {
                                    var portalAccount = $scope.scopeModel.portalAccounts[i];
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
                    var menuActions = [{
                            name: "Edit",
                            clicked: editPortalAccount
                        },
                        {
                            name: "Reset Password",
                            clicked: resetAdditionalPassword
                        }
                    ];
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
                };
            }

            function resetAdditionalPassword(dataItem) {
                var userId;
                if (dataItem != undefined)
                    userId = dataItem.UserId;
                WhS_BE_PortalAccountService.resetPassword(carrierProfileId, getContext(), userId);
            }
            function editPortalAccount(portalAccountObj) {
                var userId;
                if (portalAccountObj != undefined)
                    userId = portalAccountObj.UserId;
                var onPortalAccountUpdated = function (portalAccount) {
                    var index = $scope.scopeModel.portalAccounts.indexOf(portalAccountObj);
                    $scope.scopeModel.portalAccounts[index] = portalAccount;
                    gridAPI.itemUpdated(portalAccount);
                };
                WhS_BE_PortalAccountService.editPortalAccount(carrierProfileId, userId, getContext(), onPortalAccountUpdated);
            }

            function disablePortalAccount(dataItem) {
                $scope.isLoading = true;
                var userId;
                if (dataItem != undefined)
                    userId = dataItem.UserId;
                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return WhS_BE_PortalAccountAPIService.DisablePortalAccount(carrierProfileId, userId).then(function (response) {
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
            function enablePortalAccount(dataItem) {
                $scope.isLoading = true;
                var userId;
                if (dataItem != undefined)
                    userId = dataItem.UserId;
                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return WhS_BE_PortalAccountAPIService.EnablePortalAccount(carrierProfileId, userId).then(function (response) {
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
            function unlockPortalAccount(dataItem) {
                $scope.isLoading = true;
                var userId;
                if (dataItem != undefined)
                    userId = dataItem.UserId;
                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        WhS_BE_PortalAccountAPIService.UnlockPortalAccount(carrierProfileId, userId).then(function (response) {
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
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
            
        }
        return directiveDefinitionObject;
    }]);