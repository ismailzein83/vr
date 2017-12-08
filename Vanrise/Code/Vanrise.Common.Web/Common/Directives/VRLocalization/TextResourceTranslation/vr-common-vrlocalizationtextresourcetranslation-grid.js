"use strict";

app.directive("vrCommonVrlocalizationtextresourcetranslationGrid", ["VRNotificationService", 'VRCommon_VRLocalizationTextResourceTranslationAPIService', 'VRCommon_VRLocalizationTextResourceService','VRCommon_VRLocalizationTextResourceTranslationService',
    function (VRNotificationService, VRCommon_VRLocalizationTextResourceTranslationAPIService, VRCommon_VRLocalizationTextResourceService, VRCommon_VRLocalizationTextResourceTranslationService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrLocalizationTextResourceTranslationGrid = new VrLocalizationTextResourceTranslationGrid($scope, ctrl, $attrs);
                vrLocalizationTextResourceTranslationGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: '/Client/Modules/Common/Directives/VRLocalization/TextResourceTranslation/Templates/VRLocalizationTextResourceTranslationGridTemplate.html'
        };
        function VrLocalizationTextResourceTranslationGrid($scope, ctrl, $attrs) {

            var gridAPI;

            var textResourceId;

            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.vrLocalizationTextResourcesTranslation = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRLocalizationTextResourceTranslationAPIService.GetFilteredVRLocalizationTextResourcesTranslation(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        textResourceId = payload.textResourceId;
                        return gridAPI.retrieveData(payload.query);
                    }
                };
                api.onVRLocalizationTextResourceTranslationAdded = function (textResourceTranslation) {
                    gridAPI.itemAdded(textResourceTranslation);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: "Edit",
                    clicked: editVRLocalizationTextResourceTranslation
                }];
            }
            function editVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslation) {

                var onVRLocalizationTextResourceTranslationUpdated = function (updatedvrLocalizationTextResourceTranslation) {
                    gridAPI.itemUpdated(updatedvrLocalizationTextResourceTranslation);
                };

                VRCommon_VRLocalizationTextResourceTranslationService.editVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslation.VRLocalizationTextResourceTranslationId, textResourceId, onVRLocalizationTextResourceTranslationUpdated);
            }
        }

        return directiveDefinitionObject;
    }
]);