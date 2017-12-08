"use strict";

app.directive("vrCommonVrlocalizationtextresourcetranslationSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VRCommon_VRLocalizationTextResourceAPIService', 'VRCommon_VRLocalizationTextResourceService', 'VRCommon_VRLocalizationTextResourceTranslationService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, VRCommon_VRLocalizationTextResourceAPIService, VRCommon_VRLocalizationTextResourceService, VRCommon_VRLocalizationTextResourceTranslationService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var vrLocalizationTextResourceTranslationSearch = new VrLocalizationTextResourceTranslationSearch($scope, ctrl, $attrs);
            vrLocalizationTextResourceTranslationSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Common/Directives/VRLocalization/TextResourceTranslation/Templates/VrLocalizationTextResourceTranslationSearch.html'

    };

    function VrLocalizationTextResourceTranslationSearch($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        var LanguageIds;

        var textResourceId;

        var gridPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};


            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(defineAPI());
            };

            $scope.scopeModel.addResourceTranslation = function () {
                var onVRLocalizationTextResourceTranslationAdded = function (addedItem) {
                    gridAPI.onVRLocalizationTextResourceTranslationAdded(addedItem);
                };

                VRCommon_VRLocalizationTextResourceTranslationService.addVRLocalizationTextResourceTranslation(onVRLocalizationTextResourceTranslationAdded, textResourceId);
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                if (payload != undefined) {
                    textResourceId = payload.textResourceIds[0];
                    var payload = {
                        query: {
                            ResourceIds: payload.textResourceIds,
                            LanguageIds: LanguageIds,
                        },
                        textResourceId: textResourceId
                    };

                    gridAPI.load(payload);
                }
                
            };

            return api;
        }
       
    }

    return directiveDefinitionObject;

}]);
