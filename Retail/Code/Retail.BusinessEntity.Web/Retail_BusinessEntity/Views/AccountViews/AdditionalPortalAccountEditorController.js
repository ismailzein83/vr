(function (appControllers) {

    "use strict";

    additionalPortalAccountEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_BE_AccountTypeAPIService', 'Retail_BE_PortalAccountAPIService'];

    function additionalPortalAccountEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_BE_AccountTypeAPIService, Retail_BE_PortalAccountAPIService) {

        var accountBEDefinitionId;
        var parentAccountId;
        var accountViewDefinitionId;
        var portalAccountEntity;
        var isEditMode;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                parentAccountId = parameters.parentAccountId;
                if (parameters.accountViewDefinition != undefined)
                    accountViewDefinitionId = parameters.accountViewDefinition.AccountViewDefinitionId
                portalAccountEntity = parameters.portalAccountEntity;

            }
            isEditMode = (portalAccountEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    updatePortalAccount();
                else
                    insertPortalAccount();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.hasSavePortalAccountPermission = function () {
                return Retail_BE_PortalAccountAPIService.HasAddPortalAccountPermission();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
                loadAllControls();
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        }
        function setTitle() {
            if(!isEditMode)
                $scope.title = UtilsService.buildTitleForAddEditor('PortalAccount');
            else
                $scope.title = UtilsService.buildTitleForUpdateEditor(portalAccountEntity.Name, 'PortalAccount');
        }
        function loadStaticData() {
            if (portalAccountEntity != undefined) {
                $scope.scopeModel.name = portalAccountEntity.Name;
                $scope.scopeModel.email = portalAccountEntity.Email;
            }
        }

        function insertPortalAccount() {
            $scope.scopeModel.isLoading = true;

            return Retail_BE_PortalAccountAPIService.AddAdditionalPortalAccount(buildPortalAccountObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Portal Account", response, "Email")) {
                        if ($scope.onPortalAccountAdded != undefined)
                            $scope.onPortalAccountAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function updatePortalAccount() {
            $scope.scopeModel.isLoading = true;

            return Retail_BE_PortalAccountAPIService.UpdateAdditionalPortalAccount(buildPortalAccountObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Portal Account", response, "Email")) {
                        if ($scope.onPortalAccountUpdated != undefined)
                            $scope.onPortalAccountUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function buildPortalAccountObjFromScope() {
            var obj = {
                AccountBEDefinitionId: accountBEDefinitionId,
                AccountId: parentAccountId,
                AccountViewDefinitionId: accountViewDefinitionId,
                Name: $scope.scopeModel.name,
                Email: $scope.scopeModel.email
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_AdditionalPortalAccountEditorController', additionalPortalAccountEditorController);

})(appControllers);
