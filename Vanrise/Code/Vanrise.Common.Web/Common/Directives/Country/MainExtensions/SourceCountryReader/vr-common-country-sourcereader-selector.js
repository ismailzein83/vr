"use strict";

app.directive("vrCommonCountrySourcereaderSelector", ['UtilsService', '$compile', 'VRUIUtilsService', 'VRCommon_CountryAPIService', function (UtilsService, $compile, VRUIUtilsService, VRCommon_CountryAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/Common/Directives/Country/MainExtensions/SourceCountryReader/Templates/CountrySourceReaderSelector.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        var countrySourceDirectiveAPI;
        var countrySourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        $scope.oncountrySourceTypeDirectiveReady = function (api) {
            countrySourceDirectiveAPI = api;
            countrySourceDirectiveReadyPromiseDeferred.resolve();
        }

        function initializeController() {            
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var sourceCountry;
                if ($scope.selectedCountrySourceTypeTemplate != undefined) {
                    if (countrySourceDirectiveAPI != undefined) {
                        sourceCountry = countrySourceDirectiveAPI.getData();
                        sourceCountry.ConfigId = $scope.selectedCountrySourceTypeTemplate.ExtensionConfigurationId

                    }
                }
                return sourceCountry;
               

            };


            api.load = function (payload) {
                var promises = [];
                $scope.countrySourceTypeTemplates = [];
                var sourceConfigId;
                var connectionString;

                if (payload != undefined) {
                    sourceConfigId = payload.sourceConfigId;
                    connectionString = payload.connectionString;

                }
                var loadCountryTypeTemplatesPromise = VRCommon_CountryAPIService.GetCountrySourceTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.countrySourceTypeTemplates.push(item);
                    });
                    if (sourceConfigId != undefined)
                        $scope.selectedCountrySourceTypeTemplate = UtilsService.getItemByVal($scope.countrySourceTypeTemplates, sourceConfigId, "ExtensionConfigurationId");

                });
                promises.push(loadCountryTypeTemplatesPromise);

                if (payload != undefined && payload.sourceConfigId != undefined) {
                    var loadCountrySourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                    countrySourceDirectiveReadyPromiseDeferred.promise.then(function() {
                        var payload = {
                            connectionString: connectionString
                        };
                        VRUIUtilsService.callDirectiveLoad(countrySourceDirectiveAPI, payload, loadCountrySourceTemplatePromiseDeferred);
                    });

                    promises.push(loadCountrySourceTemplatePromiseDeferred.promise);
                }
                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
