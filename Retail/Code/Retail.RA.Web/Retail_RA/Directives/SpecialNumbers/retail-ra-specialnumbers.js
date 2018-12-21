

(function (app) {

    'use strict';

    specialNumbers.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function specialNumbers(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FaultCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_RA/Directives/SpecialNumbers/Template/SpecialNumbersTemplate.html'

        };
        function FaultCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.rangeList = [];
                $scope.scopeModel.numberList = [];

                $scope.scopeModel.isEmpty = function (value) {
                    return (value == undefined || value == null || value == '');
                };
                $scope.scopeModel.validateNewRange = function () {
                    if (!$scope.scopeModel.isEmpty($scope.scopeModel.to) && parseInt($scope.scopeModel.from, 10) >= parseInt($scope.scopeModel.to, 10))
                        return "To value should be greater than From value.";

                    else {
                        for (var i = 0; i < $scope.scopeModel.rangeList.length; i++) {
                            var item = $scope.scopeModel.rangeList[i];
                            if (parseInt($scope.scopeModel.from, 10) >= parseInt(item.From, 10) && (!$scope.scopeModel.isEmpty(item.To) && parseInt($scope.scopeModel.from, 10) <= parseInt(item.To, 10)))
                                return "Special numbers range overlapped with existing one.";

                            else if (!$scope.scopeModel.isEmpty($scope.scopeModel.to) && parseInt($scope.scopeModel.to, 10) >= parseInt(item.From, 10) && (!$scope.scopeModel.isEmpty(item.To) && parseInt($scope.scopeModel.to, 10) <= parseInt(item.To, 10)))
                                return "Special numbers range overlapped with existing one.";

                            else if (parseInt($scope.scopeModel.from, 10) <= parseInt(item.From, 10) && !$scope.scopeModel.isEmpty($scope.scopeModel.to) && parseInt($scope.scopeModel.to, 10) >= parseInt(item.To, 10))
                                return "Special numbers range overlapped with existing one.";

                            else if (parseInt($scope.scopeModel.from, 10) == parseInt(item.From, 10))
                                return "Special numbers range overlapped with existing one.";
                        }
                    }
                    for (var i = 0; i < $scope.scopeModel.numberList.length; i++) {
                        var number = $scope.scopeModel.numberList[i];
                        if (number >= parseInt($scope.scopeModel.from, 10) && number <= parseInt($scope.scopeModel.to, 10))
                            return "Number(s) in range already exists in numbers section";
                    }
                };
                $scope.scopeModel.validateNumber = function () {
                    if ($scope.scopeModel.numberToAdd == undefined)
                        return;
                    var number = parseInt($scope.scopeModel.numberToAdd);
                    for (var i = 0; i < $scope.scopeModel.rangeList.length; i++) {
                        var range = $scope.scopeModel.rangeList[i];
                        if (number >= range.From && number <= range.To)
                            return "Special number exists in one of the ranges";
                    }
                    if (!$scope.scopeModel.isNumberValid()) {
                        return "Special number already exists";
                    }
                };
                $scope.scopeModel.isNumberValid = function () {
                   
                    var numberIsValid = true;
                    var numberToAdd = $scope.scopeModel.numberToAdd;
                    if (numberToAdd == undefined || numberToAdd.length == 0 || numberToAdd == '' || isNaN(numberToAdd) ) {
                        numberIsValid = false;
                    }
                    else {
                        //var indexOfnumber = $scope.scopeModel.numberList.indexOf(number);
                        //numberIsValid = indexOfnumber < 0;
                        for (var j = 0; j < $scope.scopeModel.numberList.length; j++) {
                            var specialNumber = $scope.scopeModel.numberList[j];
                            if (numberToAdd == specialNumber) {
                                numberIsValid = false;
                            }
                        }
                    }
                    for (var i = 0; i < $scope.scopeModel.rangeList.length; i++) {
                        var range = $scope.scopeModel.rangeList[i];
                        if (numberToAdd >= range.From && numberToAdd <= range.To)
                            numberIsValid = false;
                    }
                    return numberIsValid;
                };
                $scope.scopeModel.validateSpecialNumbers = function () {
                    if (($scope.scopeModel.numberList == undefined || $scope.scopeModel.numberList.length == 0) && ($scope.scopeModel.rangeList == undefined || $scope.scopeModel.rangeList.length == 0))
                        return "One number or range at least should be added"; 
                };
                $scope.scopeModel.isRangeValid = function () {
                    if ($scope.scopeModel.isEmpty($scope.scopeModel.from) || $scope.scopeModel.isEmpty($scope.scopeModel.to))
                        return false;

                    return ($scope.scopeModel.validateNewRange() == undefined);
                };

                $scope.scopeModel.addRange = function () {
                    var range = {
                        From: $scope.scopeModel.from,
                        To: $scope.scopeModel.to,
                    };

                    $scope.scopeModel.rangeList.push(range);
                    $scope.scopeModel.from = undefined;
                    $scope.scopeModel.to = undefined;
                };

                $scope.scopeModel.addNumber = function () {
                    $scope.scopeModel.numberList.push($scope.scopeModel.numberToAdd);
                    $scope.scopeModel.numberToAdd = undefined;
                };
                defineAPI();
            }
            function defineAPI() {


                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.selectedValues != undefined && payload.selectedValues.Settings != undefined) {
                        $scope.scopeModel.numberList = payload.selectedValues.Settings.Numbers;
                        $scope.scopeModel.rangeList = payload.selectedValues.Settings.Range;
                    }

                };

                api.setData = function (data) {
                    data.Settings = {
                        $type: "Retail.RA.Entities.SpecialNumbersSetting,Retail.RA.Entities",
                        Numbers: $scope.scopeModel.numberList,
                        Range: $scope.scopeModel.rangeList
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailRaSpecialnumbers', specialNumbers);

})(app);
