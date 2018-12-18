(function (appControllers) {

    'use strict';

    textPatternHelperController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService'];

    function textPatternHelperController($scope, VRNavigationService, UtilsService, VRNotificationService) {

        var parts;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                context = parameters.context;
                parts = context.getParts();
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function (dataItem) {
                var patternValue = dataItem.Name;
                if ($scope.onSetPattern != undefined) {
                    $scope.onSetPattern(patternValue);
                }
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = 'Pattern Helper';
            }

            function loadStaticData() {
                if (parts != undefined) {
                    $scope.scopeModel.dataSource = parts;
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }
    }

    appControllers.controller('VR_Common_TextPatternHelperController', textPatternHelperController);
})(appControllers);