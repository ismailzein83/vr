(function (appControllers) {

    "use strict";
    itemEditorController.$inject = ['$scope', 'Demo_Module_ItemAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function itemEditorController($scope, Demo_Module_ItemAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var itemId;
        var itemEntity;

        var productIdItem;

        var productDirectiveApi;
        var productReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var itemShapeDirectiveApi;
        var itemShapeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                itemId = parameters.itemId;
                productIdItem = parameters.productIdItem;

            }
            isEditMode = (itemId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.disableProduct = productIdItem != undefined;
            $scope.scopeModel.onProductDirectiveReady = function (api) {
                productDirectiveApi = api;
                productReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onItemShapeDirectiveReady = function (api) {
                itemShapeDirectiveApi = api;
                itemShapeReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveItem = function () {
                if (isEditMode)
                    return updateItem();
                else
                    return insertItem();

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getItem().then(function () {
                    loadAllControls().finally(function () {
                        itemEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        };

        function getItem() {
            return Demo_Module_ItemAPIService.GetItemById(itemId).then(function (response) {
                itemEntity = response;
            });
        };

        function loadAllControls() {

            function loadItemShapeDirective() {
                var itemShapeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                itemShapeReadyPromiseDeferred.promise.then(function () {
                    var itemShapePayload;
                    if (itemEntity != undefined && itemEntity.Settings != undefined)
                        itemShapePayload = {
                            itemShapeEntity: itemEntity.Settings.ItemShape
                        };
                    VRUIUtilsService.callDirectiveLoad(itemShapeDirectiveApi, itemShapePayload, itemShapeLoadPromiseDeferred);
                });
                return itemShapeLoadPromiseDeferred.promise;
            }

            function loadProductSelector() {
                var productLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                productReadyPromiseDeferred.promise.then(function () {
                    var productPayload = {};
                    if (productIdItem != undefined)
                        productPayload.selectedIds = productIdItem.ProductId;

                    if (itemEntity != undefined)
                        productPayload.selectedIds = itemEntity.ProductId;

                    VRUIUtilsService.callDirectiveLoad(productDirectiveApi, productPayload, productLoadPromiseDeferred);
                });
                return productLoadPromiseDeferred.promise;

            }

            function setTitle() {
                if (isEditMode && itemEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(itemEntity.Name, "Item");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Item");
            };

            function loadStaticData() {
                if (itemEntity != undefined)
                    $scope.scopeModel.name = itemEntity.Name;
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadProductSelector, loadItemShapeDirective])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function buildItemObjectFromScope() {
            var object = {
                ItemId: (itemId != undefined) ? itemId : undefined,
                Name: $scope.scopeModel.name,
                ProductId: productDirectiveApi.getSelectedIds(),
                Settings: {
                    ItemShape: itemShapeDirectiveApi.getData()
                }
            };
            return object;
        };

        function insertItem() {

            $scope.scopeModel.isLoading = true;
            var itemObject = buildItemObjectFromScope();
            return Demo_Module_ItemAPIService.AddItem(itemObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Item", response, "Name")) {
                    if ($scope.onItemAdded != undefined) {
                        $scope.onItemAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        };

        function updateItem() {
            $scope.scopeModel.isLoading = true;
            var itemObject = buildItemObjectFromScope();
            Demo_Module_ItemAPIService.UpdateItem(itemObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Item", response, "Name")) {
                    if ($scope.onItemUpdated != undefined) {
                        $scope.onItemUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;

            });
        };

    };
    appControllers.controller('Demo_Module_ItemEditorController', itemEditorController);
})(appControllers);