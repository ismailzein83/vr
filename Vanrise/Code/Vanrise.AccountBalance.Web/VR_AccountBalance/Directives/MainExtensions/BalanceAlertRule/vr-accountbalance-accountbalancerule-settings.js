'use strict';

app.directive('vrAccountbalanceAccountbalanceruleSettings', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
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
            this.initializeController = initializeController;

            var genericRuleDefinitionEntity;
            var accountTypeId, notificationTypeId;
            var ruleObjects, criteriaFields;

            var accountTypeSelectorAPI;
            var accountTypeSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();

            var notificationTypeSettingsSelectorAPI;
            var notificationTypeSettingsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var objectDirectiveAPI;
            var objectDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var criteriaDirectiveAPI;
            var criteriaDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorAPIReadyDeferred.resolve();
                };

                $scope.scopeModel.onVRNotificationTypeSettingsSelectorReady = function (api) {
                    notificationTypeSettingsSelectorAPI = api;
                    notificationTypeSettingsSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onCriteriaDirectiveReady = function (api) {
                    criteriaDirectiveAPI = api;
                    criteriaDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onObjectDirectiveReady = function (api) {
                    objectDirectiveAPI = api;
                    objectDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.settings != undefined) {
                        accountTypeId = payload.settings.AccountTypeId;
                        notificationTypeId = payload.settings.NotificationTypeId;
                        criteriaFields = payload.settings.CriteriaDefinition.Fields;
                        ruleObjects = payload.settings.Objects;

                        $scope.scopeModel.thresholdExtensionType = payload.settings.ThresholdExtensionType;
                    }

                    return UtilsService.waitMultipleAsyncOperations([loadAccountTypeSelector, loadNotificationTypeSelector, loadCriteriaDirective, loadObjectDirective]).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.isLoading = false;
                    });
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.AccountBalance.Business.Extensions.AccountBalanceAlertRuleTypeSettings, Vanrise.AccountBalance.Business",
                        AccountTypeId: accountTypeSelectorAPI.getSelectedIds(),
                        NotificationTypeId: notificationTypeSettingsSelectorAPI.getSelectedIds(),
                        CriteriaDefinition: criteriaDirectiveAPI.getData(),
                        Objects: objectDirectiveAPI.getData(),
                        ThresholdExtensionType: $scope.scopeModel.thresholdExtensionType,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
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
            function loadNotificationTypeSelector() {
                var notificationSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                notificationTypeSettingsSelectorReadyDeferred.promise.then(function () {

                    var notificationSelectorPayload = {
                        filter: {
                            Filters: [{
                                $type: "Vanrise.AccountBalance.MainExtensions.Filters.AccountBalanceNotificationTypeFilter, Vanrise.AccountBalance.MainExtensions"
                            }]
                        }
                    };
                    if (notificationTypeId != undefined) {
                        notificationSelectorPayload.selectedIds = notificationTypeId;
                    }
                    VRUIUtilsService.callDirectiveLoad(notificationTypeSettingsSelectorAPI, notificationSelectorPayload, notificationSelectorLoadDeferred);
                });

                return notificationSelectorLoadDeferred.promise;
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

            function buildContext() {

                var context = {
                    getObjectVariables: function () { return objectDirectiveAPI.getData(); }
                };
                return context;
            }
        }

        return directiveDefinitionObject;
    }
]);