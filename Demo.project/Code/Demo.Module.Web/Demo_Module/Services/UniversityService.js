app.service('Demo_Module_UniversityService', ['VRModalService', 'Demo_Module_UniversityAPIService', 'VRNotificationService',
function (VRModalService, Demo_Module_UniversityAPIService, VRNotificationService) {
    var drillDownDefinitions = [];
    return ({
        editUniversity: editUniversity,
        addUniversity: addUniversity,
        deleteUniversity: deleteUniversity

    });
    function addUniversity(onUniversityAdded) {
        var settings = {
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onUniversityAdded = onUniversityAdded;
        };
        var parameters = {};


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/UniversityEditor.html', parameters, settings);
    }
    function editUniversity(universityId, onUniversityUpdated) {
        var settings = {
        };
        var parameters = {
            universityId: universityId
        };
        settings.onScopeReady = function (modalScope) {
            modalScope.onUniversityUpdated = onUniversityUpdated;
        };


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/UniversityEditor.html', parameters, settings);
    }
    function deleteUniversity(scope, dataItem, onUniversityDeleted) {
        VRNotificationService.showConfirmation().then(function (confirmed) {
            if (confirmed) {
                return Demo_Module_UniversityAPIService.DeleteUniversity(dataItem.Entity.UniversityId).then(function (responseObject) {
                    var deleted = VRNotificationService.notifyOnItemDeleted('University', responseObject);

                    if (deleted && onUniversityDeleted && typeof onUniversityDeleted == 'function') {
                        onUniversityDeleted(dataItem);
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, scope);
                })
            }
        });
    }

}]);