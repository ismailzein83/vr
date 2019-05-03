(function (appControllers) {

    'use strict';

    DataRecordFieldService.$inject = ['VRModalService', 'VRNotificationService'];

    function DataRecordFieldService(VRModalService, VRNotificationService) {

        function addDataRecordField(onDataRecordFieldAdded, existingFields, showDependantFieldsGrid) {

            var modalParameters = {
                ExistingFields: existingFields,
                showDependantFieldsGrid: showDependantFieldsGrid
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldAdded = onDataRecordFieldAdded;
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericDataRecord/DataRecordFieldEditor.html', modalParameters, modalSettings);
        }

        function editDataRecordField(onDataRecordFieldUpdated, dataRecordField, existingFields, showDependantFieldsGrid) {

            var modalParameters = {
                DataRecordField: dataRecordField,
                ExistingFields: existingFields,
                showDependantFieldsGrid: showDependantFieldsGrid
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldUpdated = onDataRecordFieldUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericDataRecord/DataRecordFieldEditor.html', modalParameters, modalSettings);
        }

        function deleteDataRecordField($scope, dataRecordFieldObj, onDataRecordFieldDeleted) {

            VRNotificationService.showConfirmation().then(function (response) {
                if (response) {
                    onDataRecordFieldDeleted(dataRecordFieldObj);
                }
            });
        }

        return {
            addDataRecordField: addDataRecordField,
            editDataRecordField: editDataRecordField,
            deleteDataRecordField: deleteDataRecordField
        };
    }

    appControllers.service('VR_GenericData_DataRecordFieldService', DataRecordFieldService);
})(appControllers);