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
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/CostColumnsTemplate.html"
    };

    function CostColumns(ctrl, $scope) {
        this.initCtrl = initCtrl;

        function initCtrl() {
            defineScope();
            load();

            function defineScope() {
                ctrl.templates = [];
                ctrl.selectedTemplate = undefined;
                ctrl.dataSource = [];

                ctrl.onTemplateChange = function () {
                    ctrl.disableAddButton = (!ctrl.selectedTemplate);
                };
                ctrl.addDataItem = function () {
                    var dataItem = {
                        id: ctrl.dataSource.length + 1,
                        name: ctrl.selectedTemplate.Name,
                        ConfigId: ctrl.selectedTemplate.TemplateConfigID,
                        editor: ctrl.selectedTemplate.Editor
                    };

                    dataItem.onDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        var setLoader = function (value) { ctrl.isLoadingDirective = value };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, undefined, setLoader);
                    };

                    ctrl.dataSource.push(dataItem);
                };
                ctrl.removeDataItem = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal(ctrl.dataSource, dataItem.id, "id");
                    ctrl.dataSource.splice(index, 1);
                };
            }

            function load() {
                loadTemplates().then(function () {
                    getAPI();
                });

                function loadTemplates() {
                    return WhS_Sales_RatePlanAPIService.GetCostCalculationMethodTemplates().then(function (templates) {
                        if (templates != null && templates.length) {
                            for (var i = 0; i < templates.length; i++)
                                ctrl.templates.push(templates[i]);
                        }
                    });
                }
            }

            function getAPI() {
                var api = {};

                api.load = function (settings) {
                    if (settings && settings.CostCalculationMethods) {
                        for (var i = 0; i < settings.CostCalculationMethods.length; i++) {
                            var dataItem = getDataItem(settings.CostCalculationMethods[i]);
                            ctrl.dataSource.push(dataItem);
                        }
                    }

                    function getDataItem(costColumn) {
                        var template = UtilsService.getItemByVal(ctrl.templates, costColumn.ConfigId, "TemplateConfigID");

                        if (template) {
                            var dataItem = {
                                id: ctrl.dataSource.length + 1,
                                name: template.Name,
                                ConfigId: template.TemplateConfigID,
                                editor: template.Editor
                            };

                            dataItem.onDirectiveReady = function (api) {
                                dataItem.directiveAPI = api;
                                var setLoader = function (value) { ctrl.isLoadingDirective = value };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, costColumn, setLoader);
                            };

                            return dataItem;
                        }

                        return null;
                    }
                };

                api.getData = function () {
                    var costCalculationMethods = [];

                    for (var i = 0; i < ctrl.dataSource.length; i++) {
                        if (ctrl.dataSource[i].directiveAPI) {
                            var data = ctrl.dataSource[i].directiveAPI.getData();
                            
                            if (data) {
                                data.ConfigId = ctrl.dataSource[i].ConfigId;
                                costCalculationMethods.push(data);
                            }
                        }
                    }
                    
                    return (costCalculationMethods.length > 0) ? costCalculationMethods : null;
                };

                if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }
    }
}]);
