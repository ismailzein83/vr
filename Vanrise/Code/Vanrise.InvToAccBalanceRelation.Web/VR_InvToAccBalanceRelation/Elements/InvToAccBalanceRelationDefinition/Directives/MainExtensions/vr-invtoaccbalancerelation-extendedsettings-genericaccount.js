'use strict';

app.directive('vrInvtoaccbalancerelationExtendedsettingsGenericaccount', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrInvtoaccbalancerelationExtendedsettingsGenericaccount = new VRInvtoaccbalancerelationExtendedsettingsGenericaccount($scope, ctrl, $attrs);
                vrInvtoaccbalancerelationExtendedsettingsGenericaccount.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_InvToAccBalanceRelation/Elements/InvToAccBalanceRelationDefinition/Directives/MainExtensions/Templates/VRInvToAccBalanceRelationExtendedSettingsGenericAccount.html'
        };

        function VRInvtoaccbalancerelationExtendedsettingsGenericaccount($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var extendedSettingsEntity;
            var invoiceTypeIds;
            var balanceAccountTypeId;

            var accountTypeSelectorApi;
            var accountTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var invoiceTypeSelectorAPI;
            var invoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var fieldsBySourceId;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onInvoiceTypeSelectorReady = function (api) {
                    invoiceTypeSelectorAPI = api;
                    invoiceTypeSelectorReadyDeferred.resolve();
                }

                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorApi = api;
                    accountTypeSelectorPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        var extendedSettingsEntity = payload.extendedSettingsEntity;

                        if (extendedSettingsEntity != undefined) {
                            $scope.scopeModel.name = extendedSettingsEntity.Name;
                            invoiceTypeIds = extendedSettingsEntity.InvoiceTypeIds;
                            balanceAccountTypeId = extendedSettingsEntity.BalanceAccountTypeId;
                        }
                    }
                    promises.push(loadInvoiceTypeSelector());
                    promises.push(loadAccountTypeSelector());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return GetSettingsObj();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                function GetSettingsObj() {
                    return {
                        $type: "Vanrise.InvToAccBalanceRelation.Business.GenericAccountInvToAccBalanceRelationDefinitionExtendedSettings, Vanrise.InvToAccBalanceRelation.Business",
                        InvoiceTypeIds: invoiceTypeSelectorAPI.getSelectedIds(),
                        BalanceAccountTypeId: accountTypeSelectorApi.getSelectedIds()
                    };
                }

                function loadInvoiceTypeSelector() {
                    var invoiceTypeSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();

                    invoiceTypeSelectorReadyDeferred.promise.then(function () {
                        var invoiceTypeSelectorPayload;
                        if (invoiceTypeIds != undefined) {
                            invoiceTypeSelectorPayload = { selectedIds: invoiceTypeIds };
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceTypeSelectorAPI, invoiceTypeSelectorPayload, invoiceTypeSelectorPayloadLoadDeferred);
                    });

                    return invoiceTypeSelectorPayloadLoadDeferred.promise;
                }

                function loadAccountTypeSelector() {
                    var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    accountTypeSelectorPromiseDeferred.promise.then(function () {
                        var accountTypeSelectorPayload;
                        if (balanceAccountTypeId != undefined) {
                            accountTypeSelectorPayload = { selectedIds: balanceAccountTypeId };
                        };
                        VRUIUtilsService.callDirectiveLoad(accountTypeSelectorApi, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
                    });
                    return accountTypeSelectorLoadDeferred.promise;
                }
            }
        }
    }]);
