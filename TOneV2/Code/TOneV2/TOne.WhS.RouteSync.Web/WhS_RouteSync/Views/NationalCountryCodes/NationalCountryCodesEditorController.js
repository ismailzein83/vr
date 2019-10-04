(function (appControllers) {

    'use strict';

    NationalCountryCodesEditor.$inject = ['$scope', 'VRNavigationService', 'UtilsService'];

    function NationalCountryCodesEditor($scope, VRNavigationService, UtilsService) {

        var nationalCountryCodes;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined)
                nationalCountryCodes = parameters.nationalCountryCodes;
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                return update();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls().catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = 'National Country Codes';
            }

            function loadStaticData() {
                $scope.scopeModel.nationalCountryCodes = nationalCountryCodes;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]);
        }

        function update() {
            $scope.scopeModel.isLoading = true;

            if ($scope.onNationalCountryCodesUpdated != undefined)
                $scope.onNationalCountryCodesUpdated($scope.scopeModel.nationalCountryCodes);

            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('WhS_RouteSync_NationalCountryCodesEditorController', NationalCountryCodesEditor);

})(appControllers);