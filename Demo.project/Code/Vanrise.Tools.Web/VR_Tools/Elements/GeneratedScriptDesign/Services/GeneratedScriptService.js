appControllers.service('VR_Tools_GeneratedScriptService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addGeneratedScriptDesign(onGeneratedScriptDesignAdded, generatedScripts) {

        var settings = {};
        var parameters = {};
        settings.onScopeReady = function (modalScope) {

            modalScope.onGeneratedScriptDesignAdded = onGeneratedScriptDesignAdded;

        };
        VRModalService.showModal('/Client/Modules/VR_Tools/Elements/GeneratedScriptDesign/Views/GeneratedScriptDesignEditor.html', parameters, settings);
    }

    function editGeneratedScriptDesign(onGeneratedScriptDesignUpdated, design) {
        var settings = {};
        var parameters = {
            design: design
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onGeneratedScriptDesignUpdated = onGeneratedScriptDesignUpdated;
        };
        VRModalService.showModal('/Client/Modules/VR_Tools/Elements/GeneratedScriptDesign/Views/GeneratedScriptDesignEditor.html', parameters, settings);
    }

    function displayQueries(queries) {
        var settings = {};
        var parameters = {
            queries: queries
        };

        settings.onScopeReady = function (modalScope) {
        };
        VRModalService.showModal('/Client/Modules/VR_Tools/Elements/GeneratedScriptDesign/Views/GeneratedScriptQueriesDisplayer.html', parameters, settings);
    }

    function editTableCell(rowIndex, columnName, selectedTableData) {
        var settings = {};
        var parameters = {
            rowIndex: rowIndex,
            columnName: columnName,
            selectedTableData: selectedTableData
        };

    settings.onScopeReady = function (modalScope) {
        };
        VRModalService.showModal('/Client/Modules/VR_Tools/Elements/GeneratedScriptDesign/Views/GeneratedScriptTableCellEditor.html', parameters, settings);
    }
   
    return {
        addGeneratedScriptDesign: addGeneratedScriptDesign,
        editGeneratedScriptDesign: editGeneratedScriptDesign,
        displayQueries: displayQueries,
        editTableCell: editTableCell
    };

}]);