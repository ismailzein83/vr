(function (appControllers) {
    
    'use strict';

    GenericBusinessEntityManagementController.$inject = ['$scope', 'VRNavigationService'];

    function GenericBusinessEntityManagementController($scope, VRNavigationService) {
        var genericBusinessEntityId;

        loadParameters();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                genericBusinessEntityId = parameters.genericBusinessEntityId;
            }
        }
    }

    appControllers.controller('VR_GenericData_GenericBusinessEntityManagementController', GenericBusinessEntityManagementController);

})(appControllers);