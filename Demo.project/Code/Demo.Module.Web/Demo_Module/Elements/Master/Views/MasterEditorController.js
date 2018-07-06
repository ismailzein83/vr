(function (appControllers) {

    "use strict";
    masterEditorController.$inject = ['$scope', 'Demo_Module_MasterAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function masterEditorController($scope, Demo_Module_MasterAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var masterId;
        var masterEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                masterId = parameters.masterId;
            }
            isEditMode = (masterId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.saveMaster = function () {
                if (isEditMode)
                    return updateMaster();
                else
                    return insertMaster();

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getMaster().then(function () {
                    loadAllControls().finally(function () {
                        masterEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        };

        function getMaster() {
            return Demo_Module_MasterAPIService.GetMasterById(masterId).then(function (response) {
                masterEntity = response;
            });
        };

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && masterEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(masterEntity.Name, "Master");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Master");
            };

            function loadStaticData() {
                if (masterEntity != undefined)
                    $scope.scopeModel.name = masterEntity.Name;
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function buildMasterObjectFromScope() {
            var object = {
                MasterId: (masterId != undefined) ? masterId : undefined,
                Name: $scope.scopeModel.name,
            };
            return object;
        };

        function insertMaster() {

            $scope.scopeModel.isLoading = true;
            var masterObject = buildMasterObjectFromScope();
            return Demo_Module_MasterAPIService.AddMaster(masterObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Master", response, "Name")) {
                    if ($scope.onMasterAdded != undefined) {
                        $scope.onMasterAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        };

        function updateMaster() {
            $scope.scopeModel.isLoading = true;
            var masterObject = buildMasterObjectFromScope();
            Demo_Module_MasterAPIService.UpdateMaster(masterObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Master", response, "Name")) {
                    if ($scope.onMasterUpdated != undefined) {
                        $scope.onMasterUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;

            });
        };

    };
    appControllers.controller('Demo_Module_MasterEditorController', masterEditorController);
})(appControllers);