(function (appControllers) {

    'use stict';

    PurchasePricingRuleService.$inject = ['WhS_BE_PricingTypeEnum', 'VRModalService', 'VRNotificationService'];

    function PurchasePricingRuleService(WhS_BE_PricingTypeEnum, VRModalService, VRNotificationService) {
        return ({
            addPurchasePricingRule: addPurchasePricingRule,
            editPurchasePricingRule: editPurchasePricingRule,
            deletePurchasePricingRule: deletePurchasePricingRule
        });

        function addPurchasePricingRule(onPurchasePricingRuleAdded, type) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPricingRuleAdded = onPurchasePricingRuleAdded;
            };
            var parameters = {
                PricingType: WhS_BE_PricingTypeEnum.Purchase.value
            };
            if (type != undefined) {
                parameters.PricingRuleType = type;
            }



            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingRule/PricingRuleEditor.html', parameters, settings);
        }

        function editPurchasePricingRule(obj, onPurchasePricingRuleUpdated) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPricingRuleUpdated = onPurchasePricingRuleUpdated;
            };
            var parameters = {
                RuleId: obj.RuleId,
                PricingType: obj.PricingType
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingRule/PricingRuleEditor.html', parameters, settings);
        }

        function deletePurchasePricingRule($scope, purchasePricingRuleObj, onPurchasePricingRuleDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_BE_PurchasePricingRuleAPIService.DeleteRule(purchasePricingRuleObj.Entity.RuleId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Purchase Pricing Rule", deletionResponse);
                                onPurchasePricingRuleDeleted(purchasePricingRuleObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }
    }

    appControllers.service('WhS_BE_PurchasePricingRuleService', PurchasePricingRuleService);

})(appControllers);
