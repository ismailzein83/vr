(function (app) {

    'use strict';

    OnnetOperatorDirectionService.$inject = ["UtilsService", 'VRUIUtilsService', 'Retail_RA_OnNetOperatorDirectionServicesAPIService'];

    function OnnetOperatorDirectionService(UtilsService, VRUIUtilsService, Retail_RA_OnNetOperatorDirectionServicesAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new OperatorDirectionServicesCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_RA/Directives/OnNetOperatorDeclaration/Templates/OnNetOperationDirectionServicesTemplate.html"

        };
        function OperatorDirectionServicesCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var gridAPI;
            var directiveAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.extensionConfigs = [];
                $scope.scopeModel.onNetOperatorDeclarationServices = [];

                $scope.removerow = function (dataItem) {
                    var index = $scope.scopeModel.onNetOperatorDeclarationServices.indexOf(dataItem);
                    $scope.scopeModel.onNetOperatorDeclarationServices.splice(index, 1);
                };

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.onNetOperatorDeclarationServices != undefined && $scope.scopeModel.onNetOperatorDeclarationServices.length > 0)
                        return null;
                    return "You Should add at least one item.";
                };


                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.addExtensionConfiguration = function () {
                    var onNetOperatorDeclarationService = {
                        Title: $scope.scopeModel.selectedExtensionConfig.Title,
                        Editor: $scope.scopeModel.selectedExtensionConfig.Editor,
                    };
                    onNetOperatorDeclarationService.onDirectiveReady = function (api) {
                        onNetOperatorDeclarationService.directiveAPI = api;
                        var setLoader = function (value) { onNetOperatorDeclarationService.isLoadingDirective = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, onNetOperatorDeclarationService.directiveAPI, undefined, setLoader);
                    };
                    $scope.scopeModel.onNetOperatorDeclarationServices.push(onNetOperatorDeclarationService);
                };

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedValues;
                    if (payload != undefined)
                        selectedValues = payload.selectedValues;

                    var promises = [];

                    var filterItems;
                    if (selectedValues != undefined && selectedValues.OperatorDeclarationServices != undefined && selectedValues.OperatorDeclarationServices.Services != undefined) {
                        filterItems = [];
                        for (var i = 0; i < selectedValues.OperatorDeclarationServices.Services.length; i++) {
                            var filterItem = {
                                payload: selectedValues.OperatorDeclarationServices.Services[i],
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(filterItem.loadPromiseDeferred.promise);
                            filterItems.push(filterItem);
                        }
                    }

                    promises.push(loadOnNetOperationDeclarationServicesConfigs());

                    function loadOnNetOperationDeclarationServicesConfigs() {
                        return Retail_RA_OnNetOperatorDirectionServicesAPIService.GetMappedCellsExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++)
                                    $scope.scopeModel.extensionConfigs.push(response[i]);
                            }
                            if (filterItems != undefined) {
                                for (var i = 0; i < filterItems.length; i++) {
                                    addFilterItemToGrid(filterItems[i]);
                                }
                            }

                        });
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                function addFilterItemToGrid(filterItem) {
                    var matchItem = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, filterItem.payload.Settings.ConfigId, "ExtensionConfigurationId");
                    if (matchItem == null)
                        return;

                    var dataItem = {
                        Title: matchItem.Title,
                        Editor: matchItem.Editor,
                        Revenue: filterItem.payload.Revenue,
                    };
                    var dataItemPayload = { settings: filterItem.payload.Settings };

                    dataItem.onDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        filterItem.readyPromiseDeferred.resolve();
                    };

                    filterItem.readyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, filterItem.loadPromiseDeferred);
                        });
                    $scope.scopeModel.onNetOperatorDeclarationServices.push(dataItem);
                }

                api.setData = function (Object) {
                    Object.OperatorDeclarationServices = {
                        $type: "Retail.RA.Entities.OnNetOperatorDeclarationServices,Retail.RA.Entities",
                        Services: {
                            $type: "Retail.RA.Entities.OnNetOperatorDeclarationServicesCollection,Retail.RA.Entities",
                            $values: getServices()
                        }
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            function getServices() {
                var services = [];
                for (var i = 0; i < $scope.scopeModel.onNetOperatorDeclarationServices.length; i++) {
                    var onNetOperatorDeclarationService = $scope.scopeModel.onNetOperatorDeclarationServices[i];
                    services.push({
                        $type: "Retail.RA.Entities.OnNetOperatorDeclarationService,Retail.RA.Entities",
                        Revenue: onNetOperatorDeclarationService.Revenue,
                        Settings: onNetOperatorDeclarationService.directiveAPI.getData()
                    });
                }
                return services;
            }
        }
    }

    app.directive('retailRaOnnetoperatordeclarationservice', OnnetOperatorDirectionService);

})(app);
