'use strict';

app.directive('retailBeAccountEditor', ['UtilsService', 'VRUIUtilsService', 'Retail_BE_AccountBEAPIService', 'Retail_BE_AccountTypeAPIService', 'Retail_BE_AccountPartDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, Retail_BE_AccountBEAPIService, Retail_BE_AccountTypeAPIService, Retail_BE_AccountPartDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new accountEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Account/Templates/AccountEditor.html"
        };

        function accountEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var isEditMode;
            var accountBEDefinitionId;
            var accountId;
            var accountEntity;
            var parentAccountId;
            var onAccountLoaded;

            var accountTypeSelectorAPI;
            var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var accountTypeSelectedDeferred;

            var accountEditorRuntime;

            var requiredPartsDirectiveAPI;
            var requiredPartsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var partDefinitionExtensionConfigs;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.notRequiredParts = [];

                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onRequiredPartsDirectiveReady = function (api) {
                    requiredPartsDirectiveAPI = api;
                    requiredPartsDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onAccountTypeSelectionChanged = function () {
                    var selectedId = accountTypeSelectorAPI.getSelectedIds();
                    if (selectedId == undefined)
                        return;
                    if (accountTypeSelectedDeferred != undefined) {
                        accountTypeSelectedDeferred.resolve();
                        return;
                    }

                    $scope.scopeModel.isLoading = true;
                    loadRuntime().then(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                };

                //$scope.scopeModel.hasSaveAccountPermission = function () {
                //    return (isEditMode) ? Retail_BE_AccountBEAPIService.HasUpdateAccountPermission() : Retail_BE_AccountBEAPIService.HasAddAccountPermission();
                //};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        accountId = payload.accountId;
                        parentAccountId = payload.parentAccountId;
                        onAccountLoaded = payload.onAccountLoaded;

                        isEditMode = (accountId != undefined);
                    }

                    var accountEditorDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    
                    if (isEditMode) {
                        getAccount().then(function () {
                            loadAllControls();
                        });
                    }
                    else {
                        loadAllControls();
                    }


                    function getAccount() {
                        return Retail_BE_AccountBEAPIService.GetAccount(accountBEDefinitionId, accountId).then(function (response) {
                            accountEntity = response;
                            parentAccountId = response.ParentAccountId;
                        });
                    }

                    function loadAllControls() {
                        var _promises = [];

                        //setTitle for AccountEditorController
                        if (onAccountLoaded != undefined) {
                            onAccountLoaded(accountEntity);
                        }

                        //Loading StaticData
                        if (accountEntity != undefined) {
                            $scope.scopeModel.name = accountEntity.Name;
                        }

                        //Loading AccountTypeSelectorWithRuntime
                        var loadAccountTypeSelectorWithRuntimePromise = getLoadAccountTypeSelectorWithRuntimePromise();
                        _promises.push(loadAccountTypeSelectorWithRuntimePromise);


                        UtilsService.waitMultiplePromises(_promises).then(function () {
                            accountEntity = undefined;
                            accountEditorDirectiveLoadDeferred.resolve();
                        });
                    }
                    function getLoadAccountTypeSelectorWithRuntimePromise() {
                        var _promises = [];

                        var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        _promises.push(accountTypeSelectorLoadDeferred.promise);

                        var accountTypeSelectorPayload = {
                            filter: {
                                AccountBEDefinitionId: accountBEDefinitionId,
                                RootAccountTypeOnly: parentAccountId == undefined ? true : false,
                                ParentAccountId: parentAccountId
                            }
                        };

                        if (accountEntity != undefined) {
                            accountTypeSelectorPayload.selectedIds = accountEntity.TypeId;
                            accountTypeSelectedDeferred = UtilsService.createPromiseDeferred();
                        }

                        accountTypeSelectorReadyDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });

                        if (accountEntity != undefined) {
                            var runtimeLoadDeferred = UtilsService.createPromiseDeferred();
                            _promises.push(runtimeLoadDeferred.promise);

                            accountTypeSelectedDeferred.promise.then(function () {
                                accountTypeSelectedDeferred = undefined;

                                loadRuntime().then(function () {
                                    runtimeLoadDeferred.resolve();
                                }).catch(function (error) {
                                    runtimeLoadDeferred.reject(error);
                                });
                            });
                        }

                        return UtilsService.waitMultiplePromises(_promises);
                    }

                    return accountEditorDirectiveLoadDeferred.promise;
                };

                api.getData = function () {

                    var accountObj = buildAccountObjFromScope();

                    function buildAccountObjFromScope() {
                        var obj = {
                            AccountBEDefinitionId: accountBEDefinitionId,
                            AccountId: accountId,
                            Name: $scope.scopeModel.name,
                            TypeId: accountTypeSelectorAPI.getSelectedIds()
                        };

                        obj.Settings = {};
                        obj.Settings.Parts = buildAccountParts();

                        if (!isEditMode) {
                            obj.StatusId = accountEntity != undefined ? accountEntity.StatusId : undefined;
                            obj.ParentAccountId = parentAccountId;
                        }

                        return obj;
                    }
                    function buildAccountParts() {
                        var accountParts = {};

                        var requiredParts = requiredPartsDirectiveAPI.getData();
                        if (requiredParts != undefined) {
                            for (var key in requiredParts)
                                accountParts[key] = requiredParts[key];
                        }

                        for (var i = 0; i < $scope.scopeModel.notRequiredParts.length; i++) {
                            var notRequiredPart = $scope.scopeModel.notRequiredParts[i];
                            if (notRequiredPart.isSelected) {
                                var key = notRequiredPart.definition.AccountPartDefinitionId;
                                var partSettings = notRequiredPart.directiveAPI.getData();
                                accountParts[key] = { Settings: partSettings };
                            }
                        }

                        return accountParts;
                    }

                    return accountObj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadRuntime() {
                var promises = [];
                var accountTypeId = accountTypeSelectorAPI.getSelectedIds();

                var getAccountEditorRuntimePromise = getAccountEditorRuntime();
                promises.push(getAccountEditorRuntimePromise);

                var loadAccountPartDefinitionExtensionConfigsPromise = loadAccountPartDefinitionExtensionConfigs();
                promises.push(loadAccountPartDefinitionExtensionConfigsPromise);

                var loadPartsDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadPartsDeferred.promise);

                var requiredPartDefinitions = [];
                $scope.scopeModel.notRequiredParts.length = 0;

                UtilsService.waitMultiplePromises([getAccountEditorRuntimePromise, loadAccountPartDefinitionExtensionConfigsPromise]).then(function () {
                    if (accountEditorRuntime != undefined && accountEditorRuntime.Parts != null) {
                        for (var i = 0; i < accountEditorRuntime.Parts.length; i++) {
                            var part = accountEditorRuntime.Parts[i];
                            if (part.IsRequired) {
                                requiredPartDefinitions.push(part.PartDefinition);
                            }
                            else {
                                var notRequiredPart = buildNotRequiredPart(part.PartDefinition);
                                $scope.scopeModel.notRequiredParts.push(notRequiredPart);
                            }
                        }
                    }

                    UtilsService.waitMultipleAsyncOperations([loadRequiredPartsDirective, loadNotRequiredParts]).then(function () {
                        loadPartsDeferred.resolve();
                    }).catch(function (error) {
                        loadPartsDeferred.reject(error, $scope);
                    });
                });

                function getAccountEditorRuntime() {
                    return Retail_BE_AccountBEAPIService.GetAccountEditorRuntime(accountBEDefinitionId, accountTypeId, parentAccountId).then(function (response) {
                        accountEditorRuntime = response;
                    });
                }
                function loadAccountPartDefinitionExtensionConfigs() {
                    return Retail_BE_AccountPartDefinitionAPIService.GetAccountPartDefinitionExtensionConfigs().then(function (response) {
                        if (response != null) {
                            partDefinitionExtensionConfigs = [];
                            for (var i = 0; i < response.length; i++) {
                                partDefinitionExtensionConfigs.push(response[i]);
                            }
                        }
                    });
                }
                function loadRequiredPartsDirective() {
                    var requiredPartsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    requiredPartsDirectiveReadyDeferred.promise.then(function () {
                        var requiredPartsDirectivePayload = {};
                        requiredPartsDirectivePayload.context = {};
                        requiredPartsDirectivePayload.context.getPartDefinitionRuntimeEditor = getPartDefinitionRuntimeEditor;
                        requiredPartsDirectivePayload.partDefinitions = requiredPartDefinitions;
                        requiredPartsDirectivePayload.accountBEDefinitionId = accountBEDefinitionId;

                        if (accountEntity != undefined && accountEntity.Settings != null)
                            requiredPartsDirectivePayload.parts = accountEntity.Settings.Parts;

                        VRUIUtilsService.callDirectiveLoad(requiredPartsDirectiveAPI, requiredPartsDirectivePayload, requiredPartsDirectiveLoadDeferred);
                    });

                    return requiredPartsDirectiveLoadDeferred.promise;
                }
                function loadNotRequiredParts() {
                    var loadPromises = [];
                    for (var i = 0; i < $scope.scopeModel.notRequiredParts.length; i++) {
                        var notRequiredPart = $scope.scopeModel.notRequiredParts[i];
                        if (notRequiredPart.isSelected)
                            loadPromises.push(notRequiredPart.directiveLoadDeferred.promise);
                    }
                    return UtilsService.waitMultiplePromises(loadPromises);
                }
                function buildNotRequiredPart(partDefinition) {
                    var notRequiredPart = {};

                    notRequiredPart.definition = partDefinition;
                    notRequiredPart.runtimeEditor = getPartDefinitionRuntimeEditor(partDefinition.Settings.ConfigId);
                    notRequiredPart.directiveLoadDeferred = UtilsService.createPromiseDeferred();
                    notRequiredPart.isSelected = isNotRequiredPartSelected(partDefinition.AccountPartDefinitionId);

                    notRequiredPart.onDirectiveReady = function (api) {
                        notRequiredPart.directiveAPI = api;
                        var directivePayload = {
                            partDefinition: partDefinition,
                            partSettings: (accountEntity != undefined && accountEntity.Settings != null) ?
                                accountEntity.Settings.Parts[partDefinition.AccountPartDefinitionId].Settings : undefined
                        };
                        notRequiredPart.isLoading = true;
                        VRUIUtilsService.callDirectiveLoad(notRequiredPart.directiveAPI, directivePayload, notRequiredPart.directiveLoadDeferred);
                        notRequiredPart.directiveLoadDeferred.promise.finally(function () {
                            notRequiredPart.isLoading = false;
                        });
                    };

                    notRequiredPart.directiveLoadDeferred.promise;

                    return notRequiredPart;
                }
                function isNotRequiredPartSelected(partDefinitionId) {
                    if (accountEntity == undefined || accountEntity.Settings == null || accountEntity.Settings.Parts == null)
                        return false;
                    return (accountEntity.Settings.Parts[partDefinitionId] != null);
                }

                return UtilsService.waitMultiplePromises(promises);
            }
            function getPartDefinitionRuntimeEditor(partDefinitionId) {
                var partExtensionConfig = UtilsService.getItemByVal(partDefinitionExtensionConfigs, partDefinitionId, 'ExtensionConfigurationId');
                return (partExtensionConfig != undefined) ? partExtensionConfig.RuntimeEditor : null;
            }
        }

        return directiveDefinitionObject;
    }]);