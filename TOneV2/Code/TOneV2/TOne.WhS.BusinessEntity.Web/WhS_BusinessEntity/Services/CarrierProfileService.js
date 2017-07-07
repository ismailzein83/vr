(function (appControllers) {

    'use strict';

    CarrierProfileService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService', 'UtilsService'];

    function CarrierProfileService(VRModalService, VRCommon_ObjectTrackingService, UtilsService) {
        var drillDownDefinitions = [];

        return ({
            addCarrierProfile: addCarrierProfile,
            editCarrierProfile: editCarrierProfile,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            getEntityUniqueName: getEntityUniqueName,
            registerHistoryViewAction: registerHistoryViewAction
        });

        function viewHistoryCarrierProfile(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierProfileEditor.html', modalParameters, modalSettings);
        };


        function addCarrierProfile(onCarrierProfileAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onCarrierProfileAdded = onCarrierProfileAdded;
            };            
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierProfileEditor.html', null, settings);
        }

        function editCarrierProfile(carrierProfileObj, onCarrierProfileUpdated) {
            var modalSettings = {
            };

            var parameters = {
                CarrierProfileId: carrierProfileObj.Entity.CarrierProfileId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCarrierProfileUpdated = onCarrierProfileUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierProfileEditor.html', parameters, modalSettings);
        }
        function getEntityUniqueName() {
            return "WhS_BusinessEntity_CarrierProfile";
        }
        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "WhS_BusinessEntity_CarrierProfile_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryCarrierProfile(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }

      
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    }

    appControllers.service('WhS_BE_CarrierProfileService', CarrierProfileService);

})(appControllers);
