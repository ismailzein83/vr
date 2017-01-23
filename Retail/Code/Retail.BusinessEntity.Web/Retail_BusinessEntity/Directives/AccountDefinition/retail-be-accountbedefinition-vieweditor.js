"use strict";

app.directive("retailBeAccountbedefinitionVieweditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountBEDefinitionViewEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/Templates/AccountBEDefinitionViewEditor.html"
        };
        function AccountBEDefinitionViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountBEDefinitionViewSettings;
                    var accountDefinitionSelectorLabel;

                    if (payload != undefined) {
                        accountBEDefinitionViewSettings = payload.Settings;
                        accountDefinitionSelectorLabel = payload.AccountDefinitionSelectorLabel;
                    }

                    $scope.scopeModel.accountDefinitionSelectorLabel = accountDefinitionSelectorLabel;

                    //Loading BusinessEntityDefinition Selector
                    var businessEntityDefinitionSelectorLoadPromise = getBusinessEntityDefinitionSelectorLoadPromise();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);


                    function getBusinessEntityDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {

                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                },
                                selectedIds: buildBESelectorIdsObj(accountBEDefinitionViewSettings)
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        function buildBESelectorIdsObj(accountBEDefinitionViewSettings)
                        {
                            var _seletedIds = [];

                            if (accountBEDefinitionViewSettings != undefined) {
                                for (var index = 0; index < accountBEDefinitionViewSettings.length; index++) {
                                    var currentBEDefinitionSetting = accountBEDefinitionViewSettings[index];
                                    _seletedIds.push(currentBEDefinitionSetting.BusinessEntityId)
                                }
                            }
                            return _seletedIds;
                        }

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.Entities.AccountBEDefinitionViewSettings, Retail.BusinessEntity.Entities",
                        Settings: buildAccountBEDefinitionViewSettingsObj(),
                        AccountDefinitionSelectorLabel: $scope.scopeModel.accountDefinitionSelectorLabel
                    };

                    function buildAccountBEDefinitionViewSettingsObj() {
                        var accountBEDefinitionSettings = [];

                        var accountBEDefinitionIds = beDefinitionSelectorApi.getSelectedIds()
                        if (accountBEDefinitionIds != undefined) {
                            for (var index = 0; index < accountBEDefinitionIds.length; index++) {
                                accountBEDefinitionSettings.push({ BusinessEntityId: accountBEDefinitionIds[index] })
                            }
                        }
                        return accountBEDefinitionSettings;
                    }

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);