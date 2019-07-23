(function (app) {
    'use strict';
    PostpaidTaxRuleSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_RA_SMSTaxChargeTypeEnum', 'Retail_RA_VoiceTaxChargeTypeEnum', 'Retail_RA_UsageTaxRuleTypeEnum'];
    function PostpaidTaxRuleSettingsDirective(UtilsService, VRUIUtilsService, Retail_RA_SMSTaxChargeTypeEnum, Retail_RA_VoiceTaxChargeTypeEnum, Retail_RA_UsageTaxRuleTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var postpaidTaxRuleSettings = new PostpaidTaxRuleSettings(ctrl, $scope, $attrs);
                postpaidTaxRuleSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_RA/Directives/TaxRules/Templates/PostpaidTaxRuleSettings.html"
        };

        function PostpaidTaxRuleSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var usageTaxRulePromiseChangeDeferred;
            var voiceTaxRulePromiseChangeDeferred;
            var smsTaxRulePromiseChangeDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.usagePostpaidTaxRuleTypes = UtilsService.getArrayEnum(Retail_RA_UsageTaxRuleTypeEnum);
                $scope.scopeModel.voiceTaxChargeTypes = UtilsService.getArrayEnum(Retail_RA_VoiceTaxChargeTypeEnum);
                $scope.scopeModel.smsTaxChargeTypes = UtilsService.getArrayEnum(Retail_RA_SMSTaxChargeTypeEnum);

                $scope.scopeModel.onUsageTaxRuleChanged = function (item) {
                    if (usageTaxRulePromiseChangeDeferred != undefined)
                        usageTaxRulePromiseChangeDeferred = undefined;

                    else {
                        $scope.scopeModel.usageTaxPercentage = undefined;
                        $scope.scopeModel.voiceTaxChargeType = undefined;
                        $scope.scopeModel.smsTaxChargeType = undefined;
                    }
                };

                $scope.scopeModel.onVoiceTaxRuleChanged = function (item) {
                    if (voiceTaxRulePromiseChangeDeferred != undefined)
                        voiceTaxRulePromiseChangeDeferred = undefined;

                    else {
                        $scope.scopeModel.voiceTaxPercentage = undefined;
                        $scope.scopeModel.taxPerMinute = undefined;
                    }
                };

                $scope.scopeModel.onSMSTaxRuleChanged = function (item) {
                    if (smsTaxRulePromiseChangeDeferred != undefined)
                        smsTaxRulePromiseChangeDeferred = undefined;

                    else {
                        $scope.scopeModel.smsTaxPercentage = undefined;
                        $scope.scopeModel.taxPerSMS = undefined;
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined && payload.settings != undefined) {

                        if (payload.settings.UsageSettings != undefined) {
                            usageTaxRulePromiseChangeDeferred = UtilsService.createPromiseDeferred();
                            var usageSettings = payload.settings.UsageSettings;
                            $scope.scopeModel.usagePostpaidTaxRuleType = UtilsService.getItemByVal($scope.scopeModel.usagePostpaidTaxRuleTypes, usageSettings.UsagePostpaidTaxRuleType, 'value');
                            $scope.scopeModel.usageTaxPercentage = usageSettings.OverallTaxPercentage;

                            if (usageSettings.VoiceSettings != undefined) {
                                voiceTaxRulePromiseChangeDeferred = UtilsService.createPromiseDeferred();
                                $scope.scopeModel.voiceTaxChargeType = UtilsService.getItemByVal($scope.scopeModel.voiceTaxChargeTypes, usageSettings.VoiceSettings.ChargeType, 'value');
                                $scope.scopeModel.voiceTaxPercentage = usageSettings.VoiceSettings.TaxPercentage;
                                $scope.scopeModel.taxPerMinute = usageSettings.VoiceSettings.TaxPerMinute;
                            }
                            if (usageSettings.SMSSettings != undefined) {
                                smsTaxRulePromiseChangeDeferred = UtilsService.createPromiseDeferred();
                                $scope.scopeModel.smsTaxChargeType = UtilsService.getItemByVal($scope.scopeModel.smsTaxChargeTypes, usageSettings.SMSSettings.ChargeType, 'value');
                                $scope.scopeModel.smsTaxPercentage = usageSettings.SMSSettings.TaxPercentage;
                                $scope.scopeModel.taxPerSMS = usageSettings.SMSSettings.TaxPerSMS;
                            }
                        }

                        if (payload.settings.NonUsageSettings != undefined) {
                            var nonUsageSettings = payload.settings.NonUsageSettings;

                            if (nonUsageSettings.TransactionSettings != undefined) {
                                $scope.scopeModel.transactionTaxPercentage = nonUsageSettings.TransactionSettings.TaxPercentage;
                            }
                        }
                    }

                    var rootPromiseNode = {
                        promises: promises
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.PostpaidTaxRuleSettings, Retail.RA.Business",
                        UsageSettings: {
                            UsagePostpaidTaxRuleType: $scope.scopeModel.usagePostpaidTaxRuleType != undefined ? $scope.scopeModel.usagePostpaidTaxRuleType.value : undefined,
                            OverallTaxPercentage: $scope.scopeModel.usageTaxPercentage,
                            VoiceSettings: {
                                ChargeType: $scope.scopeModel.voiceTaxChargeType != undefined ? $scope.scopeModel.voiceTaxChargeType.value : undefined,
                                TaxPercentage: $scope.scopeModel.voiceTaxChargeType != undefined && $scope.scopeModel.voiceTaxChargeType.value == Retail_RA_VoiceTaxChargeTypeEnum.Overall.value ? $scope.scopeModel.voiceTaxPercentage : undefined,
                                TaxPerMinute: $scope.scopeModel.voiceTaxChargeType != undefined && $scope.scopeModel.voiceTaxChargeType.value == Retail_RA_VoiceTaxChargeTypeEnum.PerMinute.value ? $scope.scopeModel.taxPerMinute : undefined
                            },
                            SMSSettings: {
                                ChargeType: $scope.scopeModel.smsTaxChargeType != undefined ? $scope.scopeModel.smsTaxChargeType.value : undefined,
                                TaxPercentage: $scope.scopeModel.smsTaxChargeType != undefined && $scope.scopeModel.smsTaxChargeType.value == Retail_RA_SMSTaxChargeTypeEnum.Overall.value ? $scope.scopeModel.smsTaxPercentage : undefined,
                                TaxPerSMS: $scope.scopeModel.smsTaxChargeType != undefined && $scope.scopeModel.smsTaxChargeType.value == Retail_RA_SMSTaxChargeTypeEnum.PerSMS.value ? $scope.scopeModel.taxPerSMS : undefined
                            }
                        },
                        NonUsageSettings: {
                            TransactionSettings: {
                                TaxPercentage: $scope.scopeModel.transactionTaxPercentage
                            }
                        }
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('raRulesTaxrulesettingsPostpaid', PostpaidTaxRuleSettingsDirective);

})(app);