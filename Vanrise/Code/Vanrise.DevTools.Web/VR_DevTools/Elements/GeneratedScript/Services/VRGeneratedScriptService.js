appControllers.service('VR_Devtools_GeneratedScriptService', ['VRModalService', 'VRNotificationService',
    function (VRModalService, VRNotificationService) {

        function addGeneratedScriptDesign(onGeneratedScriptDesignAdded, generatedScripts) {

            var settings = {};
            var parameters = {};
            settings.onScopeReady = function (modalScope) {

                modalScope.onGeneratedScriptDesignAdded = onGeneratedScriptDesignAdded;

            };
            VRModalService.showModal('/Client/Modules/VR_DevTools/Elements/GeneratedScript/Views/VRGeneratedScriptDesignEditor.html', parameters, settings);
        } 

        function editGeneratedScriptDesign(onGeneratedScriptDesignUpdated, design) {
            var settings = {};
            var parameters = {
                design: design
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGeneratedScriptDesignUpdated = onGeneratedScriptDesignUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_DevTools/Elements/GeneratedScript/Views/VRGeneratedScriptDesignEditor.html', parameters, settings);
        }

        function chooseSelectedTableDataColumns(payload,deselectAllItems,columnNames, generateSelectedTableDataGrid) {
            var settings = {};
            var parameters = {
                payload: payload,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.generateSelectedTableDataGrid = generateSelectedTableDataGrid;
                modalScope.columnNames = columnNames;
                modalScope.deselectAllItems = deselectAllItems;

            };
            VRModalService.showModal('/Client/Modules/VR_DevTools/Elements/GeneratedScript/Views/VRGeneratedScriptColumnsSelectorEditor.html', parameters, settings);
        }

        function displayQueries(queries) {
            var settings = {};
            var parameters = {
                queries: queries
            };

            settings.onScopeReady = function (modalScope) {
            };
            VRModalService.showModal('/Client/Modules/VR_DevTools/Elements/GeneratedScript/Views/VRGeneratedScriptQueriesDisplayer.html', parameters, settings);
        }

        function editTableCell(modifySelectedTableData, cellValue, getVariables, originalCellValue) {
            var settings = {};
            var parameters = {
                cellValue: cellValue,
                originalCellValue: originalCellValue
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.modifySelectedTableData = modifySelectedTableData;
                modalScope.getVariables = getVariables;
            };
            VRModalService.showModal('/Client/Modules/VR_DevTools/Elements/GeneratedScript/Views/VRGeneratedScriptTableCellEditor.html', parameters, settings);
        }

        function addGeneratedScriptVariable(onGeneratedScriptVariableAdded, connectionStringId) {

            var settings = {};
            var parameters = {
                connectionStringId: connectionStringId
            };
            settings.onScopeReady = function (modalScope) {

                modalScope.onGeneratedScriptVariableAdded = onGeneratedScriptVariableAdded;

            };
            VRModalService.showModal('/Client/Modules/VR_DevTools/Elements/GeneratedScript/Views/VRGeneratedScriptVariableEditor.html', parameters, settings);
        }

        function editGeneratedScriptVariable(onGeneratedScriptVariableUpdated, variable, connectionStringId) {
            var settings = {};
            var parameters = {
                variable: variable,
                connectionStringId: connectionStringId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGeneratedScriptVariableUpdated = onGeneratedScriptVariableUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_DevTools/Elements/GeneratedScript/Views/VRGeneratedScriptVariableEditor.html', parameters, settings);
        }
        return {
            addGeneratedScriptDesign: addGeneratedScriptDesign,
            editGeneratedScriptDesign: editGeneratedScriptDesign,
            displayQueries: displayQueries,
            editTableCell: editTableCell,
            addGeneratedScriptVariable: addGeneratedScriptVariable,
            editGeneratedScriptVariable: editGeneratedScriptVariable,
            chooseSelectedTableDataColumns: chooseSelectedTableDataColumns
        };

    }]);