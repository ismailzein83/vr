"use strict";

app.directive("vrCommonObjecttrackingGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VRCommon_ObjectTrackingAPIService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, VRUIUtilsService, VRCommon_ObjectTrackingAPIService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var objecttrackingGrid = new objectTrackingGrid($scope, ctrl, $attrs);
            objecttrackingGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/ObjectTracking/Templates/ObjectTrackingGridTemplate.html"

    };

    function objectTrackingGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var actionHistoryName;
        var viewHistoryAction;
        var uniqueName;
        var vrLoggableEntitySettings;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.objects = [];
            $scope.onGridReady = function (api) {

                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.load = function (query) {
                        var promiseDeferred = UtilsService.createPromiseDeferred();
                        if (actionHistoryName == null) {
                            uniqueName = query.EntityUniqueName;
                            getVRLoggableEntitySettings().then(function () {
                                if (vrLoggableEntitySettings != null)
                                {
                                    actionHistoryName = vrLoggableEntitySettings.ViewHistoryItemClientActionName;
                                    viewHistoryAction = VRCommon_ObjectTrackingService.getActionTrackIfExist(actionHistoryName);
                                    if (vrLoggableEntitySettings.ChangeInfoDefinition != undefined)
                                    {
                                        AddDrillDownDefinition(vrLoggableEntitySettings.ChangeInfoDefinition);
                                    }

                                    return gridAPI.retrieveData(query).finally(function () {
                                        promiseDeferred.resolve();
                                    });
                                }
                              
                            }).catch(function (error) {
                                promiseDeferred.reject(error);
                                VRNotificationService.notifyException(error, $scope);
                            });
                        } else {
                            gridAPI.retrieveData(query).finally(function () {
                                promiseDeferred.resolve();
                            });
                        }
                        
                        return promiseDeferred.promise;
                    };
                    return directiveAPI;
                }
            };

            function AddDrillDownDefinition(changeInfoDefinition)
            {
                if(changeInfoDefinition != undefined)
                {
                }
            }

            function getVRLoggableEntitySettings() {
                return VRCommon_ObjectTrackingAPIService.GetVRLoggableEntitySettings(uniqueName).then(function (response) {
                    vrLoggableEntitySettings = response;
                });
            }

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_ObjectTrackingAPIService.GetFilteredObjectTracking(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            var defaultMenuActions = [{
                name: "View",
                clicked: viewHistory,
            }];

            $scope.gridMenuActions = function (dataItem) {
                if (dataItem.Entity.HasDetail && viewHistoryAction != undefined) {
                    return defaultMenuActions;
                }
            };

        }
        function viewHistory(dataItem) {
            var payload = {
                historyId: dataItem.Entity.VRObjectTrackingId
            };
            if (viewHistoryAction != undefined)
                viewHistoryAction.actionMethod(payload);
        }
    }

    return directiveDefinitionObject;

}]);