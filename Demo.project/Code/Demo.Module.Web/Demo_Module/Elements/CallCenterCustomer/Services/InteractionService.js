app.service('Demo_Module_InteractionService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function displayCall() {

        var settings = {};
        var parameters = {};

        
        VRModalService.showModal('/Client/Modules/Demo_Module/Elements/CallCenterCustomer/Views/CallEditor.html', parameters, settings);
    };


    return {
        displayCall: displayCall
    };

}]);