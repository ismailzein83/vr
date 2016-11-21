'use strict';
app.directive('vrAccountbalanceAccountbalanceruleSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new AccountBalanceRuleDefinitionCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_AccountBalance/Directives/MainExtensions/BalanceAlertRule/Templates/BalanceAlertRuleSettingsTemplate.html';
            }

        };

        function AccountBalanceRuleDefinitionCtor(ctrl, $scope) {

            var genericRuleDefinitionEntity;

            var ruleObjects;
            var criteriaFields;
            var accountTypeId;
            var criteriaDirectiveAPI;
            var criteriaDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var objectDirectiveAPI;
            var objectDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var accountTypeSelectorAPI;
            var accountTypeSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCriteriaDirectiveReady = function (api) {
                    criteriaDirectiveAPI = api;
                    criteriaDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.scopeModel.onObjectDirectiveReady = function (api) {
                    objectDirectiveAPI = api;
                    objectDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.accountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorAPIReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.settings != undefined) {
                        criteriaFields = payload.settings.CriteriaDefinition.Fields;
                        ruleObjects = payload.settings.Objects;
                        accountTypeId = payload.settings.AccountTypeId;
                        $scope.scopeModel.thresholdExtensionType = payload.settings.ThresholdExtensionType;
                        $scope.scopeModel.actionExtensionType = payload.settings.VRActionExtensionType;
                    }

                    return UtilsService.waitMultipleAsyncOperations([loadCriteriaDirective, loadObjectDirective, loadAccountTypeSelector]).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.isLoading = false;
                    });
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.AccountBalance.Business.Extensions.AccountBalanceAlertRuleTypeSettings, Vanrise.AccountBalance.Business",
                        AccountTypeId: accountTypeSelectorAPI.getSelectedIds(),
                        CriteriaDefinition: criteriaDirectiveAPI.getData(),
                        Objects: objectDirectiveAPI.getData(),
                        ThresholdExtensionType: $scope.scopeModel.thresholdExtensionType,
                        VRActionExtensionType: $scope.scopeModel.actionExtensionType
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadObjectDirective() {
                var objectDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                objectDirectiveReadyDeferred.promise.then(function () {
                    var objectDirectivePayload;

                    if (ruleObjects != undefined) {

                        var objects = [];
                        for (var key in ruleObjects) {
                            if (key != "$type")
                                objects.push(ruleObjects[key]);
                        }

                        objectDirectivePayload = {
                            objects: objects
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(objectDirectiveAPI, objectDirectivePayload, objectDirectiveLoadDeferred);
                });

                return objectDirectiveLoadDeferred.promise;
            }

            function loadCriteriaDirective() {
                var criteriaDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                criteriaDirectiveReadyPromiseDeferred.promise.then(function () {
                    var criteriaDirectivePayload = { context: buildContext() };

                    if (criteriaFields != undefined) {
                        criteriaDirectivePayload.GenericRuleDefinitionCriteriaFields = criteriaFields;
                    }

                    VRUIUtilsService.callDirectiveLoad(criteriaDirectiveAPI, criteriaDirectivePayload, criteriaDirectiveLoadDeferred);
                });

                return criteriaDirectiveLoadDeferred.promise;
            }

            function loadAccountTypeSelector() {
                var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                accountTypeSelectorAPIReadyDeferred.promise.then(function () {
                    var payload = {
                        selectedIds: accountTypeId
                    };

                    VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, payload, accountTypeSelectorLoadDeferred);
                });

                return accountTypeSelectorLoadDeferred.promise;
            }

            function buildContext() {

                var context = {
                    getObjectVariables: function () { return objectDirectiveAPI.getData(); }
                };
                return context;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);