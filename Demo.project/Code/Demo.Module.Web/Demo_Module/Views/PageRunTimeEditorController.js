(function (appControllers) {
"use strict";
pageRunTimeEditorController.$inject = ['$scope','Demo_Module_PageDefinitionAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_PageRunTimeAPIService'];

function pageRunTimeEditorController($scope, Demo_Module_PageDefinitionAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Demo_Module_PageRunTimeAPIService) {

    var isEditMode;
    var pageRunTimeId;
    var pageDefinitionId;
    var pageRunTimeEntity;
    var pageDefinitionEntity;
   
    $scope.scopeModel = {};
    $scope.scopeModel.fields = [];

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != undefined && parameters != null) {
            pageDefinitionId = parameters.pageDefinitionId;
            pageRunTimeId = parameters.pageRunTimeId;
        }
        isEditMode = (pageRunTimeId != undefined);

    }

    function defineScope() {

        $scope.scopeModel.savePageRunTime = function () {
            if (isEditMode)
                return updatePageRunTime();
            else
                return insertPageRunTime();

        };

        $scope.scopeModel.close = function () {
            $scope.modalContext.closeModal();
        };

        $scope.scopeModel.onGridReady = function () {
        };

       
    }

    function load()
    {
        $scope.scopeModel.isLoading = true;
        getPageDefinition().then(function (response) {
            if (isEditMode) {
                getPageRunTime().then(function () {
                    loadAllControls().finally(function () {
                        pageRunTime = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        });
    }

    function getPageRunTime() {
        return Demo_Module_PageRunTimeAPIService.GetPageRunTimeById(pageRunTimeId).then(function (response) {
            pageRunTimeEntity = response;
        });
    }

    function getPageDefinition() {
        return Demo_Module_PageDefinitionAPIService.GetPageDefinitionById(pageDefinitionId).then(function (response) {
            pageDefinitionEntity = response;

            if (pageDefinitionEntity.Details != undefined && pageDefinitionEntity.Details.Fields != undefined) {
                for (var i = 0; i < pageDefinitionEntity.Details.Fields.length; i++) {
                    var field = pageDefinitionEntity.Details.Fields[i];
                    $scope.scopeModel.fields.push(field);
                } 
            } 
        });
    }
  
    function loadAllControls() {

        function loadRunTimeEditorDirective(field) {

            field.runTimeEditorReadyPromiseDeferred.promise.then(function (response) {
                field.runTimeEditorPayload = { label: field.Title,isRequired:field.IsRequired}

                if (pageRunTimeEntity != undefined && pageRunTimeEntity.Details != undefined) {
                    field.runTimeEditorPayload.runTimeEditor = pageRunTimeEntity.Details.FieldValues[field.Name];
                }

                VRUIUtilsService.callDirectiveLoad(field.runTimeEditorDirectiveApi, field.runTimeEditorPayload, field.runTimeEditorLoadPromiseDeferred);
             }); 
            return field.runTimeEditorLoadPromiseDeferred.promise;
        }

        function prepareItem(field) {
            field.runTimeEditorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            field.onRunTimeEditorReady = function (api) {
                field.runTimeEditorDirectiveApi = api;
                field.runTimeEditorReadyPromiseDeferred.resolve();
            };
            field.runTimeEditorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        }

        function setTitle() {
            if (isEditMode && pageRunTimeEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(pageDefinitionEntity.Name, "Page RunTime");
            else
                $scope.title = UtilsService.buildTitleForAddEditor(pageDefinitionEntity.Name, "Page RunTime");
        }

        function loadStaticData() {

        }

        function loadRunTimeEditorDirectives() {
                var promises = [];
                for (var i = 0; i < $scope.scopeModel.fields.length; i++) {
                    var field = $scope.scopeModel.fields[i];
                    prepareItem(field);
                    promises.push(loadRunTimeEditorDirective(field));
                }
                return UtilsService.waitMultiplePromises(promises);
        }

        return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRunTimeEditorDirectives])
         .catch(function (error) {
             VRNotificationService.notifyExceptionWithClose(error, $scope);
         })
           .finally(function () {
               $scope.scopeModel.isLoading = false;
           });
    }

    function buildPageRunTimeObjectFromScope() {
        var object = {
            PageRunTimeId: (pageRunTimeId != undefined) ? pageRunTimeId : undefined,
            PageDefinitionId:pageDefinitionId,
            Details: {}
        };
        object.Details.FieldValues = {};

        for (var j = 0; j < $scope.scopeModel.fields.length; j++) {
            var field = $scope.scopeModel.fields[j];
            object.Details.FieldValues[field.Name]=field.runTimeEditorDirectiveApi.getData();
        }
        return object;
    }

    function insertPageRunTime() {

        $scope.scopeModel.isLoading = true;
        var pageRunTimeObject = buildPageRunTimeObjectFromScope();
        return Demo_Module_PageRunTimeAPIService.AddPageRunTime(pageRunTimeObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded(pageDefinitionEntity.Name, response)) {
                if ($scope.onPageRunTimeAdded != undefined) {
                    $scope.onPageRunTimeAdded(response.InsertedObject);
                    $scope.scopeModel.fields = [];
                } 
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            $scope.scopeModel.isLoading = false;
            VRNotificationService.notifyException(error, $scope);
        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        });

    }

    function updatePageRunTime() {
        $scope.scopeModel.isLoading = true;
        var pageRunTimeObject = buildPageRunTimeObjectFromScope();
        Demo_Module_PageRunTimeAPIService.UpdatePageRunTime(pageRunTimeObject).then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated(pageDefinitionEntity.Name, response)) {
                if ($scope.onPageRunTimeUpdated != undefined) {
                    $scope.onPageRunTimeUpdated(response.UpdatedObject);
                    $scope.scopeModel.fields = [];

                }
                $scope.modalContext.closeModal();
            }
           
        }).catch(function (error) {
            $scope.scopeModel.isLoading = false;
            VRNotificationService.notifyException(error, $scope);
        }).finally(function () {

            $scope.scopeModel.isLoading = false;

        });
    }

};
appControllers.controller('Demo_Module_PageRunTimeEditorController', pageRunTimeEditorController);
})(appControllers);