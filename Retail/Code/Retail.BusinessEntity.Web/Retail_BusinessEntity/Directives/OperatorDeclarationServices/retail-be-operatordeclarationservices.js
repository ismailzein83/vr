

(function (app) {

    'use strict';

    operatorDirectionServices.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService','Retail_BE_OperatorDirectionServicesAPIService'];

    function operatorDirectionServices(UtilsService, VRUIUtilsService, VRNotificationService,Retail_BE_OperatorDirectionServicesAPIService) {
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
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/OperatorDeclarationServices/Templates/OperationDirectionServicesTemplate.html"

        };
        function OperatorDirectionServicesCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var gridAPI;
            var directiveAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.extensionConfigs = [];
                $scope.scopeModel.operatorDeclarationServices = [];

                $scope.removerow = function (dataItem) {
                    var index = $scope.scopeModel.operatorDeclarationServices.indexOf(dataItem);
                    $scope.scopeModel.operatorDeclarationServices.splice(index, 1);
                };

                $scope.scopeModel.onGridReady = function(api){
                    gridAPI=api;
                };

                $scope.scopeModel.addExtensionConfiguration = function () {
                    var operatorDeclarationService = {
                        Title: $scope.scopeModel.selectedExtensionConfig.Title,
                        Editor: $scope.scopeModel.selectedExtensionConfig.Editor,
                    };
                    operatorDeclarationService.onDirectiveReady = function (api) {
                        operatorDeclarationService.directiveAPI = api;
                        var setLoader = function (value) { operatorDeclarationService.isLoadingDirective = value };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, operatorDeclarationService.directiveAPI, undefined, setLoader);
                    };
                    $scope.scopeModel.operatorDeclarationServices.push(operatorDeclarationService);
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
                    if (selectedValues != undefined && selectedValues.OperatorDeclarationServices != undefined && selectedValues.OperatorDeclarationServices.Services != undefined)
                    {
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

                    promises.push(loadOperationDeclarationServicesConfigs());


                    function loadOperationDeclarationServicesConfigs() {
                        return Retail_BE_OperatorDirectionServicesAPIService.GetMappedCellsExtensionConfigs().then(function (response) {
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
                    $scope.scopeModel.operatorDeclarationServices.push(dataItem);
                }


                api.setData = function (Object) {

                    Object.OperatorDeclarationServices = {
                        $type: "Retail.BusinessEntity.Entities.OperatorDeclarationServices,Retail.BusinessEntity.Entities",
                        Services: {
                            $type: "Retail.BusinessEntity.Entities.OperatorDeclarationServicesCollection,Retail.BusinessEntity.Entities",
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

                for (var i = 0; i < $scope.scopeModel.operatorDeclarationServices.length; i++)
                {
                    var operatorDeclarationService = $scope.scopeModel.operatorDeclarationServices[i];
                    services.push({
                        $type: "Retail.BusinessEntity.Entities.OperatorDeclarationService,Retail.BusinessEntity.Entities",
                        Revenue: operatorDeclarationService.Revenue,
                        Settings: operatorDeclarationService.directiveAPI.getData()
                    });
                }
                
                return services;
            }
        }
    }

    app.directive('retailBeOperatordeclarationservices', operatorDirectionServices);

})(app);
