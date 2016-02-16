(function (appControllers) {

    "use strict";

    operatorAccountEditorController.$inject = ['$scope', 'Demo_OperatorAccountAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'Demo_OperatorAccountTypeEnum', 'VRUIUtilsService', 'Demo_OperatorAccountActivationStatusEnum'];

    function operatorAccountEditorController($scope, Demo_OperatorAccountAPIService, UtilsService, VRNotificationService, VRNavigationService, Demo_OperatorAccountTypeEnum, VRUIUtilsService, Demo_OperatorAccountActivationStatusEnum) {
        var operatorProfileDirectiveAPI;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;

        var isEditMode;
        $scope.scopeModal = {};

        var operatorAccountId;
        var operatorProfileId;
        var operatorAccountEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                operatorAccountId = parameters.OperatorAccountId;
                operatorProfileId = parameters.OperatorProfileId
            }
            isEditMode = (operatorAccountId != undefined);
            $scope.scopeModal.disableOperatorProfile = ((operatorProfileId != undefined));
        }

        function defineScope() {
            $scope.scopeModal.SaveOperatorAccount = function () {
                if (isEditMode) {
                    return updateOperatorAccount();
                } else {
                    return insertOperatorAccount();
                }
            };

            $scope.scopeModal.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
            }

            $scope.scopeModal.onOperatorProfileDirectiveReady = function (api) {
                operatorProfileDirectiveAPI = api;
                operatorProfileReadyPromiseDeferred.resolve();

            }

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getOperatorAccount()
                    .then(function () {
                        loadAllControls()
                            .finally(function () {
                                operatorAccountEntity = undefined;
                            });
                    })
                    .catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModal.isLoading = false;
                    });
            } else {
                loadAllControls();
            }

        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadFilterBySection, loadOperatorProfileDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(operatorAccountEntity ? operatorAccountEntity.NameSuffix : null, 'Operator Account') : UtilsService.buildTitleForAddEditor('Operator Account');
        }

        function loadOperatorProfileDirective() {

            var loadOperatorProfilePromiseDeferred = UtilsService.createPromiseDeferred();

            operatorProfileReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: (operatorAccountEntity != undefined ? operatorAccountEntity.OperatorProfileId : (operatorProfileId != undefined ? operatorProfileId : undefined))
                    }
                    VRUIUtilsService.callDirectiveLoad(operatorProfileDirectiveAPI, directivePayload, loadOperatorProfilePromiseDeferred);
                });

            return loadOperatorProfilePromiseDeferred.promise;
        }

        function getOperatorAccount() {
            return Demo_OperatorAccountAPIService.GetOperatorAccount(operatorAccountId)
                .then(function (operatorAccount) {
                    operatorAccountEntity = operatorAccount;
                });
        }

        function buildOperatorAccountObjFromScope() {
            var obj = {
                OperatorAccountId: (operatorAccountId != null) ? operatorAccountId : 0,
                NameSuffix: $scope.scopeModal.name,
                OperatorProfileId: operatorProfileDirectiveAPI.getSelectedIds(),
                SupplierSettings: {},
                CustomerSettings: {},
            };
            return obj;
        }

        function loadFilterBySection() {
            if (operatorAccountEntity != undefined) {
                $scope.scopeModal.name = operatorAccountEntity.NameSuffix;
            }
        }


        function insertOperatorAccount() {
            $scope.scopeModal.isLoading = true;

            var operatorAccountObject = buildOperatorAccountObjFromScope();
            return Demo_OperatorAccountAPIService.AddOperatorAccount(operatorAccountObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Operator Account", response, "Name")) {
                        if ($scope.onOperatorAccountAdded != undefined)
                            $scope.onOperatorAccountAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModal.isLoading = false;
                });

        }

        function updateOperatorAccount() {
            $scope.scopeModal.isLoading = true;

            var operatorAccountObject = buildOperatorAccountObjFromScope();
            Demo_OperatorAccountAPIService.UpdateOperatorAccount(operatorAccountObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Operator Account", response, "Name")) {
                        if ($scope.onOperatorAccountUpdated != undefined)
                            $scope.onOperatorAccountUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }
    }

    appControllers.controller('Demo_OperatorAccountEditorController', operatorAccountEditorController);
})(appControllers);