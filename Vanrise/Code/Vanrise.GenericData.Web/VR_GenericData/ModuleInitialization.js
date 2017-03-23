app.run(['VR_GenericData_GenericRule', 'VR_GenericData_GenericBusinessEntityService', function (VR_GenericData_GenericRule, VR_GenericData_GenericBusinessEntityService) {
    VR_GenericData_GenericRule.registerObjectTrackingDrillDownToGenericRule();
    VR_GenericData_GenericBusinessEntityService.registerObjectTrackingDrillDownToGenericBusinessEntity();
}]);
