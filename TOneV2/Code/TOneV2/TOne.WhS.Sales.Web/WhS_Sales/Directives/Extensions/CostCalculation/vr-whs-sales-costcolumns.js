"use strict";

app.directive("vrWhsSalesCostcolumns", ["WhS_Sales_RatePlanAPIService", "UtilsService", "VRUIUtilsService","VRDragdropService",
function (WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService,VRDragdropService) {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var costColumns = new CostColumns(ctrl, $scope);
            costColumns.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/CostCalculation/Templates/CostColumnsTemplate.html"
    };

    function CostColumns(ctrl, $scope)
    {
        this.initializeController = initializeController;

        function initializeController()
        {
            ctrl.templates = [];
            ctrl.dataItems = [];               
         
            ctrl.removeDataItem = function (dataItem) {
                var index = UtilsService.getItemIndexByVal(ctrl.dataItems, dataItem.id, "id");
                ctrl.dataItems.splice(index, 1);
            };

            ctrl.dragdropGroupCorrelation = VRDragdropService.createCorrelationGroup();
            ctrl.dragdropSetting = {
                groupCorrelation: ctrl.dragdropGroupCorrelation,
                canReceive: true,
                onItemReceived: function (item, dataSource) {    
                    var dataItem = {};
                    dataItem.configId = item.ExtensionConfigurationId;
                    dataItem.title = item.Title;
                    dataItem.directive = item.Editor;
                    dataItem.directivePayload = undefined;

                    dataItem.loadDeferred = UtilsService.createPromiseDeferred();
                    dataItem.loadDeferred.promise.finally(function () { dataItem.isLoadingDirective = false; });

                    dataItem.onDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        dataItem.isLoadingDirective = true;
                        VRUIUtilsService.callDirectiveLoad(api, dataItem.directivePayload, dataItem.loadDeferred);
                    };

                    return dataItem;
                },
                enableSorting: true
            };
            defineAPI();
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (payload)
            {
                var promises = [];
                ctrl.dataItems.length = 0;

                var costCalculationMethods;

                if (payload != undefined) {
                    costCalculationMethods = payload.costCalculationMethods;
                }

                var loadTemplatesPromise = loadTemplates();
                promises.push(loadTemplatesPromise);

                var loadDirectivesDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadDirectivesDeferred.promise);

                loadTemplatesPromise.then(function ()
                {
                    var directiveLoadPromises = [];

                    if (costCalculationMethods != undefined)
                    {
                        for (var i = 0; i < costCalculationMethods.length; i++)
                        {
                            var dataItem = getDataItem(costCalculationMethods[i], false);
                            directiveLoadPromises.push(dataItem.loadDeferred.promise);
                            ctrl.dataItems.push(dataItem);
                        }
                    }

                    UtilsService.waitMultiplePromises(directiveLoadPromises).then(function () {
                        loadDirectivesDeferred.resolve();
                    }).catch(function (error) {
                        loadDirectivesDeferred.reject(error);
                    });
                });

                function loadTemplates() {
                    return WhS_Sales_RatePlanAPIService.GetCostCalculationMethodTemplates().then(function (templates) {
                        if (templates != null) {
                            for (var i = 0; i < templates.length; i++) {
                                ctrl.templates.push(templates[i]);
                            }
                        }
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
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

        function getDataItem(costCalculationMethod,item, showLoader)
        {
            var dataItem = {};

            dataItem.id = ctrl.dataItems.length;

            if (costCalculationMethod != undefined)
            {
                var template = UtilsService.getItemByVal(ctrl.templates, costCalculationMethod.ConfigId, "ExtensionConfigurationId");
                dataItem.configId = costCalculationMethod.ConfigId;
                dataItem.title = template.Title;
                dataItem.directive = template.Editor;
                dataItem.directivePayload = costCalculationMethod
            }
            else {
                dataItem.configId = ctrl.selectedTemplate.ExtensionConfigurationId;
                dataItem.title = ctrl.selectedTemplate.Title;
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
}]);
