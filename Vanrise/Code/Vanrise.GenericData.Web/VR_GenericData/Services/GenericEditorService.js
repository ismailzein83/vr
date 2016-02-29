(function (appControllers) {

    'use strict';

    GenericEditorService.$inject = ['VR_GenericData_GenericEditorAPIService', 'VRModalService', 'VRNotificationService'];

    function GenericEditorService(VR_GenericData_GenericEditorAPIService, VRModalService, VRNotificationService) {
        return {
            editGenericEditor: editGenericEditor,
            addExtendedSettings: addExtendedSettings,
            deleteSection: deleteSection,
            addRow: addRow,
            editRow: editRow,
            deleteRow: deleteRow
        };

        function editGenericEditor(genericEditorDefinitionId, onGenericEditorUpdated) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericEditorUpdated = onGenericEditorUpdated;
            };

            var parameters = {
                genericEditorDefinitionId: genericEditorDefinitionId
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/GenericEditor.html', parameters, modalSettings);
        }

        function addExtendedSettings(businessEntityDefinitionId, onExtendedSettingsAdded) {
            var modalParameters = {
                businessEntityDefinitionId: businessEntityDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericEditorAdded = onExtendedSettingsAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/GenericEditor.html', modalParameters, modalSettings);
        }

        function deleteSection($scope, sectionObj, onSectionDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        onSectionDeleted(sectionObj);
                    }
                });
        }
        function addRow(onRowAdded, recordTypeFields) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRowAdded = onRowAdded;
            };

            var parameters = {
                recordTypeFields: recordTypeFields,
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/GenericRowEditor.html', parameters, modalSettings);
        }
        function editRow(onRowUpdated, recordTypeFields,rowEntity) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRowUpdated = onRowUpdated;
            };

            var parameters = {
                recordTypeFields: recordTypeFields,
                rowEntity: rowEntity
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/GenericRowEditor.html', parameters, modalSettings);
        }

        function deleteRow($scope, rowObj, onRowDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        onRowDeleted(rowObj);
                    }
                });
        }
    }

    appControllers.service('VR_GenericData_GenericEditorService', GenericEditorService);

})(appControllers);