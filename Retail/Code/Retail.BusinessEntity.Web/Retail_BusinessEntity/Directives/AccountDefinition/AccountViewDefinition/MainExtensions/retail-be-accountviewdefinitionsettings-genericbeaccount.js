"use strict";

app.directive("retailBeAccountviewdefinitionsettingsGenericbeaccount", ["UtilsService", "VRUIUtilsService", "VR_GenericData_GenericBEDefinitionAPIService",
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BEParentChildRelationViewDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountViewDefinition/MainExtensions/Templates/GenericBEAccountViewSettingsTemplate.html"
        };

        function BEParentChildRelationViewDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var onBusinessEntityDefinitionSelectionChangeDeffered;

            var accountIdMappingFieldSelectorAPI;
            var accountIdMappingFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var accountBEDefinitionMappingFieldSelectorAPI;
            var accountBEDefinitionMappingFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeId;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onAccountIdMappingFieldSelectorReady = function (api) {
                    accountIdMappingFieldSelectorAPI = api;
                    accountIdMappingFieldSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onAccountBEDefinitionMappingFieldSelectorReady = function (api) {
                    accountBEDefinitionMappingFieldSelectorAPI = api;
                    accountBEDefinitionMappingFieldSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEDefinitionSelectionChanged = function (beDefinitionId) {
                    if (beDefinitionId != undefined) {
                        if (onBusinessEntityDefinitionSelectionChangeDeffered != undefined) {
                            onBusinessEntityDefinitionSelectionChangeDeffered.resolve();
                        }
                        else {
                            dataRecordTypeId = undefined;
                            var bEDefinitionId = beDefinitionId.BusinessEntityDefinitionId;
                            VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(bEDefinitionId).then(function (response) {
                                dataRecordTypeId = response.DataRecordTypeId;

                                var setAccountIdMappingFieldSelectorLoader = function (value) { $scope.scopeModel.isAccountIdMappingFieldSelectorloading = value; };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountIdMappingFieldSelectorAPI, { dataRecordTypeId: dataRecordTypeId }, setAccountIdMappingFieldSelectorLoader);

                                var setAccountBEDefinitionMappingFieldSelectorLoader = function (value) { $scope.scopeModel.isAccountBEDefinitionMappingFieldSelectorloading = value; };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountBEDefinitionMappingFieldSelectorAPI, { dataRecordTypeId: dataRecordTypeId }, setAccountBEDefinitionMappingFieldSelectorLoader);
                            });
                        }
                    }
                };
                defineAPI();
            }

            function loadAccountIdMappingFieldSelector(payload) {
                var loadAccountIdMappingFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                accountIdMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(accountIdMappingFieldSelectorAPI, payload, loadAccountIdMappingFieldSelectorPromiseDeferred);
                });
                return loadAccountIdMappingFieldSelectorPromiseDeferred.promise;
            }

            function loadAccountBEDefinitionMappingFieldSelector(payload) {
                var loadAccountBEDefinitionMappingFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                accountBEDefinitionMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(accountBEDefinitionMappingFieldSelectorAPI, payload, loadAccountBEDefinitionMappingFieldSelectorPromiseDeferred);
                });
                return loadAccountBEDefinitionMappingFieldSelectorPromiseDeferred.promise;
            }

            function loadBusinessEntityDefinitionSelector(payloadSelector) {
                var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                beDefinitionSelectorPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
                });
                return businessEntityDefinitionSelectorLoadDeferred.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var rootPromiseNode = {};
                    onBusinessEntityDefinitionSelectionChangeDeffered = UtilsService.createPromiseDeferred();
                    if (payload != undefined && payload.accountViewDefinitionSettings != undefined && payload.accountViewDefinitionSettings.BusinessEntityDefinitionId != undefined) {
                        var accountViewDefinitionSettings = payload.accountViewDefinitionSettings;
                        rootPromiseNode.promises = [loadBusinessEntityDefinitionSelector({ selectedIds: accountViewDefinitionSettings.BusinessEntityDefinitionId })];
                        rootPromiseNode.getChildNode = function () {
                            var beDefinitionId = accountViewDefinitionSettings.BusinessEntityDefinitionId;
                            var object;
                            var promise = VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(beDefinitionId).then(function (response) {
                                object = response;
                            });
                            return {
                                promises: [promise],
                                getChildNode: function () {
                                    var genericBEDefinitionSettings = object;
                                    if (genericBEDefinitionSettings != undefined && genericBEDefinitionSettings.DataRecordTypeId != undefined) {
                                        dataRecordTypeId = genericBEDefinitionSettings.DataRecordTypeId;
                                        return {
                                            promises: [loadAccountIdMappingFieldSelector({
                                                dataRecordTypeId: dataRecordTypeId,
                                                selectedIds: accountViewDefinitionSettings.AccountIdMappingField
                                            }), loadAccountBEDefinitionMappingFieldSelector({
                                                dataRecordTypeId: dataRecordTypeId,
                                                selectedIds: accountViewDefinitionSettings.AccountBEDefinitionMappingField
                                            })],
                                            getChildNode: function () {
                                                onBusinessEntityDefinitionSelectionChangeDeffered = undefined;
                                                return { promises: [] };
                                            }
                                        };
                                    }
                                    return { promises: [] };
                                }
                            };
                        };
                    }
                    else {
                        rootPromiseNode= {
                            promises: [loadBusinessEntityDefinitionSelector()],
                            getChildNode: function () {
                                onBusinessEntityDefinitionSelectionChangeDeffered = undefined;
                                return { promises:[] }
                            }
                        };
                    }
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountViews.GenericBEAccountSubview, Retail.BusinessEntity.MainExtensions",
                        BusinessEntityDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
                        AccountIdMappingField: accountIdMappingFieldSelectorAPI.getSelectedIds(),
                        AccountBEDefinitionMappingField: accountBEDefinitionMappingFieldSelectorAPI.getSelectedIds(),
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);