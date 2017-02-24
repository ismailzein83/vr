(function (appControllers) {

    'use strict';

    GridActionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function GridActionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var gridActionEntity;

        var isEditMode;

        var gridActionSettingsAPI;
        var gridActionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                gridActionEntity = parameters.gridActionEntity;
            }
            isEditMode = (gridActionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGridActionSettingsReady = function (api) {
                gridActionSettingsAPI = api;
                gridActionSettingsReadyPromiseDeferred.resolve();
            };
         
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateGridAction() : addGridAction();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builGridActionObjFromScope() {
                return {
                    Title: $scope.scopeModel.actionTitle,
                    InvoiceViewerTypeGridActionId: gridActionEntity != undefined ? gridActionEntity.GridActionId : UtilsService.guid(),
                    Settings: gridActionSettingsAPI.getData(),
                };
            }
            function addGridAction() {
                var gridActionObj = builGridActionObjFromScope();
                if ($scope.onGridActionAdded != undefined) {
                    $scope.onGridActionAdded(gridActionObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateGridAction() {
                var gridActionObj = builGridActionObjFromScope();
                if ($scope.onGridActionUpdated != undefined) {
                    $scope.onGridActionUpdated(gridActionObj);
                }
                $scope.modalContext.closeModal();
            }

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            function setTitle() {
                if (isEditMode && gridActionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(gridActionEntity.Title, 'Grid Action');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Grid Action');
            }
            function loadStaticData() {
                if (gridActionEntity != undefined) {
                    $scope.scopeModel.actionTitle = gridActionEntity.Title;
                }
            }
            function loadGridActionSettingsDirective() {
                var gridActionSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                gridActionSettingsReadyPromiseDeferred.promise.then(function () {
                    var gridActionPayload = { context: getContext() };
                    if (gridActionEntity != undefined) {
                        gridActionPayload.gridActionEntity = gridActionEntity.Settings;
                    }
                    VRUIUtilsService.callDirectiveLoad(gridActionSettingsAPI, gridActionPayload, gridActionSettingsLoadPromiseDeferred);
                });
                return gridActionSettingsLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadGridActionSettingsDirective]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }

    }
    appControllers.controller('PartnerPortal_Invoice_GridActionEditorController', GridActionEditorController);

})(appControllers);