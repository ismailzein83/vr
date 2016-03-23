(function (appControllers) {

    'use strict';

    KeyFieldMappingService.$inject = ['VRModalService', 'VRNotificationService'];

    function KeyFieldMappingService(VRModalService, VRNotificationService) {
        return ({
            addItem: addItem,
            editItem: editItem,
            deleteItem: deleteItem
        });

        function addItem(rawDataRecordTypeId, summaryDataRecordTypeId, onDataItemAdded, existingFields) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {

                modalScope.onDataItemAdded = onDataItemAdded;
            };
            var modalParameters = {
                ExistingFields: existingFields,
                RawDataRecordTypeId: rawDataRecordTypeId,
                SummaryDataRecordTypeId: summaryDataRecordTypeId
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/SummaryTransformationDefinition/SummaryGroupingColumnsEditor.html', modalParameters, modalSettings);
        }

        function editItem(rawDataRecordTypeId, summaryDataRecordTypeId, keyFieldMapping, onDataItemUpdated, existingFields) {
            var modalParameters = {
                KeyFieldMapping: keyFieldMapping,
                ExistingFields: existingFields,
                RawDataRecordTypeId: rawDataRecordTypeId,
                SummaryDataRecordTypeId: summaryDataRecordTypeId
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataItemUpdated = onDataItemUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/SummaryTransformationDefinition/SummaryGroupingColumnsEditor.html', modalParameters, modalSettings);
        }
        function deleteItem($scope, dataItem, onDataItemDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        onDataItemDeleted(dataItem);
                    }
                });
        }
    };

    appControllers.service('VR_GenericData_KeyFieldMappingService', KeyFieldMappingService);

})(appControllers);
