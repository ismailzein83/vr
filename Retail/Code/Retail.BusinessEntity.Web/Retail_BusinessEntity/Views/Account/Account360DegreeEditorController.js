(function (appControllers) {

    'use strict';

    Account360DegreeEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_AccountAPIService', 'Retail_BE_AccountDefinitionAPIService'];

    function Account360DegreeEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountAPIService, Retail_BE_AccountDefinitionAPIService) {

        var accountId;
        var accountEntity;
        var accountViewsDefinitions;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
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
            return Retail_BE_AccountAPIService.GetAccount(accountId).then(function (response) {
                accountEntity = response;
            });
        }
        function getAccountViewsDefinitions() { 
            return Retail_BE_AccountDefinitionAPIService.GetAccountViewDefinitionsByAccountId(accountId).then(function (response) {
                accountViewsDefinitions = response;
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

            if (accountViewsDefinitions != undefined) {
                for (var i = 0; i < accountViewsDefinitions.length ; i++) {
                    var accountViewDefinition = accountViewsDefinitions[i];
                    AddAccountView(accountViewDefinition);
                }
            }

            function AddAccountView(viewDefinition) {
                if (viewDefinition.Account360DegreeSectionName != undefined) {
                    var view = {
                        header: viewDefinition.Account360DegreeSectionName,
                        runtimeEditor: viewDefinition.Settings.RuntimeEditor,
                        onDirectiveReady: function (api) {
                            view.directiveAPI = api;
                            var setLoader = function (value) {
                                view.isLoadingDirective = value;
                            };
                            var payloadDirective = {
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