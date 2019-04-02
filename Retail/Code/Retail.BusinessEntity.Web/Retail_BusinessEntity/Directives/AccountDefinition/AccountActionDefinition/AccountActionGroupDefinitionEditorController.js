(function (appControllers) {

    "use strict";

    AccountActionGroupDefintionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function AccountActionGroupDefintionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var accountActionGroupDefinition;
        var context;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                accountActionGroupDefinition = parameters.accountActionGroupDefinition;
            }
            isEditMode = (accountActionGroupDefinition != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }
        function load() {

            loadAllControls();

            function loadAllControls() {


                var initialPromises = [];

                $scope.scopeModel.isLoading = true;
                function setTitle() {
                    if (isEditMode && accountActionGroupDefinition != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(accountActionGroupDefinition.Title, 'Action Group');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Action Group');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.title = accountActionGroupDefinition.Title;
                }


                var rootPromiseNode = {
                    promises: initialPromises,
                    getChildNode: function () {
                        var directivePromises = [];
                        directivePromises.push(UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]));

                        return {
                            promises: directivePromises
                        };
                    }
                };

                return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildActionGroupDefinitionFromScope() {
            return {
                AccountActionGroupId: accountActionGroupDefinition != undefined ? accountActionGroupDefinition.AccountActionGroupId : UtilsService.guid(),
                Title: $scope.scopeModel.title,
            };
        }

        function insert() {
            var actionGroupObj = buildActionGroupDefinitionFromScope();
            if ($scope.onActionGroupAdded != undefined) {
                $scope.onActionGroupAdded(actionGroupObj);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var actionGroupObj = buildActionGroupDefinitionFromScope();
            if ($scope.onActionGroupUpdated != undefined) {
                $scope.onActionGroupUpdated(actionGroupObj);
            }
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('Retail_BE_AccountActionGroupDefinitionEditorController', AccountActionGroupDefintionController);
})(appControllers);
