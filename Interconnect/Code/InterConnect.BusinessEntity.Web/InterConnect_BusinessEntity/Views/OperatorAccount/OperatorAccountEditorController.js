(function (appControllers) {

    "use strict";

    operatorAccountEditorController.$inject = ['$scope', 'InterConnect_BE_OperatorAccountAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function operatorAccountEditorController($scope, InterConnect_BE_OperatorAccountAPIService, UtilsService, VRNotificationService, VRNavigationService) {
        var isEditMode;
        var operatorAccountId;
        //var operatorAccountEntity;

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
                    loadAllControls()
                        .finally(function () {
                            //operatorAccountEntity = undefined;
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
                console.log(operatorAccount);
                $scope.scopeModal = operatorAccount;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle])
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

        function buildOperatorAccountObjFromScope() {

            var obj = {
                OperatorAccountId: (operatorAccountId != null) ? operatorAccountId : 0,
                Suffix: $scope.scopeModal.Suffix
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
