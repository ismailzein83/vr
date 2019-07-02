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
                // dataRecordTypeId = parameters.dataRecordTypeId;
                parentFieldValues = parameters.parentFieldValues;
                customAction = parameters.customAction;
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
            }
        }

        function defineScope() {
            $scope.scopeModel.hasBEentity = function () {
                $scope.scopeModel.isLoading = true;
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
                VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityInfo(businessEntityDefinitionId, UtilsService.serializetoJson(filter))
                    .then(function (response) {
                        if (response != undefined && response.length != 0) {
                            console.log(response);
                        }
                        else {
                            var onGenericBusinessEntityAdded = function () { };
                            VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(onGenericBusinessEntityAdded, businessEntityDefinitionId, fieldValues);
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                        $scope.modalContext.closeModal();
                    });
                //VR_GenericData_GenericBusinessEntityAPIService.GetGenericBEEntityId(buildEntityScope()).then(function (response) {
                //    if (response == undefined) {
                //        var fieldValues =
                //        {
                //            FirstName: $scope.firstName,
                //            middleName: $scope.middleName,
                //            lastName: $scope.lastName
                //        };
                //        var onGenericBusinessEntityAdded = function () { };
                //        VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(onGenericBusinessEntityAdded, businessEntityDefinitionId, fieldValues);
                //    }
                //}).catch(function (error) {
                //    VRNotificationService.notifyExceptionWithClose(error, $scope);
                //}).finally(function () {
                //    $scope.scopeModel.isLoading = false;
                //    $scope.modalContext.closeModal();
                //});
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {
            $scope.title = "Add";
        }

        function buildEntityScope() {
            return {
                BusinessEntityDefinitionId: businessEntityDefinitionId,
                FirstName: $scope.firstName,
                middleName: $scope.middleName,
                lastName: $scope.lastName
            };
        }
    }
    appControllers.controller('VR_GenericData_NewOrExistingRuntimeEditorController', NewOrExistingRuntimEditorController);
})(appControllers);