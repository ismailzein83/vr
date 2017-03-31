'use strict';

app.directive('vrCommonStyledefinitionGrid', ['VRCommon_StyleDefinitionAPIService', 'VRCommon_StyleDefinitionService', 'VRNotificationService', 'VRUIUtilsService',
    function (VRCommon_StyleDefinitionAPIService, VRCommon_StyleDefinitionService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var styleDefinitionGrid = new StyleDefinitionGrid($scope, ctrl, $attrs);
                styleDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/StyleDefinition/Templates/StyleDefinitionGridTemplate.html'
        };

        function StyleDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.styleDefinition = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VRCommon_StyleDefinitionService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_StyleDefinitionAPIService.GetFilteredStyleDefinitions(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onStyleDefinitionAdded = function (addedStyleDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedStyleDefinition);
                    gridAPI.itemAdded(addedStyleDefinition);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editStyleDefinition,
                    haspermission: hasEditStyleDefinitionPermission
                });
            }
            function editStyleDefinition(styleDefinitionItem) {
                var onStyleDefinitionUpdated = function (updatedStyleDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedStyleDefinition);
                    gridAPI.itemUpdated(updatedStyleDefinition);
                };

                VRCommon_StyleDefinitionService.editStyleDefinition(styleDefinitionItem.Entity.StyleDefinitionId, onStyleDefinitionUpdated);
            }
            function hasEditStyleDefinitionPermission() {
                return VRCommon_StyleDefinitionAPIService.HasEditStyleDefinitionPermission();
            }

        }
    }]);
