'use strict';

app.directive('vrNotificationAlertlevelGrid', ['VR_Notification_AlertLevelAPIService', 'VR_Notification_AlertLevelService', 'VRNotificationService', 'VRUIUtilsService',
    function (VR_Notification_AlertLevelAPIService, VR_Notification_AlertLevelService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',

            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var alertLevelGrid = new AlertLevelGrid($scope, ctrl, $attrs);
                alertLevelGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Notification/Directives/VRAlertLevel/Templates/AlertLevelGridTemplate.html'
        };

        function AlertLevelGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.alertLevel = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_Notification_AlertLevelService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Notification_AlertLevelAPIService.GetFilteredAlertLevels(dataRetrievalInput).then(function (response) {
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

                api.onAlertLevelAdded = function (addedAlertLevel) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedAlertLevel);
                    gridAPI.itemAdded(addedAlertLevel);
                };

                api.onAlertLevelUpdated = function (updatedAlertLevel) {
                    gridAPI.itemUpdated(updatedAlertLevel);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editAlertLevel,
                  
                });
            }

            
            function editAlertLevel(alertLevelItem) {
               
                var onAlertLevelUpdated = function (updatedAlertLevel) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedAlertLevel);
                    gridAPI.itemUpdated(updatedAlertLevel);
                };

                VR_Notification_AlertLevelService.editAlertLevel(alertLevelItem.Entity.VRAlertLevelId, onAlertLevelUpdated);
            }
        }
    }]);
