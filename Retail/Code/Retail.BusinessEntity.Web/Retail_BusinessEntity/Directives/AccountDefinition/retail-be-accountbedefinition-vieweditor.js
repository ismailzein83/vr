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
                var accountBEDefinitionViewEditor = new AccountBEDefinitionViewEditor($scope, ctrl, $attrs);
                accountBEDefinitionViewEditor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/Templates/AccountBEDefinitionViewEditor.html"
        };
        function AccountBEDefinitionViewEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeSelectorDirectiveReady = function (api) {
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
                                        $type: "Retail.BusinessEntity.Entities.AccountBEDefinitionViewFilter, Retail.BusinessEntity.Entities"
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