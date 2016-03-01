﻿(function (appControllers) {

    "use strict";

    operatorProfileEditorController.$inject = ['$scope', 'InterConnect_BE_OperatorProfileAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','VR_GenericData_DataRecordFieldTypeConfigAPIService','VR_GenericData_GenericEditorAPIService'];

    function operatorProfileEditorController($scope, InterConnect_BE_OperatorProfileAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordFieldTypeConfigAPIService, VR_GenericData_GenericEditorAPIService) {
        $scope.scopeModal = {};
        $scope.scopeModal.isEditMode;
        var operatorProfileId;
        var operatorProfileEntity;
        var businessEntityId;
        var extendedSettings;
        var countryId;
        var cityId;

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var cityDirectiveApi;
        var cityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var extendedSettingsAPI;
        var extendedSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                operatorProfileId = parameters.OperatorProfileId;
            }
            $scope.scopeModal.isEditMode = (operatorProfileId != undefined);

        }

        function defineScope() {

            $scope.scopeModal.dataRecordTypes = [];

            $scope.scopeModal.onSettingSelectionChanged = function () {
                if ($scope.scopeModal.selectedDataRecordTypes != undefined && $scope.scopeModal.dataRecordTypes.length > 1 && !$scope.scopeModal.isLoading)
                {
                    $scope.scopeModal.isLoading = true;
                    loadExtendedSettings($scope.scopeModal.selectedDataRecordTypes.DataRecordTypeId).then(function () {
                        $scope.scopeModal.isLoading = false;
                    });
                }
                   
            }

            $scope.scopeModal.onExtendedSettingsDirectiveReady = function (api)
                {
                extendedSettingsAPI = api;
                extendedSettingsReadyPromiseDeferred.resolve();
            }

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            }

            $scope.onCityDirectiveReady = function (api) {
                cityDirectiveApi = api;
                cityReadyPromiseDeferred.resolve();
            }

            $scope.SaveOperatorProfile = function () {
                if ($scope.scopeModal.isEditMode) {
                    return updateOperatorProfile();
                }
                else {
                    return insertOperatorProfile();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.onCountrySelctionChanged = function (item, datasource) {

                if (item != undefined) {
                    var payload = {};
                    payload.filter = { CountryId: item.CountryId }
                    cityDirectiveApi.load(payload)
                }
                else {
                    $scope.scopeModal.city = undefined;

                }
            }
        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if ($scope.scopeModal.isEditMode) {
                getOperatorProfile().then(function () {
                    loadAllControls()
                        .finally(function () {
                            $scope.scopeModal.isLoading = false;

                           operatorProfileEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                loadAllControls().finally(function () {
                    $scope.scopeModal.isLoading = false;
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadCountries, loadCities, loadStaticSection, loadDataRecordTypes]).catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = $scope.scopeModal.isEditMode ? UtilsService.buildTitleForUpdateEditor(operatorProfileEntity ? operatorProfileEntity.Name : null, 'Operator Profile') : UtilsService.buildTitleForAddEditor('Operator Profile');
        }

        function loadCountries() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: (operatorProfileEntity != undefined) ? operatorProfileEntity.Settings.CountryId : undefined
                };

                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });

            return loadCountryPromiseDeferred.promise;
        }

        function loadCities() {
            var loadCityPromiseDeferred = UtilsService.createPromiseDeferred();
            cityReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: (operatorProfileEntity != undefined) ? operatorProfileEntity.Settings.CityId : undefined
                };
                if (operatorProfileEntity != undefined && operatorProfileEntity.Settings.CountryId != undefined)
                    payload.filter = { CountryId: operatorProfileEntity.Settings.CountryId }
                VRUIUtilsService.callDirectiveLoad(cityDirectiveApi, payload, loadCityPromiseDeferred);
            });

            return loadCityPromiseDeferred.promise;
        }

        function loadStaticSection() {
            if (operatorProfileEntity != undefined) {
                $scope.scopeModal.name = operatorProfileEntity.Name;
                if (operatorProfileEntity.Settings != null) {
                    $scope.scopeModal.company = operatorProfileEntity.Settings.Company;
                }
            }
        }

        function loadDataRecordTypes()
        {
            var promiseDeferred = UtilsService.createPromiseDeferred();
            InterConnect_BE_OperatorProfileAPIService.GetDataRecordTypes().then(function (response) {
               $scope.scopeModal.dataRecordTypes = response;
               if ($scope.scopeModal.dataRecordTypes.length == 1 && !$scope.scopeModal.isEditMode) {
                   loadExtendedSettings($scope.scopeModal.dataRecordTypes[0].DataRecordTypeId).then(function () {
                       promiseDeferred.resolve();
                   }).catch(function (error) {
                       promiseDeferred.reject(error);
                   });
               } else if (operatorProfileEntity != undefined && operatorProfileEntity.ExtendedSettingsRecordTypeId != undefined) {
                   $scope.scopeModal.selectedDataRecordTypes = UtilsService.getItemByVal($scope.scopeModal.dataRecordTypes, operatorProfileEntity.ExtendedSettingsRecordTypeId, "DataRecordTypeId");
                   loadExtendedSettings($scope.scopeModal.selectedDataRecordTypes.DataRecordTypeId).then(function () {
                       promiseDeferred.resolve();
                   }).catch(function(error)
                   {
                       promiseDeferred.reject(error);
                   });
               } else {
                   promiseDeferred.resolve();
               }

            });
            return promiseDeferred.promise;
        }

        function loadExtendedSettings(recordTypeId)
        {
            var promiseDeferred = UtilsService.createPromiseDeferred();

            InterConnect_BE_OperatorProfileAPIService.GetRunTimeExtendedSettings(recordTypeId).then(function (response) {
                $scope.scopeModal.sections = response.Sections;
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
                        sections: $scope.scopeModal.sections,
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
                Name: $scope.scopeModal.name,
                Settings: {
                    CountryId: countryDirectiveApi.getSelectedIds(),
                    CityId: cityDirectiveApi.getSelectedIds(),
                    Company: $scope.scopeModal.company,
                },
                ExtendedSettings: extendedSettingsAPI != undefined ? extendedSettingsAPI.getData() : undefined,
                ExtendedSettingsRecordTypeId: $scope.scopeModal.selectedDataRecordTypes != undefined ? $scope.scopeModal.selectedDataRecordTypes.DataRecordTypeId : undefined
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
