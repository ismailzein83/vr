(function (appControllers) {

    'use strict';

    ExtensibleBEItemService.$inject = ['VR_GenericData_ExtensibleBEItemAPIService', 'VRModalService', 'VRNotificationService'];

    function ExtensibleBEItemService(VR_GenericData_ExtensibleBEItemAPIService, VRModalService, VRNotificationService) {
        return {
            editExtensibleBEItem: editExtensibleBEItem,
            addExtendedSettings: addExtendedSettings,
            deleteSection: deleteSection,
            addRow: addRow,
            editRow: editRow,
            deleteRow: deleteRow,
            editSection: editSection,
            addSection: addSection
        };

        function editExtensibleBEItem(extensibleBEItemId, onExtensibleBEItemUpdated) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericEditorUpdated = onExtensibleBEItemUpdated;
            };

            var parameters = {
                genericEditorDefinitionId: extensibleBEItemId
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/GenericExtensibleBEEditor.html', parameters, modalSettings);
        }

        function addExtendedSettings(businessEntityDefinitionId, onExtendedSettingsAdded) {
            var modalParameters = {
                businessEntityDefinitionId: businessEntityDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericEditorAdded = onExtendedSettingsAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/GenericExtensibleBEEditor.html', modalParameters, modalSettings);
        }

        function addSection(onSectionAdded, exitingSections) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSectionAdded = onSectionAdded;
            };

            var parameters = {
                exitingSections: exitingSections
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/SectionEditor.html', parameters, modalSettings);
        }

        function editSection(onSectionUpdated, exitingSections, sectionEntity) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSectionUpdated = onSectionUpdated;
            };

            var parameters = {
                sectionTitleValue: sectionEntity,
                exitingSections: exitingSections
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/SectionEditor.html', parameters, modalSettings);
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

        function editRow(onRowUpdated, recordTypeFields, rowEntity) {
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

    appControllers.service('VR_GenericData_ExtensibleBEItemService', ExtensibleBEItemService);

})(appControllers);