(function (appControllers) {

    "use strict";

    companyDefinitionEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function companyDefinitionEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var companyDefinitionEntity;

        var companyDefinitionSelelctiveApi;
        var companyDefinitionSelelctiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                companyDefinitionEntity = parameters.companyDefinitionEntity;
            }
            isEditMode = (companyDefinitionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onCompanyDefinitionSelectiveReady = function (api) {
                companyDefinitionSelelctiveApi = api;
                companyDefinitionSelelctiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveCompanyDefinition = function () {
                if (isEditMode)
                    return updateCompanyDefinition();
                else
                    return insertCompanyDefinition();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            function setTitle() {
                if (isEditMode && companyDefinitionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(companyDefinitionEntity.Name, "Company Definition");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Company Definition");
            }

            function loadStaticData() {

                if (companyDefinitionEntity == undefined)
                    return;
                $scope.scopeModel.name = companyDefinitionEntity.Name;

            }

            function loadCompanyDefinitionSelective() {
                var companyDefinitionSelectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                companyDefinitionSelelctiveReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload = {
                            selectedIds: companyDefinitionEntity != undefined ? companyDefinitionEntity.Setting : undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(companyDefinitionSelelctiveApi, directivePayload, companyDefinitionSelectiveLoadPromiseDeferred);
                    });
                return companyDefinitionSelectiveLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCompanyDefinitionSelective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }
        function buildCompanyDefinitionObjFromScope() {
            var obj = {
                Name: $scope.scopeModel.name,
                Setting: companyDefinitionSelelctiveApi.getData(),
                CompanyDefinitionSettingId: (isEditMode) ? companyDefinitionEntity.CompanyDefinitionSettingId : UtilsService.guid(),
            };
            return obj;
        }

        function insertCompanyDefinition() {
            var companyDefinitionObject = buildCompanyDefinitionObjFromScope();

            if ($scope.onCompanyDefinitionAdded != undefined)
                $scope.onCompanyDefinitionAdded(companyDefinitionObject);
            $scope.modalContext.closeModal();
        }

        function updateCompanyDefinition() {
            var companyDefinitionObject = buildCompanyDefinitionObjFromScope();
            if ($scope.onCompanyDefinitionUpdated != undefined)
                $scope.onCompanyDefinitionUpdated(companyDefinitionObject);
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('VRCommon_CompanyDefinitionEditorController', companyDefinitionEditorController);
})(appControllers);
