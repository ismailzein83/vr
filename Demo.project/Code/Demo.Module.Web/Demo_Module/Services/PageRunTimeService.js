app.service('Demo_Module_PageRunTimeService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addPageRunTime(onPageRunTimeAdded, selectedPageDefinitionId) {

        var settings = {};
        var parameters = {
            pageDefinitionId: selectedPageDefinitionId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onPageRunTimeAdded = onPageRunTimeAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/PageRunTimeEditor.html', parameters, settings);
    };

    function editPageRunTime(pageRunTimeId,pageDefinitionId, onPageRunTimeUpdated) {
        var settings = {};
        var parameters = {
            pageDefinitionId:pageDefinitionId,
            pageRunTimeId: pageRunTimeId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onPageRunTimeUpdated = onPageRunTimeUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/PageRunTimeEditor.html', parameters, settings);
    };

   
   
    
    return {
        addPageRunTime: addPageRunTime,
        editPageRunTime: editPageRunTime
        
    };

}]);