"use strict";

app.directive("retailBeAccountServiceGrid", ["UtilsService", "VRNotificationService", "Retail_BE_AccountServiceAPIService",
    "Retail_BE_AccountServiceService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Retail_BE_AccountServiceAPIService, Retail_BE_AccountServiceService, VRUIUtilsService) {

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

            $scope.accountServices = [];
            defineMenuActions();
            $scope.onGridReady = function (api) {
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
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
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
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editAccountService,
                haspermission: hasUpdateAccountServicePermission
            }];
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