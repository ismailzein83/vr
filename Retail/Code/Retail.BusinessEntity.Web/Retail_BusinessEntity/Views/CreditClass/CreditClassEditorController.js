(function (appControllers) {

    "use strict";

    CreditClassEditorController.$inject = ['$scope', 'UtilsService', 'Retail_BE_CreditClassAPIService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function CreditClassEditorController($scope, UtilsService, Retail_BE_CreditClassAPIService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var creditClassId;
        var creditClassEntity;

        var currencyAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                creditClassId = parameters.creditClassId;
            }

            isEditMode = (creditClassId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencyAPI = api;
                currencySelectorReadyDeferred.resolve();
            }
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getCreditClass().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }


        function getCreditClass() {
            return Retail_BE_CreditClassAPIService.GetCreditClass(creditClassId).then(function (response) {
                creditClassEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadEntityTypeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode) {
                var creditClassName = (creditClassEntity != undefined) ? creditClassEntity.Name : null;
                $scope.title = UtilsService.buildTitleForUpdateEditor(creditClassName, 'CreditClass');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('CreditClass');
            }
        }
        function loadStaticData() {
            if (creditClassEntity == undefined)
                return;
            $scope.scopeModel.name = creditClassEntity.Name;

            if(creditClassEntity.Settings != undefined)
            $scope.scopeModel.balanceLimit = creditClassEntity.Settings.BalanceLimit;
        }
        function loadEntityTypeSelector() {
            var currencySelectorLoadDeferred = UtilsService.createPromiseDeferred();
            currencySelectorReadyDeferred.promise.then(function () {
                var currencySelectorPayload = null;
                if (isEditMode) {
                    currencySelectorPayload = {
                        selectedIds: creditClassEntity.Settings.CurrencyId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(currencyAPI, currencySelectorPayload, currencySelectorLoadDeferred);
            });
            return currencySelectorLoadDeferred.promise;
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return Retail_BE_CreditClassAPIService.AddCreditClass(buildCreditClassObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('CreditClass', response, 'Name')) {
                    if ($scope.onCreditClassAdded != undefined)
                        $scope.onCreditClassAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return Retail_BE_CreditClassAPIService.UpdateCreditClass(buildCreditClassObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('CreditClass', response, 'Name')) {
                    if ($scope.onCreditClassUpdated != undefined) {
                        $scope.onCreditClassUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildCreditClassObjFromScope() {

            var settings = {
                BalanceLimit: $scope.scopeModel.balanceLimit,
                CurrencyId: currencyAPI.getSelectedIds()
            };

            return {
                CreditClassId: creditClassEntity != undefined ? creditClassEntity.CreditClassId : undefined,
                Name: $scope.scopeModel.name,
                Settings: settings
            };
        }
    }

    appControllers.controller('Retail_BE_CreditClassEditorController', CreditClassEditorController);

})(appControllers);