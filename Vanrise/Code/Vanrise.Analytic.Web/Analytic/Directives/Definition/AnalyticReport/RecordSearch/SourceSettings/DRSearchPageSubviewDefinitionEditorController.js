(function (appControllers) {

    'use strict';

    DRSearchPageSubviewDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function DRSearchPageSubviewDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;
        var subviewDefinition;
        var dataRecordTypeId;

        var subviewDefinitionSettingsDirectiveAPI;
        var subviewDefinitionSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                subviewDefinition = parameters.subviewDefinition;
                dataRecordTypeId = parameters.dataRecordTypeId;
            }

            isEditMode = (subviewDefinition != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onSubviewDefinitionSettingsDirectiveReady = function (api) {
                subviewDefinitionSettingsDirectiveAPI = api;
                subviewDefinitionSettingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSubviewDefinitionSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor(subviewDefinition.Name ? subviewDefinition.Name : null, 'Subview') :
                UtilsService.buildTitleForAddEditor('Subview');
        }
        function loadStaticData() {

            if (subviewDefinition == undefined)
                return;

            $scope.scopeModel.name = subviewDefinition.Name;
        }
        function loadSubviewDefinitionSettingsDirective() {
            var subviewDefinitionSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            subviewDefinitionSettingsDirectiveReadyDeferred.promise.then(function () {

                var subviewDefinitionSettingsDirectivePayload = { dataRecordTypeId: dataRecordTypeId };
                if (subviewDefinition != undefined) {
                    subviewDefinitionSettingsDirectivePayload.subviewDefinitionSettings = subviewDefinition.Settings;
                }
                VRUIUtilsService.callDirectiveLoad(subviewDefinitionSettingsDirectiveAPI, subviewDefinitionSettingsDirectivePayload, subviewDefinitionSettingsDirectiveLoadDeferred);
            });

            return subviewDefinitionSettingsDirectiveLoadDeferred.promise;
        }

        function insert() {
            var subviewDefinition = buildDRSearchPageSubviewDefinition();

            if ($scope.onSubviewDefinitionAdded != undefined && typeof ($scope.onSubviewDefinitionAdded) == 'function') {
                $scope.onSubviewDefinitionAdded(subviewDefinition);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var subviewDefinition = buildDRSearchPageSubviewDefinition();

            if ($scope.onSubviewDefinitionUpdated != undefined && typeof ($scope.onSubviewDefinitionUpdated) == 'function') {
                $scope.onSubviewDefinitionUpdated(subviewDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function buildDRSearchPageSubviewDefinition() {

            var subviewDefinition = {
                SubviewDefinitionId: subviewDefinition && subviewDefinition.SubviewDefinitionId ? subviewDefinition.SubviewDefinitionId : UtilsService.guid(),
                Name: $scope.scopeModel.name,
                Settings: subviewDefinitionSettingsDirectiveAPI.getData()
            };

            return subviewDefinition;
        }
    }

    appControllers.controller('VR_Analytic_DRSearchPageSubviewDefinitionEditorController', DRSearchPageSubviewDefinitionEditorController);

})(appControllers);