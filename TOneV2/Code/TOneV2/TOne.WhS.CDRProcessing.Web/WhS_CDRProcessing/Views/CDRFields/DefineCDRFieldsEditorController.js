(function (appControllers) {

    "use strict";

    defineCDRFieldsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_CDRProcessing_DefineCDRFieldsAPIService', 'VRValidationService'];

    function defineCDRFieldsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_CDRProcessing_DefineCDRFieldsAPIService, VRValidationService) {

        var isEditMode;
        var cdrFieldID;
        var cdrFieldEntity;

        var directiveReadyAPI;
        var directiveReadyPromiseDeferred;

        loadParameters();
        defineScope();
        load(); 

        function loadParameters() {
            
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                cdrFieldID = parameters.ID
            }
            isEditMode = (cdrFieldID != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};
            $scope.scopeModal.SaveCDRField = function () {
                if (isEditMode) {
                    return updateCDRField();
                }
                else {
                    return insertCDRField();
                }
            };
            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };
         
            $scope.scopeModal.onDirectiveReady = function (api) {
                directiveReadyAPI = api;
                var setLoader = function (value) { $scope.isLoadingDirective = value };
                var payload;
                if (cdrFieldEntity != undefined)
                {
                    payload = cdrFieldEntity.Type;
                }
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveReadyAPI, payload, setLoader, directiveReadyPromiseDeferred);
            }

            $scope.scopeModal.cdrFieldTypeTemplates = [];
        }

        function load() {
            $scope.scopeModal.isLoading = true;
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("CDR Field");
                getCDRField().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
                       
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor("CDR Field");
                loadAllControls();
            }

        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadCDRFieldTypeTemplates])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModal.isLoading = false;
               });
        }
      
        function loadFilterBySection() {
            if(cdrFieldEntity != undefined)
            {
                $scope.scopeModal.name = cdrFieldEntity.FieldName;
            }
        }

        function loadCDRFieldTypeTemplates() {
            return WhS_CDRProcessing_DefineCDRFieldsAPIService.GetCDRFieldTypeTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.scopeModal.cdrFieldTypeTemplates.push(item);
                });

                if (cdrFieldEntity != undefined)
                    $scope.scopeModal.selectedCDRFieldTypeTemplate = UtilsService.getItemByVal($scope.scopeModal.cdrFieldTypeTemplates, cdrFieldEntity.Type.ConfigId, "TemplateConfigID");
            });
        }

        function getCDRField() {
            return WhS_CDRProcessing_DefineCDRFieldsAPIService.GetCDRField(cdrFieldID).then(function (cdrField) {
                cdrFieldEntity = cdrField;
            });
        }

        function buildCDRFieldObjectObjFromScope() {
            var cdrField = {};
            cdrField.FieldName = $scope.scopeModal.name;
            cdrField.Type = directiveReadyAPI.getData();
            cdrField.Type.ConfigId = $scope.scopeModal.selectedCDRFieldTypeTemplate.TemplateConfigID;
            return cdrField;
        }

        function insertCDRField() {

            var cdrFieldObject = buildCDRFieldObjectObjFromScope();
            return WhS_CDRProcessing_DefineCDRFieldsAPIService.AddCDRField(cdrFieldObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("CDR Field", response, "Name")) {
                    if ($scope.onCDRFieldAdded != undefined)
                        $scope.onCDRFieldAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateCDRField() {
            var cdrFieldObject = buildCDRFieldObjectObjFromScope();
            cdrFieldObject.ID = cdrFieldID;
            return WhS_CDRProcessing_DefineCDRFieldsAPIService.UpdateCDRField(cdrFieldObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("CDR Field", response, "Name")) {
                    if ($scope.onCDRFieldUpdated != undefined)
                        $scope.onCDRFieldUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('WhS_CDRProcessing_DefineCDRFieldsEditorController', defineCDRFieldsEditorController);
})(appControllers);
