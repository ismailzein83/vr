(function (appControllers) {

    'use strict';

    DataRecordFieldService.$inject = ['VRModalService', 'VRNotificationService'];


    function DataRecordFieldService(VRModalService, VRNotificationService) {
        function editDataRecordField(onDataRecordFieldUpdated, formula, existingFields, showDependantFieldsGrid) {

            var modalParameters = {
                ExistingFields: existingFields,
                showDependantFieldsGrid: showDependantFieldsGrid,
                Formula: formula
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldUpdated = onDataRecordFieldUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericDataRecord/DataRecordFieldEditor.html', modalParameters, modalSettings);
        }
        return {
            editDataRecordField: editDataRecordField
        };
    }
    appControllers.service('VR_GenericData_DataRecordFieldService', DataRecordFieldService);
})(appControllers);