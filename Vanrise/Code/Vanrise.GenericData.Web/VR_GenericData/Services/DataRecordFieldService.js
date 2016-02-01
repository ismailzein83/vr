(function (appControllers) {

    'use strict';

    DataRecordFieldService.$inject = ['VRModalService','VRNotificationService'];

    function DataRecordFieldService(VRModalService, VRNotificationService) {
        return ({
            addDataRecordField: addDataRecordField,
            editDataRecordField: editDataRecordField,
            deleteDataRecordField: deleteDataRecordField
        });

        function addDataRecordField(onDataRecordFieldAdded, existingFields) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldAdded = onDataRecordFieldAdded;
            };
            var modalParameters = {
                ExistingFields: existingFields
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericDataRecord/DataRecordFieldEditor.html', modalParameters, modalSettings);
        }

        function editDataRecordField(dataRecordField, onDataRecordFieldUpdated, existingFields) {
            var modalParameters = {
                DataRecordField: dataRecordField,
                ExistingFields: existingFields
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldUpdated = onDataRecordFieldUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericDataRecord/DataRecordFieldEditor.html', modalParameters, modalSettings);
        }
        function deleteDataRecordField($scope, dataRecordFieldObj, onDataRecordFieldDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                       onDataRecordFieldDeleted(dataRecordFieldObj);
                    }
                });
        }
    };

    appControllers.service('VR_GenericData_DataRecordFieldService', DataRecordFieldService);

})(appControllers);
