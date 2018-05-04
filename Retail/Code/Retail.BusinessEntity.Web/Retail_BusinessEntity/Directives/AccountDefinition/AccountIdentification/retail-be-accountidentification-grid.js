'use strict';

app.directive('retailBeAccountidentificationGrid', ['Retail_BE_AccountIdentificationAPIService', 'VR_GenericData_GenericRule', 'VRNotificationService',
    function (Retail_BE_AccountIdentificationAPIService, VR_GenericData_GenericRule, VRNotificationService)
{
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountIdentificationGrid = new AccountIdentificationGrid($scope, ctrl, $attrs);
            accountIdentificationGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountIdentification/Templates/AccountIdentificationGridTemplate.html'
    };

    function AccountIdentificationGrid($scope, ctrl, $attrs)
    {
        this.initializeController = initializeController;
       
        var accountBEDefinitionId;
        var accountId;

        var gridAPI;

        function initializeController()
        {
            $scope.scopeModel = {};
            $scope.scopeModel.accountIdentificationRules = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api)
            {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady)
            {
                return Retail_BE_AccountIdentificationAPIService.GetFilteredAccountIdentificationRules(dataRetrievalInput).then(function (response)
                {
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            defineMenuActions();
        }
        function defineAPI()
        {
            var api = {};

            api.load = function (query) {

                accountBEDefinitionId = query.accountBEDefinitionId;
                accountId = query.AccountId;
                return gridAPI.retrieveData(query);
            };

            api.onAccountIdentificationRuleAdded = function () {
                return gridAPI.retrieveData({ AccountId: accountId });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editAccountIdentificationRule
            });
        }
        function editAccountIdentificationRule(identificationRule) {
            var onAccountIdentificationUpdated = function () {
                return gridAPI.retrieveData({ AccountId: accountId });
            };

            var preDefinedData = {
                settings: {
                    Value: accountId,
                }
            };

            var accessibility = {
                settingNotAccessible :true
            };

            VR_GenericData_GenericRule.editGenericRule(identificationRule.GenericRuleId, identificationRule.GenericRuleDefinitionId,onAccountIdentificationUpdated, preDefinedData, accessibility);
        }
    }
}]);
