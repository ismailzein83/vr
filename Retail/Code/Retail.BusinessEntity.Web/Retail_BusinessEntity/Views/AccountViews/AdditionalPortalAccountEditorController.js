(function (appControllers) {

    "use strict";

    additionalPortalAccountEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_BE_AccountTypeAPIService', 'Retail_BE_PortalAccountAPIService'];

    function additionalPortalAccountEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_BE_AccountTypeAPIService, Retail_BE_PortalAccountAPIService) {

        var accountBEDefinitionId;
        var parentAccountId;
        var accountViewDefinitionId;
        var portalAccountEntity;
        var isEditMode;
        var name;
        var email;
        var accountGenericFieldValuesByName;
        var isPrimaryPortalAccount;
        var userId;
        var portalAccountEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                userId=parameters.userId;
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                parentAccountId = parameters.parentAccountId;
                var context = parameters.context;
                if (context != undefined) {
                    accountViewDefinitionId = context.getAccountViewDefinitionId();
                    name = context.getName();
                    email = context.getEmail();
                    isPrimaryPortalAccount = parameters.isPrimaryPortalAccount;
                }
            }
            isEditMode = (userId != undefined);
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
            if (isEditMode) {
                Retail_BE_PortalAccountAPIService.GetPortalAccount(accountBEDefinitionId, parentAccountId, accountViewDefinitionId, userId).then(function (response) {
                    portalAccountEntity = response;

                    getAccountGenericFieldValues().then(function () {
                        loadAllControls();
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModel.isLoading = false;
                    });
                });
            }
            else
            {
                getAccountGenericFieldValues().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
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
            if (isPrimaryPortalAccount) {
                if (accountGenericFieldValuesByName == undefined)
                    return;
                $scope.scopeModel.name = accountGenericFieldValuesByName[name];
                $scope.scopeModel.email = accountGenericFieldValuesByName[email];
            }
            if (portalAccountEntity != undefined) {
                $scope.scopeModel.name = portalAccountEntity.Name;
                $scope.scopeModel.email = portalAccountEntity.Email;
            }
        }

        function getAccountGenericFieldValues() {
            var accountGenericFieldNames = [];
            accountGenericFieldNames.push(name);
            accountGenericFieldNames.push(email);
            return Retail_BE_AccountTypeAPIService.GetAccountGenericFieldValues(accountBEDefinitionId, parentAccountId, UtilsService.serializetoJson(accountGenericFieldNames)).then(function (response) {
                accountGenericFieldValuesByName = response;
                
            });
        }

        function insertPortalAccount() {
            $scope.scopeModel.isLoading = true;
            return Retail_BE_PortalAccountAPIService.AddPortalAccount(buildPortalAccountObjFromScope())
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

            return Retail_BE_PortalAccountAPIService.UpdatePortalAccount(buildPortalAccountObjFromScope())
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
            if (isEditMode)
                obj.UserId = userId;
            return obj;
        }
    }

    appControllers.controller('Retail_BE_AdditionalPortalAccountEditorController', additionalPortalAccountEditorController);

})(appControllers);
