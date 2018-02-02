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

        var gridDrillDownTabsObj;
        var drillDownDefinitions = [];

        this.initializeController = initializeController;

        function initializeController() {
            $scope.isExpandable = function (dataItem) {
                if (vrLoggableEntitySettings.ChangeInfoDefinition != undefined) {
                    if (vrLoggableEntitySettings.ChangeInfoDefinition.RuntimeEditor != undefined && dataItem.Entity.HasChangeInfo)
                        return true;

                    if (vrLoggableEntitySettings.ChangeInfoDefinition.ObjectRuntimeEditor != undefined && dataItem.Entity.HasDetail)
                        return true;
                }
                return false;
            };
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
                                if (vrLoggableEntitySettings != null) {
                                    actionHistoryName = vrLoggableEntitySettings.ViewHistoryItemClientActionName;
                                    viewHistoryAction = VRCommon_ObjectTrackingService.getActionTrackIfExist(actionHistoryName);

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



            function getVRLoggableEntitySettings() {
                return VRCommon_ObjectTrackingAPIService.GetVRLoggableEntitySettings(uniqueName).then(function (response) {
                    vrLoggableEntitySettings = response;
                });
            }

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_ObjectTrackingAPIService.GetFilteredObjectTracking(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                AddChangeInfoDrillDown(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            function AddChangeInfoDrillDown(dataItem) {
                if (vrLoggableEntitySettings != undefined && vrLoggableEntitySettings.ChangeInfoDefinition != undefined) {


                    if (vrLoggableEntitySettings.ChangeInfoDefinition.RuntimeEditor != undefined && dataItem.Entity.HasChangeInfo) {
                        dataItem.changeInfoDirective = vrLoggableEntitySettings.ChangeInfoDefinition.RuntimeEditor;
                        dataItem.onChangeInfoDirectiveReady = function (api) {
                            dataItem.changeInfoDirectiveAPI = api;
                            dataItem.loadChangeInfoDirective = true;
                            VRCommon_ObjectTrackingAPIService.GetObjectTrackingChangeInfo(dataItem.Entity.VRObjectTrackingId).then(function (response) {
                                var query = {
                                    changeInfoDefinition: vrLoggableEntitySettings.ChangeInfoDefinition,
                                    changeInfo: response,
                                    loggableEntityUniqueName: uniqueName
                                };
                                dataItem.changeInfoDirectiveAPI.load(query).then(function () {
                                    dataItem.loadChangeInfoDirective = false;
                                });
                            });
                        };

                    }

                    if (vrLoggableEntitySettings.ChangeInfoDefinition.ObjectRuntimeEditor != undefined && dataItem.Entity.HasDetail) {
                        dataItem.changeInfoDirective = vrLoggableEntitySettings.ChangeInfoDefinition.ObjectRuntimeEditor;
                        dataItem.onChangeInfoDirectiveReady = function (api) {
                            dataItem.changeInfoDirectiveAPI = api;
                            dataItem.loadChangeInfoDirective = true;

                            var query = {
                                changeInfoDefinition: vrLoggableEntitySettings.ChangeInfoDefinition,
                                historyId: dataItem.Entity.VRObjectTrackingId,
                                loggableEntityUniqueName: uniqueName
                            };
                            dataItem.changeInfoDirectiveAPI.load(query).then(function () {
                                dataItem.loadChangeInfoDirective = false;
                            });

                        };
                    }
            }
        }

            var defaultMenuActions =[{
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