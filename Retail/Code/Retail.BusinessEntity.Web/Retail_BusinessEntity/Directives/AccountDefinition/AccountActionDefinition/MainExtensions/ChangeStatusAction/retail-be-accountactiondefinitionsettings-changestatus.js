'use strict';

app.directive('retailBeAccountactiondefinitionsettingsChangestatus', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ChangeStatusActionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/ChangeStatusAction/Templates/ChangeStatusActionSettingsTemplate.html'
        };

        function ChangeStatusActionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var statusDefinitionSelectorAPI;
            var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var accountBEDefinitionId;
            function initializeController() {
                $scope.scopeModel = {};
                
                $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                    statusDefinitionSelectorAPI = api;
                    statusDefinitionSelectorReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([statusDefinitionSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });

                
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var accountActionDefinitionSettings;
                    if (payload != undefined) {
                        accountActionDefinitionSettings = payload.accountActionDefinitionSettings;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        if (accountActionDefinitionSettings != undefined)
                            $scope.scopeModel.applyToChildren = accountActionDefinitionSettings.ApplyToChildren;
                    }

                    function loadStatusDefinitionSelector() {
                        var selectorPayload = {
                            selectedIds: accountActionDefinitionSettings != undefined ? accountActionDefinitionSettings.StatusId : undefined,
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountBEStatusDefinitionFilter,Retail.BusinessEntity.Business",
                                    AccountBEDefinitionId: accountBEDefinitionId
                                }]
                            }
                        };
                        return statusDefinitionSelectorAPI.load(selectorPayload);
                    }

                    var promises = [];

                    promises.push(loadStatusDefinitionSelector());

                    return UtilsService.waitMultiplePromises(promises)
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountBEActionTypes.ChangeStatusActionSettings, Retail.BusinessEntity.MainExtensions',
                        StatusId: statusDefinitionSelectorAPI.getSelectedIds(),
                        ApplyToChildren: $scope.scopeModel.applyToChildren
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);