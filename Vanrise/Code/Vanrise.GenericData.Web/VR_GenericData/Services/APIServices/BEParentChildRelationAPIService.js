(function (appControllers) {

    'use strict';

    BEParentChildRelationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig', 'SecurityService'];

    function BEParentChildRelationAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig, SecurityService) {

        var controllerName = 'BEParentChildRelation';

        function GetFilteredBEParentChildRelations(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetFilteredBEParentChildRelations'), input);
        }

        function GetBEParentChildRelation(beParentChildRelationDefinitionId, beParentChildRelationId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetBEParentChildRelation'), {
                beParentChildRelationDefinitionId: beParentChildRelationDefinitionId,
                beParentChildRelationId: beParentChildRelationId
            });
        }

        function AddBEParentChildrenRelation(beParentChildrenRelation) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'AddBEParentChildrenRelation'), beParentChildrenRelation);
        }

        function UpdateBEParentChildRelation(beParentChildRelationItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'UpdateBEParentChildRelation'), beParentChildRelationItem);
        }

        function GetLastAssignedEED(beParentChildRelationDefinitionId, beChildId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetLastAssignedEED'), {
                beParentChildRelationDefinitionId: beParentChildRelationDefinitionId,
                beChildId: beChildId
            });
        }

        function HasAddChildRelationPermission(beParentChildRelationDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'DoesUserHaveAddAccess'), {
                beParentChildRelationDefinitionId: beParentChildRelationDefinitionId
            });
        }

        return {
            GetFilteredBEParentChildRelations: GetFilteredBEParentChildRelations,
            GetBEParentChildRelation: GetBEParentChildRelation,
            AddBEParentChildrenRelation: AddBEParentChildrenRelation,
            UpdateBEParentChildRelation: UpdateBEParentChildRelation,
            GetLastAssignedEED: GetLastAssignedEED,
            HasAddChildRelationPermission: HasAddChildRelationPermission
        };
    }

    appControllers.service('VR_GenericData_BEParentChildRelationAPIService', BEParentChildRelationAPIService);

})(appControllers);