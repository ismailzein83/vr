(function (appControllers) {

    "use strict";

    sellingNumberPlanEditorController.$inject = ['$scope', 'WhS_BE_SellingNumberPlanAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function sellingNumberPlanEditorController($scope, WhS_BE_SellingNumberPlanAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var sellingNumberPlanId;
        var sellingNumberPlanEntity;
        var editMode;
        var context;
        var isViewHistoryMode;

        var lobSelectorAPI;
        var lobSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                sellingNumberPlanId = parameters.SellingNumberPlanId;
                context = parameters.context;
            }
            editMode = (sellingNumberPlanId != undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
        }
        function defineScope() {

            $scope.hasSaveSellingNumberPlanPermission = function () {
                if (editMode)
                    return WhS_BE_SellingNumberPlanAPIService.HasUpdateSellingNumberPlanPermission();
                else
                    return WhS_BE_SellingNumberPlanAPIService.HasAddSellingNumberPlanPermission();
            };

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


            $scope.onLOBSelectorReady = function (api) {
                lobSelectorAPI = api;
                lobSelectorReadyDeferred.resolve();
            };
        }

        function load() {
            $scope.isGettingData = true;
            loadAllControls().finally(function () {
                $scope.isGettingData = false;
            });
        }

        function loadAllControls() {
            var promises = [];
            if (editMode) {
                promises.push(getSellingNumberPlan());
            } else if (isViewHistoryMode) {
                promises.push(getSellingNumberPlanHistory());
            } else {
                loadStaticData();
                promises.push(loadLOBSelector());
            }

            var rootPromiseNode = {
                promises: promises,
                getChildNode: function () {
                    var childPromises = [];
                    if (editMode || isViewHistoryMode) {
                        loadStaticData();
                        childPromises.push(loadLOBSelector());
                    }
                    return {
                        promises: childPromises
                    };
                }
            };
            return UtilsService.waitPromiseNode(rootPromiseNode);
        }

        function getSellingNumberPlanHistory() {
            return WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlanHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                sellingNumberPlanEntity = response;
            });
        }

        function getSellingNumberPlan() {
            return WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlan(sellingNumberPlanId).then(function (sellingNumberPlan) {
                sellingNumberPlanEntity = sellingNumberPlan;
            });
        }
        function loadStaticData() {
            if (sellingNumberPlanEntity != undefined) {
                $scope.name = sellingNumberPlanEntity.Name ;
                $scope.title = UtilsService.buildTitleForUpdateEditor(sellingNumberPlanEntity.Name, "Selling Number Plan");
            } else {
                $scope.title = UtilsService.buildTitleForAddEditor("Selling Number Plan");
            }
        }

        function insertsellingNumberPlan() {
            $scope.isGettingData = true;
            return WhS_BE_SellingNumberPlanAPIService.AddSellingNumberPlan(buildSellingNumberPlanObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Selling Number Plan", response, "Name")) {

                        if ($scope.onSellingNumberPlanAdded != undefined)
                            $scope.onSellingNumberPlanAdded(response.InsertedObject);

                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isGettingData = false;
                });
        }
        function updatesellingNumberPlan() {
            $scope.isGettingData = true;
            return WhS_BE_SellingNumberPlanAPIService.UpdateSellingNumberPlan(buildSellingNumberPlanObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Selling Number Plan", response, "Name")) {
                        if ($scope.onSellingNumberPlanUpdated != undefined)
                            $scope.onSellingNumberPlanUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isGettingData = false;
                });
        }
        function buildSellingNumberPlanObjFromScope() {
            var obj = {
                SellingNumberPlanId: (sellingNumberPlanId != null) ? sellingNumberPlanId : 0,
                Name: $scope.name,
                LOBId: lobSelectorAPI.getSelectedIds()
            };
            return obj;
        }

        function loadLOBSelector() {
            var lobSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            lobSelectorReadyDeferred.promise.then(function () {
                var lobSelectorPayload = {
                    selectedIds: sellingNumberPlanEntity != undefined ? sellingNumberPlanEntity.LOBId : undefined
                };

                VRUIUtilsService.callDirectiveLoad(lobSelectorAPI, lobSelectorPayload, lobSelectorLoadPromiseDeferred);
            });
            return lobSelectorLoadPromiseDeferred.promise;
        }
    }

    appControllers.controller('WhS_BE_SellingNumberPlanEditorController', sellingNumberPlanEditorController);
})(appControllers);
