(function (appControllers) {

    "use strict";

    newNumberPrefixDialogController.$inject = ['$scope', 'VRNavigationService', 'UtilsService'];

    function newNumberPrefixDialogController($scope, VRNavigationService, UtilsService) {

        var treeNumberPrefixes;
        var numberPrefixEntity;

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
            $scope.numberPrefixes = [];

            $scope.validateNumberPrefixes = function () {
                if ($scope.numberPrefixes != undefined && $scope.numberPrefixes.length == 0)
                    return "Enter at least one prefix.";
                return null;
            };
            $scope.saveNumberPrefix = function () {
                if ($scope.onNumberPrefixAdded != undefined) {
                    var addedNumberPrefixes = [];
                    for (var i = 0; i < $scope.numberPrefixes.length; i++) {
                        addedNumberPrefixes.push({ Prefix: $scope.numberPrefixes[i].prefix });
                    }

                    $scope.onNumberPrefixAdded(addedNumberPrefixes);
                    $scope.numberPrefixes.length = 0;
                    $scope.close();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.disabledNumberPrefix = true;
            $scope.onNumberPrefixValueChange = function (value) {
                $scope.disabledNumberPrefix = (value == undefined) || UtilsService.getItemIndexByVal($scope.numberPrefixes, value, "prefix") != -1 || UtilsService.getItemIndexByVal(treeNumberPrefixes, value, "Prefix") != -1;
            }
            $scope.addNumberPrefixValue = function () {
                $scope.numberPrefixes.push({ prefix: $scope.numberPrefixValue });
                $scope.numberPrefixValue = undefined;
                $scope.disabledNumberPrefix = true;
            };
        }
       
    }

    appControllers.controller('cdranalysis-fa-numberprefix-newprefixdialog', newNumberPrefixDialogController);
})(appControllers);
