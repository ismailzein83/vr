(function (appControllers) {

    'use strict';

    TagValueParserEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function TagValueParserEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var tagTypeEntity;

        var isEditMode;

        var tagValueParserSelectorAPI;
        var tagValueParserSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                tagTypeEntity = parameters.tagTypeEntity;

            }
            isEditMode = (tagTypeEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onTagValueParserSelectorReady = function (api) {
                tagValueParserSelectorAPI = api;
                tagValueParserSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateTagValueParser() : addTagValueParser();
            }

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            function buildTagTypesObjFromScope() {
                return {
                    Key: $scope.scopeModel.name,
                    Value: {
                        ValueParser: tagValueParserSelectorAPI.getData()
                    }
                };
            }

            function addTagValueParser() {
                var tagTypeObj = buildTagTypesObjFromScope();
                if ($scope.onHexTLVTagTypeAdded != undefined) {
                    $scope.onHexTLVTagTypeAdded(tagTypeObj);
                }
                $scope.modalContext.closeModal();
            }

            function updateTagValueParser() {
                var tagTypeObj = buildTagTypesObjFromScope();
                if ($scope.onEditHexTLVTagType != undefined) {
                    $scope.onEditHexTLVTagType(tagTypeObj);
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
                if (isEditMode && tagTypeEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(tagTypeEntity.Key, 'Tag');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Tag');
            }

            function loadStaticData() {
                if (tagTypeEntity != undefined) {
                    $scope.scopeModel.name = tagTypeEntity.Key;
                }
            }

            function loadTagValueParserSelector() {
                var tagValueParserSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                tagValueParserSelectorReadyPromiseDeferred.promise.then(function () {
                    var tagValueParserSelectorPayload = { context: getContext() };
                    if (tagTypeEntity != undefined)
                        tagValueParserSelectorPayload.tagValueParserEntity = tagTypeEntity.Value.ValueParser;
                    VRUIUtilsService.callDirectiveLoad(tagValueParserSelectorAPI, tagValueParserSelectorPayload, tagValueParserSelectorLoadPromiseDeferred);
                });
                return tagValueParserSelectorLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTagValueParserSelector]).then(function () {

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
    appControllers.controller('VR_DataParser_TagValueParserEditorController', TagValueParserEditorController);

})(appControllers);