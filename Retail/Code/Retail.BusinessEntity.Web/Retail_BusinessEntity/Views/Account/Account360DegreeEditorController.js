(function (appControllers) {

    'use strict';

    Account360DegreeEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_AccountBEAPIService', 'Retail_BE_AccountBEDefinitionAPIService'];

    function Account360DegreeEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountBEAPIService, Retail_BE_AccountBEDefinitionAPIService) {

        var accountBEDefinitionId;
        var accountId;
        var accountEntity;
        var accountViewDefinitions;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                accountId = parameters.accountId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.accountViews = [];

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            UtilsService.waitMultipleAsyncOperations([getAccount, getAccountViewsDefinitions]).then(function () {
                loadAllControls();
            });
        }

        function getAccount() {
            return Retail_BE_AccountBEAPIService.GetAccount(accountBEDefinitionId, accountId).then(function (response) {
                accountEntity = response;
            });
        }
        function getAccountViewsDefinitions() {
            return Retail_BE_AccountBEDefinitionAPIService.GetAccountViewDefinitionsByAccountId(accountBEDefinitionId, accountId).then(function (response) {
                accountViewDefinitions = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountViews]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (accountEntity != undefined) ? "Account Name: " + accountEntity.Name : undefined;
        }
        function loadStaticData() {
            if (accountEntity == undefined)
                return;
        }
        function loadAccountViews() {

            if (accountViewDefinitions != undefined) {
                for (var i = 0; i < accountViewDefinitions.length ; i++) {
                    var accountViewDefinition = accountViewDefinitions[i];
                    AddAccountView(accountViewDefinition);
                }
            }

            function AddAccountView(accountViewDefinition) {
                if (accountViewDefinition.Account360DegreeSectionName != undefined) {
                    var view = {
                        header: accountViewDefinition.Account360DegreeSectionName,
                        runtimeEditor: accountViewDefinition.Settings.RuntimeEditor,
                        onDirectiveReady: function (api) {
                            view.directiveAPI = api;
                            var setLoader = function (value) {
                                view.isLoadingDirective = value;
                            };
                            var payloadDirective = {
                                accountViewDefinition: accountViewDefinition,
                                accountBEDefinitionId: accountBEDefinitionId,
                                parentAccountId: accountId
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, view.directiveAPI, payloadDirective, setLoader);
                        }
                    };
                    $scope.scopeModel.accountViews.push(view);
                }
            }
        }
    }

    appControllers.controller('Retail_BE_Account360DegreeEditorController', Account360DegreeEditorController);

})(appControllers);