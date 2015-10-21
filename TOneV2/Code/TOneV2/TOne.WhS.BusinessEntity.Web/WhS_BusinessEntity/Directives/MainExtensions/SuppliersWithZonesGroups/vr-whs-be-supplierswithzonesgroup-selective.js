'use strict';
app.directive('vrWhsBeSupplierswithzonesSelective', ['UtilsService', '$compile', 'WhS_BE_PricingRuleAPIService',
function (UtilsService, $compile, WhS_BE_PricingRuleAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var beSuppliersWithZonesObject = new beSuppliersWithZones(ctrl, $scope, $attrs);
            beSuppliersWithZonesObject.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SuppliersWithZonesGroups/Templates/SelectiveSuppliersWithZonesGroup.html"

    };


    function beSuppliersWithZones(ctrl, $scope, $attrs) {
        var carrierAccountDirectiveAPI;
        var supplierZonesDirectiveAPI;
        function initializeController() {
            $scope.suppliers = [];
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                declareDirectiveAsReady()
            }
            $scope.onSelectionChanged = function () {
                if (carrierAccountDirectiveAPI != undefined) {
                    var obj = carrierAccountDirectiveAPI.getData();
                    $scope.suppliers.length = 0;
                    for (var i = 0; i < obj.length; i++) {
                        addSupplierZoneAPIFunction(obj[i]);
                    }
                    
                }
            };
            function addSupplierZoneAPIFunction(obj) {
                obj.onSupplierZonesDirectiveReady = function (api) {
                    obj.supplierZonesDirectiveAPI = api;
                    obj.supplierZonesDirectiveAPI.load();
                }
                $scope.suppliers.push(obj);
            }
            $scope.removeSupplier = function (supplier) {
                var index = UtilsService.getItemIndexByVal($scope.suppliers, supplier.CarrierAccountId, 'CarrierAccountId');
                $scope.suppliers.splice(index, 1);
            };
            
        }
        function declareDirectiveAsReady() {
            if (carrierAccountDirectiveAPI == undefined)
                return;

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
                for (var i = 0; i < settings.Actions.length; i++) {
                    var action = settings.Actions[i];
                    for (var j = 0; j < $scope.pricingRuleExtraChargeTemplates.length; j++)
                        if (action.ConfigId == $scope.pricingRuleExtraChargeTemplates[j].TemplateConfigID)
                            action.Editor = $scope.pricingRuleExtraChargeTemplates[j].Editor;
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
                return carrierAccountDirectiveAPI.load();
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);