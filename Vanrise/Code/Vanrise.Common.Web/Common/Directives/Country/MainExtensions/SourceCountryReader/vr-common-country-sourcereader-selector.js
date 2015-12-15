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
        var countrySourceDirectiveReadyPromiseDeferred;

        function initializeController() {            
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
               
                return countrySourceDirectiveAPI.getData();

            };


            api.load = function (payload) {
                $scope.countrySourceTypeTemplates = [];
                var sourceConfigId;


                if (payload != undefined) {
                    sourceConfigId = payload.sourceConfigId;
                }
                return  VRCommon_CountryAPIService.GetCountrySourceTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.countrySourceTypeTemplates.push(item);
                    });
                    if (sourceConfigId != undefined)
                        $scope.selectedCountrySourceTypeTemplate = UtilsService.getItemByVal($scope.countrySourceTypeTemplates, sourceConfigId, "TemplateConfigID");

                });
              
                
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
