"use strict";

app.directive("vrCommonDashboardVieweditor", ["UtilsService", "VRUIUtilsService",
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
            templateUrl: "/Client/Modules/Common/Directives/VRDashboard/Templates/VRDashboardViewEditor.html"
        };
        function ViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var vrDashboardDefinitionDirectiveApi;
            var vrDashboardDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDashboardDefinitionDirectiveReady = function (api) {
                    vrDashboardDefinitionDirectiveApi = api;
                    vrDashboardDefinitionReadyPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([vrDashboardDefinitionReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    promises.push(loadDashboardDefinitionDirective());

                    function loadDashboardDefinitionDirective() {
                        var dashboardDefinitionLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        vrDashboardDefinitionReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = {
                                    DashboardDefinitionItems: payload != undefined ? payload.DashboardDefinitionItems : undefined
                                };

                                VRUIUtilsService.callDirectiveLoad(vrDashboardDefinitionDirectiveApi, directivePayload, dashboardDefinitionLoadPromiseDeferred);
                            });
                        return dashboardDefinitionLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return vrDashboardDefinitionDirectiveApi.getData();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };
        }

        return directiveDefinitionObject;
    }
]);