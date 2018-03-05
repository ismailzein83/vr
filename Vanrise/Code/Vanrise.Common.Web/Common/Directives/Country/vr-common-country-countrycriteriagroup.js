'use strict';
app.directive('vrCommonCountryCountrycriteriagroup', ['UtilsService', '$compile', 'VRCommon_CountryAPIService', 'VRNotificationService', 'VRUIUtilsService',
function (UtilsService, $compile, VRCommon_CountryAPIService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new countryGroupCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/Country/Templates/CountryCriteriaDirectiveTemplate.html"

    };

    function countryGroupCtor(ctrl, $scope, $attrs) {

        var countryGroupSelectorAPI;
        var countryGroupDirectiveAPI;
        var countryGroupDirectiveReadyPromiseDeferred;

        function initializeController() {
            $scope.countryGroupTemplates = [];

            $scope.onCountryGroupSelectorReady = function (api) {
                countryGroupSelectorAPI = api;
                defineAPI();
            };

            $scope.onCountryGroupDirectiveReady = function (api) {
                countryGroupDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingCountryGroupDirective = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, countryGroupDirectiveAPI, undefined, setLoader, countryGroupDirectiveReadyPromiseDeferred);
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                countryGroupSelectorAPI.clearDataSource();

                var countryGroupConfigId;
                var countryGroupPayload;

                if (payload != undefined) {
                    countryGroupPayload = {
                        countryGroupSettings: payload.countryGroupSettings
                    };
                    countryGroupConfigId = payload.countryGroupSettings != undefined ? payload.countryGroupSettings.ConfigId : undefined;
                }
                var promises = [];

                var loadCountryGroupTemplatesPromise = VRCommon_CountryAPIService.GetCountryCriteriaGroupTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.countryGroupTemplates.push(item);
                    });

                    if (countryGroupConfigId != undefined)
                        $scope.selectedCountryGroupTemplate = UtilsService.getItemByVal($scope.countryGroupTemplates, countryGroupConfigId, "ExtensionConfigurationId");

                });
                promises.push(loadCountryGroupTemplatesPromise);

                if (countryGroupConfigId != undefined) {
                    countryGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var countryGroupDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(countryGroupDirectiveLoadPromiseDeferred.promise);

                    countryGroupDirectiveReadyPromiseDeferred.promise.then(function () {
                        countryGroupDirectiveReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(countryGroupDirectiveAPI, countryGroupPayload, countryGroupDirectiveLoadPromiseDeferred);
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var countryGroupSettings;
                if ($scope.selectedCountryGroupTemplate != undefined) {
                    if (countryGroupDirectiveAPI != undefined) {
                        countryGroupSettings = countryGroupDirectiveAPI.getData();
                        countryGroupSettings.ConfigId = $scope.selectedCountryGroupTemplate.ExtensionConfigurationId;
                    }
                }
                return countryGroupSettings;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);