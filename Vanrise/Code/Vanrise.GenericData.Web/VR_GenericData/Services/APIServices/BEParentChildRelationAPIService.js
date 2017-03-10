﻿(function (appControllers) {

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

        function HasAddChildRelationPermission(beParentChildRelationDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'DoesUserHaveAddccess'), {
                beParentChildRelationDefinitionId: beParentChildRelationDefinitionId
            });
        }


        function AddBEParentChildRelation(beParentChildRelationItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'AddBEParentChildRelation'), beParentChildRelationItem);
        }

        function UpdateBEParentChildRelation(beParentChildRelationItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'UpdateBEParentChildRelation'), beParentChildRelationItem);
        }

        //function GetBEParentChildRelationsInfo(filter) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetBEParentChildRelationsInfo'), {
        //        filter: filter
        //    });
        //}

        return {
            GetFilteredBEParentChildRelations: GetFilteredBEParentChildRelations,
            GetBEParentChildRelation: GetBEParentChildRelation,
            AddBEParentChildRelation: AddBEParentChildRelation,
            UpdateBEParentChildRelation: UpdateBEParentChildRelation,
            HasAddChildRelationPermission: HasAddChildRelationPermission
            //GetBEParentChildRelationsInfo: GetBEParentChildRelationsInfo
        };
    }

    appControllers.service('VR_GenericData_BEParentChildRelationAPIService', BEParentChildRelationAPIService);

})(appControllers);