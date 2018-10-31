(function (appControllers) {
"use strict";
pageDefinitionEditorController.$inject = ['$scope', 'Demo_Module_PageDefinitionAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_PageDefinitionService'];

function pageDefinitionEditorController($scope, Demo_Module_PageDefinitionAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Demo_Module_PageDefinitionService) {

    var isEditMode;
    var pageDefinitionId;
    var pageDefinitionEntity;
    var pageDefinitionFieldsDirectiveApi;
    var pageDefinitionSubviewsDirectiveApi;
    var pageDefinitionFieldFiltersDirectiveApi;
    var pageDefinitionFieldsReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    var pageDefinitionSubviewsReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    var pageDefinitionFieldFiltersReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var fields = [];
    var subviews = [];
    var fieldFilters = [];
    var fieldsForFiltering = [];
    $scope.scopeModel = {};
   
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != undefined && parameters != null) {
            pageDefinitionId = parameters.pageDefinitionId;
        }
        isEditMode = (pageDefinitionId != undefined);
    }

    function defineScope() {


        $scope.scopeModel.savePageDefinition = function () {
            if (isEditMode)
                return updatePageDefinition();
            else
                return insertPageDefinition();

        };

        $scope.scopeModel.close = function () {
            $scope.modalContext.closeModal();
        };

        $scope.scopeModel.onGridReady = function () {
        };

        $scope.scopeModel.onFieldsReady = function (api) {

            pageDefinitionFieldsDirectiveApi = api;
            pageDefinitionFieldsReadyPromiseDeferred.resolve();

        };

        $scope.scopeModel.onSubviewsReady = function (api) {

            pageDefinitionSubviewsDirectiveApi = api;
            pageDefinitionSubviewsReadyPromiseDeferred.resolve();

        };

        $scope.scopeModel.onFieldFiltersReady = function (api) {

            pageDefinitionFieldFiltersDirectiveApi = api;
            pageDefinitionFieldFiltersReadyPromiseDeferred.resolve();

        }

    }

    function load() {
        $scope.scopeModel.isLoading = true;
        if (isEditMode) {
            getPageDefinition().then(function () {
                loadAllControls().finally(function () {
                    pageDefinitionEntity = undefined;
                });
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }
        else
            loadAllControls();
    }

    function getPageDefinition() {
        return Demo_Module_PageDefinitionAPIService.GetPageDefinitionById(pageDefinitionId).then(function (response) {
            pageDefinitionEntity = response;
        });
    }
  
    function loadAllControls() {

        function setTitle() {
            if (isEditMode && pageDefinitionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(pageDefinitionEntity.Name, "Page Definition");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Page Definition");
        }

        function loadStaticData() {

            if (pageDefinitionEntity != undefined) {
                $scope.scopeModel.name = pageDefinitionEntity.Name;
                if (pageDefinitionEntity.Details != undefined) {
                    if (pageDefinitionEntity.Details.Fields!=undefined) {
                        for (var j = 0; j < pageDefinitionEntity.Details.Fields.length; j++) {
                            var pageDefinitionField = pageDefinitionEntity.Details.Fields[j];
                            fields.push(pageDefinitionField);
                            fieldsForFiltering.push(pageDefinitionField)
                        }
                    }
                    if (pageDefinitionEntity.Details.SubViews != undefined) {
                        for (var j = 0; j < pageDefinitionEntity.Details.SubViews.length; j++) {
                            var pageDefinitionSubview = pageDefinitionEntity.Details.SubViews[j];
                            subviews.push(pageDefinitionSubview);
                        }
                    }

                    if (pageDefinitionEntity.Details.Filters != undefined) {
                        fieldFilters = [];
                        for (var j = 0; j < pageDefinitionEntity.Details.Filters.length; j++) {
                            var pageDefinitionFieldFilter = pageDefinitionEntity.Details.Filters[j];
                            fieldFilters.push(pageDefinitionFieldFilter.fieldName);
                        }
                    }

                }
            }
        }

        function loadPageDefinitionFieldsDirective() {
            var pageDefinitionFieldsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            pageDefinitionFieldsReadyPromiseDeferred.promise.then(function (response) {

                var fieldAdder = function (field) {
                    fieldsForFiltering.push(field);
                    loadPageDefinitionFieldFiltersDirective();
                }


                var fieldUpdater = function (field,index) {
                    fieldsForFiltering[index] = field;
                    fieldFilters = [];
                    loadPageDefinitionFieldFiltersDirective();
                }
                var pageDefinitionFieldsPayload = { Fields: fields, FieldAdder: fieldAdder, FieldUpdater: fieldUpdater };

                VRUIUtilsService.callDirectiveLoad(pageDefinitionFieldsDirectiveApi, pageDefinitionFieldsPayload, pageDefinitionFieldsLoadPromiseDeferred);
            });
            return pageDefinitionFieldsLoadPromiseDeferred.promise;
        }

        function loadPageDefinitionSubviewsDirective() {
            var pageDefinitionSubviewsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            pageDefinitionSubviewsReadyPromiseDeferred.promise.then(function (response) {

                var pageDefinitionSubviewsPayload = { Subviews: subviews };

                VRUIUtilsService.callDirectiveLoad(pageDefinitionSubviewsDirectiveApi, pageDefinitionSubviewsPayload, pageDefinitionSubviewsLoadPromiseDeferred);
            });
            return pageDefinitionSubviewsLoadPromiseDeferred.promise;
        }

        function loadPageDefinitionFieldFiltersDirective() {
            var pageDefinitionFieldFiltersLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            pageDefinitionFieldFiltersReadyPromiseDeferred.promise.then(function (response) {

                var pageDefinitionFieldFiltersPayload = { Fields: fieldsForFiltering, FieldFilters: fieldFilters };

                VRUIUtilsService.callDirectiveLoad(pageDefinitionFieldFiltersDirectiveApi, pageDefinitionFieldFiltersPayload, pageDefinitionFieldFiltersLoadPromiseDeferred);
            });
            return pageDefinitionFieldFiltersLoadPromiseDeferred.promise;
        }

        return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPageDefinitionFieldsDirective, loadPageDefinitionSubviewsDirective, loadPageDefinitionFieldFiltersDirective])
         .catch(function (error) {
             VRNotificationService.notifyExceptionWithClose(error, $scope);
         })
           .finally(function () {
               $scope.scopeModel.isLoading = false;
           });
    }

    function buildPageDefinitionObjectFromScope() {
        var object = {
            PageDefinitionId: (pageDefinitionId != undefined) ? pageDefinitionId : undefined,
            Name: $scope.scopeModel.name,
            Details:{}
        };
        if (pageDefinitionFieldsDirectiveApi.getData() != undefined) {
            var getFields = pageDefinitionFieldsDirectiveApi.getData().Fields;
            object.Details.Fields = [];
            for (var i = 0; i < getFields.length; i++) {
                var field = getFields[i];
                object.Details.Fields.push(field);
            }
        }
        if (pageDefinitionSubviewsDirectiveApi.getData() != undefined) {
            var getSubviews = pageDefinitionSubviewsDirectiveApi.getData().Subviews;
            object.Details.SubViews = [];
            for (var i = 0; i < getSubviews.length; i++) {
                var subview = getSubviews[i];
                object.Details.SubViews.push(subview);
            }
        }
        if (pageDefinitionFieldFiltersDirectiveApi.getSelectedFilters() != undefined) {
            var getFieldFilters = pageDefinitionFieldFiltersDirectiveApi.getSelectedFilters();
            object.Details.Filters = [];
            for (var i = 0; i < getFieldFilters.length; i++) {
                var fieldFilter = { fieldName: getFieldFilters[i] };
                object.Details.Filters.push(fieldFilter);
            }
        }
        return object;
    }

    function insertPageDefinition() {

        $scope.scopeModel.isLoading = true;
        var pageDefinitionObject = buildPageDefinitionObjectFromScope();
        return Demo_Module_PageDefinitionAPIService.AddPageDefinition(pageDefinitionObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("PageDefinition", response, "Name")) {
                if ($scope.onPageDefinitionAdded != undefined) {
                    $scope.onPageDefinitionAdded(response.InsertedObject);
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

    function updatePageDefinition() {
        $scope.scopeModel.isLoading = true;
        var pageDefinitionObject = buildPageDefinitionObjectFromScope();
        Demo_Module_PageDefinitionAPIService.UpdatePageDefinition(pageDefinitionObject).then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("PageDefinition", response, "Name")) {
                if ($scope.onPageDefinitionUpdated != undefined) {
                    $scope.onPageDefinitionUpdated(response.UpdatedObject);
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
appControllers.controller('Demo_Module_PageDefinitionEditorController', pageDefinitionEditorController);
})(appControllers);