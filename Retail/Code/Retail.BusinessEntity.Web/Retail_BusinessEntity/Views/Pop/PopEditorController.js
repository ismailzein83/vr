(function (appControllers) {

    "use strict";

    popEditorController.$inject = ['$scope', 'Retail_BE_PopAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function popEditorController($scope, Retail_BE_PopAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {
       

        var isEditMode;
        var popId;
        var popEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                popId = parameters.PopId;
            }
            isEditMode = (popId != undefined);
        }

        function defineScope() {
            $scope.SavePop = function () {
                if (isEditMode) {
                    return updatePop();
                } else {
                    return insertPop();
                }
            };
           
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.isLoading = true;

            if (isEditMode) {
                getPop()
                    .then(function () {
                        loadAllControls()
                            .finally(function () {
                                popEntity = undefined;
                            });
                    })
                    .catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.isLoading = false;
                    });
            } else {
                loadAllControls();
            }

        }
        function getPop() {
            return Retail_BE_PopAPIService.GetPop(popId).then(function (pop) {
                popEntity = pop;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(popEntity ? popEntity.Name : null, 'Probing Pop') : UtilsService.buildTitleForAddEditor('Probing Pop');
        }

       
        function loadStaticData() {

            if (popEntity == undefined)
                return;

            $scope.name = popEntity.Name;

            $scope.description = popEntity.Description;

            $scope.quantity = popEntity.Quantity;

            $scope.location = popEntity.Location;
        }


        function buildPopObjFromScope() {
            var obj = {
                PopId:(popId!=undefined)?popId:0,
                Name: $scope.name,
                Description:  $scope.description,
                Quantity: $scope.quantity,
                Location:  $scope.location
            };
            return obj;
        }

        


        function insertPop() {
            $scope.isLoading = true;

            var popObject = buildPopObjFromScope();
            return Retail_BE_PopAPIService.AddPop(popObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Probing Pop", response, "Name")) {
                        if ($scope.onPopAdded != undefined)
                            $scope.onPopAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });

        }

        function updatePop() {
            $scope.isLoading = true;

            var popObject = buildPopObjFromScope();
            Retail_BE_PopAPIService.UpdatePop(popObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Probing Pop", response, "Name")) {
                        if ($scope.onPopUpdated != undefined)
                            $scope.onPopUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });
        }
    }

    appControllers.controller('Retail_BE_PopEditorController', popEditorController);
})(appControllers);