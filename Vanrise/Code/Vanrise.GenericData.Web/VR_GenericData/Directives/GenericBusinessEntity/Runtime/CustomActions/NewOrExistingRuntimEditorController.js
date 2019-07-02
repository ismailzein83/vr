(function (appControllers) {
    "use strict";
    NewOrExistingRuntimEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBusinessEntityAPIService', 'VR_GenericData_GenericBusinessEntityService'];

    function NewOrExistingRuntimEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService, VR_GenericData_GenericBusinessEntityService) {

        $scope.scopeModel = {};

        var customAction;
        var businessEntityDefinitionId;
        var parentFieldValues;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                parentFieldValues = parameters.parentFieldValues;
                customAction = parameters.customAction;
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
            }
        }

        function defineScope() {
            $scope.scopeModel.getBEentity = function () {
                $scope.scopeModel.isLoading = true;

                var filter = buildFilter();
                $scope.modalContext.closeModal();
                var promiseDeferred = UtilsService.createPromiseDeferred();

                 VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityInfo(businessEntityDefinitionId, UtilsService.serializetoJson(filter))
                    .then(function (response) {
                        if (response != undefined && response.length != 0) {
                            var firstEntityId = response[0].GenericBusinessEntityId;
                            var idFieldValue =
                            {
                                ID: firstEntityId
                            };
                            parentFieldValues.trigerSearch(idFieldValue).then(function () {
                                parentFieldValues.expendRow(firstEntityId);
                                promiseDeferred.resolve();
                            }).catch(function (error) {
                                VRNotificationService.notifyExceptionWithClose(error, $scope);
                                promiseDeferred.reject();
                            }).finally(function () {
                                $scope.scopeModel.isLoading = false;
                            });
                        }
                        else {
                            var fieldValues = buildFieldValues();
                            promiseDeferred.resolve();
                            //var onGenericBusinessEntityAdded = function () { };
                            VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(undefined, businessEntityDefinitionId, "large", fieldValues);
                        }
                    }).catch(function (error) {
                        $scope.scopeModel.isLoading = false;
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        promiseDeferred.reject();
                    });
                return promiseDeferred;
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.title = "Add";
        }
        function buildFieldValues() {
            var fieldValues =
            {
                FirstName: {
                    value: $scope.firstName
                },
                MiddleName: {
                    value: $scope.middleName
                },
                LastName: {
                    value: $scope.lastName
                },
                MotherName: {
                    value: $scope.motherName
                },
                BirthPlace: {
                    value: $scope.birthPlace
                },
                BirthDate: {
                    value: $scope.birthDate
                }
            };
            return fieldValues;
        }
        function buildFilter() {
            var filters = [];
            var firstNameFilter =
            {
                FieldName: "FirstName",
                FilterValues: [$scope.firstName]
            };
            filters.push(firstNameFilter);
            var lastNameFilter =
            {
                FieldName: "LastName",
                FilterValues: [$scope.lastName]
            };
            filters.push(lastNameFilter);
            var middleNameFilter =
            {
                FieldName: "MiddleName",
                FilterValues: [$scope.middleName]
            };
            filters.push(middleNameFilter);
            var filter =
            {
                FieldFilters: filters
            };
            return filter;
        }
    }
    appControllers.controller('VR_GenericData_NewOrExistingRuntimeEditorController', NewOrExistingRuntimEditorController);
})(appControllers);