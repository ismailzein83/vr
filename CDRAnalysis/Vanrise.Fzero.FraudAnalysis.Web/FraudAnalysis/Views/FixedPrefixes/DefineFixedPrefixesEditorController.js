(function (appControllers) {

    "use strict";

    defineFixedPrefixesEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Fzero_FraudAnalysis_DefineFixedPrefixesAPIService', 'VRValidationService'];

    function defineFixedPrefixesEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Fzero_FraudAnalysis_DefineFixedPrefixesAPIService, VRValidationService) {

        var isEditMode;
        var fixedPrefixID;
        var fixedPrefixEntity;

        var directiveReadyAPI;
        var directiveReadyPromiseDeferred;

        loadParameters();
        defineScope();
        load(); 

        function loadParameters() {
            
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                fixedPrefixID = parameters.ID
            }
            isEditMode = (fixedPrefixID != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};
            $scope.scopeModal.SaveFixedPrefix = function () {
                if (isEditMode) {
                    return updateFixedPrefix();
                }
                else {
                    return insertFixedPrefix();
                }
            };
            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };
         
            $scope.scopeModal.onDirectiveReady = function (api) {
                directiveReadyAPI = api;
                var setLoader = function (value) { $scope.isLoadingDirective = value };
                var payload;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveReadyAPI, payload, setLoader, directiveReadyPromiseDeferred);
            }

        }

        function load() {
            $scope.scopeModal.isLoading = true;
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("Fixed Prefix");
                getFixedPrefix().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
                       
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor("Fixed Prefix");
                loadAllControls();
            }

        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModal.isLoading = false;
               });
        }
      
        function loadFilterBySection() {
            if(fixedPrefixEntity != undefined)
            {
                $scope.scopeModal.name = fixedPrefixEntity.Prefix;
            }
        }


        function getFixedPrefix() {
            return Fzero_FraudAnalysis_DefineFixedPrefixesAPIService.GetFixedPrefix(fixedPrefixID).then(function (fixedPrefix) {
                fixedPrefixEntity = fixedPrefix;
            });
        }

        function buildFixedPrefixObjectObjFromScope() {
            var fixedPrefix = {};
            fixedPrefix.Prefix = $scope.scopeModal.name;
            return fixedPrefix;
        }

        function insertFixedPrefix() {

            var fixedPrefixObject = buildFixedPrefixObjectObjFromScope();
            return Fzero_FraudAnalysis_DefineFixedPrefixesAPIService.AddFixedPrefix(fixedPrefixObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Fixed Prefix", response, "Name")) {
                    if ($scope.onFixedPrefixAdded != undefined)
                        $scope.onFixedPrefixAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateFixedPrefix() {
            var fixedPrefixObject = buildFixedPrefixObjectObjFromScope();
            fixedPrefixObject.ID = fixedPrefixID;
            return Fzero_FraudAnalysis_DefineFixedPrefixesAPIService.UpdateFixedPrefix(fixedPrefixObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Fixed Prefix", response, "Name")) {
                    if ($scope.onFixedPrefixUpdated != undefined)
                        $scope.onFixedPrefixUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('Fzero_FraudAnalysis_DefineFixedPrefixesEditorController', defineFixedPrefixesEditorController);
})(appControllers);
