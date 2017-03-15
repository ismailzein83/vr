(function (appControllers) {

    'use strict';

    InputPriceListTemplateEditorController.$inject = ['$scope', 'XBooster_PriceListConversion_PriceListTemplateAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function InputPriceListTemplateEditorController($scope, XBooster_PriceListConversion_PriceListTemplateAPIService, VRNotificationService, VRNavigationService, UtilsService) {
        var isEditMode;
        var priceListTemplateId;
        var priceListTemplateEntity;
        var configDetails;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
            {
                configDetails = parameters.ConfigDetails;
                priceListTemplateId = parameters.PriceListTemplateId;

            }

            isEditMode = (priceListTemplateId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return updatePriceListTemplate();
                else
                    return insertPriceListTemplate();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }
        $scope.hasSavePriceListTemplatePermission = function () {
            if (isEditMode) {
                return XBooster_PriceListConversion_PriceListTemplateAPIService.HasUpdateInputPriceListTemplatePermission();
            }
            else {
                return XBooster_PriceListConversion_PriceListTemplateAPIService.HasaddInputPriceListTemplatePermission();
            }
        };

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getPriceListTemplate().then(function () {
                    loadAllControls().finally(function () {
                        priceListTemplateEntity = undefined;
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

        function getPriceListTemplate() {
            return XBooster_PriceListConversion_PriceListTemplateAPIService.GetPriceListTemplate(priceListTemplateId).then(function (response) {
                priceListTemplateEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && priceListTemplateEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(priceListTemplateEntity.Name, 'PriceList Template');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('PriceList Template');
        }

        function loadStaticData() {

            if (priceListTemplateEntity == undefined)
                return;

            $scope.scopeModel.name = priceListTemplateEntity.Name;
        }

        function buildPriceListTemplateObjFromScope() {
            var priceListTemplateObject = {
                PriceListTemplateId: priceListTemplateId,
                Name: $scope.scopeModel.name,
                ConfigDetails: configDetails
            };
            return priceListTemplateObject;
        }

        function insertPriceListTemplate() {
            $scope.scopeModel.isLoading = true;

            var priceListTemplateObject = buildPriceListTemplateObjFromScope();

            return XBooster_PriceListConversion_PriceListTemplateAPIService.AddInputPriceListTemplate(priceListTemplateObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('PriceListTemplate', response, 'Name')) {
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }

        function updatePriceListTemplate() {
            $scope.scopeModel.isLoading = true;

            var priceListTemplateObject = buildPriceListTemplateObjFromScope();

            return XBooster_PriceListConversion_PriceListTemplateAPIService.UpdateInputPriceListTemplate(priceListTemplateObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('PriceListTemplate', response, 'Name')) {
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    appControllers.controller('XBooster_PriceListConversion_InputPriceListTemplateEditorController', InputPriceListTemplateEditorController);

})(appControllers);
