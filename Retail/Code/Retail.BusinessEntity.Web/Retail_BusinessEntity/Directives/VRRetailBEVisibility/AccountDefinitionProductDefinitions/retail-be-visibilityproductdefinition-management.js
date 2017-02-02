'use strict';

app.directive('retailBeVisibilityproductdefinitionManagement', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityProductDefinition(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionProductDefinitions/Templates/VisibilityProductDefinitionManagementTemplate.html';
            }
        };

        function VisibilityProductDefinition(ctrl, $scope) {
            this.initializeController = initializeController;

            var productDefinitionSelectorAPI;
            var productDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.productDefinitionDefinitions = [];
                $scope.scopeModel.productDefinitions = [];

                $scope.scopeModel.onProductDefinitionsSelectorReady = function (api) {
                    productDefinitionSelectorAPI = api;
                    productDefinitionSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSelectProductDefinition = function (selectedItem) {

                    $scope.scopeModel.productDefinitions.push({
                        ProductDefinitionId: selectedItem.ProductDefinitionId,
                        Name: selectedItem.Name
                    });
                };
                $scope.scopeModel.onDeselectProductDefinition = function (deselectedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.productDefinitions, deselectedItem.ProductDefinitionId, 'ProductDefinitionId');
                    $scope.scopeModel.productDefinitions.splice(index, 1);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedProductDefinitions, deletedItem.ProductDefinitionId, 'ProductDefinitionId');
                    $scope.scopeModel.selectedProductDefinitions.splice(index, 1);
                    $scope.scopeModel.onDeselectProductDefinition(deletedItem);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var productDefinitions;
                    var accountBEDefinitionId;

                    if (payload != undefined) {
                        productDefinitions = payload.productDefinitions;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                    }

                    var loadProductDefinitionSelectorPromise = getProductDefinitionSelectorLoadPromise();
                    promises.push(loadProductDefinitionSelectorPromise);

                    loadProductDefinitionSelectorPromise.then(function () {

                        //Loading Grid
                        if ($scope.scopeModel.selectedProductDefinitions != undefined) {
                            for (var i = 0; i < $scope.scopeModel.selectedProductDefinitions.length; i++) {
                                var productDefinitionDefinition = $scope.scopeModel.selectedProductDefinitions[i];

                                $scope.scopeModel.productDefinitions.push({
                                    ProductDefinitionId: productDefinitionDefinition.ProductDefinitionId,
                                    Name: productDefinitionDefinition.Name
                                });
                            }
                        }
                    });

                    function getProductDefinitionSelectorLoadPromise() {
                        var productDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        productDefinitionSelectorPromiseDeferred.promise.then(function () {

                            var selectorPayload = {
                                includeHiddenProductDefinitions: true,
                                accountBEDefinitionId: accountBEDefinitionId,
                                selectedIds: []
                            };
                            if (productDefinitions != undefined) {
                                for (var index = 0; index < productDefinitions.length; index++) {
                                    selectorPayload.selectedIds.push(productDefinitions[index].ProductDefinitionId);
                                }
                            }

                            VRUIUtilsService.callDirectiveLoad(productDefinitionSelectorAPI, selectorPayload, productDefinitionSelectorLoadDeferred);
                        });

                        return productDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var _productDefinitions;
                    if ($scope.scopeModel.productDefinitions.length > 0) {
                        _productDefinitions = [];
                        for (var i = 0; i < $scope.scopeModel.productDefinitions.length; i++) {
                            var currentProductDefinition = $scope.scopeModel.productDefinitions[i];
                            _productDefinitions.push({
                                ProductDefinitionId: currentProductDefinition.ProductDefinitionId
                            });
                        }
                    }
                    return _productDefinitions
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);



