(function (appControllers) {

    'use strict';

    DealService.$inject = ['VRModalService', 'VRNotificationService', 'WhS_BE_DealContractTypeEnum', 'WhS_BE_DealAgreementTypeEnum', 'UtilsService'];

    function DealService(VRModalService, VRNotificationService, WhS_BE_DealContractTypeEnum, WhS_BE_DealAgreementTypeEnum, UtilsService) {
        var editorUrl = '/Client/Modules/WhS_BusinessEntity/Views/Deal/DealEditor.html';

        function addDeal(onDealAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealAdded = onDealAdded;
            };

            VRModalService.showModal(editorUrl, null, settings);
        }

        function editDeal(dealId, onDealUpdated) {
            var parameters = {
                dealId: dealId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealUpdated = onDealUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function addNeedsFields(entity) {
            if (entity.Settings.EndDate != null) {
                var myDate = new Date(entity.Settings.EndDate);
                entity.Settings.GraceDate = new Date(myDate.setDate(myDate.getDate() + entity.Settings.GracePeriod));
                    } else {
                entity.Settings.GraceDate = null;
                    }

            entity.Settings.AgreementType =
                        UtilsService.getEnum(WhS_BE_DealAgreementTypeEnum, 'value', entity.Settings.AgreementType).description;

            entity.Settings.ContractType =
                        UtilsService.getEnum(WhS_BE_DealContractTypeEnum, 'value', entity.Settings.ContractType).description;
            var today = new Date();
            var bed = new Date(entity.Settings.BeginDate);
            var eed = new Date(entity.Settings.EndDate);

            entity.Settings.Effective = (today >= bed && today <= eed) == true ? "Yes" : "No";
            entity.Settings.Active = entity.Settings.Active == true ? "Yes" : "No";
        }
        return {
            addDeal: addDeal,
            editDeal: editDeal,
            addNeedsFields: addNeedsFields
        };
    }

    appControllers.service('WhS_BE_DealService', DealService);

})(appControllers);