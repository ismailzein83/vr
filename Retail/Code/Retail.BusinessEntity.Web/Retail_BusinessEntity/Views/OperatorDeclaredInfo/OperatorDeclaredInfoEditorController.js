(function (appControllers) {

    'use strict';

    OperatorDeclaredInfoEditorController.$inject = ['$scope', 'Retail_BE_OperatorDeclaredInfoAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function OperatorDeclaredInfoEditorController($scope, Retail_BE_OperatorDeclaredInfoAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var operatorDeclaredInfoId;
        var operatorDeclaredInfoEntity;

        var operatorItemsAPI;
        var operatorItemsReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                operatorDeclaredInfoId = parameters.operatorDeclaredInfoId;
            }
            isEditMode = (operatorDeclaredInfoId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onOperatorItemsReady = function (api) {
                operatorItemsAPI = api;
                operatorItemsReadyDeferred.resolve()
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateOperatorDeclaredInfo() : insertOperatorDeclaredInfo();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getOperatorDeclaredInfo().then(function () {
                    loadAllControls().finally(function () {
                        operatorDeclaredInfoEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getOperatorDeclaredInfo() {
            return Retail_BE_OperatorDeclaredInfoAPIService.GetOperatorDeclaredInfo(operatorDeclaredInfoId).then(function (response) {
                operatorDeclaredInfoEntity = response;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadOperatorItemsGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode) {
                $scope.title ='Edit : Operator Declared Info';
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Operator Declared Info');
            }
        }

      
        function loadStaticData() {
            if (operatorDeclaredInfoEntity == undefined)
                return;

            $scope.scopeModel.fromDate = operatorDeclaredInfoEntity.Settings.FromDate;
            $scope.scopeModel.toDate = operatorDeclaredInfoEntity.Settings.ToDate;
            $scope.scopeModel.notes = operatorDeclaredInfoEntity.Settings.Notes;

            if (operatorDeclaredInfoEntity.Settings.AttachmentFileId > 0)
                $scope.scopeModel.file = {
                    fileId: operatorDeclaredInfoEntity.Settings.AttachmentFileId
                };
        }

        function loadOperatorItemsGrid() {
            var operatorItemsLoadDeferred = UtilsService.createPromiseDeferred();
            operatorItemsReadyDeferred.promise.then(function () {
                var payload;
                if (operatorDeclaredInfoEntity != undefined && operatorDeclaredInfoEntity.Settings != undefined && operatorDeclaredInfoEntity.Settings.Items != undefined)
                    payload = {
                        items: operatorDeclaredInfoEntity.Settings.Items
                    };
                VRUIUtilsService.callDirectiveLoad(operatorItemsAPI, payload, operatorItemsLoadDeferred);
            });
            return operatorItemsLoadDeferred.promise;
          
        }
      
        function insertOperatorDeclaredInfo() {
            $scope.scopeModel.isLoading = true;

            var OperatorDeclaredInfoObj = buildOperatorDeclaredInfoObjFromScope();

            return Retail_BE_OperatorDeclaredInfoAPIService.AddOperatorDeclaredInfo(OperatorDeclaredInfoObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Operator Declared Info',response)) {
                    if ($scope.onOperatorDeclaredInfoAdded != undefined)
                        $scope.onOperatorDeclaredInfoAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateOperatorDeclaredInfo() {
            $scope.scopeModel.isLoading = true;

            var OperatorDeclaredInfoObj = buildOperatorDeclaredInfoObjFromScope();

            return Retail_BE_OperatorDeclaredInfoAPIService.UpdateOperatorDeclaredInfo(OperatorDeclaredInfoObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Operator Declared Info',response)) {
                    if ($scope.onOperatorDeclaredInfoUpdated != undefined) {
                        $scope.onOperatorDeclaredInfoUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildOperatorDeclaredInfoObjFromScope() {      
            var obj = {
                OperatorDeclaredInfoId: operatorDeclaredInfoId,               
                Settings: {
                    FromDate:$scope.scopeModel.fromDate,
                    ToDate:$scope.scopeModel.toDate,
                    Notes: $scope.scopeModel.notes,
                    AttachmentFileId: $scope.scopeModel.file != null ? $scope.scopeModel.file.fileId : 0,
                    Items: operatorItemsAPI.getData()
                }
            };
           
            return obj;
        }
    }

    appControllers.controller('Retail_BE_OperatorDeclaredInfoEditorController', OperatorDeclaredInfoEditorController);

})(appControllers);