(function (appControllers) {

    "use strict";

    studentEditorController.$inject = ['$scope', 'Demo_Module_StudentAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function studentEditorController($scope, Demo_Module_StudentAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var studentId;
        var studentEntity;

        var roomDirectiveApi;
        var roomReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var buildingDirectiveApi;
        var buildingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var paymentAPI;
        var paymentReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var paymentEntity;
        
        var buildingSelectPromiseDefered;
        

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                studentId = parameters.studentId;
            }
            isEditMode = (studentId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.saveStudent = function () {
                if (isEditMode)
                    return updateStudent();
                else
                    return insertStudent();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.onPaymentReady = function (api) {
                paymentAPI = api;
                paymentReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onRoomDirectiveReady = function (api) {
                roomDirectiveApi = api;
                roomReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onBuildingDirectiveReady = function (api) {
                buildingDirectiveApi = api;
                buildingReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onBuildingSelectionChanged = function (selectedBuilding) {
                if (selectedBuilding != undefined) {
                    if (buildingSelectPromiseDefered != undefined) {
                        buildingSelectPromiseDefered.resolve();
                    }
                    else {
                        var setLoaderRoom = function (value) {
                            $scope.scopeModel.isLoadingRoomSelector = value;
                        };
                        var directivePayload = {
                            filter:
                                {
                                    buildingIds: [selectedBuilding.BuildingId],
                                }
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, roomDirectiveApi, directivePayload, setLoaderRoom);

                    }
                }

            };

        }
        
        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getStudent().then(function () {
                    buildingSelectPromiseDefered = UtilsService.createPromiseDeferred();
                    loadAllControls()
                        .finally(function () {
                            studentEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getStudent() {
            return Demo_Module_StudentAPIService.GetStudentById(studentId).then(function (studentObject) {
                studentEntity = studentObject;
                paymentEntity = studentEntity.Payment;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && studentEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(studentEntity.Name, "Student");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Student");
            }

            function loadStaticData() {
                if (studentEntity != undefined)
                    $scope.scopeModel.name = studentEntity.Name;
            }

            function loadPaymentDirective() {
                var paymentDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                paymentReadyPromiseDeferred.promise.then(function () {
                    var paymentDirectivePayload;
                    if (paymentEntity != undefined) {
                        paymentDirectivePayload = paymentEntity;
                    }
                    VRUIUtilsService.callDirectiveLoad(paymentAPI, paymentDirectivePayload, paymentDeferredLoadPromiseDeferred);

                });
                return paymentDeferredLoadPromiseDeferred.promise;;

            }

            function loadRoomSelector() {
                if (studentEntity != undefined) {
                    var promises = [];
                    var roomLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(roomReadyPromiseDeferred.promise);
                    promises.push(buildingSelectPromiseDefered.promise);
                    setTimeout(function () {
                        UtilsService.waitMultiplePromises(promises).then(function () {
                            buildingSelectPromiseDefered = undefined;
                            var directivePayload = {
                                selectedIds: studentEntity.RoomId,
                                filter: {
                                    BuildingIds: [studentEntity.BuildingId],
                                }
                            };

                            VRUIUtilsService.callDirectiveLoad(roomDirectiveApi, directivePayload, roomLoadPromiseDeferred);
                        });
                    }, 1000);
                    
                    return roomLoadPromiseDeferred.promise;
                }
            }

            function loadBuildingSelector() {
                var buildingLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                buildingReadyPromiseDeferred.promise.then(function () {
                    var directivePayload = {
                        selectedIds: studentEntity != undefined ? studentEntity.BuildingId : undefined,
                    };
                    VRUIUtilsService.callDirectiveLoad(buildingDirectiveApi, directivePayload, buildingLoadPromiseDeferred);
                });

                return buildingLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPaymentDirective, loadBuildingSelector, loadRoomSelector])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
              });
        }
      
        function buildStudentObjFromScope() {
            var obj = {
                StudentId: (studentId != null) ? studentId : 0,
                Name: $scope.scopeModel.name,
                Payment: paymentAPI.getData(),
                RoomId: roomDirectiveApi.getSelectedIds(),
                BuildingId: buildingDirectiveApi.getSelectedIds()
                
            };
            return obj;
        }

        function insertStudent() {
            $scope.scopeModel.isLoading = true;

            var studentObject = buildStudentObjFromScope();
            return Demo_Module_StudentAPIService.AddStudent(studentObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Student", response, "Name")) {
                    if ($scope.onStudentAdded != undefined) {

                        $scope.onStudentAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }

        function updateStudent() {
            $scope.scopeModel.isLoading = true;

            var studentObject = buildStudentObjFromScope();
            Demo_Module_StudentAPIService.UpdateStudent(studentObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Student", response, "Name")) {
                    if ($scope.onStudentUpdated != undefined)
                        $scope.onStudentUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

    }

    appControllers.controller('Demo_Module_StudentEditorController', studentEditorController);
})(appControllers);
