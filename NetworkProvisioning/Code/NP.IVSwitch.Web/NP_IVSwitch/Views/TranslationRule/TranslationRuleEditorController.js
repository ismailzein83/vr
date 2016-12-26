(function (appControllers) {

    "use strict";

    TranslationRuleEditorController.$inject = ['$scope', 'NP_IVSwitch_TranslationRuleAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService'];

    function TranslationRuleEditorController($scope, NP_IVSwitch_TranslationRuleAPIService, VRNotificationService, UtilsService, VRNavigationService) {

        var isEditMode;

        var translationRuleId;
        var translationRuleEntity;

        loadParameters();

        defineScope();

        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                translationRuleId = parameters.TranslationRuleId;
            }


            isEditMode = (translationRuleId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.hasSaveTranslationRulePermission = function () {
                if (isEditMode) {
                    return NP_IVSwitch_TranslationRuleAPIService.HasEditTranslationRulePermission();
                }
                else {
                    return NP_IVSwitch_TranslationRuleAPIService.HasAddTranslationRulePermission();
                }
            };

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
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getTranslationRule().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }


        }

        function getTranslationRule() {
            return NP_IVSwitch_TranslationRuleAPIService.GetTranslationRule(translationRuleId).then(function (response) {
                translationRuleEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var translationRuleName = (translationRuleEntity != undefined) ? translationRuleEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(translationRuleName, 'Translation Rule');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Translation Rule');
                }
            }
            function loadStaticData() {
                if (translationRuleEntity == undefined)
                    return;
                $scope.scopeModel.name = translationRuleEntity.Name;
                $scope.scopeModel.dnisPattern = translationRuleEntity.DNISPattern;
                $scope.scopeModel.cliPattern = translationRuleEntity.CLIPattern;

            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;

            return NP_IVSwitch_TranslationRuleAPIService.AddTranslationRule(buildTranslationRuleObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Translation Rule', response, 'Name')) {

                    if ($scope.onTranslationRuleAdded != undefined)
                        $scope.onTranslationRuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return NP_IVSwitch_TranslationRuleAPIService.UpdateTranslationRule(buildTranslationRuleObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Translation Rule', response, 'Name')) {

                    if ($scope.onTranslationRuleUpdated != undefined) {
                        $scope.onTranslationRuleUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildTranslationRuleObjFromScope() {
            return {
                TranslationRuleId: translationRuleEntity != undefined ? translationRuleEntity.TranslationRuleId : undefined,
                Name: $scope.scopeModel.name,
                DNISPattern: $scope.scopeModel.dnisPattern,
                CLIPattern: $scope.scopeModel.cliPattern
            };
        }
    }

    appControllers.controller('NP_IVSwitch_TranslationRuleEditorController', TranslationRuleEditorController);

})(appControllers);