(function (appControllers) {

    "use strict";

    GenericBEColumnDefintionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericBEColumnDefintionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var columnDefinition;
        var context;

        var dataRecordTypeFieldsSelectorAPI;
        var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var columnSettingsDirectiveAPI;
        var columnSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                columnDefinition = parameters.columnDefinition;
                context = parameters.context;
            }
            isEditMode = (columnDefinition != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                dataRecordTypeFieldsSelectorAPI = api;
                dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onColumnSettingDirectiveReady = function (api) {
                columnSettingsDirectiveAPI = api;
                columnSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDataRecordTypeFieldsSelectionChanged = function () {
                if (dataRecordTypeFieldsSelectorAPI.getSelectedValue() != undefined) {
                    $scope.scopeModel.fieldTitle = dataRecordTypeFieldsSelectorAPI.getSelectedValue().Title;
                }
                else {
                    $scope.scopeModel.fieldTitle = undefined;
                }

            };

            $scope.scopeModel.saveColumnDefinition = function () {
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

        }
        function load() {

            loadAllControls();

            function loadAllControls() {
                $scope.scopeModel.isLoading = true;
                function setTitle() {
                    if (isEditMode && columnDefinition != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(columnDefinition.FieldName, 'Column Definition Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Column Definition Editor');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.fieldTitle = columnDefinition.FieldTitle;
                }

                function loadDataRecordTypeFieldsSelector() {
                    var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                        var typeFieldsPayload = {
                            dataRecordTypeId: context.getDataRecordTypeId(),
                            selectedIds: columnDefinition != undefined ? columnDefinition.FieldName : undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, typeFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                    });
                    return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
                }

                function loadSettingDirectiveSection() {
                    var loadColumnSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                    columnSettingsReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            data: columnDefinition != undefined && columnDefinition.GridColumnSettings != undefined ? columnDefinition.GridColumnSettings : undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(columnSettingsDirectiveAPI, payload, loadColumnSettingsPromiseDeferred);
                    });
                    return loadColumnSettingsPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadDataRecordTypeFieldsSelector, loadSettingDirectiveSection]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildColumnDefinitionFromScope() {
            return {
                FieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                FieldTitle: $scope.scopeModel.fieldTitle,
                GridColumnSettings: columnSettingsDirectiveAPI.getData()
            };
        }

        function insert() {
            var columnDefinition = buildColumnDefinitionFromScope();
            if ($scope.onGenericBEColumnDefinitionAdded != undefined) {
                $scope.onGenericBEColumnDefinitionAdded(columnDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var columnDefinition = buildColumnDefinitionFromScope();
            if ($scope.onGenericBEColumnDefinitionUpdated != undefined) {
                $scope.onGenericBEColumnDefinitionUpdated(columnDefinition);
            }
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('VR_GenericData_GenericBEColumnDefintionController', GenericBEColumnDefintionController);
})(appControllers);
