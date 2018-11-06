app.service('Demo_Module_PageDefinitionService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addPageDefinition(onPageDefinitionAdded) {

        var settings = {};
        var parameters = {

        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onPageDefinitionAdded = onPageDefinitionAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/PageDefinitionEditor.html', parameters, settings);
    };

    function editPageDefinition(pageDefinitionId, onPageDefinitionUpdated) {
        var settings = {};
        var parameters = {
            pageDefinitionId:pageDefinitionId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onPageDefinitionUpdated = onPageDefinitionUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/PageDefinitionEditor.html', parameters, settings);
    };

    function addPageDefinitionField(onPageDefinitionFieldAdded, fieldAdder) {

        var settings = {};
        var parameters = {

        };
        settings.onScopeReady = function (modalScope) {
            modalScope.onPageDefinitionFieldAdded = onPageDefinitionFieldAdded;
            modalScope.fieldAdder = fieldAdder;

        };

        VRModalService.showModal('/Client/Modules/Demo_Module/Views/PageDefinitionFieldEditor.html', parameters, settings);

    }

    function editPageDefinitionField(onPageDefinitionFieldUpdated, pageDefinitionFieldEntity, fieldUpdater,index) {

        var settings = {};
        var parameters = { pageDefinitionFieldEntity: pageDefinitionFieldEntity,index:index };
        settings.onScopeReady = function (modalScope) {
            modalScope.onPageDefinitionFieldUpdated = onPageDefinitionFieldUpdated;
            modalScope.fieldUpdater = fieldUpdater;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/PageDefinitionFieldEditor.html', parameters, settings);

    }
   
    function addPageDefinitionSubview(onPageDefinitionSubviewAdded) {

        var settings = {};
        var parameters = {

        };
        settings.onScopeReady = function (modalScope) {
            modalScope.onPageDefinitionSubviewAdded = onPageDefinitionSubviewAdded;
        };

        VRModalService.showModal('/Client/Modules/Demo_Module/Views/PageDefinitionSubviewEditor.html', parameters, settings);

    }

    function editPageDefinitionSubview(onPageDefinitionSubviewUpdated, pageDefinitionSubviewEntity) {

        var settings = {};
        var parameters = { pageDefinitionSubviewEntity: pageDefinitionSubviewEntity };
        settings.onScopeReady = function (modalScope) {
            modalScope.onPageDefinitionSubviewUpdated = onPageDefinitionSubviewUpdated;
        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/PageDefinitionSubviewEditor.html', parameters, settings);

    }
    
   
    
    return {
        addPageDefinition: addPageDefinition,
        editPageDefinition: editPageDefinition,
        addPageDefinitionField: addPageDefinitionField,
        editPageDefinitionField: editPageDefinitionField,
        addPageDefinitionSubview: addPageDefinitionSubview,
        editPageDefinitionSubview: editPageDefinitionSubview
        
    };

}]);