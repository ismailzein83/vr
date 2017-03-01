(function (appControllers) {

    "use strict";

    portalAccountEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_BE_AccountTypeAPIService', 'Retail_BE_PortalAccountAPIService'];

    function portalAccountEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_BE_AccountTypeAPIService, Retail_BE_PortalAccountAPIService) {

        var accountBEDefinitionId;
        var parentAccountId;
        var name;
        var email;
        var connectionId;
        var tenantId;
        var accountGenericFieldValuesByName;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                parentAccountId = parameters.parentAccountId;

                var context = parameters.context;
                if (context != undefined) {
                    name = context.getName();
                    email = context.getEmail();
                    connectionId = context.getConnectionId();
                    tenantId = context.getTenantId();
                }
            }
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                return insertPortalAccount();
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

            getAccountGenericFieldValues().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getAccountGenericFieldValues() {
            var accountGenericFieldNames = [];
            accountGenericFieldNames.push(name);
            accountGenericFieldNames.push(email);

            return Retail_BE_AccountTypeAPIService.GetAccountGenericFieldValues(accountBEDefinitionId, parentAccountId, UtilsService.serializetoJson(accountGenericFieldNames)).then(function (response) {
                accountGenericFieldValuesByName = response;
            });
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
            $scope.title = UtilsService.buildTitleForAddEditor('PortalAccount');
        }
        function loadStaticData() {
            if (accountGenericFieldValuesByName == undefined)
                return;

            $scope.scopeModel.name = accountGenericFieldValuesByName[name];
            $scope.scopeModel.email = accountGenericFieldValuesByName[email];
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

        function buildPortalAccountObjFromScope() {
            var obj = {
                AccountBEDefinitionId: accountBEDefinitionId,
                AccountId: parentAccountId,
                Name: $scope.scopeModel.name,
                Email: $scope.scopeModel.email,
                ConnectionId: connectionId,
                TenantId: tenantId
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_PortalAccountEditorController', portalAccountEditorController);

})(appControllers);
