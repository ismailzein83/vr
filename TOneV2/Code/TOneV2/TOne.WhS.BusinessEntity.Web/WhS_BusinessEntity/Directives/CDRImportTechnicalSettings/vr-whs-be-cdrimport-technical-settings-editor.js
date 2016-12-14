'use strict';

app.directive('vrWhsBeCdrimportTechnicalSettingsEditor', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_CDRImportTechnicalSettingsService',
    function (utilsService, vruiUtilsService, whSBeCdrImportTechnicalSettingsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new cdrImporTechnicalEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CDRImportTechnicalSettings/Templates/CDRImportTechnicalSettingsTemplate.html"
        };

        function cdrImporTechnicalEditorCtor(ctrl, $scope, $attrs) {

            var ruleDefinitionEntity;
            var customerRuleDefinitionReadyApi;
            var customerRuleDefinitionReadyPromiseDeferred = utilsService.createPromiseDeferred();

            var supplierRuleDefinitionReadyApi;
            var supplierRuleDefinitionReadyPromiseDeferred = utilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onCustomerRuleDefinitionReady = function (api) {
                    customerRuleDefinitionReadyApi = api;
                    customerRuleDefinitionReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSupplierRuleDefinitionReady = function (api) {
                    supplierRuleDefinitionReadyApi = api;
                    supplierRuleDefinitionReadyPromiseDeferred.resolve();
                };
                defineApi();
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {
                        ruleDefinitionEntity = payload.data;
                    }
                    var promises = [];
                    promises.push(loadCustomerRuleDefinitionSelector());
                    promises.push(loadsupplierRuleDefinitionSelector());
                    return utilsService.waitMultiplePromises(promises);
                };
                api.getData = function () {
                    var cdrImportTechnicalSettingData =
                    {
                        CustomerRuleDefinitionGuid: customerRuleDefinitionReadyApi.getSelectedIds(),
                        SupplierRuleDefinitionGuid: supplierRuleDefinitionReadyApi.getSelectedIds(),
                    };
                    var obj = {
                        $type: "TOne.WhS.BusinessEntity.Entities.CDRImportTechnicalSettings, TOne.WhS.BusinessEntity.Entities",
                        CdrImportTechnicalSettingData: cdrImportTechnicalSettingData
                    };
                    return obj;
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadCustomerRuleDefinitionSelector() {
                var customerRuleDefinitionSelectorLoadDeferred = utilsService.createPromiseDeferred();
                whSBeCdrImportTechnicalSettingsService.getRuleDefinitionType().then(function (response) {
                    customerRuleDefinitionReadyPromiseDeferred.promise.then(function () {
                        var selectorPayload = { filter: { RuleTypeId: response } };
                        if (ruleDefinitionEntity != undefined) {
                            selectorPayload.selectedIds = ruleDefinitionEntity.CdrImportTechnicalSettingData.CustomerRuleDefinitionGuid;
                        }
                        vruiUtilsService.callDirectiveLoad(customerRuleDefinitionReadyApi, selectorPayload, customerRuleDefinitionSelectorLoadDeferred);
                    }
                    );
                });
                return customerRuleDefinitionSelectorLoadDeferred.promise;
            }

            function loadsupplierRuleDefinitionSelector() {
                var supplierRuleDefinitionSelectorLoadDeferred = utilsService.createPromiseDeferred();
                whSBeCdrImportTechnicalSettingsService.getRuleDefinitionType().then(function (response) {
                    supplierRuleDefinitionReadyPromiseDeferred.promise.then(function () {
                        var selectorPayload = { filter: { RuleTypeId: response } };
                        if (ruleDefinitionEntity != undefined) {
                            selectorPayload.selectedIds = ruleDefinitionEntity.CdrImportTechnicalSettingData.SupplierRuleDefinitionGuid;
                        }
                        vruiUtilsService.callDirectiveLoad(supplierRuleDefinitionReadyApi, selectorPayload, supplierRuleDefinitionSelectorLoadDeferred);
                    }
                    );
                });
                return supplierRuleDefinitionSelectorLoadDeferred.promise;
            }
        }
        return directiveDefinitionObject;
    }]);