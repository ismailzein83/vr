'use strict';

app.directive('retailBeDatarecordtypefieldsFormulaParentaccount', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new parentAccountEvaluatorFieldFormulaCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/DataRecordFieldFormulas/Templates/ParentAccountFieldFormulaTemplate.html"
        };

        function parentAccountEvaluatorFieldFormulaCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var accountField;
            var accountBEDefinitionId;

            var accountSelectionChangedPromiseDeffered = UtilsService.createPromiseDeferred();

            var accountTypeSelectorAPI;
            var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];
                $scope.scopeModel.showAccountTypeSelector = false;

                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onAccountSelectionChanged = function (selectedItem) {

                    console.log(selectedItem);

                    if (selectedItem != undefined) {
                        $scope.scopeModel.showAccountTypeSelector = true;

                        var accountField = UtilsService.getItemByVal($scope.scopeModel.fields, selectedItem.fieldName, "fieldType");
                        accountBEDefinitionId = accountField.fieldType.BusinessEntityDefinitionId;

                        var accountTypeSelectorPayload = {
                            filter: { AccountBEDefinitionId: accountBEDefinitionId }
                        };

                        var setLoader = function (value) {
                            $scope.scopeModel.isAccountTypeSelectorLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountTypeSelectorAPI, accountTypeSelectorPayload, setLoader, accountSelectionChangedPromiseDeffered);
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    console.log(payload);

                    var parentAccountTypeId;

                    if (payload != undefined) {
                        var context = payload.context;
                        if (context != undefined && context.getFields != undefined) {
                            $scope.scopeModel.fields = context.getFields();
                        }

                        if (payload.formula != undefined) {
                            parentAccountTypeId = payload.formula.ParentAccountTypeId;
                            accountField = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.AccountFieldName, "fieldName");
                        }
                    }

                    $scope.scopeModel.selectedAccount = accountField;

                    console.log("Loading AccountTypeSelector!!");;

                    if (parentAccountTypeId != undefined) {
                        var accountTypeSelectorLoadPromise = getAccountTypeSelectorLoadPromise();
                        promises.push(accountTypeSelectorLoadPromise);
                    }
                    else {
                        accountSelectionChangedPromiseDeffered.resolve();
                    }


                    function getAccountTypeSelectorLoadPromise() {
                        var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([accountTypeSelectorReadyDeferred, accountSelectionChangedPromiseDeffered]).promise.then(function () {

                            var accountTypeSelectorPayload = {
                                filter: { AccountBEDefinitionId: accountBEDefinitionId },
                                selectedIds: parentAccountTypeId
                            };
                            VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
                        });

                        return accountTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.DataRecordFieldFormulas.ParentRetailAccountFieldFormula, Retail.BusinessEntity.MainExtensions",
                        AccountFieldName: $scope.scopeModel.selectedAccount.fieldName,
                        ParentAccountTypeId: accountTypeSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);