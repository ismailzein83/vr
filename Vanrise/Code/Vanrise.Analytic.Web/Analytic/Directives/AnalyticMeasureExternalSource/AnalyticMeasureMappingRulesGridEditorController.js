(function (appControllers) {

    "use strict";
    MeasureMappingRulesGridEditorController.$inject = ["$scope", "VRNavigationService", "UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function MeasureMappingRulesGridEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var ruleEntity;
        var isEditMode;
        var tableId;

        var measureSelectiveAPI;
        var measureSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {

                context = parameters.context;
                ruleEntity = parameters.ruleEntity;
                tableId = parameters.tableId;
             
            }
            isEditMode = (ruleEntity != undefined);

        }
        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onMeasureSelectiveReady = function (api) {
                measureSelectiveAPI = api;
                measureSelectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateRule() : addRule();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function buildRuleObjFromScope() {
                return {
                    MeasureMappingRuleId: ruleEntity != undefined ? ruleEntity.MeasureMappingRuleId : UtilsService.guid(),
                    Name: $scope.scopeModel.name,
                    Settings: measureSelectiveAPI.getData(),
                };
            }

            function addRule() {
                var ruleObject = buildRuleObjFromScope();
                if ($scope.onRuleAdded != undefined)
                    $scope.onRuleAdded(ruleObject);
                $scope.modalContext.closeModal();
            }
            function updateRule() {
                var ruleObject = buildRuleObjFromScope();
                if ($scope.onRuleUpdated != undefined)
                    $scope.onRuleUpdated(ruleObject);
                $scope.modalContext.closeModal();

            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && ruleEntity != undefined) {
                    var entity = ruleEntity.Entity;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(entity.Name, " Measure Mapping Rule");
                }
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Measure Mapping Rule");
            }

            function loadStaticData() {
                if (ruleEntity != undefined)
                    $scope.scopeModel.name = ruleEntity.Entity != undefined ? ruleEntity.Entity.Name : undefined;
            }

            function loadMeasureSelective() {

                var loadMeasureSelectiveDeferred = UtilsService.createPromiseDeferred();

                measureSelectiveReadyDeferred.promise.then(function () {

                    var payload = {
                        context: getContext(),
                        ruleEntity: ruleEntity,
                        tableId: tableId,
                    };
                    VRUIUtilsService.callDirectiveLoad(measureSelectiveAPI, payload, loadMeasureSelectiveDeferred);
                });
                return loadMeasureSelectiveDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadMeasureSelective])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};

            return currentContext;
        }
    }
    app.controller("VR_Analytic_MeasureMappingRulesGridEditorController", MeasureMappingRulesGridEditorController);
})(appControllers);
