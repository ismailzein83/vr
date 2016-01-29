(function (appControllers) {

    "use strict";

    newNumberPrefixDialogController.$inject = ['$scope', 'FraudAnalysis_NumberPrefixAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'FraudAnalysis_NewFAOutputResultEnum'];

    function newNumberPrefixDialogController($scope, FraudAnalysis_NumberPrefixAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, FraudAnalysis_NewFAOutputResultEnum) {

        var treeNumberPrefixes;
        var numberPrefixId;
        var countryId;
        var editMode;
        var numberPrefixEntity;
        var sellingNumberPlanId;
        var disableCountry;

        defineScope();
        loadParameters();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                numberPrefixId = parameters.NumberPrefixId;
                countryId = parameters.CountryId;
                $scope.countryName = parameters.CountryName;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
                treeNumberPrefixes = parameters.NumberPrefixes;
            }
            editMode = (numberPrefixId != undefined);
            load();
        }

        function defineScope() {
            $scope.bed;
            $scope.eed;
            $scope.countryName;
            $scope.numberPrefixes = [];

            $scope.validateNumberPrefixs = function () {
                if ($scope.numberPrefixes != undefined && $scope.numberPrefixes.length == 0)
                    return "Enter at least one prefix.";
                return null;
            };
            $scope.saveNumberPrefix = function () {
                if (editMode) {
                    return updateNumberPrefix();
                }
                else {
                    return insertNumberPrefix();
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

        function load() {
            $scope.isGettingData = true;
            if (countryId != undefined) {
                $scope.title = UtilsService.buildTitleForAddEditor("NumberPrefix for Country " + $scope.countryName);
                loadAllControls();
            }
            else if (editMode) {
                getNumberPrefix().then(function () {
                    loadAllControls()
                        .finally(function () {
                            numberPrefixEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
                $scope.title = UtilsService.buildTitleForAddEditor("Number Prefix");
            }
        }

        function loadAllControls() {
            $scope.isGettingData = false;
        }

        function getNumberPrefix() {

        }

        function fillScopeFromNumberPrefixObj(prefix) {
            $scope.name = prefix.Name;
            $scope.title = UtilsService.buildTitleForUpdateEditor($scope.name, "NumberPrefix");
        }

        function insertNumberPrefix() {
            if ($scope.onNumberPrefixAdded != undefined) {
                var addedNumberPrefixes = [];
                for (var i = 0; i < $scope.numberPrefixes.length; i++) {
                    addedNumberPrefixes.push({ Prefix: $scope.numberPrefixes[i].prefix });
                }

                $scope.onNumberPrefixAdded(addedNumberPrefixes);
                $scope.numberPrefixes.length = 0;
                $scope.modalContext.closeModal();
            }
        }

        function updateNumberPrefix() {

        }
    }

    appControllers.controller('cdranalysis-fa-numberprefix-newprefixdialog', newNumberPrefixDialogController);
})(appControllers);
