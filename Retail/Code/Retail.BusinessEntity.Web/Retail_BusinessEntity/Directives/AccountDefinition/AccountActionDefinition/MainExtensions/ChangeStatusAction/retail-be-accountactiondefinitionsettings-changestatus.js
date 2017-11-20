'use strict';

app.directive('retailBeAccountactiondefinitionsettingsChangestatus', ['UtilsService','VRUIUtilsService',
    function (UtilsService,VRUIUtilsService) {
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

            var applicableStatusDefinitionSelectorAPI;
            var applicableStatusDefinitionSelectorReadyDeferred;


            var accountBEDefinitionId;
            function initializeController() {
                $scope.scopeModel = {};
                
                $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                    statusDefinitionSelectorAPI = api;
                    statusDefinitionSelectorReadyDeferred.resolve();
                };
                  
                $scope.scopeModel.onApplicableOnStatusesDefinitionSelectorReady = function (api) {
                    applicableStatusDefinitionSelectorAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var selectorStatusPayload = {
                        filter: {
                            Filters: [{
                                $type: "Retail.BusinessEntity.Business.AccountBEStatusDefinitionFilter,Retail.BusinessEntity.Business",
                                AccountBEDefinitionId: accountBEDefinitionId
                            }]
                        }
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, applicableStatusDefinitionSelectorAPI, selectorStatusPayload, setLoader, applicableStatusDefinitionSelectorReadyDeferred);
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
                        {
                            $scope.scopeModel.applyToChildren = accountActionDefinitionSettings.ApplyToChildren;

                            if (accountActionDefinitionSettings.AllowOverlapping)
                            {
                                applicableStatusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
                            }
                            $scope.scopeModel.allowOverlapping = accountActionDefinitionSettings.AllowOverlapping;
                        }
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
                   
                    function loadApplicableStatusDefinitionSelector() {
                        var  applicableStatusDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        applicableStatusDefinitionSelectorReadyDeferred.promise.then(function () {
                            applicableStatusDefinitionSelectorReadyDeferred = undefined;

                            var selectorStatusPayload = {
                                selectedIds: accountActionDefinitionSettings != undefined ? accountActionDefinitionSettings.ApplicableOnStatuses : undefined,
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEStatusDefinitionFilter,Retail.BusinessEntity.Business",
                                        AccountBEDefinitionId: accountBEDefinitionId
                                    }]
                                }
                            };
                            VRUIUtilsService.callDirectiveLoad(applicableStatusDefinitionSelectorAPI, selectorStatusPayload, applicableStatusDefinitionSelectorLoadDeferred);
                        });
                        return applicableStatusDefinitionSelectorLoadDeferred.promise;
                    }
                    var promises = [];

                    promises.push(loadStatusDefinitionSelector());
                    if($scope.scopeModel.allowOverlapping)
                    {           
                        promises.push(loadApplicableStatusDefinitionSelector());
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountBEActionTypes.ChangeStatusActionSettings, Retail.BusinessEntity.MainExtensions',
                        StatusId: statusDefinitionSelectorAPI.getSelectedIds(),
                        ApplyToChildren: $scope.scopeModel.applyToChildren,
                        AllowOverlapping: $scope.scopeModel.allowOverlapping,
                        ApplicableOnStatuses:$scope.scopeModel.allowOverlapping? applicableStatusDefinitionSelectorAPI.getSelectedIds():undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);