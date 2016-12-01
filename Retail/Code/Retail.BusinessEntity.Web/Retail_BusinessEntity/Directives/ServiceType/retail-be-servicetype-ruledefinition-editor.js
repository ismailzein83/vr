'use strict';
app.directive('retailBeServicetypeRuledefinitionEditor', ['UtilsService', 'VRUIUtilsService',
function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new selectiveCtor(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/ServiceType/Templates/ServiceTypeRuleDefinitionEditor.html"

    };


    function selectiveCtor(ctrl, $scope, $attrs) {
        var ruleDefinitionSelectorAPI;
        var ruleDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            ctrl.selectedRuleDefinitions = [];
            ctrl.onRuleDefinitionSelectorReady = function (api) {
                ruleDefinitionSelectorAPI = api;
                ruleDefinitionSelectorReadyDeferred.resolve();
            };
            ctrl.datasource = [];
            ctrl.onSelectItem = function (selectedItem) {
                addRuleDefinitionFunction(selectedItem);
            };
            ctrl.onDeselectItem = function (dataItem) {
                var datasourceIndex = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.RuleDefinitionId, 'RuleDefinitionId');
                ctrl.datasource.splice(datasourceIndex, 1);
            };

            function addRuleDefinitionFunction(selectedItem) {
                var dataItem = {
                    RuleDefinitionId: selectedItem.GenericRuleDefinitionId,
                    Name: selectedItem.Name,
                    Title: selectedItem.Name,
                };
                ctrl.datasource.push(dataItem);
            }
        
            ctrl.removeFilter = function (dataItem) {
                var index = UtilsService.getItemIndexByVal(ctrl.selectedRuleDefinitions, dataItem.RuleDefinitionId, 'GenericRuleDefinitionId');
                ctrl.selectedRuleDefinitions.splice(index, 1);
                var datasourceIndex = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.RuleDefinitionId, 'RuleDefinitionId');
                ctrl.datasource.splice(datasourceIndex, 1);
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var ruleDefinitions = [];
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    ruleDefinitions.push({
                        RuleDefinitionId: ctrl.datasource[i].RuleDefinitionId,
                        Title: ctrl.datasource[i].Title,
                    });
                }
                return ruleDefinitions;
            };

            api.load = function (payload) {

                var ruleDefinitionIds = [];
                if (payload != undefined && payload.ruleDefinitions != undefined) {
                    for (var i = 0; i < payload.ruleDefinitions.length; i++) {
                        var obj = payload.ruleDefinitions[i];
                        ruleDefinitionIds.push(obj.RuleDefinitionId);
                    }
                }

                var ruleDefinitionLoadDeferred = UtilsService.createPromiseDeferred();

                ruleDefinitionSelectorReadyDeferred.promise.then(function () {
                    var ruleDefinitionPayload = { filter: { Filters: [{ $type: "Retail.BusinessEntity.Business.ChargingPolicyRuleDefinitionFilter,Retail.BusinessEntity.Business" }] } };
                    if (ruleDefinitionIds.length > 0) {
                        ruleDefinitionPayload.selectedIds = ruleDefinitionIds;
                    }
                    VRUIUtilsService.callDirectiveLoad(ruleDefinitionSelectorAPI, ruleDefinitionPayload, ruleDefinitionLoadDeferred);
                });

                return ruleDefinitionLoadDeferred.promise.then(function () {
                    if (payload != undefined && payload.ruleDefinitions != undefined) {
                        for (var i = 0; i < payload.ruleDefinitions.length; i++) {
                            var ruleDefinition = payload.ruleDefinitions[i];
                            addDataItemToGrid(ruleDefinition)
                        }
                    }
                });

            };

            function addDataItemToGrid(ruleDefinition) {
                var item = UtilsService.getItemByVal(ctrl.selectedRuleDefinitions, ruleDefinition.RuleDefinitionId, 'GenericRuleDefinitionId');
                var dataItem = {
                    RuleDefinitionId: ruleDefinition.RuleDefinitionId,
                    Title: ruleDefinition.Title,
                    Name: item != undefined ? item.Name : undefined
                };
                ctrl.datasource.push(dataItem);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);