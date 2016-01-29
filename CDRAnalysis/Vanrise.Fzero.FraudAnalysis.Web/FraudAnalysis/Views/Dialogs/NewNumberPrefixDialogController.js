(function (appControllers) {

    "use strict";

    newNumberPrefixDialogController.$inject = ['$scope', 'FraudAnalysis_NumberPrefixAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'FraudAnalysis_NewFAOutputResultEnum'];

    function newNumberPrefixDialogController($scope, FraudAnalysis_NumberPrefixAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, FraudAnalysis_NewFAOutputResultEnum) {

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
                    return "Enter at least one numberPrefix.";
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
                $scope.disabledNumberPrefix = (value == undefined) || UtilsService.getItemIndexByVal($scope.numberPrefixes, value, "numberPrefix") != -1;
            }
            $scope.addNumberPrefixValue = function () {
                $scope.numberPrefixes.push({ numberPrefix: $scope.numberPrefixValue });
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

        function buildNumberPrefixObjFromScope() {
            var result = [];
            for (var i = 0; i < $scope.numberPrefixes.length; i++) {
                result.push({
                    Name: $scope.numberPrefixes[i].numberPrefix,
                    CountryId: countryId
                });
            }

            return result;
        }

        function getNewNumberPrefixFromNumberPrefixObj(numberPrefixObj) {
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                NewNumberPrefixs: numberPrefixObj
            }
        }

        function fillScopeFromNumberPrefixObj(numberPrefix) {
            $scope.name = numberPrefix.Name;
            $scope.title = UtilsService.buildTitleForUpdateEditor($scope.name, "NumberPrefix");
        }

        function insertNumberPrefix() {
            var numberPrefixItem = buildNumberPrefixObjFromScope();
            var input = getNewNumberPrefixFromNumberPrefixObj(numberPrefixItem);
            return FraudAnalysis_NumberPrefixAPIService.SaveNewNumberPrefix(input)
            .then(function (response) {

                if (response.Result == FraudAnalysis_NewFAOutputResultEnum.Existing.value) {
                    VRNotificationService.showWarning(response.Message);
                    $scope.numberPrefixes.length = 0;
                    for (var i = 0; i < response.NumberPrefixItems.length; i++) {
                        $scope.numberPrefixes.push({ numberPrefix: response.NumberPrefixItems[i].Name, message: response.NumberPrefixItems[i].Message });
                    }
                }
                else if (response.Result == FraudAnalysis_NewFAOutputResultEnum.Inserted.value) {
                    VRNotificationService.showSuccess(response.Message);
                    $scope.modalContext.closeModal();
                }
                else if (response.Result == FraudAnalysis_NewFAOutputResultEnum.Failed.value) {
                    VRNotificationService.showError(response.Message);
                }
                if ($scope.onNumberPrefixAdded != undefined)
                    $scope.onNumberPrefixAdded(response.NumberPrefixItems);

            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function updateNumberPrefix() {

        }
    }

    appControllers.controller('cdranalysis-fa-numberprefix-newprefixdialog', newNumberPrefixDialogController);
})(appControllers);
