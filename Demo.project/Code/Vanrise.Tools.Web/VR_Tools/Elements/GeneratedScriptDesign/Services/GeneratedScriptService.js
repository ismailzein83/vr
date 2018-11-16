appControllers.service('VR_Tools_GeneratedScriptService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addGeneratedScriptDesign(onGeneratedScriptDesignAdded, generatedScripts) {

        var settings = {};
        var parameters = {};
        settings.onScopeReady = function (modalScope) {

            modalScope.onGeneratedScriptDesignAdded = onGeneratedScriptDesignAdded;

        };
        VRModalService.showModal('/Client/Modules/VR_Tools/Elements/GeneratedScriptDesign/Views/GeneratedScriptDesignEditor.html', parameters, settings);
    };

    function editGeneratedScriptDesign(onGeneratedScriptDesignUpdated, design) {
        var settings = {};
        var parameters = {
            design: design
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onGeneratedScriptDesignUpdated = onGeneratedScriptDesignUpdated;
        };
        VRModalService.showModal('/Client/Modules/VR_Tools/Elements/GeneratedScriptDesign/Views/GeneratedScriptDesignEditor.html', parameters, settings);
    };

   
    return {
        addGeneratedScriptDesign: addGeneratedScriptDesign,
        editGeneratedScriptDesign: editGeneratedScriptDesign,
    };

}]);