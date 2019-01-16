"use strict";
app.directive("tcanalAccounttypePartRuntimeCountrycodes", ["UtilsService",
    function (UtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CountryCodesPart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/TestCallAnalysis/Elements/AccountParts/MainExtensions/CountryCodes/Templates/CountryCodesPartRuntimeTemplate.html"

        };

        function CountryCodesPart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.countryCodes = [];

                $scope.scopeModel.addCountryCode = function () {
                    $scope.scopeModel.countryCodes.push({
                        Code: $scope.scopeModel.countryCode
                    });
                    $scope.scopeModel.countryCode = undefined;
                };


                $scope.scopeModel.validateCountryCode = function () {
                    if ($scope.scopeModel.countryCodes.length > 0 && $scope.scopeModel.countryCode) {
                        for (var i = 0; i < $scope.scopeModel.countryCodes.length; i++) {
                            if ($scope.scopeModel.countryCode == $scope.scopeModel.countryCodes[i].Code)
                                return "This code has already been entered.";
                        }
                        return null;
                    }
                    return null;
                };

                $scope.scopeModel.isCountryCodeValid = function () {
                    if ($scope.scopeModel.countryCodes.length > 0 && $scope.scopeModel.countryCode) {
                        for (var i = 0; i < $scope.scopeModel.countryCodes.length; i++) {
                            if ($scope.scopeModel.countryCode == $scope.scopeModel.countryCodes[i].Code)
                                return false;
                        }
                        return true;
                    }
                    return true;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined && payload.partSettings != undefined) {
                        if (payload.partSettings.CountryCodes != undefined && payload.partSettings.CountryCodes.length > 0) {
                            for (var i = 0; i < payload.partSettings.CountryCodes.length; i++) {
                                $scope.scopeModel.countryCodes.push({
                                    Code: payload.partSettings.CountryCodes[i].Code
                                });
                            }
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {

                    function getCountryCodes() {
                        var countryCodes = [];
                        if ($scope.scopeModel.countryCodes.length > 0) {
                            for (var i = 0; i < $scope.scopeModel.countryCodes.length; i++) {
                                countryCodes.push({
                                    Code: $scope.scopeModel.countryCodes[i].Code
                                });
                            }
                        }
                        return countryCodes;
                    }
                    return {
                        $type:"TestCallAnalysis.Business.CountryCodesPartSettings, TestCallAnalysis.Business",
                        CountryCodes: getCountryCodes()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);