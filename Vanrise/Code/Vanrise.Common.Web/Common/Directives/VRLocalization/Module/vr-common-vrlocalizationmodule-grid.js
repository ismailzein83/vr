"use strict";

app.directive("vrCommonLocalizationmoduleGrid", ["VRNotificationService", 'VRCommon_VRLocalizationModuleAPIService', 'VRCommon_VRLocalizationModuleService',
    function (VRNotificationService, VRCommon_VRLocalizationModuleAPIService, VRCommon_VRLocalizationModuleService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrLocalizationModuleGrid = new VRLocalizationModuleGrid($scope, ctrl, $attrs);
                vrLocalizationModuleGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: '/Client/Modules/Common/Directives/VRLocalization/Module/Templates/VRLocalizationModuleGridTemplate.html'
        };
        function VRLocalizationModuleGrid($scope, ctrl, $attrs) {
            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.vrLocalizationModules = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRLocalizationModuleAPIService.GetFilteredVRLocalizationModules(dataRetrievalInput)
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

                api.onVRLocalizationModuleAdded = function (addedVRLocalizationModule) {
                    gridAPI.itemAdded(addedVRLocalizationModule);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: "Edit",
                    clicked: editVRLocalizationModule
                }];
            }

            function editVRLocalizationModule(vrLocalizationModuleItem) {
                var onVRLocalizationModuleUpdated = function (updatedvrLocalizationModule) {
                    gridAPI.itemUpdated(updatedvrLocalizationModule);
                };
                VRCommon_VRLocalizationModuleService.editVRLocalizationModule(vrLocalizationModuleItem.VRLocalizationModuleId, onVRLocalizationModuleUpdated);
            }
        }

        return directiveDefinitionObject;
    }
]);