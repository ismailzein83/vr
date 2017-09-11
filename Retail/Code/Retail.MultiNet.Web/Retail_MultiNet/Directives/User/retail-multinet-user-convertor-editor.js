'use strict';

app.directive('retailMultinetUserConvertorEditor', ['UtilsService', 'VRUIUtilsService', 'Retail_BE_EntityTypeEnum', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, Retail_BE_EntityTypeEnum, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailMultiNetUserConvertorEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_MultiNet/Directives/User/Templates/UserConvertorEditor.html"
        };

        function retailMultiNetUserConvertorEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var accountTypeId;
            var statusDefinitionId;

            var accountDefinitionSelectorApi;
            var accountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var businessEntityDefinitionSelectionChangedDeferred;

            var statusDefinitionSelectorAPI;
            var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var accountTypeSelectorAPI;
            var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();



            var userProfileDefinitionSelectorAPI;
            var userProfileDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();





            $scope.scopeModel = {};

            $scope.scopeModel.onAccountDefinitionSelectorReady = function (api) {
                accountDefinitionSelectorApi = api;
                accountDefinitionSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.onInitialStatusDefinitionSelectorReady = function (api) {
                statusDefinitionSelectorAPI = api;
                statusDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorReadyDeferred.resolve();
            };


            $scope.scopeModel.onUserProfileDefinitionSelectorReady = function (api) {
                userProfileDefinitionSelectorAPI = api;
                userProfileDefinitionSelectorReadyDeferred.resolve();
            };




            $scope.scopeModel.onAccountDefinitionSelectionChanged = function (selectedItem) {
                if (selectedItem != undefined && businessEntityDefinitionSelectionChangedDeferred == undefined) {
                    businessEntityDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                    businessEntityDefinitionSelectionChangedDeferred.resolve();

                    $scope.scopeModel.isAccountTypeSelectorLoading = true;
                    var promises = [];

                    var accountTypeSelectorPayload = {
                        filter: {
                            AccountBEDefinitionId: selectedItem.BusinessEntityDefinitionId
                        },
                        selectedIds: accountTypeId
                    };
                    var statusSelectorPayload = {
                        filter: {
                            Filters: [{
                                $type: "Retail.BusinessEntity.Business.AccountBEStatusDefinitionFilter, Retail.BusinessEntity.Business",
                                AccountBEDefinitionId: selectedItem.BusinessEntityDefinitionId

                            }]
                        },
                        selectedIds: statusDefinitionId
                    };
                    promises.push(loadAccountTypeSelector(accountTypeSelectorPayload));
                    promises.push(loadStatusDefinitionSelector(statusSelectorPayload));

                    UtilsService.waitMultiplePromises(promises).then(function () {
                        businessEntityDefinitionSelectionChangedDeferred = undefined;
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isAccountTypeSelectorLoading = false;
                    });
                } else if (businessEntityDefinitionSelectionChangedDeferred != undefined) {
                    businessEntityDefinitionSelectionChangedDeferred.resolve();
                }
            };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var accountTypeSelectorPayload;
                    var statusDefinitionSelectorPayload;

                    promises.push(loadAccountDefinitionSelectorLoad());

                    if (payload != undefined) {
                        accountTypeId = payload.AccountTypeId;
                        statusDefinitionId = payload.InitialStatusId;
                        accountBEDefinitionId = payload.AccountBEDefinitionId;

                        $scope.scopeModel.accountIdColumnName = payload.AccountIdColumnName;
                        $scope.scopeModel.subscriberIdColumnName = payload.SubscriberIdColumnName;
                        $scope.scopeModel.nameColumnName = payload.NameColumnName;

                        accountTypeSelectorPayload = {
                            filter: {
                                AccountBEDefinitionId: accountBEDefinitionId
                            },
                            selectedIds: payload.AccountTypeId
                        };

                        statusDefinitionSelectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountBEStatusDefinitionFilter, Retail.BusinessEntity.Business",
                                    AccountBEDefinitionId: payload.AccountBEDefinitionId

                                }]
                            },
                            selectedIds: payload.InitialStatusId
                        };

                        businessEntityDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadAccountTypeSelector(accountTypeSelectorPayload));
                        promises.push(loadStatusDefinitionSelector(statusDefinitionSelectorPayload));
                    }

                    promises.push(loadUserProfileDefinitionSelector());

                    function loadAccountDefinitionSelectorLoad() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        accountDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.AccountBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(accountDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    };

                    function loadUserProfileDefinitionSelector() {
                        var userProfileSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        userProfileDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('3900317c-b982-4d8b-bd0d-01215ac1f3d9')
                                },
                                selectedIds: payload != undefined ? payload.UserProfilePartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(userProfileDefinitionSelectorAPI, selectorPayload, userProfileSelectorLoadDeferred);
                        });
                        return userProfileSelectorLoadDeferred.promise;
                    };



                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        businessEntityDefinitionSelectionChangedDeferred = undefined;
                    });
                };


                api.getData = function () {
                    var data = {
                        $type: "Retail.MultiNet.Business.Convertors.UserConvertor, Retail.MultiNet.Business",
                        Name: "MultiNet User Convertor",
                        AccountBEDefinitionId: accountDefinitionSelectorApi.getSelectedIds(),
                        InitialStatusId: statusDefinitionSelectorAPI.getSelectedIds(),
                        AccountTypeId: accountTypeSelectorAPI.getSelectedIds(),
                        UserProfilePartDefinitionId: userProfileDefinitionSelectorAPI.getSelectedIds(),
                        AccountIdColumnName: $scope.scopeModel.accountIdColumnName,
                        SubscriberIdColumnName: $scope.scopeModel.subscriberIdColumnName,
                        NameColumnName: $scope.scopeModel.nameColumnName
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function loadAccountTypeSelector(accountTypeSelectorPayload) {
                var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([accountTypeSelectorReadyDeferred.promise, businessEntityDefinitionSelectionChangedDeferred.promise]).then(function () {

                    VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
                });

                return accountTypeSelectorLoadDeferred.promise;
            }

            function loadStatusDefinitionSelector(accountTypeSelectorPayload) {
                var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([statusDefinitionSelectorReadyDeferred.promise, businessEntityDefinitionSelectionChangedDeferred.promise]).then(function () {

                    VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
                });

                return accountTypeSelectorLoadDeferred.promise;
            }

            function getAccountPartDefinitionIds(id) {
                var partDefinitionIds = [];
                partDefinitionIds.push(id);
                return partDefinitionIds;
            }
        }
        return directiveDefinitionObject;
    }]);