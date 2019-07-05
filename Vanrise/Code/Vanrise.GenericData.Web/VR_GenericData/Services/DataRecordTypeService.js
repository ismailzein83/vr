(function (appControllers) {

    'use strict';

    DataRecordTypeService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function DataRecordTypeService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addDataRecordType: addDataRecordType,
            editDataRecordType: editDataRecordType,
            addDataRecordTypeFieldFilter: addDataRecordTypeFieldFilter,
            registerObjectTrackingDrillDownToDataRecordType: registerObjectTrackingDrillDownToDataRecordType,
            getDrillDownDefinition: getDrillDownDefinition,
            addListDataRecordTypeRow: addListDataRecordTypeRow,
            editListDataRecordTypeRow: editListDataRecordTypeRow,
            addListDataRecordTypeGridGenericEditorViewRow: addListDataRecordTypeGridGenericEditorViewRow,
            editListDataRecordTypeGridGenericEditorViewRow: editListDataRecordTypeGridGenericEditorViewRow
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
        function addListDataRecordTypeRow(dataRecordTypeId,definitionSettings,onRowAdded,title) {
            var modalParameters = {
                dataRecordTypeId: dataRecordTypeId,
                definitionSettings: definitionSettings,
                title: title
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRowAdded = onRowAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeRuntime/Templates/GridViewEditorTemplate.html', modalParameters, modalSettings);
        }
        function editListDataRecordTypeRow(dataRow, dataRecordTypeId, definitionSettings, onRowUpdated, title) {
            var modalParameters = {
                dataRecordTypeId: dataRecordTypeId,
                definitionSettings: definitionSettings,
                dataRow: dataRow,
                title: title
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRowUpdated = onRowUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeRuntime/Templates/GridViewEditorTemplate.html', modalParameters, modalSettings);
        }

        function addListDataRecordTypeGridGenericEditorViewRow(context, onRowAdded) {
            var modalParameters = {
                context: context,
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRowAdded = onRowAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeDefinition/Templates/GridGenericEditorViewTypeTemplate.html', modalParameters, modalSettings);
        }
        function editListDataRecordTypeGridGenericEditorViewRow(dataRow, context,  onRowUpdated) {
            var modalParameters = {
                context: context,
                dataRow: dataRow,
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRowUpdated = onRowUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeDefinition/Templates/GridGenericEditorViewTypeTemplate.html', modalParameters, modalSettings);
        }
        func


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
        function getEntityUniqueName() {
            return "VR_GenericData_DataRecordType";
        }

        function registerObjectTrackingDrillDownToDataRecordType() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, dataRecordTypeItem) {
                dataRecordTypeItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: dataRecordTypeItem.Entity.DataRecordTypeId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return dataRecordTypeItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    };

    appControllers.service('VR_GenericData_DataRecordTypeService', DataRecordTypeService);

})(appControllers);
