(function (appControllers) {

    'use strict';

    PriceListTemplateEditorController.$inject = ['$scope', 'XBooster_PriceListConversion_PriceListTemplateAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService','VRUIUtilsService'];

    function PriceListTemplateEditorController($scope, XBooster_PriceListConversion_PriceListTemplateAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
        var isEditMode;
        var priceListTemplateId;
        var priceListTemplateEntity;

        var outPutWorkBookAPI;
        var outputConfigurationSelectiveAPI;
        var outputPriceListConfigurationReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                priceListTemplateId = parameters.PriceListTemplateId;

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

            $scope.scopeModel.outPutFieldMappings = [{ fieldTitle: "First Row", isRequired: true, type: "row", fieldName: "FirstRow" }, { fieldTitle: "Code", isRequired: true, type: "cell", fieldName: "Code" }, { fieldTitle: "Zone", isRequired: true, type: "cell", fieldName: "Zone" }, { fieldTitle: "Rate", isRequired: true, type: "cell", fieldName: "Rate" }, { fieldTitle: "Effective Date", isRequired: true, type: "cell", fieldName: "EffectiveDate" }]

            $scope.scopeModel.onOutputConfigurationSelectiveReady = function (api) {
                outputConfigurationSelectiveAPI = api;
                outputPriceListConfigurationReadyPromiseDeferred.resolve();
            };
        }
        //$scope.hasSaveUserPermission = function () {
        //    if (isEditMode) {
        //        return VR_Sec_UserAPIService.HasUpdateUserPermission();
        //    }
        //    else {
        //        return VR_Sec_UserAPIService.HasAddUserPermission();
        //    }
        //};

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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadOutputPriceListConfigurationSelective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function loadOutputPriceListConfigurationSelective() {
            var loadOutputPriceListConfigurationPromiseDeferred = UtilsService.createPromiseDeferred();
            outputPriceListConfigurationReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    configDetails: priceListTemplateEntity != undefined ? priceListTemplateEntity.ConfigDetails : undefined,
                    fieldMappings:$scope.scopeModel.outPutFieldMappings 
                };
                VRUIUtilsService.callDirectiveLoad(outputConfigurationSelectiveAPI, payload, loadOutputPriceListConfigurationPromiseDeferred);
            });

            return loadOutputPriceListConfigurationPromiseDeferred.promise;
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
            var outPutExcel;
            if(outputConfigurationSelectiveAPI !=undefined)
            {
                outPutExcel = outputConfigurationSelectiveAPI.getData();
            }
            var priceListTemplateObject = {
                PriceListTemplateId:priceListTemplateId,
                Name: $scope.scopeModel.name,
                ConfigDetails: outPutExcel
            };
            return priceListTemplateObject;
        }

        function insertPriceListTemplate() {
            $scope.scopeModel.isLoading = true;

            var priceListTemplateObject = buildPriceListTemplateObjFromScope();

            return XBooster_PriceListConversion_PriceListTemplateAPIService.AddInputPriceListTemplate(priceListTemplateObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('PriceListTemplate', response, 'Name')) {
                    if ($scope.onPriceListTemplateAdded != undefined)
                        $scope.onPriceListTemplateAdded(response.InsertedObject);
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
                    if ($scope.onPriceListTemplateUpdated != undefined)
                        $scope.onPriceListTemplateUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    appControllers.controller('XBooster_PriceListConversion_PriceListTemplateEditorController', PriceListTemplateEditorController);

})(appControllers);
