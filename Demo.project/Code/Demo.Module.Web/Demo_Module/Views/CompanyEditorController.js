(function (appControllers) {

    "use strict";
    companyEditorController.$inject = ['$scope', 'Demo_Module_CompanyAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function companyEditorController($scope, Demo_Module_CompanyAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
        
        var isEditMode;
        var companyId;
        var companyEntity;
        

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                companyId = parameters.companyId;
            }
            isEditMode = (companyId != undefined);

        };

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.saveCompany = function () {
                if (isEditMode)
                    return updateCompany();
                else
                    return insertCompany();
                
            };

            $scope.scopeModel.close = function () {

                $scope.modalContext.closeModal();
            };
        };

        function buildCompanyObjectFromScope() {

            var object = {
                CompanyId: (companyId != null) ? companyId : 0,
                Name: $scope.scopeModel.name,

            };
            return object;
        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getCompany().then(function () {
                    loadAllControls().finally(function () {
                        companyEntity = undefined;
                    });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else
                loadAllControls();
        };
        function getCompany() {
            return Demo_Module_CompanyAPIService.GetCompanyById(companyId).then(function (companyObject) {
                companyEntity = companyObject;
            });
        };

        function loadAllControls() {
            function setTitle() {
                if (isEditMode && companyEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(companyEntity.Name, "Company");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Company");
            };
            function loadStaticData() {
                if (companyEntity != undefined)
                    $scope.scopeModel.name = companyEntity.Name;
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function insertCompany() {

            $scope.scopeModel.isLoading = true;
            var companyObject = buildCompanyObjectFromScope();

            return Demo_Module_CompanyAPIService.AddCompany(companyObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Company", response, "Name")) {
                    if ($scope.onCompanyAdded != undefined) {

                        $scope.onCompanyAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        };

        function updateCompany() {
            $scope.scopeModel.isLoading = true;
            var companyObject = buildCompanyObjectFromScope();
            Demo_Module_CompanyAPIService.UpdateCompany(companyObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Company", response, "Name")) {
                    if ($scope.onCompanyUpdated != undefined) {

                        $scope.onCompanyUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
                             
            });
        };
};
appControllers.controller('Demo_Module_CompanyEditorController', companyEditorController);
})(appControllers);