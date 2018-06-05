(function (appControllers) {

    "use strict";

    itemEditorController.$inject = ['$scope', 'Demo_Module_ItemAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function itemEditorController($scope, Demo_Module_ItemAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var itemId;
        var itemEntity;
        var context;

        var ProductDirectiveApi;
        var ProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        //var infoDirectiveAPI;
        //var infoReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        //var infoEntity;

        //var descriptionDirectiveAPI;
        //var descriptionReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        //var descriptionEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                itemId = parameters.itemId;
                context = parameters.context;
            }
            isEditMode = (itemId != undefined);

        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.saveItem = function () {
                if (isEditMode)
                    return updateItem();
                else
                    return insertItem();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.onProductDirectiveReady = function (api) {
                ProductDirectiveApi = api;
                ProductReadyPromiseDeferred.resolve();
            };

            //$scope.scopeModel.onItemInfoReady = function (api) {
            //    infoDirectiveAPI = api;
            //    infoReadyPromiseDeferred.resolve();
            //};

            //$scope.onDescriptionReady = function (api) {
            //    descriptionDirectiveAPI = api;
            //    descriptionReadyPromiseDeferred.resolve();
            //};
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getItem().then(function () {
                    loadAllControls()
                      .finally(function () {
                          itemEntity = undefined;
                      });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getItem() {
            return Demo_Module_ItemAPIService.GetItemById(itemId).then(function (itemObject) {
                itemEntity = itemObject;
             
            });
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadProductSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && itemEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(itemEntity.Name, "Item");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Item");
        }

        function loadStaticData() {
            if (itemEntity != undefined) {
                $scope.scopeModel.name = itemEntity.Name;
            }
        }

        function loadProductSelector() {
            var ProductLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            ProductReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: itemEntity != undefined ? itemEntity.ProductId : undefined,
                };

                VRUIUtilsService.callDirectiveLoad(ProductDirectiveApi, directivePayload, ProductLoadPromiseDeferred);

            });
            return ProductLoadPromiseDeferred.promise;
        }

        //function loadInfoDirective() {
        //    var infoDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        //    infoReadyPromiseDeferred.promise.then(function () {
        //        var infoDirectivePayload;
        //        if (infoEntity != undefined) {
        //            infoDirectivePayload = infoEntity;
        //        }

        //        VRUIUtilsService.callDirectiveLoad(infoDirectiveAPI, infoDirectivePayload, infoDeferredLoadPromiseDeferred);

        //    });
        //    return infoDeferredLoadPromiseDeferred.promise;
        //}

        //function loadDescriptionDirective() {
        //    var descriptionDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        //    descriptionReadyPromiseDeferred.promise.then(function () {
        //        var descriptionDirectivePayload;
        //        if (descriptionEntity != undefined) {
        //            descriptionDirectivePayload = descriptionEntity;
        //        }
        //        VRUIUtilsService.callDirectiveLoad(descriptionDirectiveAPI, descriptionDirectivePayload, descriptionDeferredLoadPromiseDeferred);

        //    });
        //    return descriptionDeferredLoadPromiseDeferred.promise;
        //}

        function buildItemObjFromScope() {
            return {
                ItemId: (itemId != null) ? itemId : 0,
                Name: $scope.scopeModel.name,
                ProductId: $scope.scopeModel.selector.ProductId,
                ProductName: $scope.scopeModel.selector.Name
            };
        }

        function insertItem() {
            $scope.scopeModel.isLoading = true;

            var itemObject = buildItemObjFromScope();
            return Demo_Module_ItemAPIService.AddItem(itemObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Item", response, "Name")) {
                    if ($scope.onItemAdded != undefined) {
                        $scope.onItemAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateItem() {
            $scope.scopeModel.isLoading = true;

            var itemObject = buildItemObjFromScope();
            Demo_Module_ItemAPIService.UpdateItem(itemObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Item", response, "Name")) {
                    if ($scope.onItemUpdated != undefined)
                        $scope.onItemUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }
    appControllers.controller('Demo_Module_ItemEditorController', itemEditorController);
})(appControllers);
