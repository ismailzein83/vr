"use strict";

app.directive("retailBeAccountbedefinitionVieweditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountBEDefinitionViewEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/Templates/AccountBEDefinitionViewEditor.html"
        };
        function AccountBEDefinitionViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };
                 
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    promises.push(loadBusinessEntityDefinitionSelector());

                    function loadBusinessEntityDefinitionSelector() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                selectedIds: payload != undefined ? payload.BusinessEntityDefinitionId : undefined,
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Entities.AccountBEDefinitionFilter, Retail.BusinessEntity.Entities"
                                    }]
                                }
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Entities.AccountBEDefinitionViewSettings, Retail.BusinessEntity.Entities",
                        AccountBEDefinitionSettings: beDefinitionSelectorApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);