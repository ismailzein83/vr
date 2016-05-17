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

        function addDataRecordTypeFieldFilter(fields, filterObj, onDataRecordFieldTypeFilterAdded) {
            var modalParameters = {
                Fields: fields,
                FilterObj: filterObj
            };

            var modalSettings = {width:'80%'};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldTypeFilterAdded = onDataRecordFieldTypeFilterAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordTypeField/DataRecordTypeFieldFilterEditor.html', modalParameters, modalSettings);
        }
    };

    appControllers.service('VR_GenericData_DataRecordTypeService', DataRecordTypeService);

})(appControllers);
