'use strict';

app.directive('retailBeAccountManagement',['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'Retail_BE_AccountBEService', 'Retail_BE_AccountBEAPIService', 'Retail_BE_AccountTypeAPIService', 'VR_Sec_ViewAPIService','Retail_BE_AccountBEDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, Retail_BE_AccountBEService, Retail_BE_AccountBEAPIService, Retail_BE_AccountTypeAPIService, VR_Sec_ViewAPIService, Retail_BE_AccountBEDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountManagementController(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Account/Templates/AccountManagementTemplate.html"
        };
        function AccountManagementController(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;

            var viewId;
            var bulkActionId;
            var accountBEDefinitionId;
            var accountFields;
            var accountBEDefinitionSettings;
            var mailMessageTypeId;
            var bulkAction;

            var businessEntityDefinitionSelectorAPI;
            var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var accountRootTypeSelectorAPI;
            var accountRootTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var accountTypeSelectorAPI;
            var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var statusSelectorAPI;
            var statusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var runtimeDirectiveAPI;
            var runtimeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;
            var context;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isGridLoaded = false;
                $scope.scopeModel.isAccountBEDefinitionSelected = false;
                $scope.scopeModel.showBusinessEntityDefinitionSelector = false;
                $scope.scopeModel.showAddAccount = false;
                $scope.scopeModel.onlyRootAccount = true;

                $scope.scopeModel.showActionButtons = false;
                $scope.scopeModel.showMenuActions = false;

                $scope.scopeModel.onStatusSelectorReady = function (api) {
                    statusSelectorAPI = api;
                    statusSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    businessEntityDefinitionSelectorAPI = api;
                    businessEntityDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onAccountRootTypeSelectorReady = function (api) {
                    accountRootTypeSelectorAPI = api;
                    accountRootTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyDeferred.resolve();
                };
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var gridPayload = {
                        accountBEDefinitionId: accountBEDefinitionId,
                        query: {
                            OnlyRootAccount: $scope.scopeModel.onlyRootAccount,
                            StatusIds: statusSelectorAPI.getSelectedIds()
                        },
                        context: getGridContext(),
                        bulkActionId: bulkActionId
                    };
                    gridAPI.load(gridPayload);
                };

                $scope.scopeModel.onBusinessEntityDefinitionSelectionChanged = function (selectedBusinessEntityDefinition) {

                    if (selectedBusinessEntityDefinition != undefined) {
                        $scope.scopeModel.isLoading = true;
                        $scope.scopeModel.isGridLoaded = false;
                        $scope.scopeModel.isAccountBEDefinitionSelected = true;

                        accountBEDefinitionId = selectedBusinessEntityDefinition.BusinessEntityDefinitionId;

                        Retail_BE_AccountBEAPIService.DoesUserHaveAddAccess(accountBEDefinitionId).then(function (response) {
                            $scope.scopeModel.showAddAccount = response;
                        });
                        loadAllControls().then(function () {
                            $scope.scopeModel.isGridLoaded = true;
                        });
                    }
                };
                $scope.scopeModel.onlyRootAccountValueChanged = function () {
                    if (accountRootTypeSelectorAPI.getSelectedIds() != undefined) {
                        var payload = {
                            filter: {
                                AccountBEDefinitionId: accountBEDefinitionId,
                                RootAccountTypeOnly: accountRootTypeSelectorAPI.getSelectedIds()
                            }
                        };
                        var setLoader = function (value) {
                            $scope.scopeModel.isAccountTypeSelectorLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountTypeSelectorAPI, payload, setLoader, undefined);
                    }
                };

                $scope.scopeModel.onRuntimeDirectiveReady = function (api) {
                    runtimeDirectiveAPI = api;
                    runtimeDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.search = function () {

                    var gridPayload = {
                        accountBEDefinitionId: accountBEDefinitionId,
                        query: buildGridQuery(),
                        context: getGridContext(),
                        bulkActionId: bulkActionId
                    };
                    return gridAPI.load(gridPayload);
                };
                $scope.scopeModel.add = function () {
                    var onAccountAdded = function (addedAccount) {
                        gridAPI.onAccountAdded(addedAccount);
                    };

                    Retail_BE_AccountBEService.addAccount(accountBEDefinitionId, undefined, onAccountAdded);
                };

                //$scope.scopeModel.hasAddAccountPermission = function () {
                //    return Retail_BE_AccountBEAPIService.HasAddAccountPermission();
                //};

                $scope.scopeModel.deselectAllClicked = function () {
                    gridAPI.deselectAllAccounts();
                };
                $scope.scopeModel.selectAllClicked = function () {
                    gridAPI.selectAllAccounts();
                };

                $scope.scopeModel.addBulkActions = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        viewId = payload.viewId;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        bulkAction = payload.bulkAction;
                    }

                    bulkActionId =  bulkAction != undefined ? bulkAction.AccountBulkActionId : undefined;
                    mailMessageTypeId = (bulkAction != undefined && bulkAction.Settings != undefined) ? bulkAction.Settings.MailMessageTypeId : undefined;

                    if (bulkActionId != undefined) {
                        $scope.scopeModel.showActionButtons = true;
                        $scope.scopeModel.showActionInformation = true;
                    }

                    $scope.scopeModel.isLoading = true;
                    loadBEDefinitionSelectorLabel().then(function () {
                        loadBusinessEntityDefinitionSelector();
                    });

                    $scope.scopeModel.selectedFilternIndex = bulkActionId != undefined ? 2 : 0;

                    $scope.scopeModel.bulkActionSettings = bulkAction != undefined ? bulkAction.Settings : undefined;
                    function loadBEDefinitionSelectorLabel() {
                        return VR_Sec_ViewAPIService.GetView(viewId).then(function (response) {
                            if (response != undefined && response.Settings != undefined) {
                                $scope.scopeModel.BEDefinitionSelectorLabel = response.Settings.AccountDefinitionSelectorLabel;
                            }
                        });
                    }
                    function loadBusinessEntityDefinitionSelector() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        businessEntityDefinitionSelectorReadyDeferred.promise.then(function () {

                            var payload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionViewFilter, Retail.BusinessEntity.Business",
                                        ViewId: viewId
                                    }]
                                },
                                selectFirstItem: bulkActionId != undefined ? false : true,
                                selectedIds: bulkActionId != undefined ? accountBEDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(businessEntityDefinitionSelectorAPI, payload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise.then(function () {
                            setTimeout(function () {
                                $scope.scopeModel.showBusinessEntityDefinitionSelector = bulkActionId != undefined ? false : !businessEntityDefinitionSelectorAPI.hasSingleItem();
                            });
                        }).catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        }).finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                    }

                };
              
                api.finalizeBulkActionDraft = function () {
                    return gridAPI.finalizeBulkActionDraft();
                };

                api.getRuntimeDirectiveData = function () {
                    return runtimeDirectiveAPI.getData(); 
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadAllControls() {
                var promises = [loadRootAccountTypeSelector, loadRecordFilterDirective, loadStatusSelector, loadBulkActions];
                
                if (bulkActionId != undefined)
                    promises.push(loadRuntimeDirective);
                
                return UtilsService.waitMultipleAsyncOperations(promises).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }

            function loadRootAccountTypeSelector() {
                var rootAccountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                accountRootTypeSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        selectedIds: bulkActionId != undefined ? false : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(accountRootTypeSelectorAPI, payload, rootAccountTypeSelectorLoadDeferred);
                });

                return rootAccountTypeSelectorLoadDeferred.promise;
            }
            function loadStatusSelector() {
                var statusSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                Retail_BE_AccountBEDefinitionAPIService.GetAccountBEStatusDefinitionId(accountBEDefinitionId).then(function (response) {
                    statusSelectorReadyPromiseDeferred.promise.then(function () {
                        var selectorPayload = {
                            businessEntityDefinitionId: response
                        };
                        VRUIUtilsService.callDirectiveLoad(statusSelectorAPI, selectorPayload, statusSelectorLoadDeferred);
                    });
                }).catch(function (error) {
                    statusSelectorLoadDeferred.reject(error);
                });

                return statusSelectorLoadDeferred.promise;
            }
            function loadRecordFilterDirective() {
                var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                recordFilterDirectiveReadyDeferred.promise.then(function () {

                    loadAccountFields().then(function () {

                        var recordFilterDirectivePayload = {
                            context: buildContext()
                        };
                        VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                    });
            });

                return recordFilterDirectiveLoadDeferred.promise;
            }

            function loadAccountFields() {
                return Retail_BE_AccountTypeAPIService.GetGenericFieldDefinitionsInfo(accountBEDefinitionId).then(function (response) {
                    accountFields = response;
                });
            }

            function loadRuntimeDirective() {
                var runtimeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                runtimeDirectiveReadyDeferred.promise.then(function () {
                var directivePayload = {
                    mailMessageTypeId: mailMessageTypeId
                };
                VRUIUtilsService.callDirectiveLoad(runtimeDirectiveAPI, directivePayload, runtimeDirectiveLoadDeferred);
                });
                return runtimeDirectiveLoadDeferred.promise;
            }

            function loadBulkActions() {
                $scope.scopeModel.addBulkActions.length = 0;
                return Retail_BE_AccountBEDefinitionAPIService.GetAccountBEDefinitionSettingsWithHidden(accountBEDefinitionId).then(function (response) {
                    accountBEDefinitionSettings = response;
                    if (accountBEDefinitionSettings && accountBEDefinitionSettings.AccountBulkActions && accountBEDefinitionSettings.AccountBulkActions.length != 0) {
                        if (bulkActionId==undefined) {
                            $scope.scopeModel.showMenuActions = true;
                            var accountBulkActions = accountBEDefinitionSettings.AccountBulkActions;
                            for (var i = 0; i < accountBulkActions.length; i++) {
                                var accountBulkAction = accountBulkActions[i];
                                addMenuAction(accountBulkAction);
                            }
                            function addMenuAction(accountBulkAction) {
                                $scope.scopeModel.addBulkActions.push({
                                    name: accountBulkAction.Title,
                                    clicked: function () {
                                        Retail_BE_AccountBEService.openBulkActionsEditor(viewId, accountBulkAction, accountBEDefinitionId);
                                    }
                                });
                            }
                        }
                        else {
                            $scope.scopeModel.showMenuActions = false;
                        }
                    }
                    else {
                        $scope.scopeModel.showMenuActions = false;
                    }

                });
            }
            function buildContext() {
                var context = {
                    getFields: function () {
                        var fields = [];
                        if (accountFields != undefined) {
                            for (var i = 0; i < accountFields.length; i++) {
                                var accountField = accountFields[i];

                                fields.push({
                                    FieldName: accountField.Name,
                                    FieldTitle: accountField.Title,
                                    Type: accountField.FieldType
                                });
                            }
                        }
                        return fields;
                    }
                };
                return context;
            }
            function buildGridQuery() {
                return {
                    Name: $scope.scopeModel.name,
                    OnlyRootAccount: accountRootTypeSelectorAPI.getSelectedIds(),
                    AccountTypeIds: accountTypeSelectorAPI.getSelectedIds(),
                    FilterGroup: recordFilterDirectiveAPI.getData().filterObj,
                    StatusIds: statusSelectorAPI.getSelectedIds(),
                    AccountBulkActionId: bulkActionId
                };
            }

            function getGridContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};

                currentContext.setSelectAllEnablity = function (value) {
                    $scope.scopeModel.enableSelectAll = value;
                };
                currentContext.setDeselectAllEnablity = function (value) {
                    $scope.scopeModel.enableDeselectAll = value;

                };
                currentContext.setActionsEnablity = function (value) {

                    $scope.scopeModel.enableBulkActions = value;
                };
                return currentContext;
            }
        }
}]);
