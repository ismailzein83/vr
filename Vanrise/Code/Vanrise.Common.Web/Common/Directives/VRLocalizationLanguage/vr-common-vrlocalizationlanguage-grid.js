"use strict";

app.directive("vrCommonLocalizationlanguageGrid", ["VRNotificationService", 'VRCommon_VRLocalizationLanguageAPIService', 'VRCommon_VRLocalizationLanguageService',
    function (VRNotificationService, VRCommon_VRLocalizationLanguageAPIService, VRCommon_VRLocalizationLanguageService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrLocalizationLanguageGrid = new VRLocalizationLanguageGrid($scope, ctrl, $attrs);
                vrLocalizationLanguageGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: '/Client/Modules/Common/Directives/VRLocalizationLanguage/Templates/VRLocalizationLanguageGridTemplate.html'
        };
        function VRLocalizationLanguageGrid($scope, ctrl, $attrs) {
            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.vrLocalizationLanguages = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRLocalizationLanguageAPIService.GetFilteredVRLocalizationLanguages(dataRetrievalInput)
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

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onVRLocalizationLanguageAdded = function (addedVRLocalizationLanguage) {
                    gridAPI.itemAdded(addedVRLocalizationLanguage);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: "Edit",
                    clicked: editVRLocalizationLanguage
                }];
            }

            function editVRLocalizationLanguage(vrLocalizationLanguageItem) {
                var onVRLocalizationLanguageUpdated = function (updatedvrLocalizationLanguage) {
                    gridAPI.itemUpdated(updatedvrLocalizationLanguage);
                }
                VRCommon_VRLocalizationLanguageService.editVRLocalizationLanguage(vrLocalizationLanguageItem.VRLanguageId, onVRLocalizationLanguageUpdated);
            }
        }

        return directiveDefinitionObject;
    }
]);