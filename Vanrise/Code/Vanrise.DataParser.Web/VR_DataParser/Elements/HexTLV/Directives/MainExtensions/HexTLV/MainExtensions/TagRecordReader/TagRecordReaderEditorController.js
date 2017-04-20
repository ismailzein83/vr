(function (appControllers) {

    'use strict';

    TagRecordReaderEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function TagRecordReaderEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {
        var testContextChange;
        var context;
        var tagRecordTypeEntity;

        var isEditMode;

        var hexTLVTagTypeGridAPI;
        var hexTLVTagTypeGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                tagRecordTypeEntity = parameters.tagRecordTypeEntity;
            }
            isEditMode = (tagRecordTypeEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            if (context != undefined) {
                $scope.scopeModel.useRecordType = context.useRecordType();
            }
            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onHexTLVTagTypeGridReady = function (api) {
                hexTLVTagTypeGridAPI = api;
                hexTLVTagTypeGridReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateRecordReaderTag() : addRecordReaderTag();
            };
            $scope.scopeModel.onDataRecordTypeSelectionChange = function () {
                var hexTLVTagTypePayload = { context: getContext() };
                hexTLVTagTypeGridAPI.load(hexTLVTagTypePayload);
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function buildRecordReaderObjFromScope() {
                return {
                    Key: $scope.scopeModel.tag,
                    Value: {
                        RecordType: $scope.scopeModel.useRecordType ? dataRecordTypeSelectorAPI.getSelectedIds() : $scope.scopeModel.recordType,
                        TagTypes: hexTLVTagTypeGridAPI.getData()
                    }
                };
            }
            function addRecordReaderTag() {
                var recordReaderTagObj = buildRecordReaderObjFromScope();
                if ($scope.onTagRecordReaderAdded != undefined) {
                    $scope.onTagRecordReaderAdded(recordReaderTagObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateRecordReaderTag() {
                var recordReaderTagObj = buildRecordReaderObjFromScope();
                if ($scope.onEditTagRecordReader != undefined) {
                    $scope.onEditTagRecordReader(recordReaderTagObj);
                }
                $scope.modalContext.closeModal();
            }

        }

        function load() {
            testContextChange = getContext();
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            function setTitle() {
                if (isEditMode && tagRecordTypeEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(tagRecordTypeEntity.Key, 'Tag');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Tag');
            }
            function loadStaticData() {
                if (tagRecordTypeEntity != undefined) {
                    $scope.scopeModel.recordType = tagRecordTypeEntity.Value.RecordType;
                    $scope.scopeModel.tag = tagRecordTypeEntity.Key;
                }
            }
            function loadHexTLVTagTypeGridDirective() {
                var hexTLVTagTypeGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                hexTLVTagTypeGridReadyPromiseDeferred.promise.then(function () {
                    var hexTLVTagTypePayload = { context: getContext() };
                    if (tagRecordTypeEntity != undefined && testContextChange.recordTypeId != getContext().recordTypeId)
                        hexTLVTagTypePayload.tagTypes = tagRecordTypeEntity.Value.TagTypes;

                    VRUIUtilsService.callDirectiveLoad(hexTLVTagTypeGridAPI, hexTLVTagTypePayload, hexTLVTagTypeGridLoadPromiseDeferred);
                });
                return hexTLVTagTypeGridLoadPromiseDeferred.promise;
            }
            function loadDataRecordTypeSelector() {
                if ($scope.scopeModel.useRecordType) {
                    var dataRecordTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    dataRecordTypeSelectorReadyPromiseDeferred.promise.then(function () {
                        var dataRecordTypePayload = {};
                        if (tagRecordTypeEntity != undefined)
                            dataRecordTypePayload = {
                                selectedIds: tagRecordTypeEntity.Value.RecordType,
                            };
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypePayload, dataRecordTypeSelectorLoadPromiseDeferred);
                    });
                    return dataRecordTypeSelectorLoadPromiseDeferred.promise;
                }
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeSelector, loadHexTLVTagTypeGridDirective]).then(function () {
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function getContext() {
            var currenctContext = context;
            if (currenctContext == undefined)
                currenctContext = {};
            if (dataRecordTypeSelectorAPI != undefined) {
                currenctContext.recordTypeId = function () {
                    return dataRecordTypeSelectorAPI.getSelectedIds();
                };
            }
            return currenctContext;
        }

    }
    appControllers.controller('VR_DataParser_TagRecordReaderEditorController', TagRecordReaderEditorController);

})(appControllers);