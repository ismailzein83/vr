(function (appControllers) {

    'use strict';

    DataRecordFieldChoiceService.$inject = ['VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function DataRecordFieldChoiceService(VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function editDataRecordFieldChoice(dataRecordFieldChoiceId, onDataRecordFieldChoiceUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldChoiceUpdated = onDataRecordFieldChoiceUpdated;
            };
            var parameters = {
                DataRecordFieldChoiceId: dataRecordFieldChoiceId
            };

            VRModalService.showModal("/Client/Modules/VR_GenericData/Views/DataRecordFieldChoice/DataRecordFieldChoiceEditor.html", parameters, settings);
        }

        function addDataRecordFieldChoice(onDataRecordFieldChoiceAdded) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldChoiceAdded = onDataRecordFieldChoiceAdded;
            };
            var parameters = {};

            VRModalService.showModal("/Client/Modules/VR_GenericData/Views/DataRecordFieldChoice/DataRecordFieldChoiceEditor.html", parameters, settings);
        }
        
        function getEntityUniqueName() {
            return "VR_GenericData_DataRecordFieldChoice";
        }

        function registerObjectTrackingDrillDownToDataRecordFieldChoice() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, dataRecordFieldChoiceItem) {
                dataRecordFieldChoiceItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: dataRecordFieldChoiceItem.Entity.DataRecordFieldChoiceId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return dataRecordFieldChoiceItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return ({
            editDataRecordFieldChoice: editDataRecordFieldChoice,
            addDataRecordFieldChoice: addDataRecordFieldChoice,
            registerObjectTrackingDrillDownToDataRecordFieldChoice: registerObjectTrackingDrillDownToDataRecordFieldChoice,
            getDrillDownDefinition: getDrillDownDefinition
        });
    }

    appControllers.service('VR_GenericData_DataRecordFieldChoiceService', DataRecordFieldChoiceService);

})(appControllers);
