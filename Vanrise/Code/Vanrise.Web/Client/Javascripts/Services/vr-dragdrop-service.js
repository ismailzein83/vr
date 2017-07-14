
(function (appControllers) {

    "use strict";
    VRDragdropService.$inject = ['UtilsService'];

    function VRDragdropService(UtilsService) {

        function createCorrelationGroup() {
            var groupName = "groupe" + UtilsService.guid();
            return {
                getGroupName: function () {
                    return groupName;
                }
            }
        }
        

        return ({
            createCorrelationGroup: createCorrelationGroup
        });
    }

    appControllers.service('VRDragdropService', VRDragdropService);
})(appControllers);


