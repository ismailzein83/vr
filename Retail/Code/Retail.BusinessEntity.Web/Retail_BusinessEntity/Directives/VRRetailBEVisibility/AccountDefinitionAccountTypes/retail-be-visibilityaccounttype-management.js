(function (app) {

    'use strict';

    VisibilityAccountTypeManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_VisibilityAccountDefinitionService'];

    function VisibilityAccountTypeManagementDirective(UtilsService, VRNotificationService, Retail_BE_VisibilityAccountDefinitionService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityAccountTypesCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionAccountTypes/Templates/VisibilityAccountTypeManagementTemplate.html'
        };

        function VisibilityAccountTypesCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.accountTypes = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddAccountType = function () {
                    var onAccountTypeAdded = function (addedAccountType) {
                        $scope.scopeModel.accountTypes.push({ Entity: addedAccountType });
                    };

                    Retail_BE_VisibilityAccountDefinitionService.addVisibilityAccountType(accountBEDefinitionId, onAccountTypeAdded);
                };
                $scope.scopeModel.onDeleteAccountType = function (accountType) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.accountTypes, accountType.Entity.AccountTypeTitle, 'Entity.AccountTypeTitle');
                            $scope.scopeModel.accountTypes.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var accountTypes;
                    var accountTypeTitlesById;

                    if (payload != undefined) {
                        accountTypes = payload.accountTypes;
                        accountTypeTitlesById = payload.accountTypeTitlesById;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                    }

                    //Loading AccountTypes Grid
                    if (accountTypes != undefined) {
                        for (var index = 0 ; index < accountTypes.length; index++) {
                            if (index != "$type") {
                                var accountType = accountTypes[index];
                                extendAccountTypeObj(accountType);
                                $scope.scopeModel.accountTypes.push({ Entity: accountType });
                            }
                        }
                    }

                    function extendAccountTypeObj(accountType) {
                        if (accountTypeTitlesById == undefined || accountType.AccountTypeTitle != undefined)
                            return;

                        accountType.AccountTypeTitle = accountTypeTitlesById[accountType.AccountTypeId];
                    }
                };

                api.getData = function () {

                    var accountTypes;
                    if ($scope.scopeModel.accountTypes.length > 0) {
                        accountTypes = [];
                        for (var i = 0; i < $scope.scopeModel.accountTypes.length; i++) {
                            var accountType = $scope.scopeModel.accountTypes[i].Entity;
                            accountTypes.push(accountType);
                        }
                    }

                    return accountTypes;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editAccountType
                }];
            }
            function editAccountType(accountType) {
                var onAccountTypeUpdated = function (updatedAccountType) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.accountTypes, accountType.Entity.AccountTypeTitle, 'Entity.AccountTypeTitle');
                    $scope.scopeModel.accountTypes[index] = { Entity: updatedAccountType };
                };

                Retail_BE_VisibilityAccountDefinitionService.editVisibilityAccountType(accountType.Entity, accountBEDefinitionId, onAccountTypeUpdated);
            }


        }
    }

    app.directive('retailBeVisibilityaccounttypeManagement', VisibilityAccountTypeManagementDirective);

})(app);