(function (appControllers) {

    'use strict';

    DataRecordFieldService.$inject = ['VRModalService'];

    function DataRecordFieldService(VRModalService) {
        return ({
            addDataRecordField: addDataRecordField,
            editDataRecordField: editDataRecordField,
        });

        function addDataRecordField(onDataRecordFieldAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldAdded = onDataRecordFieldAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordFieldEditor.html', null, modalSettings);
        }

        function editDataRecordField(dataRecordField, onDataRecordTypeUpdated) {
            var modalParameters = {
                DataRecordField: dataRecordField
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordTypeUpdated = onDataRecordTypeUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordFieldEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('VR_GenericData_DataRecordFieldService', DataRecordFieldService);

})(appControllers);
