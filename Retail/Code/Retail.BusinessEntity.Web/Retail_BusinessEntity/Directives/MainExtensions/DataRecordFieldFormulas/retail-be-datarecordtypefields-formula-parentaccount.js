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

            var accountBEDefinitionId;

            var accountSelectionChangedPromiseDeffered;

            var parentAccountTypeSelectorAPI;
            var parentAccountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];

                $scope.scopeModel.onParentAccountTypeSelectorReady = function (api) {
                    parentAccountTypeSelectorAPI = api;
                    parentAccountTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onAccountSelectionChanged = function (selectedField) {

                    if (selectedField != undefined) {

                        var accountField = UtilsService.getItemByVal($scope.scopeModel.fields, selectedField.fieldName, "fieldName");
                        accountBEDefinitionId = accountField.fieldType.BusinessEntityDefinitionId;

                        if (accountSelectionChangedPromiseDeffered != undefined) {
                            accountSelectionChangedPromiseDeffered.resolve();
                        }
                        else {
                            var parentAccountTypeSelectorPayload = {
                                filter: { AccountBEDefinitionId: accountBEDefinitionId }
                            };

                            var setLoader = function (value) {
                                $scope.scopeModel.isParentAccountTypeSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, parentAccountTypeSelectorAPI, parentAccountTypeSelectorPayload, setLoader);
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var isEditMode;
                    var accountTypeFieldName, accountFieldName, parentAccountTypeId;

                    if (payload != undefined) {
                        var context = payload.context;
                        if (context != undefined && context.getFields != undefined) {
                            $scope.scopeModel.fields = context.getFields();
                        }

                        if (payload.formula != undefined) {
                            accountTypeFieldName = payload.formula.AccountTypeFieldName;
                            accountFieldName = payload.formula.AccountFieldName;
                            parentAccountTypeId = payload.formula.ParentAccountTypeId;
                            isEditMode = (accountTypeFieldName != undefined && accountFieldName != undefined && parentAccountTypeId != undefined) ? true : false;
                        }
                    }

                    $scope.scopeModel.selectedAccountType = UtilsService.getItemByVal($scope.scopeModel.fields, accountTypeFieldName, "fieldName");
                    $scope.scopeModel.selectedAccount = UtilsService.getItemByVal($scope.scopeModel.fields, accountFieldName, "fieldName");

                    if (isEditMode) {
                        accountSelectionChangedPromiseDeffered = UtilsService.createPromiseDeferred();

                        var parentAccountTypeSelectorLoadPromise = getParentAccountTypeSelectorLoadPromise();
                        promises.push(parentAccountTypeSelectorLoadPromise);
                    }


                    function getParentAccountTypeSelectorLoadPromise() {
                        var parentAccountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([parentAccountTypeSelectorReadyDeferred.promise, accountSelectionChangedPromiseDeffered.promise]).then(function () {
                            accountSelectionChangedPromiseDeffered = undefined;

                            var parentAccountTypeSelectorPayload = {
                                filter: { AccountBEDefinitionId: accountBEDefinitionId },
                                selectedIds: parentAccountTypeId
                            };
                            VRUIUtilsService.callDirectiveLoad(parentAccountTypeSelectorAPI, parentAccountTypeSelectorPayload, parentAccountTypeSelectorLoadDeferred);
                        });

                        return parentAccountTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.DataRecordFieldFormulas.ParentRetailAccountFieldFormula, Retail.BusinessEntity.MainExtensions",
                        AccountTypeFieldName: $scope.scopeModel.selectedAccountType.fieldName,
                        AccountFieldName: $scope.scopeModel.selectedAccount.fieldName,
                        ParentAccountTypeId: parentAccountTypeSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);