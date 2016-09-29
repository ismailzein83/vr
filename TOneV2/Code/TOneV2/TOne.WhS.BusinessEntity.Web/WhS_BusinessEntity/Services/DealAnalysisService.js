(function (appControllers) {

    'use strict';

    DealAnalysisService.$inject = ['VRModalService', 'VRNotificationService', 'WhS_BE_DealContractTypeEnum', 'WhS_BE_DealAgreementTypeEnum', 'UtilsService'];

    function DealAnalysisService(VRModalService, VRNotificationService, WhS_BE_DealContractTypeEnum, WhS_BE_DealAgreementTypeEnum, UtilsService) {
        var editorUrl = '/Client/Modules/WhS_BusinessEntity/Views/Deal/DealAnalysisEditor.html';
        var outboundUrl = '/Client/Modules/WhS_BusinessEntity/Views/Deal/OutboundTrafficEditor.html';
        var inboundUrl = '/Client/Modules/WhS_BusinessEntity/Views/Deal/InboundTrafficEditor.html';

        function addDealAnalysis(onDealAnalysisAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealAnalysisAdded = onDealAnalysisAdded;
            };

            VRModalService.showModal(editorUrl, null, settings);
        }

        function openDealAnalysis(dealDefinition) {
            var parameters = {
                dealDefinition: dealDefinition
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
             //   modalScope.onDealAnalyisUpdated = onDealAnalyisUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }


        function editDealAnalysis(dealId, onDealAnalyisUpdated) {
            var parameters = {
                dealId: dealId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealAnalyisUpdated = onDealAnalyisUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function addOutbound(onOutboundAdded, supplierId) {
            var settings = {};
            var parameters = {
                supplierId: supplierId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onOutboundAdded = onOutboundAdded;
            };
            VRModalService.showModal(outboundUrl, parameters, settings);
        }

        function editOutbound(outbound, supplierId, onOutboundUpdated) {
            var parameters = {
                outbound: outbound,                
                supplierId: supplierId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onOutboundUpdated = onOutboundUpdated;
            };

            VRModalService.showModal(outboundUrl, parameters, settings);
        }
        function addInbound(onInboundAdded, sellingNumberPlanId) {
            var settings = {};
            var parameters = {                
                sellingNumberPlanId: sellingNumberPlanId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onInboundAdded = onInboundAdded;
            };
            VRModalService.showModal(inboundUrl, parameters, settings);
        }

        function editInbound(inbound, sellingNumberPlanId, onInboundUpdated) {
            var parameters = {
                inbound: inbound,
                sellingNumberPlanId: sellingNumberPlanId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onInboundUpdated = onInboundUpdated;
            };

            VRModalService.showModal(inboundUrl, parameters, settings);
        }

        return {
            addDealAnalysis: addDealAnalysis,
            editDealAnalysis: editDealAnalysis,
            openDealAnalysis:openDealAnalysis,
            addOutbound: addOutbound,
            editOutbound: editOutbound,
            addInbound: addInbound,
            editInbound: editInbound
        };
    }

    appControllers.service('WhS_BE_DealAnalysisService', DealAnalysisService);

})(appControllers);