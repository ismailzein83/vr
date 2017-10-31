'use strict';

app.directive('vrWhsBeSaleareaTechnicalSettingsEditor', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_SaleAreaTechnicalSettingsService',
    function (utilsService, vruiUtilsService, WhS_BE_SaleAreaTechnicalSettingsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new saleAreaTechnicalEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SaleAreaTechnicalSettings/Templates/SaleAreaTechnicalSettingsTemplate.html"
        };

        function saleAreaTechnicalEditorCtor(ctrl, $scope, $attrs) {

            var ruleDefinitionEntity;
            var customerTariffRuleDefinitionReadyApi;
            var customerRuleDefinitionReadyPromiseDeferred = utilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onCustomerRuleDefinitionReady = function (api) {
                    customerTariffRuleDefinitionReadyApi = api;
                    customerRuleDefinitionReadyPromiseDeferred.resolve();
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
                    promises.push(loadTariffRuleDefinitionSelector());
                    return utilsService.waitMultiplePromises(promises);
                };
                api.getData = function () {
                    var saleareaTechnicalSettingData =
                    {
                        TariffRuleDefinitionGuid: customerTariffRuleDefinitionReadyApi.getSelectedIds()
                    };
                    var obj = {
                        $type: "TOne.WhS.BusinessEntity.Entities.SaleAreaTechnicalSettings, TOne.WhS.BusinessEntity.Entities",
                        SaleAreaTechnicalSettingData: saleareaTechnicalSettingData
                    };
                    return obj;
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadTariffRuleDefinitionSelector() {
                var tariffRuleDefinitionSelectorLoadDeferred = utilsService.createPromiseDeferred();
                WhS_BE_SaleAreaTechnicalSettingsService.getRuleDefinitionType().then(function (response) {
                    customerRuleDefinitionReadyPromiseDeferred.promise.then(function () {
                        var selectorPayload = { filter: { RuleTypeId: response } };
                        if (ruleDefinitionEntity != undefined) {
                            selectorPayload.selectedIds = ruleDefinitionEntity.SaleAreaTechnicalSettingData.TariffRuleDefinitionGuid;
                        }
                        vruiUtilsService.callDirectiveLoad(customerTariffRuleDefinitionReadyApi, selectorPayload, tariffRuleDefinitionSelectorLoadDeferred);
                    }
                    );
                });
                return tariffRuleDefinitionSelectorLoadDeferred.promise;
            }
        }
        return directiveDefinitionObject;
    }]);