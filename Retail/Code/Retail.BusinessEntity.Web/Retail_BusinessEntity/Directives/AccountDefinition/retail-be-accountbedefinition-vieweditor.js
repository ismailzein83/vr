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
                                }
                            };
                            if (payload != undefined) {
                                selectorPayload.selectedIds = buildBESelectorIdsObj(payload.AccountBEDefinitionSettings);
                            }
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        function buildBESelectorIdsObj(accountBEDefinitionSettings)
                        {
                            var _seletedIds = [];

                            if (accountBEDefinitionSettings != undefined) {
                                for (var index = 0; index < accountBEDefinitionSettings.length; index++) {
                                    var currentBEDefinitionSetting = accountBEDefinitionSettings[index];
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
                        AccountBEDefinitionSettings: buildAccountBEDefinitionSettingsObj()
                    };

                    function buildAccountBEDefinitionSettingsObj() {
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