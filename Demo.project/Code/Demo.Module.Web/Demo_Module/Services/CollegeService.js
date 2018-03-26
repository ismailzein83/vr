app.service('Demo_Module_CollegeService', ['VRModalService', 'Demo_Module_CollegeAPIService', 'VRNotificationService',
function (VRModalService, Demo_Module_CollegeAPIService, VRNotificationService) {
    var drillDownDefinitions = [];
    return ({
        editCollege: editCollege,
        addCollege: addCollege,
        deleteCollege: deleteCollege

    });
    function addCollege(onCollegeAdded) {
        var settings = {
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onCollegeAdded = onCollegeAdded;
        };
        var parameters = {};


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/CollegeEditor.html', parameters, settings);
    }
    function editCollege(collegeId, collegeUniversity,onCollegeUpdated) {
        var settings = {
        };
        var parameters = {
            collegeId: collegeId,
            collegeUniversity: collegeUniversity
            
        };
        settings.onScopeReady = function (modalScope) {
            modalScope.onCollegeUpdated = onCollegeUpdated;
        };


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/CollegeEditor.html', parameters, settings);
    }
    function deleteCollege(scope, dataItem, onCollegeDeleted) {
        VRNotificationService.showConfirmation().then(function (confirmed) {
            if (confirmed) {
                return Demo_Module_CollegeAPIService.DeleteCollege(dataItem.CollegeId).then(function (responseObject) {
                    var deleted = VRNotificationService.notifyOnItemDeleted('College', responseObject);

                    if (deleted && onCollegeDeleted && typeof onCollegeDeleted == 'function') {
                        onCollegeDeleted(dataItem);
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, scope);
                })
            }
        });
    }

}]);