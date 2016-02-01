(function (appControllers) {

    'use strict';

    GenericRuleDefinitionController.$inject = ['$scope', 'VR_GenericData_GenericRuleDefinitionAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function GenericRuleDefinitionController($scope, VR_GenericData_GenericRuleDefinitionAPIService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;

        var criteriaDirectiveAPI;
        var criteriaDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        
        var genericRuleDefinitionId;
        var genericRuleDefinitionEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                genericRuleDefinitionId = parameters.genericRuleDefinitionId;
            }

            isEditMode = (genericRuleDefinitionId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onCriteriaDirectiveReady = function (api) {
                criteriaDirectiveAPI = api;
                criteriaDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return updateGenericRuleDefinition();
                else
                    return insertGenericRuleDefinition();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getGenericRuleDefinition().then(function () {
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

        function getGenericRuleDefinition() {
            return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(genericRuleDefinitionId).then(function (response) {
                genericRuleDefinitionEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCriteriaDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((genericRuleDefinitionEntity != undefined) ? genericRuleDefinitionEntity.Name : null, 'Generic Rule Definition') :
                    UtilsService.buildTitleForAddEditor('Generic Rule Definition');
            }
            function loadStaticData() {
                if (genericRuleDefinitionEntity == undefined) {
                    return;
                }

                $scope.scopeModel.name = genericRuleDefinitionEntity.Name;
            }
            function loadCriteriaDirective() {
                var criteriaDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                criteriaDirectiveReadyDeferred.promise.then(function () {
                    var criteriaDirectivePayload;
                    if (genericRuleDefinitionEntity != undefined && genericRuleDefinitionEntity.CriteriaDefinition != null) {
                        criteriaDirectivePayload = {
                            GenericRuleDefinitionCriteriaFields: genericRuleDefinitionEntity.CriteriaDefinition.Fields
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(criteriaDirectiveAPI, criteriaDirectivePayload, criteriaDirectiveLoadDeferred);
                });
                
                return criteriaDirectiveLoadDeferred.promise;
            }
        }

        function insertGenericRuleDefinition() {
            $scope.isLoading = true;

            var userObject = buildGenericRuleDefinitionObjectFromScope();

            return VR_GenericData_GenericRuleDefinitionAPIService.AddUser(userObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('User', response, 'Email')) {
                    if ($scope.onUserAdded != undefined)
                        $scope.onUserAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateGenericRuleDefinition() {
            $scope.isLoading = true;

            var userObject = buildGenericRuleDefinitionObjectFromScope();

            return VR_GenericData_GenericRuleDefinitionAPIService.UpdateUser(userObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('User', response, 'Email')) {
                    if ($scope.onUserUpdated != undefined)
                        $scope.onUserUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function buildGenericRuleDefinitionObjectFromScope() {
            var userObject = {
                userId: (userId != null) ? userId : 0,
                name: $scope.name,
                email: $scope.email,
                description: $scope.description,
                Status: $scope.isActive == false ? '0' : '1'
            };
            return userObject;
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleDefinitionController', GenericRuleDefinitionController);

})(appControllers);