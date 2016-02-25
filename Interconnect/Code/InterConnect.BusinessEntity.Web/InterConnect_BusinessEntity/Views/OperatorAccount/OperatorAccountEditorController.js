(function (appControllers) {

    "use strict";

    operatorAccountEditorController.$inject = ['$scope', 'InterConnect_BE_OperatorAccountAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function operatorAccountEditorController($scope, InterConnect_BE_OperatorAccountAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {
        var isEditMode;
        var operatorAccountObj;
        var operatorAccountId;
        var operatorProfileDirectiveApi;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        $scope.scopeModel = {};

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                operatorAccountId = parameters.OperatorAccountId;
            }
            isEditMode = (operatorAccountId != undefined);

        }

        function defineScope() {

            $scope.onOperatorProfileDirectiveReady = function (api) {
                operatorProfileDirectiveApi = api;
                operatorProfileReadyPromiseDeferred.resolve();
            }

            $scope.saveOperatorAccount = function () {
                if (isEditMode) {
                    return updateOperatorAccount();
                }
                else {
                    return insertOperatorAccount();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getOperatorAccount().then(function () {
                    loadAllControls().finally(function () {
                        operatorAccountObj = undefined;
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

        function getOperatorAccount() {
            return InterConnect_BE_OperatorAccountAPIService.GetOperatorAccount(operatorAccountId).then(function (operatorAccount) {
                operatorAccountObj = operatorAccount;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadOperatorProfiles, loadStaticSection])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = 'Operator Account';
        }

        function loadOperatorProfiles() {
            var payload = {
                selectedIds: operatorAccountObj != undefined ? operatorAccountObj.ProfileId : undefined
            };
            var loadOperatorProfilePromiseDeferred = UtilsService.createPromiseDeferred();
            operatorProfileReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(operatorProfileDirectiveApi, payload, loadOperatorProfilePromiseDeferred);
            });

            return loadOperatorProfilePromiseDeferred.promise;
        }

        function loadStaticSection() {
            if (operatorAccountObj != undefined) {
                $scope.scopeModel.suffix = operatorAccountObj.Suffix;
            }
        }

        function buildOperatorAccountObjFromScope() {

            var obj = {
                OperatorAccountId: (operatorAccountId != null) ? operatorAccountId : 0,
                Suffix: $scope.scopeModel.suffix,
                ProfileId: $scope.operatorProfile.OperatorProfileId
            };
            return obj;
        }

        function insertOperatorAccount() {
            $scope.isLoading = true;

            var operatorAccountObject = buildOperatorAccountObjFromScope();

            return InterConnect_BE_OperatorAccountAPIService.AddOperatorAccount(operatorAccountObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Operator Account", response, "Suffix")) {
                    if ($scope.onOperatorAccountAdded != undefined)
                        $scope.onOperatorAccountAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateOperatorAccount() {
            $scope.isLoading = true;

            var operatorAccountObject = buildOperatorAccountObjFromScope();

            InterConnect_BE_OperatorAccountAPIService.UpdateOperatorAccount(operatorAccountObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Operator Account", response, "Suffix")) {
                    if ($scope.onOperatorAccountUpdated != undefined)
                        $scope.onOperatorAccountUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('InterConnect_BE_OperatorAccountEditorController', operatorAccountEditorController);
})(appControllers);
