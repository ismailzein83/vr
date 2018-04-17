(function (appControllers) {

    "use strict";

    paserTypeEditorController.$inject = ['$scope', 'VR_DataParser_ParserTypeAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function paserTypeEditorController($scope, VR_DataParser_ParserTypeAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
        var parserTypeId;
        var isEditMode;
        var parserTypeEntity;
        var parserTypeExtendedSettingsSelectorAPI;
        var parserTypeExtendedSettingsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {

            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                parserTypeId = parameters.parserTypeId;
            }
            isEditMode = (parserTypeId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.saveParserType = function () {
                if (isEditMode)
                    return updateParserType();
                else
                    return insertParserType();
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onParserTypeExtendedSettingsSelectorReady = function (api) {
                parserTypeExtendedSettingsSelectorAPI = api;
                parserTypeExtendedSettingsSelectorReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.isLoading = true;
            if (isEditMode) {
                getParserType().then(function () {
                    loadAllControls()
                        .finally(function () {
                            parserTypeEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else loadAllControls();
        }

        function getParserType() {
            return VR_DataParser_ParserTypeAPIService.GetParserType(parserTypeId).then(function (parserType) {
                parserTypeEntity = parserType;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadParserTypeExtendedSettingsSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && parserTypeEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(parserTypeEntity.Name, "ParserType");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("ParserType");
        }

        function loadStaticData() {
            if (parserTypeEntity == undefined)
                return;
            $scope.scopeModel.name = parserTypeEntity.Name;
            $scope.scopeModel.useRecordType = parserTypeEntity.Settings.UseRecordType;
        }

        function loadParserTypeExtendedSettingsSelector() {
            var parserTypeExtendedSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            parserTypeExtendedSettingsSelectorReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = { context: getContext() };
                    if (parserTypeEntity != undefined && parserTypeEntity.Settings != undefined) {
                        directivePayload.parserTypeEntity = parserTypeEntity.Settings;
                    }
                    VRUIUtilsService.callDirectiveLoad(parserTypeExtendedSettingsSelectorAPI, directivePayload, parserTypeExtendedSettingsLoadPromiseDeferred);
                });
            return parserTypeExtendedSettingsLoadPromiseDeferred.promise;
        }

        function buildParserTypeObjFromScope() {
            var obj = {
                ParserTypeId: parserTypeId,
                Name: $scope.scopeModel.name,
                Settings: {
                    UseRecordType: $scope.scopeModel.useRecordType,
                    ExtendedSettings: parserTypeExtendedSettingsSelectorAPI.getData()
                }
            };
            return obj;
        }

        function insertParserType() {
            $scope.isLoading = true;
            var parserTypeObject = buildParserTypeObjFromScope();
            return VR_DataParser_ParserTypeAPIService.AddParserType(parserTypeObject)
               .then(function (response) {
                   if (VRNotificationService.notifyOnItemAdded("ParserType", response, "Name")) {
                       if ($scope.onParserTypeAdded != undefined)
                           $scope.onParserTypeAdded(response.InsertedObject);
                       $scope.modalContext.closeModal();
                   }
               }).catch(function (error) {
                   VRNotificationService.notifyException(error, $scope);
               }).finally(function () {
                   $scope.isLoading = false;
               });

        }

        function updateParserType() {
            $scope.isLoading = true;
            var parserTypeObject = buildParserTypeObjFromScope();
            VR_DataParser_ParserTypeAPIService.UpdateParserType(parserTypeObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("ParserType", response, "Name")) {
                    if ($scope.onParserTypeUpdated != undefined)
                        $scope.onParserTypeUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function getContext() {
            var context = {
                useRecordType: function () {
                    return $scope.scopeModel.useRecordType;

                }
            };
            return context;
        }
    }

    appControllers.controller('VR_DataParser_ParserTypeEditorController', paserTypeEditorController);
})(appControllers);
