'use strict';

app.directive('retailBeVisibilityaccounttypeManagement', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityAccountType(ctrl, $scope);
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
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionAccountTypes/Templates/VisibilityAccountTypeManagementTemplate.html';
            }
        };

        function VisibilityAccountType(ctrl, $scope) {
            this.initializeController = initializeController;

            var accountTypeSelectorAPI;
            var accountTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.accountTypeDefinitions = [];
                $scope.scopeModel.accountTypes = [];

                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSelectAccountType = function (selectedItem) {

                    $scope.scopeModel.accountTypes.push({
                        AccountTypeId: selectedItem.AccountTypeId,
                        Name: selectedItem.Title
                    });
                };
                $scope.scopeModel.onDeselectAccountType = function (deselectedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.accountTypes, deselectedItem.AccountTypeId, 'AccountTypeId');
                    $scope.scopeModel.accountTypes.splice(index, 1);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedAccountTypes, deletedItem.AccountTypeId, 'AccountTypeId');
                    $scope.scopeModel.selectedAccountTypes.splice(index, 1);
                    $scope.scopeModel.onDeselectAccountType(deletedItem);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountTypes;
                    var accountBEDefinitionId;

                    console.log(payload);

                    if (payload != undefined) {
                        accountTypes = payload.accountTypes;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                    }

                    var loadAccountTypeSelectorPromise = getAccountTypeSelectorLoadPromise();
                    promises.push(loadAccountTypeSelectorPromise);

                    loadAccountTypeSelectorPromise.then(function () {

                        //Loading Grid
                        if ($scope.scopeModel.selectedAccountTypes != undefined) {
                            for (var i = 0; i < $scope.scopeModel.selectedAccountTypes.length; i++) {
                                var accountTypeDefinition = $scope.scopeModel.selectedAccountTypes[i];

                                $scope.scopeModel.accountTypes.push({
                                    AccountTypeId: accountTypeDefinition.AccountTypeId,
                                    Name: accountTypeDefinition.Title
                                });
                            }
                        }
                    });

                    function getAccountTypeSelectorLoadPromise() {
                        var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        accountTypeSelectorPromiseDeferred.promise.then(function () {

                            var selectorPayload = {
                                filter: {
                                    IncludeHiddenAccountTypes: true,
                                    AccountBEDefinitionId: accountBEDefinitionId
                                },
                                selectedIds: []
                            };
                            if (accountTypes != undefined) {
                                for (var index = 0; index < accountTypes.length; index++) {
                                    selectorPayload.selectedIds.push(accountTypes[index].AccountTypeId);
                                }
                            }

                            VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, selectorPayload, accountTypeSelectorLoadDeferred);
                        });

                        return accountTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var _accountTypes;
                    if ($scope.scopeModel.accountTypes.length > 0) {
                        _accountTypes = [];
                        for (var i = 0; i < $scope.scopeModel.accountTypes.length; i++) {
                            var currentAccountType = $scope.scopeModel.accountTypes[i];
                            _accountTypes.push({
                                AccountTypeId: currentAccountType.AccountTypeId
                            });
                        }
                    }
                    return _accountTypes
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);



