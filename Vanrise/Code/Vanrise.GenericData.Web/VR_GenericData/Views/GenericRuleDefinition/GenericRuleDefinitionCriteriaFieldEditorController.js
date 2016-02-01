(function (appControllers) {

    'use strict';

    GenericRuleDefinitionCriteriaFieldEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericRuleDefinitionCriteriaFieldEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;
        var criteriaFields;
        var criteriaFieldName;
        var criteriaFieldEntity;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var behaviorTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            // Note that parameters are always passed to this editor
            if (parameters != undefined) {
                criteriaFieldName = parameters.genericRuleDefinitionCriteriaFieldName;
                criteriaFields = parameters.genericRuleDefinitionCriteriaFields;
            }

            isEditMode = (criteriaFieldName != undefined);
        }

        function defineScope() {
            $scope.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };
            $scope.onBehaviorTypeSelectorReady = function (api) {
                behaviorTypeSelectorReadyDeferred.resolve();
            };

            $scope.save = function () {
                if (isEditMode)
                    return updateGenericRuleDefinition();
                else
                    return insertGenericRuleDefinition();
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                setCriteriaFieldEntityFromParameters().then(function () {
                    loadAllControls().finally(function () {
                        genericRuleDefinitionEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function setCriteriaFieldEntityFromParameters() {
            criteriaFieldEntity = UtilsService.getItemByValue(criteriaFields, criteriaFieldName, 'FieldName');

            var deferred = UtilsService.createPromiseDeferred();
            deferred.resolve();
            return deferred.promise;
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeSelector, loadBehaviorTypeDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((criteriaFieldEntity != undefined) ? criteriaFieldEntity.FieldName : null, 'Generic Rule Definition Criteria Field') :
                    UtilsService.buildTitleForAddEditor('Generic Rule Definition Criteria Field');
            }
            function loadStaticData() {
                if (criteriaFieldEntity == undefined) {
                    return;
                }
                $scope.name = criteriaFieldEntity.FieldName;
                $scope.title = criteriaFieldEntity.Title;
                $scope.priority = criteriaFieldEntity.Priority;
            }
            function loadDataRecordTypeSelector() {
                var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                    var dataRecordTypeSelectorPayload;
                    
                    if (criteriaFieldEntity != undefined) {
                        dataRecordTypeSelectorPayload = {
                            selectedIds: criteriaFieldEntity.FieldType
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypeSelectorPayload, dataRecordTypeSelectorLoadDeferred);
                });

                return dataRecordTypeSelectorLoadDeferred.promise;
            }
            function loadBehaviorTypeDirective() {
                var behaviorTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                behaviorTypeSelectorReadyDeferred.promise.then(function () {
                    if (criteriaFieldEntity != undefined) {
                        $scope.selectedBehaviorType = UtilsService.getItemByValue($scope.behaviorTypes, criteriaFieldEntity.RuleStructureBehaviorType, 'value');
                    }
                    behaviorTypeSelectorLoadDeferred.resolve();
                });

                return behaviorTypeSelectorLoadDeferred.promise;
            }
        }

        //function insertGenericRuleDefinition() {
        //    $scope.isLoading = true;

        //    var userObject = buildGenericRuleDefinitionObjectFromScope();

        //    return VR_GenericData_GenericRuleDefinitionAPIService.AddUser(userObject)
        //    .then(function (response) {
        //        if (VRNotificationService.notifyOnItemAdded('User', response, 'Email')) {
        //            if ($scope.onUserAdded != undefined)
        //                $scope.onUserAdded(response.InsertedObject);
        //            $scope.modalContext.closeModal();
        //        }
        //    }).catch(function (error) {
        //        VRNotificationService.notifyException(error, $scope);
        //    }).finally(function () {
        //        $scope.isLoading = false;
        //    });

        //}

        //function updateGenericRuleDefinition() {
        //    $scope.isLoading = true;

        //    var userObject = buildGenericRuleDefinitionObjectFromScope();

        //    return VR_GenericData_GenericRuleDefinitionAPIService.UpdateUser(userObject).then(function (response) {
        //        if (VRNotificationService.notifyOnItemUpdated('User', response, 'Email')) {
        //            if ($scope.onUserUpdated != undefined)
        //                $scope.onUserUpdated(response.UpdatedObject);
        //            $scope.modalContext.closeModal();
        //        }
        //    }).catch(function (error) {
        //        VRNotificationService.notifyException(error, $scope);
        //    }).finally(function () {
        //        $scope.isLoading = false;
        //    });
        //}

        //function buildGenericRuleDefinitionObjectFromScope() {
        //    var userObject = {
        //        userId: (userId != null) ? userId : 0,
        //        name: $scope.name,
        //        email: $scope.email,
        //        description: $scope.description,
        //        Status: $scope.isActive == false ? '0' : '1'
        //    };
        //    return userObject;
        //}
    }

    appControllers.controller('VR_GenericData_GenericRuleDefinitionCriteriaFieldEditorController', GenericRuleDefinitionCriteriaFieldEditorController);

})(appControllers);