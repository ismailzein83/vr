'use strict';
app.directive('vrWhsBePricingrulesettingsExtracharge', ['UtilsService', '$compile', 'WhS_BE_PricingRuleAPIService',
function (UtilsService, $compile, WhS_BE_PricingRuleAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            $scope.pricingRuleExtraChargeTemplates = [];
            var bePricingRuleExtraChargeSettingObject = new bePricingRuleExtraChargeSetting(ctrl, $scope, $attrs);
            bePricingRuleExtraChargeSettingObject.initializeController();
          
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/PricingRuleExtraChargeSettings.html"

    };


    function bePricingRuleExtraChargeSetting(ctrl, $scope, $attrs) {
        var pricingRuleExtraChargeTemplateDirectiveAPI;

        function initializeController() {

            $scope.disableAddButton = true;
            $scope.actions = [];
            $scope.addAction = function () {
                var action = getActionItem(null);
                $scope.actions.push(action);
            };
            $scope.onActionTemplateChanged = function () {
                $scope.disableAddButton = ($scope.selectedPricingRuleExtraChargeTemplate == undefined);
            };
            $scope.removeAction = function ($event, action) {
                $event.preventDefault();
                $event.stopPropagation();

                var index = UtilsService.getItemIndexByVal($scope.actions, action.ActionId, 'ActionId');
                $scope.actions.splice(index, 1);
            };
            defineAPI();

        }

        function getActionItem(dbAction) {

            var actionItem = {
                ActionId: $scope.actions.length + 1,

                ConfigId: (dbAction != null) ? dbAction.ConfigId : $scope.selectedPricingRuleExtraChargeTemplate.TemplateConfigID,

                Editor: (dbAction != null) ?
                    UtilsService.getItemByVal($scope.pricingRuleExtraChargeTemplates, dbAction.ConfigId, "TemplateConfigID").Editor :
                    $scope.selectedPricingRuleExtraChargeTemplate.Editor,

                Data: (dbAction != null) ? dbAction : {},
                Name: $scope.selectedPricingRuleExtraChargeTemplate.Name
            };

            actionItem.onPricingRuleExtraChargeTemplateDirectiveReady = function (api) {
                actionItem.ActionDirectiveAPI = api;
                actionItem.ActionDirectiveAPI.setData(actionItem.Data);

                actionItem.Data = undefined;
                actionItem.onPricingRuleExtraChargeTemplateDirectiveReady = undefined;
            }
            return actionItem;
        }
      
        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.BusinessEntity.Entities.PricingRuleExtraChargeSettings,TOne.WhS.BusinessEntity.Entities",
                    Actions: getActions(),
                }
                return obj;
            }
            function getActions() {
                var actionList = [];

                angular.forEach($scope.actions, function (item) {
                    var obj = item.ActionDirectiveAPI.getData();
                    obj.ConfigId = item.ConfigId;
                    actionList.push(obj);
                });

                return actionList;
            }
            api.setData = function (settings) {
                $scope.pricingRuleExtraChargeTemplates
                for (var i = 0; i < settings.Actions.length; i++)
                {
                    var action = settings.Actions[i];
                    for (var j = 0; j < $scope.pricingRuleExtraChargeTemplates.length; j++)
                        if (action.ConfigId == $scope.pricingRuleExtraChargeTemplates[j].TemplateConfigID)
                            $scope.selectedPricingRuleExtraChargeTemplate = $scope.pricingRuleExtraChargeTemplates[j];
                    action.Editor = $scope.selectedPricingRuleExtraChargeTemplate.Editor;
                    addAPIFunction(action);
                    $scope.actions.push(action);
                }
                function addAPIFunction(obj) {
                    obj.onPricingRuleExtraChargeTemplateDirectiveReady = function (api) {
                        obj.ActionDirectiveAPI = api;
                        obj.ActionDirectiveAPI.setData(obj);
                        obj = undefined;
                    }
                }
            }
            api.load = function () {
                return WhS_BE_PricingRuleAPIService.GetPricingRuleExtraChargeTemplates().then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.pricingRuleExtraChargeTemplates.push(itm);
                    });
                })
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);