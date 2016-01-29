(function (appControllers) {

    'use strict';

    DataRecordTypeService.$inject = ['VRModalService'];

    function DataRecordTypeService(VRModalService) {
        return ({
            addDataRecordType: addDataRecordType,
            editDataRecordType: editDataRecordType,
        });

        function addDataRecordType(onDataRecordTypeAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordTypeAdded = onDataRecordTypeAdded;
            };

            VRModalService.showModal('/Client/Modules/GenericData/Views/VR_GenericData/DataRecordTypeEditor.html', null, modalSettings);
        }

        function editDataRecordType(dataRecordTypeId, onDataRecordTypeUpdated) {
            var modalParameters = {
                DataRecordTypeId: dataRecordTypeId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordTypeUpdated = onDataRecordTypeUpdated;
            };

            VRModalService.showModal('/Client/Modules/GenericData/Views/VR_GenericData/DataRecordTypeEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('VR_GenericData_DataRecordTypeService', DataRecordTypeService);

})(appControllers);
