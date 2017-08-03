(function (appControllers) {

    "use strict";

    newNumberPrefixDialogController.$inject = ['$scope', 'VRNavigationService', 'UtilsService'];

    function newNumberPrefixDialogController($scope, VRNavigationService, UtilsService) {

        var treeNumberPrefixes;

        defineScope();
        loadParameters();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                treeNumberPrefixes = parameters.NumberPrefixes;
            }
        }

        function defineScope() {

            $scope.title = UtilsService.buildTitleForAddEditor("Number Prefix");

            $scope.saveNumberPrefix = function () {
                if ($scope.onNumberPrefixAdded != undefined) {
                    var addedNumberPrefix = { prefix: $scope.numberPrefixValue };
                    $scope.onNumberPrefixAdded(addedNumberPrefix);
                    $scope.close();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.disabledNumberPrefix = true;
            $scope.onNumberPrefixValueChange = function (value) {
                $scope.disabledNumberPrefix = (value == undefined) || UtilsService.getItemIndexByVal(treeNumberPrefixes, value, "Prefix") != -1;
            };
           
        }
       
    }

    appControllers.controller('cdranalysis-fa-numberprefix-newprefixdialog', newNumberPrefixDialogController);
})(appControllers);
