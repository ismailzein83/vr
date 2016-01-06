(function (appControllers) {

    "use strict";

    sellingNumberPlanEditorController.$inject = ['$scope', 'WhS_BE_SellingNumberPlanAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function sellingNumberPlanEditorController($scope, WhS_BE_SellingNumberPlanAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        var sellingNumberPlanId;
        var editMode;
        defineScope();
        loadParameters();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                sellingNumberPlanId = parameters.SellingNumberPlanId;
            }
            editMode = (sellingNumberPlanId != undefined);
        }
        function defineScope() {
            $scope.saveSellingNumberPlan = function () {
                if (editMode) {
                    return updatesellingNumberPlan();
                }
                else {
                    return insertsellingNumberPlan();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };


        }

        function load() {
            $scope.isGettingData = true;
            if (editMode) {
                getSellingNumberPlan();
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor("SellingNumberPlan");
                $scope.isGettingData = false;
            }

        }
        function getSellingNumberPlan() {
            return WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlan(sellingNumberPlanId).then(function (sellingNumberPlan) {
                fillScopeFromSellingNumberPlanObj(sellingNumberPlan);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function buildSellingNumberPlanObjFromScope() {
            var obj = {
                SellingNumberPlanId: (sellingNumberPlanId != null) ? sellingNumberPlanId : 0,
                Name: $scope.name
            };
            return obj;
        }

        function fillScopeFromSellingNumberPlanObj(sellingNumberPlan) {
            $scope.name = sellingNumberPlan.Name;
            $scope.title = UtilsService.buildTitleForUpdateEditor(sellingNumberPlan.Name, "SellingNumberPlan");
        }
        function insertsellingNumberPlan() {
            var sellingNumberPlanObject = buildSellingNumberPlanObjFromScope();
            return WhS_BE_SellingNumberPlanAPIService.AddSellingNumberPlan(sellingNumberPlanObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Selling Number Plan", response, "Name")) {

                    if ($scope.onSellingNumberPlanAdded != undefined)
                        $scope.onSellingNumberPlanAdded(response.InsertedObject);
                   
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }
        function updatesellingNumberPlan() {
            var sellingNumberPlanObject = buildSellingNumberPlanObjFromScope();
            WhS_BE_SellingNumberPlanAPIService.UpdateSellingNumberPlan(sellingNumberPlanObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Selling Number Plan", response, "Name")) {
                    if ($scope.onSellingNumberPlanUpdated != undefined)
                        $scope.onSellingNumberPlanUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('WhS_BE_SellingNumberPlanEditorController', sellingNumberPlanEditorController);
})(appControllers);
