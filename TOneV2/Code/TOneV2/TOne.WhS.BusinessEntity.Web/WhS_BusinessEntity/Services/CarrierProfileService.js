﻿(function (appControllers) {

    'use strict';

    CarrierProfileService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function CarrierProfileService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];

        return ({
            addCarrierProfile: addCarrierProfile,
            editCarrierProfile: editCarrierProfile,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            getEntityUniqueName: getEntityUniqueName
        });

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

      
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    }

    appControllers.service('WhS_BE_CarrierProfileService', CarrierProfileService);

})(appControllers);
