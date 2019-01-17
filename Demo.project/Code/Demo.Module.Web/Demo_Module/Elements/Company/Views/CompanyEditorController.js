(function (appControllers) {

    "use strict";

    companyEditorController.$inject = ['$scope', 'Demo_Module_CompanyAPIService', 'VRNavigationService', 'UtilsService', 'VRNotificationService'];

    function companyEditorController($scope, Demo_Module_CompanyAPIService, VRNavigationService, UtilsService, VRNotificationService) {

        var isEditMode;
        var companyId;
        var companyEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() { 
            var parameters = VRNavigationService.getParameters($scope); //get the parameters from the modal

            if (parameters != undefined && parameters != null) { // if edit mode: companyId is not undefined and in add mode it is undefined
                companyId = parameters.companyId;
            }

            isEditMode = (companyId != undefined); 
        };

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.saveCompany = function () {
                if (isEditMode)
                    return updateCompany(); // return a promise
                else
                    return insertCompany(); // return a promise
            };

            $scope.scopeModel.close = function () { 
                $scope.modalContext.closeModal();
            };
        };

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getCompany().then(function () { // get the entity of the edited company 
                    loadAllControls().finally(function () { // then load the controllers to put values inside them
                        companyEntity = undefined; //??????
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            } else { // add mode
                loadAllControls();
            }
        };

        function getCompany() { // return a promise 
            return Demo_Module_CompanyAPIService.GetCompanyById(companyId).then(function (response) { // pass the id to the api service and put the response in the companyEntity
                companyEntity = response;
            });
        };

        function loadAllControls() {

            function setTitle() { // set the title if is edit or add
                if (isEditMode && companyEntity != undefined) {
                    $scope.title = UtilsService.buildTitleForUpdateEditor(companyEntity.Name, "Company"); // put the name of the edited company in the title
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor("Company"); // put in the title: "New Company"
                }
            };

            function loadStaticData() {
                if (companyEntity != undefined) { // if their exist a company whith this id --> put the values inside the textboxes
                    $scope.scopeModel.name = companyEntity.Name;
                    $scope.scopeModel.settings = companyEntity.Settings.HeadquarterAddress;
                }

            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) { // wait to load the pevious functions in case of failure popup a notification
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };

        function insertCompany() {
            $scope.scopeModel.isLoading = true;

            var companyObject = buildCompanyObjectFromScope(); //read the textboxes
            return Demo_Module_CompanyAPIService.AddCompany(companyObject).then(function (response) { // use the api service and add the new company
                if (VRNotificationService.notifyOnItemAdded("Company", response, "Name")) {  // return true/false
                    if ($scope.onCompanyAdded != undefined) { // go to management and call the onCompanyAdded
                        $scope.onCompanyAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) { // in case of insert has been failed
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };

        function updateCompany() {
            $scope.scopeModel.isLoading = true;

            var companyObject = buildCompanyObjectFromScope();
            return Demo_Module_CompanyAPIService.UpdateCompany(companyObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Company", response, "Name")) { // return true/false
                    if ($scope.onCompanyUpdated != undefined) {
                        $scope.onCompanyUpdated(response.UpdatedObject); // go to grid and call the onCompanyUpdated
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) { // in case of update has been failed
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };

        function buildCompanyObjectFromScope() {
            var object = {
                CompanyId: (companyId != undefined) ? companyId : undefined,
                Name: $scope.scopeModel.name,
                Settings: {
                    HeadquarterAddress: $scope.scopeModel.settings
                }
            };
            return object;
        };
    }

    appControllers.controller("Demo_Module_CompanyEditorController", companyEditorController);
})(appControllers);