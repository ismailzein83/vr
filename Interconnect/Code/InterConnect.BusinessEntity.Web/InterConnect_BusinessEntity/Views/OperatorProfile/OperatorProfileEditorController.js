﻿(function (appControllers) {

    "use strict";

    operatorProfileEditorController.$inject = ['$scope', 'InterConnect_BE_OperatorProfileAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldTypeConfigAPIService', 'VR_GenericData_GenericUIRuntimeAPIService'];

    function operatorProfileEditorController($scope, InterConnect_BE_OperatorProfileAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordFieldTypeConfigAPIService, VR_GenericData_GenericUIRuntimeAPIService) {

        $scope.scopeModel = {};
        $scope.scopeModel.isEditMode;

        var operatorProfileId;
        var operatorProfileEntity;
        var businessEntityId;
        var extendedSettings;
        var countryId;
        var cityId;

        var singleValue;

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var cityDirectiveApi;
        var cityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var extendedSettingsAPI;
        var extendedSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeAPI;
        var dataRecordTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var isCountrySelectedProgramatically;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                operatorProfileId = parameters.OperatorProfileId;
            }

            $scope.scopeModel.isEditMode = (operatorProfileId != undefined);
        }

        function defineScope() {

            $scope.scopeModel.dataRecordTypes = [];
            $scope.scopeModel.showRecordTypeSelector = false;

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };

            $scope.onCountrySelctionChanged = function (item) {
                if (!isCountrySelectedProgramatically) {
                    if (item != undefined) {
                        cityDirectiveApi.load({ countryId: item.CountryId });
                    }
                    else {
                        $scope.scopeModel.selectedCity = undefined;
                    }
                }
            };

            $scope.onCityDirectiveReady = function (api) {
                cityDirectiveApi = api;
                cityReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeAPI = api;
                dataRecordTypeReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSettingSelectionChanged = function (item) {
                $scope.scopeModel.isLoading = true;
                loadExtendedSettings(item.DataRecordTypeId, businessEntityId).then(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.onExtendedSettingsDirectiveReady = function (api) {
                extendedSettingsAPI = api;
                extendedSettingsReadyPromiseDeferred.resolve();
            };

            $scope.SaveOperatorProfile = function () {
                if ($scope.scopeModel.isEditMode) {
                    return updateOperatorProfile();
                }
                else {
                    return insertOperatorProfile();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if ($scope.scopeModel.isEditMode) {
                getOperatorProfile().then(function () {
                    loadAllControls()
                        .finally(function () {
                            $scope.scopeModel.isLoading = false;
                           operatorProfileEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls().finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
        }

        function getOperatorProfile() {
            return InterConnect_BE_OperatorProfileAPIService.GetOperatorProfile(operatorProfileId).then(function (operatorProfile) {
                operatorProfileEntity = operatorProfile;
                extendedSettings = operatorProfile !=undefined? operatorProfile.ExtendedSettings:undefined
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadCountrySelector, loadCitySelector, loadStaticSection, loadBusinessEntityId]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                isCountrySelectedProgramatically = false;
            });
        }

        function setTitle() {
            $scope.title = $scope.scopeModel.isEditMode ? UtilsService.buildTitleForUpdateEditor(operatorProfileEntity ? operatorProfileEntity.Name : null, 'Operator Profile') : UtilsService.buildTitleForAddEditor('Operator Profile');
        }

        function loadCountrySelector() {
            var countrySelectorLoadDeferred = UtilsService.createPromiseDeferred();

            countryReadyPromiseDeferred.promise.then(function () {
                var payload;
                if (operatorProfileEntity != undefined)  {
                    var payload = {
                        selectedIds: operatorProfileEntity.Settings.CountryId
                    };
                    isCountrySelectedProgramatically = true;
                }
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, countrySelectorLoadDeferred);
            });

            return countrySelectorLoadDeferred.promise;
        }

        function loadCitySelector() {
            var citySelectorLoadDeferred = UtilsService.createPromiseDeferred();

            cityReadyPromiseDeferred.promise.then(function () {
                var payload;
                if (operatorProfileEntity != undefined) {
                    payload = {
                        countryId: operatorProfileEntity.Settings.CountryId,
                        selectedIds: operatorProfileEntity.Settings.CityId
                    };
                    VRUIUtilsService.callDirectiveLoad(cityDirectiveApi, payload, citySelectorLoadDeferred);
                }
                else {
                    citySelectorLoadDeferred.resolve();
                }
            });

            return citySelectorLoadDeferred.promise;
        }

        function loadStaticSection() {
            if (operatorProfileEntity != undefined) {
                $scope.scopeModel.name = operatorProfileEntity.Name;
                if (operatorProfileEntity.Settings != null) {
                    $scope.scopeModel.company = operatorProfileEntity.Settings.Company;
                }
            }
        }

        function loadDataRecordTypeSelector()
        {
            var loadDataRecordTypePromiseDeferred = UtilsService.createPromiseDeferred();
            dataRecordTypeReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    businessEntityId:businessEntityId,
                    selectedIds: (operatorProfileEntity != undefined) ? operatorProfileEntity.ExtendedSettingsRecordTypeId : undefined
                };

                VRUIUtilsService.callDirectiveLoad(dataRecordTypeAPI, payload, loadDataRecordTypePromiseDeferred);
            });

            return loadDataRecordTypePromiseDeferred.promise;
        }

        function loadBusinessEntityId()
        {
            var promiseDeferred = UtilsService.createPromiseDeferred();
            return InterConnect_BE_OperatorProfileAPIService.GetBusinessEntityDefinitionId().then(function (response)
            {
                businessEntityId = response;
                loadDataRecordTypeSelector().then(function () {
                    singleValue = dataRecordTypeAPI.getIfSingleItem();
                    $scope.scopeModel.showRecordTypeSelector = (singleValue == undefined);

                    if ($scope.scopeModel.isEditMode) {
                        loadExtendedSettings(dataRecordTypeAPI.getSelectedIds(), businessEntityId).then(function () {
                            promiseDeferred.resolve();
                        }).catch(function (error) {
                            promiseDeferred.reject(error);
                        });
                    }else if(singleValue !=undefined)
                    {
                        loadExtendedSettings(singleValue.DataRecordTypeId, businessEntityId).then(function () {
                            promiseDeferred.resolve();
                        }).catch(function (error) {
                            promiseDeferred.reject(error);
                        });
                    }
                    else {
                        promiseDeferred.resolve();
                    }
                });
            });
            return promiseDeferred.promise;
        }

        function loadExtendedSettings(recordTypeId, businessEntityId)
        {
            var promiseDeferred = UtilsService.createPromiseDeferred();

            VR_GenericData_GenericUIRuntimeAPIService.GetExtensibleBEItemRuntime(recordTypeId, businessEntityId).then(function (response) {
                $scope.scopeModel.sections = response.Sections;
                loadExtendedSettingsDirective().then(function () {
                    promiseDeferred.resolve();
                }).catch(function(error)
                {
                    promiseDeferred.reject(error);
                });
            });

            function loadExtendedSettingsDirective() {
                var loadExtendedSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                extendedSettingsReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        sections: $scope.scopeModel.sections,
                        selectedValues: extendedSettings

                    };
                    extendedSettings = undefined;
                    VRUIUtilsService.callDirectiveLoad(extendedSettingsAPI, payload, loadExtendedSettingsPromiseDeferred);
                });

                return loadExtendedSettingsPromiseDeferred.promise;
            }

            return promiseDeferred.promise;
        }

        function buildOperatorProfileObjFromScope() {

            var obj = {
                OperatorProfileId: (operatorProfileId != null) ? operatorProfileId : 0,
                Name: $scope.scopeModel.name,
                Settings: {
                    CountryId: countryDirectiveApi.getSelectedIds(),
                    CityId: cityDirectiveApi.getSelectedIds(),
                    Company: $scope.scopeModel.company,
                },
                ExtendedSettings: extendedSettingsAPI != undefined ? extendedSettingsAPI.getData() : undefined,
                ExtendedSettingsRecordTypeId: singleValue != undefined ? singleValue.DataRecordTypeId : dataRecordTypeAPI != undefined ? dataRecordTypeAPI.getSelectedIds() : undefined
            };
            return obj;
        }

        function insertOperatorProfile() {
            $scope.isLoading = true;

            var operatorProfileObject = buildOperatorProfileObjFromScope();

            return InterConnect_BE_OperatorProfileAPIService.AddOperatorProfile(operatorProfileObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Operator Profile", response, "Name")) {
                    if ($scope.onOperatorProfileAdded != undefined)
                        $scope.onOperatorProfileAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateOperatorProfile() {
            $scope.isLoading = true;

            var operatorProfileObject = buildOperatorProfileObjFromScope();

            InterConnect_BE_OperatorProfileAPIService.UpdateOperatorProfile(operatorProfileObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Operator Profile", response, "Name")) {
                    if ($scope.onOperatorProfileUpdated != undefined)
                        $scope.onOperatorProfileUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('InterConnect_BE_OperatorProfileEditorController', operatorProfileEditorController);

})(appControllers);
