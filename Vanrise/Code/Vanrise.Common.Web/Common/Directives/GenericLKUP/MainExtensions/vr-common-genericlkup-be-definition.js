"use strict";

app.directive("vrCommonGenericlkupBeDefinition", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new GenericLKUPBEDefinitionDirective($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/GenericLKUP/MainExtensions/Templates/GenericLKUPBEDefinition.html"

    };

    function GenericLKUPBEDefinitionDirective($scope, ctrl) {
        var genericLKUPBEDefinitionSelectorAPI;
        var genericLKUPBEDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGenericLKUPBEDefinitionSelectorReady = function (api) {
                genericLKUPBEDefinitionSelectorAPI = api;
                genericLKUPBEDefinitionSelectorReadyPromiseDeferred.resolve();
            };
            UtilsService.waitMultiplePromises([genericLKUPBEDefinitionSelectorReadyPromiseDeferred.promise]).then(function () {
                defineAPI();
            });
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                var loadSelectorGenericLKUPBEDefinitionPromise = loadSelectorGenericLKUPBEDefinition();
                promises.push(loadSelectorGenericLKUPBEDefinitionPromise);

                function loadSelectorGenericLKUPBEDefinition() {

                    return genericLKUPBEDefinitionSelectorAPI.load(undefined);
                }

                return UtilsService.waitMultiplePromises(promises);
            };
            api.getData = function () {
                return {
                    $type: "Vanrise.Common.Business.GenericLKUPBEDefinitionSettings ,Vanrise.Common.Business",
                    ExtendedSettings: genericLKUPBEDefinitionSelectorAPI.getData(),
                };
            };
            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
    return directiveDefinitionObject;

}]);