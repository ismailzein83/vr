"use strict";

app.directive("retailDataIcxdatasettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ICXDataSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Data/Directives/Settings/Templates/ICXDataSettingsTemplate.html"
        };

        function ICXDataSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beParentChildRelationDefinitionSelectorAPI;
            var beParentChildRelationDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBEParentChildRelationDefinitionSelectorReady = function (api) {
                    beParentChildRelationDefinitionSelectorAPI = api;
                    beParentChildRelationDefinitionSelectorPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.sessionOctetsLimit = payload.data.SessionOctetsLimit;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.Data.Entities.ICXDataSettings, Retail.Data.Entities",
                        SessionOctetsLimit: $scope.scopeModel.sessionOctetsLimit
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);