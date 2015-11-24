'use strict';
app.directive('vrWhsBePricingrulesettingsExtracharge', ['UtilsService', '$compile', 'WhS_BE_PricingRuleAPIService','VRUIUtilsService',
function (UtilsService, $compile, WhS_BE_PricingRuleAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.extraChargeTemplates = [];
            var ctor = new extraChargeCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
          
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Settings/Templates/PricingRuleExtraChargeSettings.html"

    };


    function extraChargeCtor(ctrl, $scope, $attrs) {
        function initializeController() {

            ctrl.datasource = [];
            ctrl.isValid = function () {

                if (ctrl.datasource.length > 0)
                    return null;
                return "You Should at least one filter type ";
            }
            ctrl.disableAddButton = true;
            ctrl.addFilter = function () {
                var dataItem = {
                    id: ctrl.datasource.length + 1,
                    configId: ctrl.selectedTemplate.TemplateConfigID,
                    editor: ctrl.selectedTemplate.Editor,
                    name: ctrl.selectedTemplate.Name
                };
                dataItem.onDirectiveReady = function (api) {
                    dataItem.directiveAPI = api;
                    var setLoader = function (value) { ctrl.isLoadingDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, undefined, setLoader);
                };
                ctrl.datasource.push(dataItem);

                
                ctrl.selectedTemplate = undefined;
            };
            ctrl.onActionTemplateChanged = function () {
                ctrl.disableAddButton = (ctrl.selectedTemplate == undefined);
            };
            ctrl.removeFilter = function (dataItem) {
                var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.id, 'id');
                ctrl.datasource.splice(index, 1);
            };
            defineAPI();

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

                angular.forEach(ctrl.datasource, function (item) {
                    var obj = item.directiveAPI.getData();
                    obj.ConfigId = item.configId;
                    actionList.push(obj);
                });

                return actionList;
            }
            api.load = function (payload) {
                return loadFiltersSection(payload);
            }

            function loadFiltersSection(payload) {
                var promises = [];

                var filterItems;
                if (payload != undefined) {
                    filterItems = [];
                    for (var i = 0; i < payload.Actions.length; i++) {
                        var filterItem = {
                            payload: payload.Actions[i],
                            readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                            loadPromiseDeferred: UtilsService.createPromiseDeferred()
                        };
                        promises.push(filterItem.loadPromiseDeferred.promise);
                        filterItems.push(filterItem);
                    }
                }

                var loadTemplatesPromise = WhS_BE_PricingRuleAPIService.GetPricingRuleExtraChargeTemplates().then(function (response) {
                    angular.forEach(response, function (itm) {
                        ctrl.extraChargeTemplates.push(itm);
                    });

                    if (filterItems != undefined) {
                        for (var i = 0; i < filterItems.length; i++) {
                            addFilterItemToGrid(filterItems[i]);
                        }
                    }
                });

                promises.push(loadTemplatesPromise);

                function addFilterItemToGrid(filterItem) {
                    var matchItem = UtilsService.getItemByVal(ctrl.extraChargeTemplates, filterItem.payload.ConfigId, "TemplateConfigID");
                    if (matchItem == null)
                        return;

                    var dataItem = {
                        id: ctrl.datasource.length + 1,
                        configId: matchItem.TemplateConfigID,
                        editor: matchItem.Editor,
                        name: matchItem.Name
                    };
                    var dataItemPayload = filterItem.payload;

                    dataItem.onDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        filterItem.readyPromiseDeferred.resolve();
                    };

                    filterItem.readyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, filterItem.loadPromiseDeferred);
                        });

                    ctrl.datasource.push(dataItem);
                }

                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);