'use strict';

app.directive('vrAccountbalanceUpdateaccountbalancesettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new UpdateAccountBalanceSettingsCtor(ctrl, $scope);
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
                return "/Client/Modules/VR_AccountBalance/Directives/UpdateAccountBalance/Templates/UpdateAccountBalanceSettingsTemplate.html"
            }
        };

        function UpdateAccountBalanceSettingsCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var balanceAccountTypeSelectorAPI;
            var balanceAccountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.updateAccountBalanceTypeCombinations = [];

                $scope.scopeModel.onBalanceAccountTypeSelectorReady = function (api) {
                    balanceAccountTypeSelectorAPI = api;
                    balanceAccountTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onSelectBalanceAccountType = function (selectedItem) {
                    if (selectedItem == undefined) 
                        return;

                    $scope.scopeModel.isGridLoading = true;

                    var updateAccountBalanceTypeCombination = {
                        BalanceAccountTypeName: selectedItem.Name,
                        BalanceAccountTypeId: selectedItem.Id
                    };
                    extendAndLoadUpdateAccountBalanceTypeCombinationObject(updateAccountBalanceTypeCombination);
                    $scope.scopeModel.updateAccountBalanceTypeCombinations.push(updateAccountBalanceTypeCombination);
                    
                    updateAccountBalanceTypeCombination.billingTransactionTypeSelectorLoadDeferred.promise.then(function () {
                        $scope.scopeModel.isGridLoading = false;
                    });
                };
                $scope.scopeModel.onDeselectBalanceAccountType = function (deselectedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.updateAccountBalanceTypeCombinations, deselectedItem.Id, 'BalanceAccountTypeId');
                    $scope.scopeModel.updateAccountBalanceTypeCombinations.splice(index, 1);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedBalanceAccountTypes, deletedItem.BalanceAccountTypeId, 'Id');
                    $scope.scopeModel.selectedBalanceAccountTypes.splice(index, 1);

                    index = UtilsService.getItemIndexByVal($scope.scopeModel.updateAccountBalanceTypeCombinations, deletedItem.BalanceAccountTypeId, 'BalanceAccountTypeId');
                    $scope.scopeModel.updateAccountBalanceTypeCombinations.splice(index, 1);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var updateAccountBalanceTypeCombinations;

                    if (payload != undefined) {
                        var updateAccountBalanceSettings = payload.updateAccountBalanceSettings;

                        if (updateAccountBalanceSettings != undefined) {
                            updateAccountBalanceTypeCombinations = updateAccountBalanceSettings.UpdateAccountBalanceTypeCombinations;
                        }
                    }

                    var balanceAccountTypeSelectorLoadPromise = getBalanceAccountTypeSelectorLoadPromise();
                    promises.push(balanceAccountTypeSelectorLoadPromise);

                    balanceAccountTypeSelectorLoadPromise.then(function () {
                        getGridLoadPromise();
                    });

                    function getBalanceAccountTypeSelectorLoadPromise() {
                        var balanceAccountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        balanceAccountTypeSelectorReadyDeferred.promise.then(function () {

                            var payload;
                            if (updateAccountBalanceTypeCombinations != undefined) {
                                payload = {
                                    selectedIds: UtilsService.getPropValuesFromArray(updateAccountBalanceTypeCombinations, "BalanceAccountTypeId")
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(balanceAccountTypeSelectorAPI, payload, balanceAccountTypeSelectorLoadDeferred);
                        });

                        return balanceAccountTypeSelectorLoadDeferred.promise;
                    }

                    function getGridLoadPromise() {
                        var _promises = [];

                        $scope.scopeModel.isGridLoading = true;

                        if ($scope.scopeModel.selectedBalanceAccountTypes != undefined) {
                            for (var index = 0; index < $scope.scopeModel.selectedBalanceAccountTypes.length; index++) {
                                var currentBalanceAccountType = $scope.scopeModel.selectedBalanceAccountTypes[index];
                                var currentUpdateAccountBalanceTypeCombination = updateAccountBalanceTypeCombinations[index];

                                var updateAccountBalanceTypeCombination = {
                                    BalanceAccountTypeName: currentBalanceAccountType.Name,
                                    BalanceAccountTypeId: currentBalanceAccountType.Id,
                                    TransactionTypeIds: currentUpdateAccountBalanceTypeCombination.TransactionTypeIds
                                };
                                extendAndLoadUpdateAccountBalanceTypeCombinationObject(updateAccountBalanceTypeCombination);
                                $scope.scopeModel.updateAccountBalanceTypeCombinations.push(updateAccountBalanceTypeCombination);
                                _promises.push(updateAccountBalanceTypeCombination.billingTransactionTypeSelectorLoadDeferred.promise);
                            }
                        }

                        return UtilsService.waitMultiplePromises(_promises).then(function () {
                            $scope.scopeModel.isGridLoading = false;
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    function getUpdateAccountBalanceTypeCombinations() {
                        var combinations = [];

                        for (var i = 0; i < $scope.scopeModel.updateAccountBalanceTypeCombinations.length; i++) {
                            var currentItem = $scope.scopeModel.updateAccountBalanceTypeCombinations[i];

                            combinations.push({
                                BalanceAccountTypeId: currentItem.BalanceAccountTypeId,
                                TransactionTypeIds: currentItem.billingTransactionTypeSelectorAPI.getSelectedIds()
                            });
                        }

                        return combinations;
                    }

                    return {
                        UpdateAccountBalanceTypeCombinations: getUpdateAccountBalanceTypeCombinations()
                    }; 
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendAndLoadUpdateAccountBalanceTypeCombinationObject(updateAccountBalanceTypeCombination) {
                updateAccountBalanceTypeCombination.billingTransactionTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                updateAccountBalanceTypeCombination.onBillingTransactionTypeSelectorReady = function (api) {
                    updateAccountBalanceTypeCombination.billingTransactionTypeSelectorAPI = api;

                    var payload;
                    if (updateAccountBalanceTypeCombination.TransactionTypeIds != undefined) {
                        payload = {
                            selectedIds: updateAccountBalanceTypeCombination.TransactionTypeIds
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(updateAccountBalanceTypeCombination.billingTransactionTypeSelectorAPI, payload, updateAccountBalanceTypeCombination.billingTransactionTypeSelectorLoadDeferred);
                };
            }
        }

        return directiveDefinitionObject;
    }]);