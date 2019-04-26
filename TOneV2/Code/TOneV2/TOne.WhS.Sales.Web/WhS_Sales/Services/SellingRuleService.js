﻿(function (appControllers) {

    'use strict';

    SellingRuleService.$inject = ['WhS_Sales_SellingRuleAPIService', 'VRModalService', 'VRNotificationService'];

    function SellingRuleService(WhS_Sales_SellingRuleAPIService, VRModalService, VRNotificationService) {
        return ({
            addSellingRule: addSellingRule,
            editSellingRule: editSellingRule,
            deleteSellingRule: deleteSellingRule
        });

        function addSellingRule(onSellingRuleAdded, routingProductId, sellingNumberPlanId) {
            var settings = {
            };

            var parameters = {
                routingProductId: routingProductId,
                sellingNumberPlanId: sellingNumberPlanId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSellingRuleAdded = onSellingRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Sales/Views/SellingRule/SellingRuleEditor.html', parameters, settings);
        }

        function editSellingRule(sellingRuleId, onSellingRuleUpdated) {
            var modalSettings = {
            };
            var parameters = {
                sellingRuleId: sellingRuleId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSellingRuleUpdated = onSellingRuleUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_Sales/Views/SellingRule/SellingRuleEditor.html', parameters, modalSettings);
        }

        function deleteSellingRule(scope, sellingRuleObj, onSellingRuleDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_Sales_SellingRuleAPIService.DeleteRule(sellingRuleObj.Entity.RuleId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Selling Rule", deletionResponse);
                                onSellingRuleDeleted(sellingRuleObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, scope);
                            });
                    }
                });
        }
    }

    appControllers.service('WhS_Sales_SellingRuleService', SellingRuleService);

})(appControllers);
(function (appControllers) {

	'use strict';

	SupplierTargetMatchService.$inject = ['VRModalService', 'VRNotificationService'];

	function SupplierTargetMatchService(VRModalService, VRNotificationService) {
		return ({
			exportSupplierTargetMatch: exportSupplierTargetMatch,
		});

		function exportSupplierTargetMatch(onExportSupplierTargetMatch) {
			var settings = {
			};

			var parameters;

			settings.onScopeReady = function (modalScope) {
				modalScope.onExportSupplierTargetMatch = onExportSupplierTargetMatch;
			};

			VRModalService.showModal('/Client/Modules/WhS_Sales/Views/SupplierTargetMatch/ExportSupplierTargetMatchEditor.html', parameters, settings);
		}
	}

	appControllers.service('WhS_Sales_SupplierTargetMatchService', SupplierTargetMatchService);

})(appControllers);
