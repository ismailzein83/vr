(function (app) {
    'use strict';
    PostpaidTaxRuleSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_RA_SMSTaxChargeTypeEnum','Retail_RA_VoiceTaxChargeTypeEnum'];
    function PostpaidTaxRuleSettingsDirective(UtilsService, VRUIUtilsService, Retail_RA_SMSTaxChargeTypeEnum, Retail_RA_VoiceTaxChargeTypeEnum) {
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


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.smsTaxChargeTypes = UtilsService.getArrayEnum(Retail_RA_SMSTaxChargeTypeEnum);

                $scope.scopeModel.voiceTaxChargeTypes = UtilsService.getArrayEnum(Retail_RA_VoiceTaxChargeTypeEnum);


                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined && payload.settings != undefined) {
                        if (payload.settings.VoiceSettings != undefined) {
                            $scope.scopeModel.voiceTaxChargeType = UtilsService.getItemByVal($scope.scopeModel.voiceTaxChargeTypes, payload.settings.VoiceSettings.ChargeType, 'value');
                            $scope.scopeModel.voiceTaxPercentage = payload.settings.VoiceSettings.TaxPercentage;
                            $scope.scopeModel.taxPerMinute = payload.settings.VoiceSettings.TaxPerMinute;
                        }
                        if (payload.settings.SMSSettings != undefined) {
                            $scope.scopeModel.smsTaxChargeType = UtilsService.getItemByVal($scope.scopeModel.smsTaxChargeTypes, payload.settings.SMSSettings.ChargeType, 'value');
                            $scope.scopeModel.smsTaxPercentage = payload.settings.SMSSettings.TaxPercentage;
                            $scope.scopeModel.taxPerSMS = payload.settings.SMSSettings.TaxPerSMS;
                        }
                        if (payload.settings.TransactionSettings != undefined) {
                            $scope.scopeModel.transactionTaxPercentage = payload.settings.TransactionSettings.TaxPercentage;
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
                        VoiceSettings: {
                            $type: "Retail.RA.Business.PostpaidVoiceTaxRuleSettings, Retail.RA.Business",
                            ChargeType: $scope.scopeModel.voiceTaxChargeType!=undefined ? $scope.scopeModel.voiceTaxChargeType.value : undefined,
                            TaxPercentage: $scope.scopeModel.smsTaxChargeType != undefined && $scope.scopeModel.smsTaxChargeType.value == Retail_RA_VoiceTaxChargeTypeEnum.Overall.value ? $scope.scopeModel.voiceTaxPercentage : undefined,
                            TaxPerMinute: $scope.scopeModel.smsTaxChargeType != undefined && $scope.scopeModel.smsTaxChargeType.value == Retail_RA_VoiceTaxChargeTypeEnum.PerMinute.value ?  $scope.scopeModel.taxPerMinute : undefined
                        },
                        SMSSettings: {
                            $type: "Retail.RA.Business.PostpaidSMSTaxRuleSettings, Retail.RA.Business",
                            ChargeType: $scope.scopeModel.smsTaxChargeType != undefined ? $scope.scopeModel.smsTaxChargeType.value : undefined,
                            TaxPercentage: $scope.scopeModel.smsTaxChargeType != undefined && $scope.scopeModel.smsTaxChargeType.value == Retail_RA_SMSTaxChargeTypeEnum.Overall.value ? $scope.scopeModel.smsTaxPercentage : undefined,
                            TaxPerSMS: $scope.scopeModel.smsTaxChargeType != undefined && $scope.scopeModel.smsTaxChargeType.value == Retail_RA_SMSTaxChargeTypeEnum.PerSMS.value ? $scope.scopeModel.taxPerSMS : undefined
                        },
                        TransactionSettings: {
                            $type: "Retail.RA.Business.PostpaidTransactionTaxRuleSettings, Retail.RA.Business",
                            TaxPercentage: $scope.scopeModel.transactionTaxPercentage
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