(function (appControllers) {

    'use strict';

    serialNumberPatternHelperController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService'];

    function serialNumberPatternHelperController($scope, VRNavigationService, UtilsService, VRNotificationService) {

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
            $scope.scopeModel.save = function (dataItem) {
                return addeSerialNumberPattern(dataItem);
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builSerialNumberPatternObjFromScope(dataItem) {
                return "#" + dataItem.VariableName + "#";
            }
            function addeSerialNumberPattern(dataItem) {
                var serialNumberPatternValue = builSerialNumberPatternObjFromScope(dataItem);
                if ($scope.onSetSerialNumberPattern != undefined) {
                    $scope.onSetSerialNumberPattern(serialNumberPatternValue);
                }
                $scope.modalContext.closeModal();
            }
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {

                function setTitle() {
                    $scope.title = 'Serial Number Pattern Helper';
                }
                function loadStaticData() {
                    if (context != undefined) {
                        $scope.scopeModel.dataSource = context.getParts();
                    }
                }
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })

            }
        }
     
    }
    appControllers.controller('VR_Voucher_SerialNumberPatternHelperController', serialNumberPatternHelperController);

})(appControllers);