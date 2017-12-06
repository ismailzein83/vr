(function (appControllers) {

    'use strict';

    AccountManagerAssignmentService.$inject = ['VRModalService', 'VRUIUtilsService'];

    function AccountManagerAssignmentService(VRModalService, VRUIUtilsService) {

        return ({
         
            getEntityUniqueName: getEntityUniqueName
        });
        function getEntityUniqueName(accountManagerAssignmnetDefinitionId) {
            return "VR_AccountManager_AccountManagerAssignment_" + accountManagerAssignmnetDefinitionId;
        }


    }

    appControllers.service('VR_AccountManager_AccountManagerAssignmentService', AccountManagerAssignmentService);

})(appControllers);
