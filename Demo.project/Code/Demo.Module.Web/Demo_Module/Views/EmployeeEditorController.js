(function (appControllers) {
"use strict";
employeeEditorController.$inject = ['$scope', 'Demo_Module_EmployeeAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_EmployeeService'];

function employeeEditorController($scope, Demo_Module_EmployeeAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Demo_Module_EmployeeService) {

    var isEditMode;
    var employeeId;
    var employeeEntity;
    var desksizeSelectedPromiseDeferred;
    var specialityDirectiveApi;
    var specialityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var desksizeDirectiveApi;
    var desksizeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var colorDirectiveApi;
    var colorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var workDirectiveApi;
    var workReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    $scope.scopeModel = {};
    $scope.scopeModel.contacts = [];

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != undefined && parameters != null) {
            employeeId = parameters.employeeId;
        }
        isEditMode = (employeeId != undefined);
    };

    function defineScope() {

        $scope.scopeModel.onSpecialityDirectiveReady = function (api) {
            specialityDirectiveApi = api;
            specialityReadyPromiseDeferred.resolve();

        }

        $scope.scopeModel.onDesksizeDirectiveReady = function (api) {
            desksizeDirectiveApi = api;
            desksizeReadyPromiseDeferred.resolve();

        }

        $scope.scopeModel.onColorDirectiveReady = function (api) {
            colorDirectiveApi = api;
            colorReadyPromiseDeferred.resolve();

        }

        $scope.scopeModel.onWorkDirectiveReady = function (api) {
            workDirectiveApi = api;
            workReadyPromiseDeferred.resolve();

        }

        $scope.scopeModel.onDesksizeChanged = function () {

            if (desksizeDirectiveApi != undefined) {
                var data = desksizeDirectiveApi.getSelectedIds();
                if (data != undefined) {

                    if (desksizeSelectedPromiseDeferred != undefined) {

                        desksizeSelectedPromiseDeferred.resolve();
                    }
                    else {

                        var colorPayload = {};

                        colorPayload = {
                            filter: {
                                DesksizeId: desksizeDirectiveApi.getSelectedIds()
                            }
                        };
                        loadColorDirective(colorPayload);
                    }
                }
            }
        }

        $scope.scopeModel.saveEmployee = function () {
            if (isEditMode)
                return updateEmployee();
            else
                return insertEmployee();

        };

        $scope.scopeModel.close = function () {
            $scope.modalContext.closeModal();
        };

        $scope.scopeModel.onGridReady = function () {
        }

        $scope.scopeModel.gridMenuActions = [{
            name: "Edit",
            clicked: editEmployeeContact,

        }];

        $scope.scopeModel.onEmployeeContactAdded = function () {
            var onEmployeeContactAdded = function (contact) {
                $scope.scopeModel.contacts.push(contact);
            };
            Demo_Module_EmployeeService.addEmployeeContact(onEmployeeContactAdded);

        };

        function editEmployeeContact(employeeContact) {

            var employeeContactIndex = $scope.scopeModel.contacts.indexOf(employeeContact);
            employeeContact.index = employeeContactIndex;

            var onEmployeeContactUpdated = function (contact) {

                var index = employeeContactIndex;
                $scope.scopeModel.contacts[index] = contact;
            };
            Demo_Module_EmployeeService.editEmployeeContact(onEmployeeContactUpdated, employeeContact);

        }

    }

    function load() {
        $scope.scopeModel.isLoading = true;
        if (isEditMode) {
            getEmployee().then(function () {
                loadAllControls().finally(function () {
                    employeeEntity = undefined;
                });
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }
        else
            loadAllControls();
    };

    function getEmployee() {
        return Demo_Module_EmployeeAPIService.GetEmployeeById(employeeId).then(function (response) {
            employeeEntity = response;
        });
    };
  
    function loadDesksizeDirective(desksizePayload) {

        var desksizeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        desksizeReadyPromiseDeferred.promise.then(function (response) {

            VRUIUtilsService.callDirectiveLoad(desksizeDirectiveApi, desksizePayload, desksizeLoadPromiseDeferred);
        });
        return desksizeLoadPromiseDeferred.promise;
    }

    function loadColorDirective(colorPayload) {
        var promises = [];
        var colorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        if (desksizeSelectedPromiseDeferred != undefined)

            promises.push(desksizeSelectedPromiseDeferred.promise);

        promises.push(desksizeReadyPromiseDeferred.promise);

        UtilsService.waitMultiplePromises(promises).then(function (response) {

            VRUIUtilsService.callDirectiveLoad(colorDirectiveApi, colorPayload, colorLoadPromiseDeferred);

            desksizeSelectedPromiseDeferred = undefined;
        });
        return colorLoadPromiseDeferred.promise;

    }

    function loadAllControls() {

        function loadSpecialityDirective() {
            var specialityLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            specialityReadyPromiseDeferred.promise.then(function (response) {

                var specialityPayload = {};

                if (employeeEntity != undefined && employeeEntity.SpecialityId != 0)
                    specialityPayload = {

                        selectedIds: employeeEntity.SpecialityId
                    }

                VRUIUtilsService.callDirectiveLoad(specialityDirectiveApi, specialityPayload, specialityLoadPromiseDeferred);
            });
            return specialityLoadPromiseDeferred.promise;
        }

        function loadWorkDirective() {
            var workLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            workReadyPromiseDeferred.promise.then(function (response) {

                var workPayload;
                if (employeeEntity != undefined && employeeEntity.Settings != undefined)

                    workPayload = {
                        workEntity: employeeEntity.Settings.Work
                    }

                VRUIUtilsService.callDirectiveLoad(workDirectiveApi, workPayload, workLoadPromiseDeferred);

            });
            return workLoadPromiseDeferred.promise;
        }


        function loadDesksizeThenColorDirectives() {

            if (isEditMode) {

                var promises = [];
                desksizeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                var desksizePayoad = {};
                if (employeeEntity != undefined && employeeEntity.DesksizeId != 0)
                    desksizePayoad = {
                        selectedIds: employeeEntity.DesksizeId
                    }
                promises.push(loadDesksizeDirective(desksizePayoad));
                var colorPayload = {};
                if (employeeEntity != undefined && employeeEntity.ColorId != 0)
                    colorPayload = {

                        selectedIds: employeeEntity.ColorId,
                        filter: {
                            DesksizeId: desksizePayoad.selectedIds
                        }
                    };
                promises.push(loadColorDirective(colorPayload));
                return UtilsService.waitMultiplePromises(promises);

            }

            else return loadDesksizeDirective();

        }

        function setTitle() {
            if (isEditMode && employeeEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(employeeEntity.Name, "Employee");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Employee");
        };

        function loadStaticData() {
            if (employeeEntity != undefined) {
                $scope.scopeModel.name = employeeEntity.Name;
                var j = 0;
                if (employeeEntity.Settings.Contacts != null) {
                    for (j = 0; j < employeeEntity.Settings.Contacts.length; j++) {
                        var element = {};
                        element.Email = employeeEntity.Settings.Contacts[j].Email;
                        element.PhoneNumber = employeeEntity.Settings.Contacts[j].PhoneNumber;
                        $scope.scopeModel.contacts.push(element);

                    }
                }
            }

        };

        return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadWorkDirective, loadSpecialityDirective, loadDesksizeThenColorDirectives])
         .catch(function (error) {
             VRNotificationService.notifyExceptionWithClose(error, $scope);
         })
           .finally(function () {
               $scope.scopeModel.isLoading = false;
           });
    }

    function buildEmployeeObjectFromScope() {
        var object = {
            EmployeeId: (employeeId != undefined) ? employeeId : undefined,
            Name: $scope.scopeModel.name,
            SpecialityId: specialityDirectiveApi.getSelectedIds(),
            DesksizeId: desksizeDirectiveApi.getSelectedIds(),
            ColorId: colorDirectiveApi.getSelectedIds(),
            Settings: {
                Work: workDirectiveApi.getData()
            },
        };
        object.Settings.Contacts = [];
        var i; 
        for (i = 0; i < $scope.scopeModel.contacts.length; i++)
        {
            var contact = {Email:$scope.scopeModel.contacts[i].Email,PhoneNumber:$scope.scopeModel.contacts[i].PhoneNumber}
            object.Settings.Contacts.push(contact);
        } 
        return object;
    };

    function insertEmployee() {

        $scope.scopeModel.isLoading = true;
        var employeeObject = buildEmployeeObjectFromScope();
        return Demo_Module_EmployeeAPIService.AddEmployee(employeeObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Employee", response, "Name")) {
                if ($scope.onEmployeeAdded != undefined) {
                    $scope.onEmployeeAdded(response.InsertedObject);
                } 
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            $scope.scopeModel.isLoading = false;
            VRNotificationService.notifyException(error, $scope);
        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        });

    };

    function updateEmployee() {
        $scope.scopeModel.isLoading = true;
        var employeeObject = buildEmployeeObjectFromScope();
        Demo_Module_EmployeeAPIService.UpdateEmployee(employeeObject).then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Employee", response, "Name")) {
                if ($scope.onEmployeeUpdated != undefined) {
                    $scope.onEmployeeUpdated(response.UpdatedObject);
                } 
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            $scope.scopeModel.isLoading = false;
            VRNotificationService.notifyException(error, $scope);
        }).finally(function () {
            $scope.scopeModel.isLoading = false;

        });
    };



};
appControllers.controller('Demo_Module_EmployeeEditorController', employeeEditorController);
})(appControllers);