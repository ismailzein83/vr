'use strict';

app.directive('retailBeAccountmanagerAccountassignmentdefinition', ['UtilsService', 'VRUIUtilsService','VRNavigationService',
function (UtilsService, VRUIUtilsService, VRNavigationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new AccountAssignmentDefinitionCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Extensions/AccountManager/Templates/AccountAssignmentDefinitionTemplate.html"
    };

    function AccountAssignmentDefinitionCtor(ctrl, $scope, $attrs) {

        var businessEntityDefinitionSelectorAPI;
        var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedBusinessEntityDefinitionPromiseDeferred;

        var accountConditionSelectiveAPI;
        var accounrConditionSelectiveReady = UtilsService.createPromiseDeferred();

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                businessEntityDefinitionSelectorAPI = api;
                businessEntityDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onAccountConditionSelectiveReady = function (api) {
                accountConditionSelectiveAPI = api;
                accounrConditionSelectiveReady.resolve();
            };
            $scope.scopeModel.onBusinessEntityDefinitionSelectionChanged = function (selectedItem) {
                if (selectedItem != undefined) {
                    if (selectedBusinessEntityDefinitionPromiseDeferred != undefined)
                        selectedBusinessEntityDefinitionPromiseDeferred.resolve();
                    else {
                        var accountConditionPayload = {
                            accountBEDefinitionId: selectedItem.BusinessEntityDefinitionId
                        };
                        var setLoader = function (value) {
                            $scope.scopeModel.isAccountConditionLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountConditionSelectiveAPI, accountConditionPayload, setLoader);
                    }

                }
            }
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                var assignmentDefinitionEntity;
                if (payload != undefined) {
                    assignmentDefinitionEntity = payload.assignmentDefinitionEntity;
                }
                if (assignmentDefinitionEntity != undefined)
                {
                    selectedBusinessEntityDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadAccountConditionSelective(assignmentDefinitionEntity));
                }
                promises.push(loadBusinessEntityDefinitionSelector(assignmentDefinitionEntity))

                   
                return UtilsService.waitMultiplePromises(promises);

            };
            api.getData = function () {
                var obj = {
                    $type: "Retail.BusinessEntity.Business.RetailAccountAssignmentDefinition,Retail.BusinessEntity.Business",
                    AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds(),
                    AccountCondition: accountConditionSelectiveAPI.getData()
                };

                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function loadBusinessEntityDefinitionSelector(assignmentDefinitionEntity) {
            var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            businessEntityDefinitionSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: {
                        Filters: [{
                            $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                        }]

                    },
                    selectedIds: assignmentDefinitionEntity != undefined ? assignmentDefinitionEntity.AccountBEDefinitionId : undefined

                };
                VRUIUtilsService.callDirectiveLoad(businessEntityDefinitionSelectorAPI, payload, businessEntityDefinitionSelectorLoadDeferred);
            });
            return businessEntityDefinitionSelectorLoadDeferred.promise;
        }
        function loadAccountConditionSelective(assignmentDefinitionEntity) {
            var accountConditionLoadDeferred = UtilsService.createPromiseDeferred();
            UtilsService.waitMultiplePromises([accounrConditionSelectiveReady.promise, selectedBusinessEntityDefinitionPromiseDeferred.promise]).then(function () {
                selectedBusinessEntityDefinitionPromiseDeferred = undefined;
                var accountConditionPayload = {
                    accountBEDefinitionId: assignmentDefinitionEntity.AccountBEDefinitionId,
                    beFilter: assignmentDefinitionEntity.AccountCondition
                };
                VRUIUtilsService.callDirectiveLoad(accountConditionSelectiveAPI, accountConditionPayload, accountConditionLoadDeferred);
            });
            return accountConditionLoadDeferred.promise;
        }

    }

    return directiveDefinitionObject;
}]);