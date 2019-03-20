//"use strict";

//app.directive("retailBeAccountviewdefinitionsettingsGenericbeaccount", ["UtilsService", "VRUIUtilsService", "VR_GenericData_GenericBEDefinitionAPIService",
//    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {

//        var directiveDefinitionObject = {
//            restrict: "E",
//            scope:
//            {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new BEParentChildRelationViewDefinitionSettingsCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountViewDefinition/MainExtensions/Templates/GenericBEAccountViewSettingsTemplate.html"
//        };

//        function BEParentChildRelationViewDefinitionSettingsCtor($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var beDefinitionSelectorApi;
//            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
//            var onBusinessEntityDefinitionSelectionChangeDeffered;

//            var accountIdMappingFieldSelectorAPI;
//            var accountIdMappingFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            var accountBEDefinitionMappingFieldNameSelectorAPI;
//            var accountBEDefinitionMappingFieldNameSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
//            var dataRecordTypeId;
//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
//                    beDefinitionSelectorApi = api;
//                    beDefinitionSelectorPromiseDeferred.resolve();
//                };
//                $scope.scopeModel.accountIdMappingFieldSelectorReady = function (api) {
//                    accountIdMappingFieldSelectorAPI = api;
//                    accountIdMappingFieldSelectorReadyPromiseDeferred.resolve();
//                };
//                $scope.scopeModel.accountBEDefinitionMappingFieldNameSelectorReady = function (api) {
//                    accountBEDefinitionMappingFieldNameSelectorAPI = api;
//                    accountBEDefinitionMappingFieldNameSelectorReadyPromiseDeferred.resolve();
//                };

//                $scope.scopeModel.onGenericBEDefinitionSelectionChanged = function (beDefinitionId) {
//                        if (beDefinitionId != undefined) {
//                            if (onBusinessEntityDefinitionSelectionChangeDeffered != undefined) {
//                                onBusinessEntityDefinitionSelectionChangeDeffered.resolve();
//                            }
//                            else {
//                                dataRecordTypeId = undefined;
//                                var bEDefinitionId = beDefinitionId.BusinessEntityDefinitionId;
//                                if (bEDefinitionId != undefined) {
//                                    VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(bEDefinitionId).then(function (response) {
//                                        dataRecordTypeId = response.DataRecordTypeId;
//                                        loadAccountIdMappingFieldSelector({ dataRecordTypeId: dataRecordTypeId });
//                                        loadAccountBEDefinitionMappingFieldNameSelector({ dataRecordTypeId: dataRecordTypeId });
//                                    });
//                                }
//                            }
//                        }

//                };
//                defineAPI();
//            }

//            function loadAccountIdMappingFieldSelector(payload) {
//                var loadAccountIdMappingFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

//                accountIdMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
//                    VRUIUtilsService.callDirectiveLoad(accountIdMappingFieldSelectorAPI, payload, loadAccountIdMappingFieldSelectorPromiseDeferred);
//                });
//                return loadAccountIdMappingFieldSelectorPromiseDeferred.promise;
//            }

//            function loadAccountBEDefinitionMappingFieldNameSelector(payload) {
//                var loadAccountBEDefinitionMappingFieldNameSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

//                accountBEDefinitionMappingFieldNameSelectorReadyPromiseDeferred.promise.then(function () {
//                    VRUIUtilsService.callDirectiveLoad(accountBEDefinitionMappingFieldNameSelectorAPI, payload, loadAccountBEDefinitionMappingFieldNameSelectorPromiseDeferred);
//                });
//                return loadAccountBEDefinitionMappingFieldNameSelectorPromiseDeferred.promise;
//            }

//            function loadBusinessEntityDefinitionSelector(payloadSelector) {
//                var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

//                beDefinitionSelectorPromiseDeferred.promise.then(function () {
//                    var payloadSelector = {
//                        //selectedIds: payload != undefined ? getGenericBEDefinitionIdsFromSettings(payload.Settings) : undefined,
//                        //filter: {
//                        //    Filters: [{
//                        //        $type: "Vanrise.GenericData.Business.GenericBusinessEntityDefinitionFilter, Vanrise.GenericData.Business"
//                        //    }]
//                        //}
//                    };
//                    VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
//                });
//                return businessEntityDefinitionSelectorLoadDeferred.promise;
//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var loadPromiseDeferred = UtilsService.createPromiseDeferred();
//                    onBusinessEntityDefinitionSelectionChangeDeffered = UtilsService.createPromiseDeferred();
//                    console.log(payload)
//                    if (payload != undefined && payload.accountViewDefinitionSettings != undefined && payload.accountViewDefinitionSettings.BusinessEntityDefinitionId != undefined) {
//                        loadBusinessEntityDefinitionSelector({ selectedIds: payload.accountViewDefinitionSettings.BusinessEntityDefinitionId }).then(function () {
//                            var beDefinitionId = beDefinitionSelectorApi.getSelectedIds();
//                            VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(beDefinitionSelectorApi.getSelectedIds(beDefinitionId)).then(function (response) {
//                                genericBEDefinitionSettings = response;
//                                dataRecordTypeId = genericBEDefinitionSettings.DataRecordTypeId;
//                                if (genericBEDefinitionSettings != undefined && dataRecordTypeId != undefined) {
//                                    loadAccountIdMappingFieldSelector({ dataRecordTypeId: dataRecordTypeId });
//                                    loadAccountBEDefinitionMappingFieldNameSelector({ dataRecordTypeId: dataRecordTypeId });
//                                }
//                                loadPromiseDeferred.resolve();
//                                onBusinessEntityDefinitionSelectionChangeDeffered = undefined;
//                            });
//                        });
//                    }
//                    else
//                        loadBusinessEntityDefinitionSelector().then(function () {
//                            loadPromiseDeferred.resolve();
//                            onBusinessEntityDefinitionSelectionChangeDeffered = undefined;
//                        });
//                    return loadPromiseDeferred.promise;
//                };

//                api.getData = function () {

//                    var obj = {
//                        $type: "Retail.BusinessEntity.MainExtensions.AccountViews.GenericBEAccountSubview, Retail.BusinessEntity.MainExtensions",
//                        BusinessEntityDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
//                        AccountIdMappingField: accountIdMappingFieldSelectorAPI.getSelectedIds(),
//                        AccountBEDefinitionMappingField: accountBEDefinitionMappingFieldNameSelectorAPI.getSelectedIds(),
//                    };
//                    return obj;
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }
//        }

//        return directiveDefinitionObject;
//    }
//]);