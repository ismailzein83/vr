(function (appControllers) {
    "use strict";
    NewOrExistingRuntimEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBusinessEntityAPIService', 'VR_GenericData_GenericBusinessEntityService'];

    function NewOrExistingRuntimEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService, VR_GenericData_GenericBusinessEntityService) {

        $scope.scopeModel = {};

        var customAction;
        var businessEntityDefinitionId;
        var parentFieldValues;
        var context;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                parentFieldValues = parameters.parentFieldValues;
                customAction = parameters.customAction;
                context = parameters.context;
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
            }
        }

        function defineScope() {
            $scope.scopeModel.getBEentity = function () {
                $scope.scopeModel.isLoading = true;

                var filter = buildFilter();
                var promiseDeferred = UtilsService.createPromiseDeferred();
                var message;
                VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityInfo(businessEntityDefinitionId, UtilsService.serializetoJson(filter))
                    .then(function (response) {
                        if (response != undefined && response.length != 0) {
                            message = "A customer with same name exist. Do you want to open the existing customer?";
                            var filter = {};
                            if (response.length == 1) {
                                var firstEntityId = response[0].GenericBusinessEntityId;
                                filter.ID = [firstEntityId];
                            }
                            else {
                                message = response.length + " customers with same name exist. Do you want to select from the existing customers?";
                                var ids = [];
                                for (var i = 0; i < response.length; i++) {
                                    var entityId = response[i].GenericBusinessEntityId;
                                    ids.push(entityId);
                                }
                                filter.ID = ids;

                            }
                            context.trigerSearch(filter);

                            VRNotificationService.showConfirmation(message).then(function (confirmed) {
                                $scope.scopeModel.isLoading = false;
                                if (confirmed) {
                                    if (response.length == 1) {
                                        var firstEntityId = response[0].GenericBusinessEntityId;
                                        context.expendRow(firstEntityId);
                                    }
                                    else {
                                        promiseDeferred.resolve();
                                    }
                                    $scope.modalContext.closeModal();
                                }
                                else {
                                    promiseDeferred.resolve();
                                    $scope.modalContext.closeModal();
                                    showAdd();
                                }
                            });
                        }
                        else {
                            $scope.modalContext.closeModal();
                            promiseDeferred.resolve();
                            showAdd();
                        }
                    }).catch(function (error) {
                        $scope.scopeModel.isLoading = false;
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        promiseDeferred.reject();
                    });
                return promiseDeferred.promise;
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function showAdd() {
            var fieldValues = buildFieldValues();
            VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(context.onGenericBEAdded, businessEntityDefinitionId, "large", fieldValues);
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