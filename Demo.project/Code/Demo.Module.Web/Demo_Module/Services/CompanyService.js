app.service('Demo_Module_CompanyService', ['VRModalService', 'Demo_Module_CompanyAPIService', 'VRNotificationService','VRCommon_ObjectTrackingService',
function (VRModalService, Demo_Module_CompanyAPIService, VRNotificationService, VRCommon_ObjectTrackingService) {
    var drillDownDefinitions = [];
    return {
        addCompany: addCompany,
        editCompany: editCompany,
        addDrillDownDefinition: addDrillDownDefinition,
        getDrillDownDefinition: getDrillDownDefinition
       
    };

    
   
    function addCompany(onCompanyAdded) {
        
        var settings = {};
        var parameters = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onCompanyAdded = onCompanyAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/CompanyEditor.html', parameters, settings);
    };

    function editCompany(companyId, onCompanyUpdated) {
        var settings = {};
        var parameters = {
            companyId: companyId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onCompanyUpdated = onCompanyUpdated;
            
        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/CompanyEditor.html', parameters, settings);
    };




    function addDrillDownDefinition(drillDownDefinition) {
        drillDownDefinitions.push(drillDownDefinition);
    };

    function getDrillDownDefinition() {
        return drillDownDefinitions;
    };



}]);