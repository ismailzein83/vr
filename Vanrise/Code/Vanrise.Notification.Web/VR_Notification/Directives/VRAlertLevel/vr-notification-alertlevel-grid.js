'use strict';

app.directive('vrNotificationAlertlevelGrid', ['VR_Notification_AlertLevelAPIService', 'VR_Notification_AlertLevelService', 'VRNotificationService',
    function (VR_Notification_AlertLevelAPIService, VR_Notification_AlertLevelService, VRNotificationService) {
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

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.alertLevel = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Notification_AlertLevelAPIService.GetFilteredAlertLevels(dataRetrievalInput).then(function (response) {
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
                    gridAPI.itemUpdated(updatedAlertLevel);
                };

                VR_Notification_AlertLevelService.editAlertLevel(alertLevelItem.Entity.VRAlertLevelId, onAlertLevelUpdated);
            }
        }
    }]);
