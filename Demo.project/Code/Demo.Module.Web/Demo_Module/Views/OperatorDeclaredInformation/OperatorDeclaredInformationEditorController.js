(function (appControllers) {

    "use strict";

    operatordeclaredinformationEditorController.$inject = ['$scope', 'Demo_OperatorDeclaredInformationAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VRValidationService'];

    function operatordeclaredinformationEditorController($scope, Demo_OperatorDeclaredInformationAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRValidationService) {
        var isEditMode;
        var operatordeclaredinformationId;
        var operatordeclaredinformationEntity;

        var operatorProfileDirectiveAPI;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var zoneDirectiveAPI;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var serviceTypeDirectiveAPI;
        var serviceTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            };

            $scope.validateDateRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }

            $scope.scopeModal.onOperatorProfileDirectiveReady = function (api) {
                operatorProfileDirectiveAPI = api;
                operatorProfileReadyPromiseDeferred.resolve();

            }

            $scope.onServiceTypeReady = function (api) {
                serviceTypeDirectiveAPI = api;
                serviceTypeReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.onZoneDirectiveReady = function (api) {
                zoneDirectiveAPI = api;
                zoneReadyPromiseDeferred.resolve();

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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadOperatorProfileDirective, loadZoneDirective, loadServiceTypes])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function loadServiceTypes() {
            var loadServiceTypesPromiseDeferred = UtilsService.createPromiseDeferred();
            serviceTypeReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: (operatordeclaredinformationEntity != undefined ? (operatordeclaredinformationEntity.AmountType != undefined ? [operatordeclaredinformationEntity.AmountType] : undefined) : (operatordeclaredinformationId != undefined ? operatordeclaredinformationId : undefined))
                }
                VRUIUtilsService.callDirectiveLoad(serviceTypeDirectiveAPI, directivePayload, loadServiceTypesPromiseDeferred);
            });
            return loadServiceTypesPromiseDeferred.promise;
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

        function loadZoneDirective() {
            var loadZonePromiseDeferred = UtilsService.createPromiseDeferred();
            zoneReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: (operatordeclaredinformationEntity != undefined ? (operatordeclaredinformationEntity.ZoneId != undefined ? [operatordeclaredinformationEntity.ZoneId] : undefined) : (operatordeclaredinformationId != undefined ? operatordeclaredinformationId : undefined))
                    }
                    VRUIUtilsService.callDirectiveLoad(zoneDirectiveAPI, directivePayload, loadZonePromiseDeferred);
                });
            return loadZonePromiseDeferred.promise;
        }

        function loadStaticSection() {
            if (operatordeclaredinformationEntity != undefined) {
                $scope.scopeModal.fromDate = operatordeclaredinformationEntity.FromDate;
                $scope.scopeModal.toDate = operatordeclaredinformationEntity.ToDate;
                $scope.scopeModal.volume = operatordeclaredinformationEntity.Volume;
            }
        }

        function buildOperatorDeclaredInformationObjFromScope() {

            var obj = {
                OperatorDeclaredInformationId: (operatordeclaredinformationId != null) ? operatordeclaredinformationId : 0,
                FromDate: $scope.scopeModal.fromDate,
                ToDate: $scope.scopeModal.toDate,
                OperatorId: operatorProfileDirectiveAPI.getSelectedIds(),
                ZoneId: zoneDirectiveAPI.getSelectedIds(),
                Volume: $scope.scopeModal.volume,
                AmountType: serviceTypeDirectiveAPI.getSelectedIds()
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
