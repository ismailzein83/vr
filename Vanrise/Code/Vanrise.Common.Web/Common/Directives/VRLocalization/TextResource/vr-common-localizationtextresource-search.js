"use strict";

app.directive("vrCommonLocalizationtextresourceSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VRCommon_VRLocalizationTextResourceAPIService', 'VRCommon_VRLocalizationTextResourceService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, VRCommon_VRLocalizationTextResourceAPIService, VRCommon_VRLocalizationTextResourceService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var vrLocalizationTextResourceSearch = new VRLocalizationTextResourceSearch($scope, ctrl, $attrs);
            vrLocalizationTextResourceSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Common/Directives/VRLocalization/TextResource/Templates/VRLocalizationTextResourceSearchTemplate.html'

    };

    function VRLocalizationTextResourceSearch($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        var moduleId;

        var gridPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};


            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridPromiseDeferred.resolve();
            };

            $scope.scopeModel.addResource = function () {
                var onVRLocalizationTextResourceAdded = function (addedItem) {
                    gridAPI.onVRLocalizationTextResourceAdded(addedItem);
                };

                VRCommon_VRLocalizationTextResourceService.addVRLocalizationTextResource(onVRLocalizationTextResourceAdded,moduleId);
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                $scope.scopeModel.isLoading = true;
                var promises = [];
                if (payload != undefined) {
                    moduleId = payload.moduleIds;
                }
                promises.push(loadTextResourceGrid());
                return UtilsService.waitMultiplePromises(promises).then(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getFilterObject() {
            var filter = {
                ModuleIds: moduleId,
                showModule: false
            };
            return filter;
        }

        function loadTextResourceGrid() {
            return gridPromiseDeferred.promise.then(function () {
                gridAPI.load(getFilterObject());
            });
        }
    }

    return directiveDefinitionObject;

}]);
