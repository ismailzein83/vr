(function (app) {

    'use strict';

    VRRetailAccountObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function VRRetailAccountObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var retailAccountObjectType = new RetailAccountObjectType($scope, ctrl, $attrs);
                retailAccountObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/RetailAccountObjectTypeTemplate.html"

        };
        function RetailAccountObjectType($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;

            var businessEntityDefinitionSelectorAPI;
            var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    businessEntityDefinitionSelectorAPI = api;
                    businessEntityDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onBusinessEntityDefinitionSelectionChanged = function (selectedItem) {
                    if (selectedItem != undefined) {
                        context.canDefineProperties(true);
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountBEDefinitionId;

                    if (payload != undefined) {
                        context = payload.context;
                        context.canDefineProperties(false);

                        if (payload.objectType != undefined) {
                            accountBEDefinitionId = payload.objectType.AccountBEDefinitionId;
                        }
                    }

                    //Loading BusinessEntityDefinition selector
                    var businessEntityDefinitionSelectorLoadPromise = loadBusinessEntityDefinitionSelector();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);


                    function loadBusinessEntityDefinitionSelector() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        businessEntityDefinitionSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business",
                                    }]
                                },
                                selectedIds: accountBEDefinitionId
                            };

                            VRUIUtilsService.callDirectiveLoad(businessEntityDefinitionSelectorAPI, payload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.VRObjectTypes.RetailAccountObjectType, Retail.BusinessEntity.MainExtensions",
                        AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeRetailaccountObjecttype', VRRetailAccountObjectType);

})(app);