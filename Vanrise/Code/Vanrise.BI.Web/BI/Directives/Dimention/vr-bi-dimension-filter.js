"use strict";

app.directive("vrBiDimensionFilter", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var dataRecordTypeManagement = new DimensionFilter($scope, ctrl, $attrs);
                dataRecordTypeManagement.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/BI/Directives/Dimention/Templates/DimensionFilterTemplate.html"

        };

        function DimensionFilter($scope, ctrl, $attrs) {
            var gridAPI;
            var entityNames=[];
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];
                ctrl.validate = function () {
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        for (var j = 0; j < ctrl.datasource.length; j++) {
                            if (i != j && ctrl.datasource[i].selectedEntityName != undefined && ctrl.datasource[j].selectedEntityName != undefined)
                                if (ctrl.datasource[i].selectedEntityName.Name == ctrl.datasource[j].selectedEntityName.Name)
                                    return "Same entity selected more than once.";
                        }
                    }
                    return null;
                };
                ctrl.removeFilter = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    if (index != -1)
                        ctrl.datasource.splice(index, 1);
                };
                ctrl.addFilter = function () {
                    var filter = {
                        entityNames : entityNames,
                        onDimensionSelectorReady : function (api)
                        {
                            filter.dimensionAPI = api;
                            var setLoader = function (value) { filter.isLoadingDimentionDirective = value };
                            var payload = { entityName: filter.selectedEntityName.Name };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filter.dimensionAPI, payload, setLoader);
                        },
                        onSelectItem: function (selectItem)
                        {
                            if (filter.dimensionAPI != undefined)
                            {
                                var setLoader = function (value) { filter.isLoadingDimentionDirective = value };
                                var payload = { entityName: selectItem.Name };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filter.dimensionAPI, payload, setLoader);
                            }

                          
                        }
                    };
                    ctrl.datasource.push(filter);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var entityFilter = [];
                    if (ctrl.datasource.length > 0) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var dataItem = ctrl.datasource[i];
                            entityFilter.push({
                                EntityName: dataItem.selectedEntityName != undefined ? dataItem.selectedEntityName.Name : undefined,
                                Values: dataItem.dimensionAPI != undefined ? dataItem.dimensionAPI.getSelectedIds() : undefined
                            });
                        }
                    }
                    return entityFilter;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        var promises = [];
                        if (payload.entityNames != undefined) {
                            entityNames.length = 0;
                            for (var i = 0; i < payload.entityNames.length ; i++) {
                                entityNames.push({ Name: payload.entityNames[i] });
                            }
                        }
                        if (payload.filter != undefined) {

                            for (var i = 0 ; i < payload.filter.length; i++) {
                                addFilterDataItemAPI(payload.filter[i], promises);
                            }


                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function addFilterDataItemAPI(filter, promises) {
                var dataItem = {
                    entityNames: entityNames,
                    selectedEntityName: { Name: filter.EntityName },
                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                    onDimensionSelectorReady: function (api) {
                        dataItem.dimensionAPI = api;
                        dataItem.readyPromiseDeferred.resolve();
                    },
                    onSelectItem: function (selectItem) {
                        if (dataItem.dimensionAPI != undefined) {
                            var setLoader = function (value) { dataItem.isLoadingDimentionDirective = value };
                            var payload = { entityName: selectItem.Name };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.dimensionAPI, payload, setLoader);
                        }
                    }
                };
                promises.push(dataItem.loadPromiseDeferred.promise);
                dataItem.readyPromiseDeferred.promise.then(function () {
                    var payload = { entityName: dataItem.selectedEntityName.Name, selectedIds: filter.Values };
                    VRUIUtilsService.callDirectiveLoad(dataItem.dimensionAPI, payload, dataItem.loadPromiseDeferred);
                }),
                ctrl.datasource.push(dataItem);
            }
        }

        return directiveDefinitionObject;

    }
]);