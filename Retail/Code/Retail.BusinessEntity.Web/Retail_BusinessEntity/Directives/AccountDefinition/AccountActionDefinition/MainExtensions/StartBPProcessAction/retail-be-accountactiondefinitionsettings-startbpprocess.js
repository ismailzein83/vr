'use strict';

app.directive('retailBeAccountactiondefinitionsettingsStartbpprocess', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPDefinitionAPIService', 'BusinessProcess_VRWorkflowAPIService', 'VRWorkflowArgumentDirectionEnum',
    function (UtilsService, VRUIUtilsService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_VRWorkflowAPIService, VRWorkflowArgumentDirectionEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new StartBPProcessActionDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/StartBPProcessAction/Templates/StartBPProcessAccountActionSettingsTemplate.html'
        };

        function StartBPProcessActionDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var bpDefinitionId;
            var settings;

            var bpDefinitionSelectorAPI;
            var bpDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var accountIdFieldNameSelectorAPI;
            var accountIdFieldNameSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var accountBEDefintionIdFieldNameSelectorAPI;
            var accountBEDefintionIdFieldNameSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.accountIdFieldName = [];
                $scope.scopeModel.accountBEDefintionIdFieldName = [];

                $scope.scopeModel.onBPDefinitionDirectiveReady = function (api) {
                    bpDefinitionSelectorAPI = api;
                    bpDefinitionSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onBPDefinitionSelectionChanged = function (item) {
                    if (item != undefined) {
                        bpDefinitionId = item.BPDefinitionID;
                        loadFiledsName(item.BPDefinitionID);
                    }
                };

                $scope.scopeModel.onAccountIdFieldNameSelectorReady = function (api) {
                    accountIdFieldNameSelectorAPI = api;
                    accountIdFieldNameSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onAccountBEDefintionIdFieldNameSelectorReady = function (api) {
                    accountBEDefintionIdFieldNameSelectorAPI = api;
                    accountBEDefintionIdFieldNameSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];
                    if (payload != undefined) {
                        if (payload.accountActionDefinitionSettings != undefined) {
                            settings = payload.accountActionDefinitionSettings;
                        }
                    }

                    function loadBPDefinitions() {
                        var loadBPDefinitionsPromiseDeferred = UtilsService.createPromiseDeferred();
                        bpDefinitionSelectorReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                filter: {
                                    $type: "Vanrise.BusinessProcess.Entities.BPDefinitionInfoFilter,Vanrise.BusinessProcess.Entities"
                                }
                            };
                            payload.selectedIds = (settings != undefined) ? settings.BPDefinitionId : undefined;
                            VRUIUtilsService.callDirectiveLoad(bpDefinitionSelectorAPI, payload, loadBPDefinitionsPromiseDeferred);
                        });

                        return loadBPDefinitionsPromiseDeferred.promise;
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];
                            directivePromises.push(loadBPDefinitions());

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);

                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountBEActionTypes.StartBPProcessActionSettings, Retail.BusinessEntity.MainExtensions',
                        BPDefinitionId: bpDefinitionId,
                        AccountIdInputFieldName: $scope.scopeModel.selectedAccountIdInputFieldName.Name,
                        AccountBEDefinitionIdInputFieldName: ($scope.scopeModel.selectedAccountBEDefintionIdInputFieldName != undefined) ? $scope.scopeModel.selectedAccountBEDefintionIdInputFieldName.Name : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadFiledsName(bpDefinitionId) {
                accountIdFieldNameSelectorReadyPromiseDeferred.promise.then(function () {
                    accountIdFieldNameSelectorAPI.clearDataSource();
                    accountBEDefintionIdFieldNameSelectorAPI.clearDataSource();

                    $scope.scopeModel.isAccountIdFieldNameLoading = true;
                    $scope.scopeModel.isAccountBEDefintionIdFieldNameLoading = true;

                    BusinessProcess_BPDefinitionAPIService.GetBPDefintion(bpDefinitionId).then(function (response) {
                        if (response != undefined) {
                            var bpDefinitionEntity = response;

                            if (bpDefinitionEntity.VRWorkflowId != undefined) {
                                BusinessProcess_VRWorkflowAPIService.GetVRWorkflowArguments(bpDefinitionEntity.VRWorkflowId).then(function (response2) {
                                    if (response2 != undefined) {
                                        var workflowsArguments = response2;
                                        for (var i = 0; i < workflowsArguments.length; i++) {
                                            var workflowargument = workflowsArguments[i];
                                            if (workflowargument.Direction == VRWorkflowArgumentDirectionEnum.In.value) {
                                                $scope.scopeModel.accountIdFieldName.push(workflowargument);
                                                $scope.scopeModel.accountBEDefintionIdFieldName.push(workflowargument);
                                            }
                                        }
                                        if (settings != undefined) {
                                            if ($scope.scopeModel.accountIdFieldName.length > 0) {
                                                $scope.scopeModel.selectedAccountIdInputFieldName = UtilsService.getItemByVal($scope.scopeModel.accountIdFieldName, settings.AccountIdInputFieldName, 'Name');
                                            }

                                            if ($scope.scopeModel.accountBEDefintionIdFieldName.length > 0) {
                                                $scope.scopeModel.selectedAccountBEDefintionIdInputFieldName = UtilsService.getItemByVal($scope.scopeModel.accountBEDefintionIdFieldName, settings.AccountBEDefinitionIdInputFieldName, 'Name');
                                            }
                                        }
                                    }
                                }).catch(function (error) {
                                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                                });
                            }
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isAccountIdFieldNameLoading = false;
                        $scope.scopeModel.isAccountBEDefintionIdFieldNameLoading = false;
                    });
                });
            }
        }
    }]);
