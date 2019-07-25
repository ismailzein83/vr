(function (appControllers) {

    'use strict';

    SwapDealAnalysisService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService', 'VR_GenericData_GenericBEActionService'];

    function SwapDealAnalysisService(VRModalService, VRNotificationService, UtilsService, VR_GenericData_GenericBEActionService) {

        function addInbound(settings, carrierAccountId, sellingNumberPlanId, beginDate, onInboundAdded) {
            var parameters = {
                settings: settings,
                carrierAccountId: carrierAccountId,
                sellingNumberPlanId: sellingNumberPlanId,
                beginDate: beginDate
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onInboundAdded = onInboundAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDealAnalysis/InboundEditor.html', parameters, settings);
        }

        function editInbound(settings, carrierAccountId, sellingNumberPlanId, beginDate, inboundEntity, onInboundUpdated) {
            var parameters = {
                settings: settings,
                carrierAccountId: carrierAccountId,
                sellingNumberPlanId: sellingNumberPlanId,
                inboundEntity: inboundEntity,
                beginDate: beginDate
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onInboundUpdated = onInboundUpdated;
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDealAnalysis/InboundEditor.html', parameters, settings);
        }

        function addOutbound(settings, carrierAccountId, onOutboundAdded) {
            var parameters = {
                settings: settings,
                carrierAccountId: carrierAccountId
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onOutboundAdded = onOutboundAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDealAnalysis/OutboundEditor.html', parameters, settings);
        }

        function editOutbound(settings, carrierAccountId, outboundEntity, onOutboundUpdated) {
            var parameters = {
                settings: settings,
                carrierAccountId: carrierAccountId,
                outboundEntity: outboundEntity
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onOutboundUpdated = onOutboundUpdated;
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDealAnalysis/OutboundEditor.html', parameters, settings);
        }

        function registerCreateSwapDealGenericBEAction() {
            var CreateSwapDealActionType = {
                ActionTypeName: "CreateSwapDealGenericBEAction",
                ExecuteAction: function (payload) {

                    if (payload == undefined)
                        return;

                    var promiseDeferred = UtilsService.createPromiseDeferred();
                    var settings = {};

                    settings.onScopeReady = function (modalScope) {
                        modalScope.IsItemInserted = function (flag) {
                            promiseDeferred.resolve(flag);
                        };
                    };
                    VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDeal/SwapDealEditor.html', payload, settings).finally(function () {
                        promiseDeferred.resolve();
                    });

                    return promiseDeferred.promise;
                }
            };
            VR_GenericData_GenericBEActionService.registerActionType(CreateSwapDealActionType);
        }

        function registerViewSwapDealGenericBEAction() {
            var CreateSwapDealActionType = {
                ActionTypeName: "ViewSwapDealGenericBEAction",
                ExecuteAction: function (payload) {
                    var settings = {};

                    if (payload == undefined)
                        return;

                    settings.onScopeReady = function (modalScope) {
                        UtilsService.setContextReadOnly(modalScope);
                    };
                    VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDeal/SwapDealEditor.html', payload, settings);

                }
            };
            VR_GenericData_GenericBEActionService.registerActionType(CreateSwapDealActionType);
        }
        return {
            addInbound: addInbound,
            editInbound: editInbound,
            addOutbound: addOutbound,
            editOutbound: editOutbound,
            registerCreateSwapDealGenericBEAction: registerCreateSwapDealGenericBEAction,
            registerViewSwapDealGenericBEAction: registerViewSwapDealGenericBEAction
        };
    }

    appControllers.service('WhS_Deal_SwapDealAnalysisService', SwapDealAnalysisService);

})(appControllers);