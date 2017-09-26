(function (appControllers) {

    'use strict';

    fileNamePatternHelperController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'WhS_BE_SalePricelistFileNameParts'];

    function fileNamePatternHelperController($scope, VRNavigationService, UtilsService, VRNotificationService, WhS_BE_SalePricelistFileNameParts) {

        var context;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.dataSource = [];
            $scope.scopeModel.save = function (dataItem) {
                return addFileNamePattern(dataItem);
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function buildFileNamePatternObjFromScope(dataItem) {
                return "#" + dataItem.variableName + "#";
            }
            function addFileNamePattern(dataItem) {
                var fileNamePatternValue = buildFileNamePatternObjFromScope(dataItem);
                if ($scope.onSetFileNamePattern != undefined) {
                    $scope.onSetFileNamePattern(fileNamePatternValue);
                }
                $scope.modalContext.closeModal();
            }
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {

                function setTitle() {
                    $scope.title = 'File Name Pattern Helper';
                }

                function loadFileNameParts() {
                    $scope.scopeModel.dataSource = UtilsService.getArrayEnum(WhS_BE_SalePricelistFileNameParts);
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadFileNameParts]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }
        }

    }
    appControllers.controller('BE_Settings_SalePricelistFileNamePatternHelperController', fileNamePatternHelperController);

})(appControllers);