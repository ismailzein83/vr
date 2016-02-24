(function (appControllers) {

    "use strict";

    operatorProfileEditorController.$inject = ['$scope', 'InterConnect_BE_OperatorProfileAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','VR_GenericData_DataRecordFieldTypeConfigAPIService','VR_GenericData_GenericEditorAPIService'];

    function operatorProfileEditorController($scope, InterConnect_BE_OperatorProfileAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordFieldTypeConfigAPIService, VR_GenericData_GenericEditorAPIService) {
        var isEditMode;
        var operatorProfileId;
        var operatorProfileEntity;
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
            isEditMode = (operatorProfileId != undefined);

        }

        function defineScope() {

            $scope.scopeModal = {};
            $scope.scopeModal.optionSettings =[{
                value: 1,
                Name:"Option1"

            }, {
                value: 2,
                Name: "Option2"

            }, ];

            $scope.scopeModal.onSettingSelectionChanged = function () {
                if ($scope.scopeModal.selectedOptionSettings != undefined)
                loadExtendedSettings($scope.scopeModal.selectedOptionSettings.value).then(function() {
                                    loadExtendedSettingsDirective();

                });
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
                if (isEditMode) {
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
            $scope.isLoading = true;

            if (isEditMode) {
                getOperatorProfile().then(function () {
                    loadAllControls()
                        .finally(function () {
                            operatorProfileEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getOperatorProfile() {
            return InterConnect_BE_OperatorProfileAPIService.GetOperatorProfile(operatorProfileId).then(function (operatorProfile) {
                operatorProfileEntity = operatorProfile;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadCountries, loadCities, loadStaticSection]).then(function () {

            })
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(operatorProfileEntity ? operatorProfileEntity.Name : null, 'Operator Profile') : UtilsService.buildTitleForAddEditor('Operator Profile');
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

        function loadExtendedSettings(id)
        {
            return VR_GenericData_GenericEditorAPIService.GetEditorRuntimeMock(id).then(function (response) {
                $scope.scopeModal.sections = response.Sections;
            });

        }

        function loadExtendedSettingsDirective() {
            var loadExtendedSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
            extendedSettingsReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    sections: $scope.scopeModal.sections
                };

                VRUIUtilsService.callDirectiveLoad(extendedSettingsAPI, payload, loadExtendedSettingsPromiseDeferred);
            });

            return loadExtendedSettingsPromiseDeferred.promise;
        }


        function buildOperatorProfileObjFromScope() {

            var obj = {
                OperatorProfileId: (operatorProfileId != null) ? operatorProfileId : 0,
                Name: $scope.scopeModal.name,
                Settings: {
                    CountryId: countryDirectiveApi.getSelectedIds(),
                    CityId: cityDirectiveApi.getSelectedIds(),
                    Company: $scope.scopeModal.company,
                }
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
