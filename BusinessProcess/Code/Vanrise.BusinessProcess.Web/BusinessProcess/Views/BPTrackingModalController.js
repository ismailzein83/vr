BPTrackingModalController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'BusinessProcessAPIService'];
function BPTrackingModalController($scope, VRNotificationService, VRNavigationService, BusinessProcessAPIService) {

    var ctrl = this;
    ctrl.close =  function () {
        $scope.modalContext.closeModal();
    };

    loadParameters();
    
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        ctrl.BPInstanceID = undefined;
        if (parameters != undefined && parameters != null)
            ctrl.BPInstanceID = parameters.BPInstanceID;
    }

    function load() {
        ctrl.isGettingData = true;
        getTracking().finally(function () {
            ctrl.isGettingData = false;
        });
    }

    function getTracking() {
        return BusinessProcessAPIService.GetTracking(ctrl.BPInstanceID)
           .then(function (response) {
               
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

}
appControllers.controller('BPTrackingModalController', BPTrackingModalController);
