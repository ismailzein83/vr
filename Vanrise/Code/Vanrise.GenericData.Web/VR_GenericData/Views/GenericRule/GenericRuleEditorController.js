(function (appControllers) {

    "use strict";

    genericRuleEditorController.$inject = ['$scope', 'VR_GenericData_GenericRuleDefinitionAPIService', 'VR_GenericData_DataRecordFieldTypeConfigAPIService', 'UtilsService', 'VRNavigationService', 'VRNotificationService'];

    function genericRuleEditorController($scope, VR_GenericData_GenericRuleDefinitionAPIService, VR_GenericData_DataRecordFieldTypeConfigAPIService, UtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        //var routeRuleId;
        var genericRuleDefinitionId;
        var genericRuleDefintion;
        var genericRuleEntity;


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {

                genericRuleDefinitionId = parameters.genericRuleDefinitionId;
            }
            isEditMode = false;
            //isEditMode = (routeRuleId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.saveGenericRule = function () {
                //if (isEditMode) {
                //    return updateRouteRule();
                //}
                //else {
                //    return insertRouteRule();
                //}
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.beginEffectiveDate = new Date();
            $scope.scopeModel.endEffectiveDate = undefined;
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            getGenericRuleDefinition().then(function () {
                if (isEditMode) {
                    getGenericRule().then(function () {
                        loadAllControls()
                            .finally(function () {
                                genericRuleEntity = undefined;
                            });
                    }).catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModal.isLoading = false;
                    });
                }
                else {
                    loadAllControls();
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModal.isLoading = false;
            });

            
        }

        function getGenericRuleDefinition() {
            return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(genericRuleDefinitionId).then(function (response) {
                genericRuleDefintion = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadCriteriaSection])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModal.isLoading = false;
               });
        }

        function getGenericRule() {
            //return WhS_Routing_RouteRuleAPIService.GetRule(routeRuleId).then(function (routeRule) {
            //    routeRuleEntity = routeRule;
            //    routingProductId = routeRuleEntity.Criteria != null ? routeRuleEntity.Criteria.RoutingProductId : undefined;
            //});
        }

        function setTitle() {
            if (isEditMode && genericRuleEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(genericRuleEntity.Name, 'Generic Rule');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Generic Rule');
        }

        function loadStaticSection() {
            if (genericRuleEntity == undefined)
                return;

            //$scope.scopeModal.beginEffectiveDate = routeRuleEntity.BeginEffectiveTime;
            //$scope.scopeModal.endEffectiveDate = routeRuleEntity.EndEffectiveTime;

            //if (routeRuleEntity.Criteria != null) {

            //    angular.forEach(routeRuleEntity.Criteria.ExcludedCodes, function (item) {
            //        $scope.scopeModal.excludedCodes.push(item);
            //    });
            //}
        }

        function loadCriteriaSection()
        {
            if (genericRuleDefintion == undefined || genericRuleDefintion.CriteriaDefinition == null)
                return;

            var promises = [];

            var dataFieldTypesConfig = [];

            var loadFieldTypeConfigPromise = VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
                dataFieldTypesConfig = response;

                angular.forEach(genericRuleDefintion.CriteriaDefinition.Fields, function (field) {
                    var dataFieldTypeConfig = UtilsService.getItemByVal(dataFieldTypesConfig, field.FieldType.ConfigId, 'DataRecordFieldTypeConfigId');
                    field.dynamicGroupUIControl.directive = dataFieldTypeConfig.DynamicGroupUIControl;
                    field.dynamicGroupUIControl.onDirectiveReady = function(api)
                        {
                            field.dynamicGroupUIControl.directiveAPI = api;
                            field.dynamicGroupUIControl.directiveAPI.load(field)
                        }
                    }
                    
                });

                $scope.scopeModel.criteriaFields = genericRuleDefintion.CriteriaDefinition.Fields;

            });

            promises.push(loadFieldTypeConfigPromise);
            
            return UtilsService.waitMultiplePromises(promises);
        }

        function buildRouteRuleObjFromScope() {

            //var routeRule = {
            //    RuleId: (routeRuleId != null) ? routeRuleId : 0,
            //    Criteria: {
            //        RoutingProductId: routingProductId,
            //        ExcludedCodes: $scope.scopeModal.excludedCodes,
            //        SaleZoneGroupSettings: saleZoneGroupSettingsAPI.getData(),//VRUIUtilsService.getSettingsFromDirective($scope, saleZoneGroupSettingsAPI, 'selectedSaleZoneGroupTemplate'),
            //        CustomerGroupSettings: customerGroupSettingsAPI.getData(),
            //        CodeCriteriaGroupSettings: VRUIUtilsService.getSettingsFromDirective($scope.scopeModal, codeCriteriaGroupSettingsAPI, 'selectedCodeCriteriaGroupTemplate')
            //    },
            //    Settings: VRUIUtilsService.getSettingsFromDirective($scope.scopeModal, routeRuleSettingsAPI, 'selectedrouteRuleSettingsTemplate'),
            //    BeginEffectiveTime: $scope.scopeModal.beginEffectiveDate,
            //    EndEffectiveTime: $scope.scopeModal.endEffectiveDate
            //};

            //return routeRule;
        }

        function insertRouteRule() {
            //var routeRuleObject = buildRouteRuleObjFromScope();
            //return WhS_Routing_RouteRuleAPIService.AddRule(routeRuleObject)
            //.then(function (response) {
            //    if (VRNotificationService.notifyOnItemAdded("Route Rule", response)) {
            //        if ($scope.onRouteRuleAdded != undefined)
            //            $scope.onRouteRuleAdded(response.InsertedObject);
            //        $scope.modalContext.closeModal();
            //    }
            //}).catch(function (error) {
            //    VRNotificationService.notifyException(error, $scope);
            //});

        }

        function updateRouteRule() {
            //var routeRuleObject = buildRouteRuleObjFromScope();
            //WhS_Routing_RouteRuleAPIService.UpdateRule(routeRuleObject)
            //.then(function (response) {
            //    if (VRNotificationService.notifyOnItemUpdated("Route Rule", response)) {
            //        if ($scope.onRouteRuleUpdated != undefined)
            //            $scope.onRouteRuleUpdated(response.UpdatedObject);
            //        $scope.modalContext.closeModal();
            //    }
            //}).catch(function (error) {
            //    VRNotificationService.notifyException(error, $scope);
            //});
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleEditorController', genericRuleEditorController);
})(appControllers);
