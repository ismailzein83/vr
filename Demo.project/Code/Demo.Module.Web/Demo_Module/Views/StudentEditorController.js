(function (appControllers) {

    "use strict";
    studentEditorController.$inject = ['$scope', 'Demo_Module_StudentAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function studentEditorController($scope, Demo_Module_StudentAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var studentId;
        var studentEntity;

        var schoolIdItem;
        var schoolDirectiveApi;
        var schoolReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var demoCountrySelectedPromiseDeferred;
        var demoCitySelectedPromiseDeferred;
        var demoCountryIdItem;
        var demoCountryDirectiveApi;
        var demoCountryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var demoCityIdItem;
        var demoCityDirectiveApi;
        var demoCityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var PaymentMethodDirectiveApi;
        var PaymentMethodReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                studentId = parameters.studentId;
                schoolIdItem = parameters.schoolIdItem;
                

            }
            isEditMode = (studentId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.disableSchool = schoolIdItem != undefined;
            $scope.scopeModel.onSchoolDirectiveReady = function (api) {
                schoolDirectiveApi = api;
                schoolReadyPromiseDeferred.resolve();
            };


            $scope.scopeModel.onDemoCountryDirectiveReady = function (api) {
                demoCountryDirectiveApi = api;
                demoCountryReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDemoCityDirectiveReady = function (api) {
                demoCityDirectiveApi = api;
                demoCityReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onPaymentMethodDirectiveReady = function (api) {
                PaymentMethodDirectiveApi = api;
                PaymentMethodReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDemoCountryChanged = function (value) {
                if (demoCountryDirectiveApi != undefined)
                {
                    var data = demoCountryDirectiveApi.getSelectedIds()
                    if (data != undefined) {
                        if (demoCountrySelectedPromiseDeferred != undefined) {
                            demoCountrySelectedPromiseDeferred.resolve();
                        }
                        else {
                            if (schoolDirectiveApi != undefined)
                                schoolDirectiveApi.clear();
                            var demoCityPayload = {};
                            var Filter = {
                                DemoCountryId: demoCountryDirectiveApi.getSelectedIds()
                            };
                            demoCityPayload.filter = Filter;

                            loadDemoCitySelector(demoCityPayload);

                        }
                    }
                }
             
            }


            $scope.scopeModel.onDemoCityChanged = function (value) {
                if (demoCityDirectiveApi != undefined) {
                    var data = demoCityDirectiveApi.getSelectedIds()
                    if (data != undefined) {
                        if (demoCitySelectedPromiseDeferred != undefined) {
                            demoCitySelectedPromiseDeferred.resolve();
                        }
                        else {
                            var schoolPayload = {};
                            var Filter = {
                                DemoCityId: demoCityDirectiveApi.getSelectedIds()
                            };
                            schoolPayload.filter = Filter;

                            loadSchoolSelector(schoolPayload);

                        }
                    }
                }

            }
            $scope.scopeModel.saveStudent = function () {
                if (isEditMode)
                    return updateStudent();
                else
                    return insertStudent();

            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        };

        function loadDemoCountrySelector(demoCountryPayload) {
            var demoCountryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            demoCountryReadyPromiseDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(demoCountryDirectiveApi, demoCountryPayload, demoCountryLoadPromiseDeferred);
            });
            return demoCountryLoadPromiseDeferred.promise;

        }

        function loadDemoCitySelector(demoCityPayload) {

            var promises = [];

            var demoCityLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            if (demoCountrySelectedPromiseDeferred != undefined)

                promises.push(demoCountrySelectedPromiseDeferred.promise);

            promises.push(demoCityReadyPromiseDeferred.promise);

            UtilsService.waitMultiplePromises(promises).then(function (response) {

                VRUIUtilsService.callDirectiveLoad(demoCityDirectiveApi, demoCityPayload, demoCityLoadPromiseDeferred);

                demoCountrySelectedPromiseDeferred = undefined;

            });


            return demoCityLoadPromiseDeferred.promise;

        }


        function loadSchoolSelector(schoolPayload) {

            var promises = [];

            var schoolLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            if (demoCitySelectedPromiseDeferred != undefined)

                promises.push(demoCitySelectedPromiseDeferred.promise);

            promises.push(schoolReadyPromiseDeferred.promise);

            UtilsService.waitMultiplePromises(promises).then(function (response) {

                VRUIUtilsService.callDirectiveLoad(schoolDirectiveApi, schoolPayload, schoolLoadPromiseDeferred);

                demoCitySelectedPromiseDeferred = undefined;

            });


            return schoolLoadPromiseDeferred.promise;

        }


        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getStudent().then(function () {
                    loadAllControls().finally(function () {
                        studentEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        };

        function getStudent() {
            return Demo_Module_StudentAPIService.GetStudentById(studentId).then(function (response) {
                studentEntity = response;
            });
        };

        function loadAllControls() {

            function loadPaymentMethodDirective() {
                var paymentMethodLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                PaymentMethodReadyPromiseDeferred.promise.then(function () {
                    var paymentMethodPayload;
                    if (studentEntity != undefined && studentEntity.Settings != undefined)
                        paymentMethodPayload = {
                            paymentMethodEntity: studentEntity.Settings.PaymentMethod
                        };
                    VRUIUtilsService.callDirectiveLoad(PaymentMethodDirectiveApi, paymentMethodPayload, paymentMethodLoadPromiseDeferred);
                });
                return paymentMethodLoadPromiseDeferred.promise;
            }




            function loadSelectors() {

                if (isEditMode) {
                    var promises = [];
                    demoCountrySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                    var demoCountryPayload = {};
                   

                    if (studentEntity != undefined)
                        demoCountryPayload.selectedIds = studentEntity.DemoCountryId;
                    promises.push(loadDemoCountrySelector(demoCountryPayload));
                    demoCitySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                    var demoCityPayload = {};
                    var Filter= {
                        DemoCountryId: demoCountryPayload.selectedIds
                    };
                    demoCityPayload.filter = Filter;
                    
                    if (studentEntity != undefined)
                        demoCityPayload.selectedIds = studentEntity.DemoCityId;
                    promises.push(loadDemoCitySelector(demoCityPayload));

                    var schoolPayload = {};
                    var schoolFilter = {
                        DemoCityId: demoCityPayload.selectedIds
                    };
                    schoolPayload.filter = schoolFilter;

                    if (studentEntity != undefined)
                        schoolPayload.selectedIds = studentEntity.SchoolId;
                    console.log(schoolPayload)
                    promises.push(loadSchoolSelector(schoolPayload));


                    return UtilsService.waitMultiplePromises(promises);


                }

                else return loadDemoCountrySelector();
            }






            function setTitle() {
                if (isEditMode && studentEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(studentEntity.Name, "Student");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Student");
            };

            function loadStaticData() {
                if (studentEntity != undefined) {
                    $scope.scopeModel.name = studentEntity.Name;
                    $scope.scopeModel.age =studentEntity.Age;
                }

            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPaymentMethodDirective, loadSelectors])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function buildStudentObjectFromScope() {
            var object = {
                StudentId: (studentId != undefined) ? studentId : undefined,
                Name: $scope.scopeModel.name,
                Age: $scope.scopeModel.age,
                SchoolId: schoolDirectiveApi.getSelectedIds(),
                DemoCountryId: demoCountryDirectiveApi.getSelectedIds(),
                DemoCityId: demoCityDirectiveApi.getSelectedIds(),

                Settings: {
                    PaymentMethod: PaymentMethodDirectiveApi.getData()
                }

            };
            return object;
        };

        function insertStudent() {

            $scope.scopeModel.isLoading = true;
            var studentObject = buildStudentObjectFromScope();
            return Demo_Module_StudentAPIService.AddStudent(studentObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Student", response, "Name")) {
                    if ($scope.onStudentAdded != undefined) {
                        $scope.onStudentAdded(response.InsertedObject);
                        $scope.myFunction(response.InsertedObject);

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

        function updateStudent() {
            $scope.scopeModel.isLoading = true;
            var studentObject = buildStudentObjectFromScope();
            Demo_Module_StudentAPIService.UpdateStudent(studentObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Student", response, "Name")) {
                    if ($scope.onStudentUpdated != undefined) {
                        $scope.onStudentUpdated(response.UpdatedObject);
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
    appControllers.controller('Demo_Module_StudentEditorController', studentEditorController);
})(appControllers);