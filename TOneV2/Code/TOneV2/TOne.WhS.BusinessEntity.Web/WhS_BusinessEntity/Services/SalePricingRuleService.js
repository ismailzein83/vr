(function (appControllers) {

    'use stict';

    SalePricingRuleService.$inject = ['WhS_BE_PricingTypeEnum', 'VRModalService', 'VRNotificationService'];

    function SalePricingRuleService(WhS_BE_PricingTypeEnum, VRModalService, VRNotificationService) {
        return ({
            addSalePricingRule: addSalePricingRule,
            editSalePricingRule: editSalePricingRule,
            deleteSalePricingRule: deleteSalePricingRule
        });

        function addSalePricingRule(onSalePricingRuleAdded, type) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPricingRuleAdded = onSalePricingRuleAdded;
            };
            var parameters = {
                PricingType: WhS_BE_PricingTypeEnum.Sale.value
            };
            if (type != undefined) {
                parameters.PricingRuleType = type;
            }



            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingRule/PricingRuleEditor.html', parameters, settings);
        }

        function editSalePricingRule(obj, onSalePricingRuleUpdated) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPricingRuleUpdated = onSalePricingRuleUpdated;
            };
            var parameters = {
                RuleId: obj.RuleId,
                PricingType: obj.PricingType
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingRule/PricingRuleEditor.html', parameters, settings);
        }

        function deleteSalePricingRule($scope, salePricingRuleObj, onSalePricingRuleDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_BE_SalePricingRuleAPIService.DeleteRule(salePricingRuleObj.Entity.RuleId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Sale Pricing Rule", deletionResponse);
                                onSalePricingRuleDeleted(salePricingRuleObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }
    }

    appControllers.service('WhS_BE_SalePricingRuleService', SalePricingRuleService);

})(appControllers);
