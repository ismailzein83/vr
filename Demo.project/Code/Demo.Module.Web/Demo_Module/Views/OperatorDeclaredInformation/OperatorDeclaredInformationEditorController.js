(function (appControllers) {

    "use strict";

    operatordeclaredinformationEditorController.$inject = ['$scope', 'Demo_OperatorDeclaredInformationAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VRValidationService'];

    function operatordeclaredinformationEditorController($scope, Demo_OperatorDeclaredInformationAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRValidationService) {
        var isEditMode;
        var operatordeclaredinformationId;
        var operatordeclaredinformationEntity;

        var operatorProfileDirectiveAPI;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                operatordeclaredinformationId = parameters.OperatorDeclaredInformationId;
            }
            isEditMode = (operatordeclaredinformationId != undefined);

        }

        function defineScope() {

            $scope.scopeModal = {
                range: []
            };

            $scope.validateDateRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }

            $scope.scopeModal.onOperatorProfileDirectiveReady = function (api) {
                operatorProfileDirectiveAPI = api;
                operatorProfileReadyPromiseDeferred.resolve();

            }
            $scope.scopeModal.disabledrange = true;
            $scope.scopeModal.onRangeValueChange = function (value) {
                $scope.scopeModal.disabledrange = (contains($scope.scopeModal.range, value) || value == undefined);
            }


            $scope.SaveOperatorDeclaredInformation = function () {
                if (isEditMode) {
                    return updateOperatorDeclaredInformation();
                }
                else {
                    return insertOperatorDeclaredInformation();
                }
            };
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.addRangeOption = function () {
                var range = $scope.scopeModal.rangevalue;
                $scope.scopeModal.range.push({
                    range: range
                });
                $scope.scopeModal.rangevalue = undefined;
                $scope.scopeModal.disabledrange = true;
            };

        }


        function contains(a, obj) {
            for (var i = 0; i < a.length; i++) {
                if (a[i].range === obj) {
                    return true;
                }
            }
            return false;
        }


        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getOperatorDeclaredInformation().then(function () {
                    loadAllControls()
                        .finally(function () {
                            operatordeclaredinformationEntity = undefined;
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

        function getOperatorDeclaredInformation() {
            return Demo_OperatorDeclaredInformationAPIService.GetOperatorDeclaredInformation(operatordeclaredinformationId).then(function (operatordeclaredinformation) {
                operatordeclaredinformationEntity = operatordeclaredinformation;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadOperatorProfileDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(operatordeclaredinformationEntity ? '' : null, 'Operator Declared Information') : UtilsService.buildTitleForAddEditor('Operator Declared Information');
        }

        function loadOperatorProfileDirective() {

            var loadOperatorProfilePromiseDeferred = UtilsService.createPromiseDeferred();

            operatorProfileReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: (operatordeclaredinformationEntity != undefined ? operatordeclaredinformationEntity.OperatorId : (operatordeclaredinformationId != undefined ? operatordeclaredinformationId : undefined))
                    }
                    VRUIUtilsService.callDirectiveLoad(operatorProfileDirectiveAPI, directivePayload, loadOperatorProfilePromiseDeferred);
                });

            return loadOperatorProfilePromiseDeferred.promise;
        }

        function loadStaticSection() {
            if (operatordeclaredinformationEntity != undefined) {
                $scope.scopeModal.fromDate = operatordeclaredinformationEntity.FromDate;
                $scope.scopeModal.toDate = operatordeclaredinformationEntity.ToDate;

                if (operatordeclaredinformationEntity.Settings != null) {
                    $scope.scopeModal.range = [];
                    if (operatordeclaredinformationEntity.Settings.Range == undefined)
                        operatordeclaredinformationEntity.Settings.Range = [];
                    for (var j = 0; j < operatordeclaredinformationEntity.Settings.Range.length; j++) {
                        $scope.scopeModal.range.push({
                            range: operatordeclaredinformationEntity.Settings.Range[j]
                        });
                    }
                }
            }
        }

        function buildOperatorDeclaredInformationObjFromScope() {

            var obj = {
                OperatorDeclaredInformationId: (operatordeclaredinformationId != null) ? operatordeclaredinformationId : 0,
                FromDate: $scope.scopeModal.fromDate,
                ToDate: $scope.scopeModal.toDate,
                OperatorId: operatorProfileDirectiveAPI.getSelectedIds(),
                Settings: {
                    Range: UtilsService.getPropValuesFromArray($scope.scopeModal.range, "range")
                }
            };

            return obj;
        }

        function insertOperatorDeclaredInformation() {
            $scope.isLoading = true;

            var operatordeclaredinformationObject = buildOperatorDeclaredInformationObjFromScope();

            return Demo_OperatorDeclaredInformationAPIService.AddOperatorDeclaredInformation(operatordeclaredinformationObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Operator Declared Information", response, undefined)) {
                    if ($scope.onOperatorDeclaredInformationAdded != undefined)
                        $scope.onOperatorDeclaredInformationAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateOperatorDeclaredInformation() {
            $scope.isLoading = true;

            var operatordeclaredinformationObject = buildOperatorDeclaredInformationObjFromScope();

            Demo_OperatorDeclaredInformationAPIService.UpdateOperatorDeclaredInformation(operatordeclaredinformationObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Operator Declared Information", response, undefined)) {
                    if ($scope.onOperatorDeclaredInformationUpdated != undefined)
                        $scope.onOperatorDeclaredInformationUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('Demo_OperatorDeclaredInformationEditorController', operatordeclaredinformationEditorController);
})(appControllers);
