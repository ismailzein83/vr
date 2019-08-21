(function (appControllers) {
    'use strict';

    manufactoryEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'Demo_Module_ManufactoryAPIService'];

    function manufactoryEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, Demo_Module_ManufactoryAPIService) {
        var isEditMode;
        var isViewMode;
        var manufactoryId;
        var manufactory;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            manufactoryId = parameters.manufactoryId;

            isViewMode = (parameters.viewMode == true);
            if (isViewMode) {
                UtilsService.setContextReadOnly($scope);
            }

            isEditMode = (manufactoryId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onSaveClicked = function () {
                if (isEditMode) {
                    return updateManufactory();
                }
                else {
                    return insertManufactory();
                }
            };

            $scope.scopeModel.onCloseClicked = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getManufactory().then(function () {
                    loadAllControls().finally(function () {
                        manufactory = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            } else {
                loadAllControls();
            }
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode) {
                    $scope.title = UtilsService.buildTitleForUpdateEditor(manufactory.Name, 'Manufactory');
                } else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Manufactory');
                }
            }

            function loadData() {
                if (manufactory != undefined && manufactory != null) {
                    $scope.scopeModel.name = manufactory.Name;
                    $scope.scopeModel.countryOfOrigin = manufactory.CountryOfOrigin;
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function getManufactory() {
            return Demo_Module_ManufactoryAPIService.GetManufactoryById(manufactoryId).then(function (response) {
                manufactory = response;
            });
        }

        function updateManufactory() {
            $scope.scopeModel.isLoading = true;

            var updatedManufactory = buildManufactory();
            return Demo_Module_ManufactoryAPIService.UpdateManufactory(updatedManufactory).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Manufactory', response, 'Name')) {
                    if ($scope.onManufactoryUpdated != undefined) {
                        $scope.onManufactoryUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insertManufactory() {
            $scope.scopeModel.isLoading = true;

            var insertedManufactory = buildManufactory();
            return Demo_Module_ManufactoryAPIService.AddManufactory(insertedManufactory).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Manufactory', response, 'Name')) {
                    if ($scope.onManufactoryAdded != undefined) {
                        $scope.onManufactoryAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildManufactory() {
            return {
                Id: manufactoryId,
                Name: $scope.scopeModel.name,
                CountryOfOrigin: $scope.scopeModel.countryOfOrigin
            };
        }
    }

    appControllers.controller('Demo_Module_ManufactoryEditorController', manufactoryEditorController);
})(appControllers);