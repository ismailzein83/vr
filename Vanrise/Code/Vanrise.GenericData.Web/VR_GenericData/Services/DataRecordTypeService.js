(function (appControllers) {

    'use strict';

    DataRecordTypeService.$inject = ['VRModalService'];

    function DataRecordTypeService(VRModalService) {
        return ({
            addDataRecordType: addDataRecordType,
            editDataRecordType: editDataRecordType,
            addDataRecordTypeFieldFilter: addDataRecordTypeFieldFilter
        });

        function addDataRecordType(onDataRecordTypeAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordTypeAdded = onDataRecordTypeAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericDataRecord/DataRecordTypeEditor.html', null, modalSettings);
        }

        function editDataRecordType(dataRecordTypeId, onDataRecordTypeUpdated) {
            var modalParameters = {
                DataRecordTypeId: dataRecordTypeId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordTypeUpdated = onDataRecordTypeUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericDataRecord/DataRecordTypeEditor.html', modalParameters, modalSettings);
        }

        function addDataRecordTypeFieldFilter(dataRecordTypeId, onDataRecordFieldTypeAdded) {
            var modalParameters = {
                DataRecordTypeId: dataRecordTypeId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldTypeAdded = onDataRecordFieldTypeAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordTypeField/DataRecordTypeFieldFilterEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('VR_GenericData_DataRecordTypeService', DataRecordTypeService);

})(appControllers);
