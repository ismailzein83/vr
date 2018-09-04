﻿"use strict";

app.directive("businessprocessBpBusinessRuleSetDetail", ["UtilsService", "VRNotificationService", "BusinessProcess_BPBusinessRuleSetService", "VRUIUtilsService", "BusinessProcess_BPSchedulerTaskService", "BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService",
function (UtilsService, VRNotificationService, BusinessProcess_BPBusinessRuleSetService, VRUIUtilsService, BusinessProcess_BPSchedulerTaskService, BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var businessprocessBpBusinessRuleSetDetail = new BpBusinessRuleSetDetail($scope, ctrl, $attrs);
            businessprocessBpBusinessRuleSetDetail.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPBusinessRuleSet/Templates/BPBusinessRuleSetDetailTemplate.html"

    };

    function BpBusinessRuleSetDetail($scope, ctrl, $attrs) {

        var gridAPI;
        var ruleActionsSelectiveAPI;
        var ruleActionsSelectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var isEditMode;
        var bpBusinessRuleSetId;
        var parentRuleSetId;

        this.initializeController = initializeController;

        function initializeController() {
            var ruleSetActions = [];
            $scope.bpBusinessRules = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.load = function (payload) {
                        gridAPI.clearDataSource();
                        var ruleSetLoadPromise = UtilsService.createPromiseDeferred();
                        if (payload != undefined) {
                            isEditMode = payload.isEditMode;
                            bpBusinessRuleSetId = payload.bpBusinessRuleSetId;
                            parentRuleSetId = payload.parentRuleSetId;
                            if (isEditMode) {
                            BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService.GetRuleSetEffectiveActions(bpBusinessRuleSetId)
                                .then(function (response) {
                                    ruleSetActions = response;
                                    BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService.GetBPBusinessRuleSetsEffectiveActions(payload.query)
                                .then(function (response) {
                                    if (response != undefined) {
                                        for (var i = 0; i < response.length; i++) {
                                            var dataItem = response[i];
                                            $scope.bpBusinessRules.push(dataItem);
                                            extendDataItem(dataItem);
                                            ruleSetLoadPromise.resolve();
                                        }
                                    }
                                })
                           .catch(function (error) {
                               VRNotificationService.notifyException(error, $scope);
                           });
                                }
                                ).catch(function (error) {
                                    VRNotificationService.notifyException(error, $scope);
                                });
                            }
                            else {
                                BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService.GetBPBusinessRuleSetsEffectiveActions(payload.query)
                                .then(function (response) {
                                    if (response != undefined) {
                                        for (var i = 0; i < response.length; i++) {
                                            var dataItem = response[i];
                                            $scope.bpBusinessRules.push(dataItem);
                                            extendDataItem(dataItem);
                                            ruleSetLoadPromise.resolve();
                                        }
                                    }
                                })
                           .catch(function (error) {
                               VRNotificationService.notifyException(error, $scope);
                           });
                            }
                        }
                        return ruleSetLoadPromise.promise;
                    };

                    directiveAPI.getData = function () {
                        var businessRuleSets = [];
                        for (var i = 0; i < $scope.bpBusinessRules.length; i++) {
                            var obj = $scope.bpBusinessRules[i];
                            if (obj.IsOverriden) {
                                obj.Entity.Action = obj.ruleActionsSelectiveAPI.getData();
                                obj.Entity.Disabled = obj.Entity.Disabled;
                                businessRuleSets.push(obj);
                            }
                            else if (!obj.IsInherited || obj.disabledChanged) { 
                                obj.Entity.Action = obj.Entity.Action;
                                businessRuleSets.push(obj);
                            }
                        }
                        return businessRuleSets;
                    };

                    directiveAPI.onBusinessRuleSetAdded = function (businessRuleSetObj) {
                        gridAPI.itemAdded(businessRuleSetObj);
                    };
                    directiveAPI.onBusinessRuleSetUpdated = function (businessRuleSetObj) {
                        gridAPI.itemUpdated(businessRuleSetObj);
                    };
                    return directiveAPI;
                }
            };

            $scope.onRuleActionsSelectiveReady = function (api) {
                ruleActionsSelectiveAPI = api;
                ruleActionsSelectiveReadyPromiseDeferred.resolve();
            };
            $scope.getRowStyle = function (dataItem) {
                return getRowStyle(dataItem);
            };

            function getRowStyle(dataItem) {
                var index = UtilsService.getItemIndexByVal(ruleSetActions, dataItem.RuleDefinitionId, 'BPBusinessRuleDefinitionId');
                if (index >= 0) {
                    dataItem.IsInherited = false;
                }
                else
                    dataItem.IsInherited = true;
                var rowStyle;
                if (!dataItem.IsInherited)
                    rowStyle = { CssClass: "bg-success" };
                return rowStyle;
            }

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            };

            function extendDataItem(dataItem) {
                dataItem.disabledChanged = false;

                if (!isEditMode && parentRuleSetId != undefined)
                    dataItem.IsInherited = true;

                dataItem.showReset = (!dataItem.IsInherited);
                dataItem.ruleActionsSelectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                dataItem.onRuleActionsSelectiveReady = function (api) {
                    console.log(dataItem);
                    dataItem.ruleActionsSelectiveAPI = api;
                    dataItem.ruleActionsSelectiveReadyPromiseDeferred.resolve();
                    var setLoader = function (value) { };
                    var payload = {};
                    payload.filter = {
                        Filters: [{
                            $type: "Vanrise.BusinessProcess.Entities.ActionTypeFilter,Vanrise.BusinessProcess.Entities",
                            ActionTypesIds: dataItem.ActionTypesIds,
                            ExcludedId: dataItem.Entity.Action.BPBusinessRuleActionTypeId
                        }]
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.ruleActionsSelectiveAPI, payload, setLoader);
                };

                dataItem.onDisabledChanged = function () {
                    dataItem.disabledChanged = !dataItem.disabledChanged;
                };

                dataItem.hideOverrideSettings = function () {
                    return isEditMode && !dataItem.IsInherited && dataItem.showReset;
                };

                dataItem.onResetClicked = function (dataItem) {
                    return BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService.GetParentActionDescription(bpBusinessRuleSetId, dataItem.RuleDefinitionId).then(function (response) {
                        if (response != undefined) {
                            dataItem.ActionDescription = response.ActionDescription;
                            dataItem.Entity.Action.BPBusinessRuleActionTypeId = response.BPBusinessRuleActionTypeId
                            dataItem.Entity.Disabled = response.Disabled;
                            dataItem.showReset = false;
                            dataItem.IsInherited = true;
                            dataItem.CssClass = "";
                        }
                    })
                      .catch(function (error) {
                          VRNotificationService.notifyException(error, $scope);
                      });

                };
            }
        }
    }
    return directiveDefinitionObject;

}]);

