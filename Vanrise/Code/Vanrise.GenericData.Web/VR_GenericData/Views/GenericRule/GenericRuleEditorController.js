(function (appControllers) {

    "use strict";

    genericRuleEditorController.$inject = ['$scope', 'UtilsService'];

    function genericRuleEditorController($scope, UtilsService) {

        var isEditMode;

        //var routeRuleId;

        var genericRuleEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            //var parameters = VRNavigationService.getParameters($scope);

            //if (parameters != undefined && parameters != null) {

            //    routeRuleId = parameters.routeRuleId;
            //    routingProductId = parameters.routingProductId;
            //    sellingNumberPlanId = parameters.sellingNumberPlanId;
            //}
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

            //if (isEditMode) {
            //    $scope.title = "Edit Route Rule";
            //    getRouteRule().then(function () {
            //        loadAllControls()
            //            .finally(function () {
            //                routeRuleEntity = undefined;
            //            });
            //    }).catch(function () {
            //        VRNotificationService.notifyExceptionWithClose(error, $scope);
            //        $scope.scopeModal.isLoading = false;
            //    });
            //}
            //else {
            //    $scope.title = "New Route Rule";
            //    loadAllControls();
            //}
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection])
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
