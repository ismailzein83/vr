"use strict";

app.directive("vrWhsBePortalaccountGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "WhS_BE_PortalAccountAPIService","WhS_BE_PortalAccountService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_PortalAccountAPIService, WhS_BE_PortalAccountService) {

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
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
            
        }
        return directiveDefinitionObject;
    }]);