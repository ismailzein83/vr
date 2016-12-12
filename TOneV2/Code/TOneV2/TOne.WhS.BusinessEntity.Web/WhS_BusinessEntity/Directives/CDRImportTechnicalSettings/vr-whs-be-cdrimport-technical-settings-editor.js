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
            var ruleDefinitionReadyApi;
            var ruleDefinitionReadyPromiseDeferred = utilsService.createPromiseDeferred();
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onRuleDefinitionReady = function (api) {
                    ruleDefinitionReadyApi = api;
                    ruleDefinitionReadyPromiseDeferred.resolve();
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
                    promises.push(loadRuleDefinitionSelector());
                    return utilsService.waitMultiplePromises(promises);
                };
                api.getData = function () {
                    var cdrImportTechnicalSettingData =
                    {
                        RuleDefinitionGuid: ruleDefinitionReadyApi.getSelectedIds()
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

            function loadRuleDefinitionSelector() {
                var ruleDefinitionSelectorLoadDeferred = utilsService.createPromiseDeferred();
                whSBeCdrImportTechnicalSettingsService.getRuleDefinitionType().then(function (response) {
                    ruleDefinitionReadyPromiseDeferred.promise.then(function () {
                        var selectorPayload = { filter: { RuleTypeId: response } };
                        if (ruleDefinitionEntity != undefined) {
                            selectorPayload.selectedIds = ruleDefinitionEntity.CdrImportTechnicalSettingData.RuleDefinitionGuid;
                        }
                        vruiUtilsService.callDirectiveLoad(ruleDefinitionReadyApi, selectorPayload, ruleDefinitionSelectorLoadDeferred);
                    }
                    );
                });
                return ruleDefinitionSelectorLoadDeferred.promise;
            }
        }
        return directiveDefinitionObject;
    }]);