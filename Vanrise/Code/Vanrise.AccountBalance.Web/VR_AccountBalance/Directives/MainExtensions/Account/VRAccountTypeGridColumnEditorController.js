(function (appControllers) {

    'use strict';

    AccountTypeGridColumnEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function AccountTypeGridColumnEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var gridColumnEntity;

        var isEditMode;

        var sourceSelectorAPI;
        var sourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var gridColCSSClassSelectorAPI;
        var gridColCSSClassSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sourceFieldsSelectorAPI;
        var sourceFieldsSelectorReadyPromiseDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                gridColumnEntity = parameters.gridColumnEntity;
            }
            isEditMode = (gridColumnEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onSourcesSelectorReady = function (api) {
                sourceSelectorAPI = api;
                sourceSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onGridColCSSClassSelectorReady = function (api) {
                gridColCSSClassSelectorAPI = api;
                gridColCSSClassSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onSourceFieldsSelectorReady = function (api) {
                sourceFieldsSelectorAPI = api;
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateGridColumn() : addGridColumn();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onSourceSelectionChanged = function () {
                var sourceId = sourceSelectorAPI.getSelectedIds();
                if (sourceId != undefined) {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload = {
                        sourceId: sourceId,
                        context: getContext()
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceFieldsSelectorAPI, payload, setLoader, sourceFieldsSelectorReadyPromiseDeferred);
                }
            };
            function builGridColumnObjFromScope() {
                return {
                    Title: $scope.scopeModel.title,
                    SourceId: sourceSelectorAPI.getSelectedIds(),
                    FieldName: sourceFieldsSelectorAPI.getSelectedIds(),
                    GridColCSSValue: gridColCSSClassSelectorAPI.getSelectedIds(),
                    UseEmptyHeader: $scope.scopeModel.useEmptyHeader
                };
            }

            function addGridColumn() {
                var gridColumnObj = builGridColumnObjFromScope();
                if ($scope.onGridColumnAdded != undefined) {
                    $scope.onGridColumnAdded(gridColumnObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateGridColumn() {
                var gridColumnObj = builGridColumnObjFromScope();
                if ($scope.onGridColumnUpdated != undefined) {
                    $scope.onGridColumnUpdated(gridColumnObj);
                }
                $scope.modalContext.closeModal();
            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {
                function setTitle() {
                    if (isEditMode && gridColumnEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(gridColumnEntity.Title, 'Grid Column');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Grid Column');
                }
                function loadStaticData() {
                    if (gridColumnEntity != undefined) {
                        $scope.scopeModel.title = gridColumnEntity.Title;
                        $scope.scopeModel.useEmptyHeader = gridColumnEntity.UseEmptyHeader;
                    }
                }
                function loadSourcesSelectorDirective() {
                    var sourcesSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    sourceSelectorReadyPromiseDeferred.promise.then(function () {
                        var sourcesSelectorPayload = { context: getContext() };
                        if (gridColumnEntity != undefined)
                            sourcesSelectorPayload.selectedIds = gridColumnEntity.SourceId;
                        VRUIUtilsService.callDirectiveLoad(sourceSelectorAPI, sourcesSelectorPayload, sourcesSelectorLoadPromiseDeferred);
                    });
                    return sourcesSelectorLoadPromiseDeferred.promise;
                }
                function loadGridColCSSClassSelectorDirective() {
                    var gridColCSSClassSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    gridColCSSClassSelectorReadyPromiseDeferred.promise.then(function () {
                        var gridColCSSClassSelectorPayload;
                        if (gridColumnEntity != undefined)
                            gridColCSSClassSelectorPayload = {
                                selectedIds: gridColumnEntity.GridColCSSValue
                            };
                        VRUIUtilsService.callDirectiveLoad(gridColCSSClassSelectorAPI, gridColCSSClassSelectorPayload, gridColCSSClassSelectorLoadPromiseDeferred);
                    });
                    return gridColCSSClassSelectorLoadPromiseDeferred.promise;
                }
                function loadSourceFieldSelectorDirective() {
                    if (gridColumnEntity != undefined)
                    {
                        sourceFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                        var sourceFieldsSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        sourceFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var sourceFieldsSelectorPayload = {
                                context: getContext(),
                                sourceId: gridColumnEntity.SourceId,
                                selectedIds:gridColumnEntity.FieldName
                            };
                            VRUIUtilsService.callDirectiveLoad(sourceFieldsSelectorAPI, sourceFieldsSelectorPayload, sourceFieldsSelectorLoadPromiseDeferred);
                        });
                        return sourceFieldsSelectorLoadPromiseDeferred.promise;
                    }
                }
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSourcesSelectorDirective, loadSourceFieldSelectorDirective, loadGridColCSSClassSelectorDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }
        }
        function getContext() {
            var currentContext = context;
            if (currentContext == undefined) {
                currentContext = {};
            }
            return currentContext;
        }

    }
    appControllers.controller('VR_AccountBalance_AccountTypeGridColumnEditorController', AccountTypeGridColumnEditorController);

})(appControllers);