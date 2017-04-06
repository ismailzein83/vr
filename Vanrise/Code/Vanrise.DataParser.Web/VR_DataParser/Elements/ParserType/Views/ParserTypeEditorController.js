(function (appControllers) {

    "use strict";

    paserTypeEditorController.$inject = ['$scope', 'VR_DataParser_ParserTypeAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function paserTypeEditorController($scope, VR_DataParser_ParserTypeAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        var parserTypeId;
        var editMode;
        var parserTypeEntity;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                parserTypeId = parameters.parserTypeId;
            }
            editMode = (parserTypeId != undefined);

        }

        function defineScope() {

            $scope.saveParserType= function () {
                if (editMode)
                    return updateParserType();
                else
                    return insertParserType();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {

            $scope.isLoading = true;

            if (editMode) {
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

            else {
                loadAllControls();
            }
        }

        function getParserType() {
            return VR_DataParser_ParserTypeAPIService.GetParserType(parserTypeId).then(function (parserType) {
                parserTypeEntity = parserType;
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
            if (editMode && parserTypeEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(parserTypeEntity.Name, "ParserType");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("ParserType");
        }

        function loadStaticData() {

            if (parserTypeEntity == undefined)
                return;

            $scope.name = parserTypeEntity.Name;
        }

        function buildParserTypeObjFromScope() {
            var obj = {
                ParserTypeId: (parserTypeId != null) ? parserTypeId : 0,
                Name: $scope.name,
                Settings:null
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
    }

    appControllers.controller('VR_DataParser_ParserTypeEditorController', paserTypeEditorController);
})(appControllers);
