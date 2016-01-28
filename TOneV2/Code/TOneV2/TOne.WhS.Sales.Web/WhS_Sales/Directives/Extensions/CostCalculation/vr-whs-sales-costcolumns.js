"use strict";

app.directive("vrWhsSalesCostcolumns", ["WhS_Sales_RatePlanAPIService", "UtilsService", "VRUIUtilsService",
function (WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var costColumns = new CostColumns(ctrl, $scope);
            costColumns.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/CostCalculation/Templates/CostColumnsTemplate.html"
    };

    function CostColumns(ctrl, $scope) {
        this.initCtrl = initCtrl;

        function initCtrl() {
            defineScope();
            getAPI();

            function defineScope() {
                ctrl.templates = [];
                ctrl.selectedTemplate;
                ctrl.disableAddButton = true;
                ctrl.dataItems = [];

                ctrl.onTemplateSelected = function (selectedItem) {
                    ctrl.disableAddButton = false;
                };
                ctrl.addDataItem = function () {
                    ctrl.dataItems.push(getDataItem(null, true));
                };
                ctrl.removeDataItem = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal(ctrl.dataItems, dataItem.id, "id");
                    ctrl.dataItems.splice(index, 1);
                };
            }

            function getAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.dataItems.length = 0;
                    var promises = [];

                    var loadTemplatesDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadTemplatesDeferred.promise);

                    loadTemplates().then(function () {
                        if (payload && payload.costCalculationMethods) {
                            for (var i = 0; i < payload.costCalculationMethods.length; i++) {
                                var dataItem = getDataItem(payload.costCalculationMethods[i], false);
                                promises.push(dataItem.loadDeferred.promise);
                                ctrl.dataItems.push(dataItem);
                            }
                        }

                        loadTemplatesDeferred.resolve(); // This ensures that all promises are taken into account
                    }).catch(function () {
                        loadTemplatesDeferred.reject();
                    });

                    return UtilsService.waitMultiplePromises(promises);

                    function loadTemplates() {
                        return WhS_Sales_RatePlanAPIService.GetCostCalculationMethodTemplates().then(function (templates) {
                            if (templates) {
                                for (var i = 0; i < templates.length; i++) {
                                    ctrl.templates.push(templates[i]);
                                }
                            }
                        });
                    }
                };

                api.getData = function () {
                    var costCalculationMethods = [];

                    for (var i = 0; i < ctrl.dataItems.length; i++) {
                        var costCalculationMethod = ctrl.dataItems[i].directiveAPI.getData();
                        costCalculationMethod.ConfigId = ctrl.dataItems[i].configId;
                        costCalculationMethods.push(costCalculationMethod);
                    }
                    
                    return costCalculationMethods.length > 0 ? costCalculationMethods : null;
                };

                if (ctrl.onReady && typeof ctrl.onReady == "function")
                    ctrl.onReady(api);
            }

            function getDataItem(costCalculationMethod, showLoader) {
                var dataItem = {};

                dataItem.id = ctrl.dataItems.length;
                
                if (costCalculationMethod) {
                    var template = UtilsService.getItemByVal(ctrl.templates, costCalculationMethod.ConfigId, "TemplateConfigID");

                    if (template) {
                        dataItem.configId = costCalculationMethod.ConfigId;
                        dataItem.name = template.Name;
                        dataItem.directive = template.Editor;
                        dataItem.directivePayload = costCalculationMethod
                    }
                }
                else {
                    dataItem.configId = ctrl.selectedTemplate.TemplateConfigID;
                    dataItem.name = ctrl.selectedTemplate.Name;
                    dataItem.directive = ctrl.selectedTemplate.Editor;
                    dataItem.directivePayload = undefined;
                }

                dataItem.loadDeferred = UtilsService.createPromiseDeferred();
                dataItem.loadDeferred.promise.finally(function () { dataItem.isLoadingDirective = false; });

                dataItem.onDirectiveReady = function (api) {
                    dataItem.directiveAPI = api;
                    dataItem.isLoadingDirective = showLoader;
                    VRUIUtilsService.callDirectiveLoad(api, dataItem.directivePayload, dataItem.loadDeferred);
                };

                return dataItem;
            }
        }
    }
}]);
