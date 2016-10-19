"use strict";

app.directive("businessprocessBpBusinessRuleSetDetail", ["UtilsService", "VRNotificationService", "BusinessProcess_BPBusinessRuleSetAPIService", "BusinessProcess_BPBusinessRuleSetService", "VRUIUtilsService", "BusinessProcess_BPBusinessRuleDefintionAPIService",
function (UtilsService, VRNotificationService, BusinessProcess_BPBusinessRuleSetAPIService, BusinessProcess_BPBusinessRuleSetService, VRUIUtilsService, BusinessProcess_BPBusinessRuleDefintionAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var bpBusinessRuleSetDetail = new BPBusinessRuleSetDetail($scope, ctrl, $attrs);
            bpBusinessRuleSetDetail.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPBusinessRuleSet/Templates/BPBusinessRuleSetDetailTemplate.html"

    };

    function BPBusinessRuleSetDetail($scope, ctrl, $attrs) {

        var gridAPI;
        var bpDefinitionId;
        var existingBusinessRules;
        $scope.businessRuleDefintions = [];
        $scope.bpBusinessRules = [];
        $scope.scopeModel = {};
        var removedItems = [];

        this.initializeController = initializeController;

        function initializeController() {

            ctrl.isValid = function () {
                if ($scope.bpBusinessRules.length > 0)
                    return null;
                return "You Should Select at least one business rule";
            }

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {

                ctrl.onReady(getDirectiveAPI());
            }
            function getDirectiveAPI() {
                var directiveAPI = {};
                directiveAPI.load = function (payload) {
                    bpDefinitionId = payload.BPDefinitionId;
                    if (payload.existingBusinessRules != undefined)
                        existingBusinessRules = payload.existingBusinessRules;
                    return UtilsService.waitMultipleAsyncOperations([loadBusinessRuleDefintions]);
                }

                directiveAPI.getData = function () {
                    var obj = [];
                    for (var x = 0; x < $scope.bpBusinessRules.length; x++) {
                        $scope.bpBusinessRules[x].Data = $scope.bpBusinessRules[x].directiveAPI.getData();
                        obj.push($scope.bpBusinessRules[x]);
                    }
                    return obj;
                }

                function loadBusinessRuleDefintions() {
                    $scope.businessRuleDefintions.length = 0;
                    BusinessProcess_BPBusinessRuleDefintionAPIService.GetBusinessRuleDefintionsByBPDefinitionID(bpDefinitionId).then(function (response) {
                        if (response != undefined) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.businessRuleDefintions.push(response[i]);
                            }
                        }

                        fillexistingBusinessRules();
                    });
                }

                function fillexistingBusinessRules() {
                    removedItems.length = 0;
                    $scope.bpBusinessRules.length = 0;
                    if (existingBusinessRules == undefined)
                        return;

                    for (var m = 0; m < existingBusinessRules.ActionDetails.length; m++) {
                        var currentRule = existingBusinessRules.ActionDetails[m];

                        for (var n = 0; n < $scope.businessRuleDefintions.length; n++) {
                            if (currentRule.BPBusinessRuleDefinitionId == $scope.businessRuleDefintions[n].Entity.BPBusinessRuleDefinitionId) {
                                var matchedRule = $scope.businessRuleDefintions[n];
                                for (var p = 0; p < matchedRule.ActionTypes.length; p++) {

                                    if (matchedRule.ActionTypes[p].ExtensionConfigurationId == currentRule.Settings.Action.BPBusinessRuleActionTypeId) {
                                        matchedRule.selectedAction = matchedRule.ActionTypes[p];
                                    }
                                }
                                setOnDirectiveReady(matchedRule, currentRule);
                                removedItems.push(matchedRule);
                                $scope.businessRuleDefintions.splice($scope.businessRuleDefintions.indexOf(matchedRule), 1);
                                $scope.bpBusinessRules.push(matchedRule);

                                break;
                            }
                        }
                    }
                    $scope.bpBusinessRules.sort(function (b, a) {
                        return b.Entity.BPBusinessRuleDefinitionId - a.Entity.BPBusinessRuleDefinitionId;
                    });
                }

                function setOnDirectiveReady(matchedRule, currentRule) {
                    matchedRule.onDirectiveReady = function (api) {
                        matchedRule.directiveAPI = api;
                        var setLoader = function (value) { ctrl.isLoadingDirective = value };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, matchedRule.directiveAPI, currentRule.Settings.Action.BPBusinessRuleActionTypeId, setLoader);
                    };
                }
                return directiveAPI;
            }
        }

        $scope.addBusinessRuleDefinition = function () {
            if ($scope.scopeModel.selectedbusinessRuleDefintion != undefined) {

                var dataItem = $scope.scopeModel.selectedbusinessRuleDefintion;

                removedItems.push($scope.scopeModel.selectedbusinessRuleDefintion);
                $scope.businessRuleDefintions.splice($scope.businessRuleDefintions.indexOf($scope.scopeModel.selectedbusinessRuleDefintion), 1);

                dataItem.onDirectiveReady = function (api) {
                    dataItem.directiveAPI = api;
                    var setLoader = function (value) { ctrl.isLoadingDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, undefined, setLoader);
                };

                $scope.bpBusinessRules.push(dataItem);

                $scope.bpBusinessRules.sort(function (b, a) {
                    return b.Entity.BPBusinessRuleDefinitionId - a.Entity.BPBusinessRuleDefinitionId;
                });
                $scope.scopeModel.selectedbusinessRuleDefintion = undefined;
            }
        }

        $scope.removeBusinessRule = function (dataItem) {
            var index = UtilsService.getItemIndexByVal($scope.bpBusinessRules, dataItem.Entity.BPBusinessRuleDefinitionId, 'Entity.BPBusinessRuleDefinitionId');
            $scope.bpBusinessRules.splice(index, 1);

            var currentIndex = -1;
            for (var t = 0; t < removedItems.length; t++) {
                if (dataItem.Entity.BPBusinessRuleDefinitionId == removedItems[t].Entity.BPBusinessRuleDefinitionId) {
                    $scope.businessRuleDefintions.push(removedItems[t]);
                    currentIndex = t;
                    break;
                }
            }

            $scope.businessRuleDefintions.sort(function (b, a) {
                return b.Entity.BPBusinessRuleDefinitionId - a.Entity.BPBusinessRuleDefinitionId;
            });
            removedItems.splice(currentIndex, 1);
        };
    }

    return directiveDefinitionObject;

}]);