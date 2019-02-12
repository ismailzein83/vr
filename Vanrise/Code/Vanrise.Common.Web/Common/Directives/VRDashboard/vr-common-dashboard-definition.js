"use strict";

app.directive("vrCommonDashboardDefinition", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ViewEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }

                
        };
        function ViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var vrDashboardDirectiveApi;
            var vrDashboardDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onVRDashboardDirectiveReady = function (api) {
                    vrDashboardDirectiveApi = api;
                    vrDashboardDirectivePromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([vrDashboardDirectivePromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    promises.push(loadVRDashboardDirective());

                    function loadVRDashboardDirective() {
                        var vrDashboardPayload;
                        vrDashboardPayload = {
                            businessEntityDefinitionId: '6243CA7F-A14C-41BE-BE48-86322D835CA6'
                        };

                        if (payload != undefined && payload.DashboardDefinitionItems != undefined) {
                            var selectedIds = [];

                            for (var i = 0; i < payload.DashboardDefinitionItems.length; i++) {
                                var dashboardItem = payload.DashboardDefinitionItems[i];
                                selectedIds.push(dashboardItem.DashboardDefinitionId);
                            }
                            vrDashboardPayload.selectedIds = selectedIds;
                        }

                        return vrDashboardDirectiveApi.load(vrDashboardPayload);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var dashboardDefinitionItems;
                    var selectedDashboards = vrDashboardDirectiveApi.getSelectedIds();
                    if (selectedDashboards != undefined) {
                        dashboardDefinitionItems = [];
                        for (var i = 0; i < selectedDashboards.length; i++) {
                            dashboardDefinitionItems[i] = {
                                DashboardDefinitionId: selectedDashboards[i]
                            };
                        }
                    }
                    return {
                        $type: "Vanrise.Common.Business.VRDashboardViewSettings, Vanrise.Common.Business",
                        DashboardDefinitionItems: dashboardDefinitionItems
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };
        }

        function getTemplate(attrs) {
            return '<vr-columns width="1/3row">'
                + '<vr-genericdata-genericbusinessentity-selector isrequired = "true" on-ready="scopeModel.onVRDashboardDirectiveReady" ismultipleselection = "true" customlabel = "Dashboards" ></vr-genericdata-genericbusinessentity-selector>'
                + '</vr-columns >';
        }

        return directiveDefinitionObject;
    }
]);