﻿(function (appControllers) {

    "use strict";

    VRNamespaceService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function VRNamespaceService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];

        function addVRNamespace(onVRNamespaceAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRNamespaceAdded = onVRNamespaceAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRNamespace/VRNamespaceEditor.html', null, settings);
        }

        function editVRNamespace(onVRNamespaceUpdated, vrNamespaceId) {
            var settings = {};

            var parameters = {
                vrNamespaceId: vrNamespaceId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRNamespaceUpdated = onVRNamespaceUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRNamespace/VRNamespaceEditor.html', parameters, settings);
        }

        function addVRDynamicCode(onVRDynamicCodeAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRDynamicCodeAdded = onVRDynamicCodeAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRNamespace/VRDynamicCodeEditor.html', null, settings);
        }

        function editVRDynamicCode(onVRDynamicCodeUpdated, vrDynamicCodeEntity) {
            var settings = {};
            var parameters = {
                vrDynamicCodeEntity: vrDynamicCodeEntity
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRDynamicCodeUpdated = onVRDynamicCodeUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRNamespace/VRDynamicCodeEditor.html', parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_Common_VRNamespace";
        }

        function registerObjectTrackingDrillDownToVRNamespace() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, vrNamespaceItem) {
                vrNamespaceItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: vrNamespaceItem.VRNamespaceId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return vrNamespaceItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }

        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        function tryCompilationResult(errorMessages, namespaceObj) {
            var modalSettings = {};
            var modalParameters = {
                errorMessages: errorMessages,
                namespaceObj: namespaceObj
            };
            modalSettings.onScopeReady = function (modalScope) {
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRNamespace/VRNamespaceCompilationResult.html', modalParameters, modalSettings);
        }

        return {
            addVRNamespace: addVRNamespace,
            editVRNamespace: editVRNamespace,
            registerObjectTrackingDrillDownToVRNamespace: registerObjectTrackingDrillDownToVRNamespace,
            getDrillDownDefinition: getDrillDownDefinition,
            tryCompilationResult: tryCompilationResult,
            addVRDynamicCode: addVRDynamicCode,
            editVRDynamicCode: editVRDynamicCode,
            addDrillDownDefinition: addDrillDownDefinition
        };
    }

    appControllers.service('VRCommon_VRNamespaceService', VRNamespaceService);

})(appControllers);