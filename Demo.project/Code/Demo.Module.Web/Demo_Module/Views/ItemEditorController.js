(function (appControllers) {

    "use strict";

    itemEditorController.$inject = ['$scope', 'Demo_Module_ItemAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function collegeEditorController($scope, Demo_Module_ItemAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var itemId;
        var itemEntity;
        var context;

        var productDirectiveApi;
        var productReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var infoDirectiveAPI;
        var infoReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        //var infoEntity;

        var descriptionDirectiveAPI;
        var descriptionReadyPromiseDeferred = UtilsService.createPromiseDeferred();
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

            $scope.saveItem = function () {
                if (isEditMode)
                    return updateItem();
                else
                    return insertItem();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.onProductDirectiveReady = function (api) {
                productDirectiveApi = api;
                productReadyPromiseDeferred.resolve();
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
            $scope.isLoading = true;
            if (isEditMode) {
                getItem().then(function () {
                    loadAllControls()
                      .finally(function () {
                          itemEntity = undefined;
                      });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getItem() {
            return Demo_Module_ItemAPIService.GetItemById(itemId).then(function (itemObject) {
                itemEntity = itemEntity;
                //infoEntity = itemEntityEntity.ItemInfo;
                //descriptionEntity = itemEntity.DescriptionString;
            });
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadProductySelector])//, loadInfoDirective, loadDescriptionDirective
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
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

        function loadProductySelector() {
            var productLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            productReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: itemEntity != undefined ? itemEntity.ProductId : undefined,
                };

                VRUIUtilsService.callDirectiveLoad(productDirectiveApi, directivePayload, productLoadPromiseDeferred);

            });
            return productLoadPromiseDeferred.promise;
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
                ItemId: (collegeId != null) ? collegeId : 0,
                Name: $scope.scopeModel.name,
                ProductId: $scope.scopeModel.selector.UniversityId,
                ProductName: $scope.scopeModel.selector.Name,
                ItemInfo: infoDirectiveAPI.getData(),
                DescriptionString: descriptionDirectiveAPI.getData()
            };
        }

        function insertCollege() {
            $scope.isLoading = true;

            var collegeObject = buildCollegeObjFromScope();
            return Demo_Module_CollegeAPIService.AddCollege(collegeObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("College", response, "Name")) {
                    if ($scope.onCollegeAdded != undefined) {
                        $scope.onCollegeAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function updateCollege() {
            $scope.isLoading = true;

            var collegeObject = buildCollegeObjFromScope();
            Demo_Module_CollegeAPIService.UpdateCollege(collegeObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("College", response, "Name")) {
                    if ($scope.onCollegeUpdated != undefined)
                        $scope.onCollegeUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }
    appControllers.controller('Demo_Module_CollegeEditorController', collegeEditorController);
})(appControllers);
