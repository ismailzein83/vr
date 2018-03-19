(function (appControllers) {

    'use strict';

    RatePlanService.$inject = ['VRModalService'];

    function RatePlanService(VRModalService) {
        return {
            manageCountries: manageCountries,
            editSettings: editSettings,
            editPricingSettings: editPricingSettings,
            viewFutureRate: viewFutureRate,
            viewInvalidRates: viewInvalidRates,
            viewZoneInfo: viewZoneInfo,
            openBulkActionWizard: openBulkActionWizard,
            openTQIEditor: openTQIEditor,
            importRatePlan: importRatePlan,
            areDatesTheSame: areDatesTheSame,
            applyDraftOnMultipleCustomers: applyDraftOnMultipleCustomers
        };

        function manageCountries(input) {

            var parameters = {
                customerId: input.customerId,
                countryChanges: input.countryChanges,
                customerPricingSettings: input.customerPricingSettings
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onCountryChangesUpdated = input.onCountryChangesUpdated;
            };

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/ManageCountries.html", parameters, settings);
        }

        function editSettings(settings, onSettingsUpdated) {
            var parameters = {
                settings: settings
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSettingsUpdated = onSettingsUpdated;
            };

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/RatePlanSettings.html", parameters, modalSettings);
        }

        function editPricingSettings(ratePlanSettings, pricingSettings, onPricingSettingsUpdated) {
            var parameters = {
                ratePlanSettings: ratePlanSettings,
                pricingSettings: pricingSettings
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPricingSettingsUpdated = onPricingSettingsUpdated;
            };

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/RatePlanPricingSettings.html", parameters, modalSettings);
        }

        function openTQIEditor(context, onTQIEvaluated) {
            var parameters = {
                context: context
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onTQIEvaluated = onTQIEvaluated;
            };

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/TQI/TQIEditor.html", parameters, settings);
        }

        function viewFutureRate(zoneName, futureRate, primarySaleEntity, ownerType) {
            var parameters = {
                zoneName: zoneName,
                futureRate: futureRate,
                primarySaleEntity: primarySaleEntity,
                ownerType:ownerType
            };

            var settings = {};

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/FutureRate.html", parameters, settings);
        }

        function viewInvalidRates(calculatedRates, onSaved) {
            var parameters = {
                calculatedRates: calculatedRates
            };

            var settings = {
                onScopeReady: function (modalScope) {
                    modalScope.onSaved = onSaved;
                }
            };

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/InvalidRate.html", parameters, settings);
        }

        function viewZoneInfo(ownerType, ownerId, zoneId, zoneName, zoneBED, zoneEED, currencyId, countryId, primarySaleEntity) {
            var parameters = {
                ownerType: ownerType,
                ownerId: ownerId,
                zoneId: zoneId,
                zoneName: zoneName,
                zoneBED: zoneBED,
                zoneEED: zoneEED,
                currencyId: currencyId,
                countryId: countryId,
                primarySaleEntity: primarySaleEntity
            };

            var settings;

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/ZoneInfo.html", parameters, settings);
        }

        function openBulkActionWizard(input) {

            var parameters = {
                ownerType: input.ownerType,
                ownerId: input.ownerId,
                ownerSellingNumberPlanId: input.ownerSellingNumberPlanId,
                gridQuery: input.gridQuery,
                routingDatabaseId: input.routingDatabaseId,
                policyConfigId: input.policyConfigId,
                numberOfOptions: input.numberOfOptions,
                currencyId: input.currencyId,
                longPrecision: input.longPrecision,
                pricingSettings: input.pricingSettings
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBulkActionAppliedToDraft = input.onBulkActionAppliedToDraft;
            };

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/BulkAction/BulkActionWizard.html", parameters, modalSettings);
        }

        function importRatePlan(ownerType, ownerId, onRatePlanImported) {
            var parameters = {
                ownerType: ownerType,
                ownerId: ownerId
            };

            var modalSettings = {
                onScopeReady: function (modalScope) {
                    modalScope.onRatePlanImported = onRatePlanImported;
                }
            };
            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/ImportRatePlan/ImportRatePlan.html", parameters, modalSettings);
        }

        function areDatesTheSame(date1, date2) {
            if (date1 && date2) {
                if (typeof date1 == 'string')
                    date1 = new Date(date1);
                if (typeof date2 == 'string')
                    date2 = new Date(date2);
                return (date1.getDay() == date2.getDay() && date1.getMonth() == date2.getMonth() && date1.getYear() == date2.getYear());
            }
            else if (!date1 && !date2)
                return true;
            else
                return false;
        }

        function applyDraftOnMultipleCustomers(executeApplyDraftOnMultipleCustomersProcess, ownerId, sellingNumberPlanId, customerSellingProductId)
        {
            var parameters = {
                ownerId: ownerId,
                sellingNumberPlanId: sellingNumberPlanId,
                customerSellingProductId: customerSellingProductId
            };
            var modalSettings = {
                onScopeReady: function (modalScope) {
                    modalScope.executeApplyDraftOnMultipleCustomersProcess = executeApplyDraftOnMultipleCustomersProcess;
                }
            };
            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/applyDraftOnMultipleCustomers.html", parameters, modalSettings);
        }
    }

    appControllers.service('WhS_Sales_RatePlanService', RatePlanService);

})(appControllers);