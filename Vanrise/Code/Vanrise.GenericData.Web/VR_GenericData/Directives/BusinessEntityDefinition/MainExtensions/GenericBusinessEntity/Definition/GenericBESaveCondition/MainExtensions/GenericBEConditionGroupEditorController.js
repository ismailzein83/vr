(function (appControllers) {

    "use strict";

    GenericBEConditionGroupEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericBEConditionGroupEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var conditionGroup;
        var context;


        var conditionsDirectiveAPI;
        var conditionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                conditionGroup = parameters.conditionGroup;
                context = parameters.context;
            }
            isEditMode = (conditionGroup != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGenericBeConditionSettingsDirectiveReady = function (api) {
                conditionsDirectiveAPI = api;
                conditionSettingsReadyPromiseDeferred.resolve();
            };



            $scope.scopeModel.saveConditionGroup = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }
        function load() {

            loadAllControls();

            function loadAllControls() {
                $scope.scopeModel.isLoading = true;
                function setTitle() {
                    if (isEditMode && conditionGroup != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(conditionGroup.Name, 'Condition Group Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Condition Group Editor');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.name = conditionGroup.Name;
                    $scope.scopeModel.applicableOnOldEntity = conditionGroup.ApplicableOnOldEntity;

                }

                function loadConditionSettingDirectiveSection() {
                    var loadConditionSettingPromiseDeferred = UtilsService.createPromiseDeferred();
                    conditionSettingsReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            context: getContext(),
                            settings: conditionGroup != undefined && conditionGroup.Condition != undefined ? conditionGroup.Condition : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(conditionsDirectiveAPI, payload, loadConditionSettingPromiseDeferred);
                    });
                    return loadConditionSettingPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadConditionSettingDirectiveSection]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildConditionGroupFromScope() {
            return {
                Name: $scope.scopeModel.name,
                ApplicableOnOldEntity: $scope.scopeModel.applicableOnOldEntity,
                Condition: conditionsDirectiveAPI.getData()
            };
        }

        function insert() {
            var conditionGroup = buildConditionGroupFromScope();
            if ($scope.onGenericBEConditionGroupAdded != undefined) {
                $scope.onGenericBEConditionGroupAdded(conditionGroup);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var conditionGroup = buildConditionGroupFromScope();
            if ($scope.onGenericBEConditionGroupUpdated != undefined) {
                $scope.onGenericBEConditionGroupUpdated(conditionGroup);
            }
            $scope.modalContext.closeModal();
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }

    appControllers.controller('VR_GenericData_GenericBEConditionGroupEditorController', GenericBEConditionGroupEditorController);
})(appControllers);
