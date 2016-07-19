"use strict";

app.directive("retailBeAccountserviceGrid", ["UtilsService", "VRNotificationService", "Retail_BE_AccountServiceAPIService",
    "Retail_BE_AccountServiceService", "VRUIUtilsService","Retail_BE_ActionRuntimeService",
function (UtilsService, VRNotificationService, Retail_BE_AccountServiceAPIService, Retail_BE_AccountServiceService, VRUIUtilsService, Retail_BE_ActionRuntimeService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var accountServiceGrid = new AccountServiceGrid($scope, ctrl, $attrs);
            accountServiceGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountService/Templates/AccountServiceGridTemplate.html"

    };

    function AccountServiceGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.accountServices = [];

            $scope.scopeModel.getStatusColor = function (dataItem) {
                return dataItem.StatusColor;
            }

            defineMenuActions();

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onAccountServiceAdded = function (accountServiceObject) {
                        gridAPI.itemAdded(accountServiceObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_AccountServiceAPIService.GetFilteredAccountServices(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }

        function defineMenuActions() {

            $scope.scopeModel.gridMenuActions = function (dataItem) {
                var menuActions = [{
                    name: "Edit",
                    clicked: editAccountService,
                    haspermission: hasUpdateAccountServicePermission
                }];
                if (dataItem.ActionDefinitions != undefined) {
                    for (var i = 0; i < dataItem.ActionDefinitions.length; i++) {
                        var actionDefinition = dataItem.ActionDefinitions[i];
                        menuActions.push(defineActionDefinitionMenuAction(actionDefinition));
                    }
                }
                return menuActions;
            };




            function defineActionDefinitionMenuAction(actionDefinition) {
                return {
                    name: actionDefinition.Name,
                    clicked: function (dataItem) {
                        return Retail_BE_ActionRuntimeService.openActionRuntime(dataItem.Entity.AccountServiceId, actionDefinition.ActionDefinitionId,
                            function () {
                                $scope.scopeModel.isloadingGrid = true;
                                return Retail_BE_AccountServiceAPIService.GetAccountServiceDetail(dataItem.Entity.AccountServiceId).then(function (response) {
                                    for (var i = 0; i < $scope.scopeModel.accountServices.length; i++) {
                                        var account = $scope.scopeModel.accountServices[i];
                                        if (account.Entity.AccountServiceId == response.Entity.AccountServiceId) {
                                            $scope.scopeModel.accountServices[i] = response;
                                            break;
                                        }
                                    }
                                    $scope.scopeModel.isloadingGrid = false;
                                });
                            });
                    }
                };
            }
        }

        function hasUpdateAccountServicePermission() {
            return Retail_BE_AccountServiceAPIService.HasUpdateAccountServicePermission();
        }

        function editAccountService(accountServiceObj) {
            var onAccountServiceUpdated = function (accountServiceObject) {
                gridAPI.itemUpdated(accountServiceObject);

            }
            Retail_BE_AccountServiceService.editAccountService(accountServiceObj.Entity.AccountServiceId, onAccountServiceUpdated);
        }

    }

    return directiveDefinitionObject;

}]);