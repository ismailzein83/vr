"use strict";

app.directive("vrCommonTimezoneGrid", ["UtilsService", "VRNotificationService", "VRCommon_VRTimeZoneAPIService", "VRCommon_VRTimeZoneService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRCommon_VRTimeZoneAPIService, VRCommon_VRTimeZoneService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var timeZoneGrid = new VRTimeZoneGrid($scope, ctrl, $attrs);
            timeZoneGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/VRTimeZone/Templates/VRTimeZoneGridTemplate.html"

    };

    function VRTimeZoneGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        function initializeController() {
           
            $scope.timeZones = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
          
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                       
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onVRTimeZoneAdded = function (timeZoneObject) {
                        gridAPI.itemAdded(timeZoneObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_VRTimeZoneAPIService.GetFilteredVRTimeZones(dataRetrievalInput)
                    .then(function (response) {                          
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();            
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editVRTimeZone,
                haspermission: hasEditVRTimeZonePermission
            }];
        }

        function hasEditVRTimeZonePermission() {
            return VRCommon_VRTimeZoneAPIService.HasEditVRTimeZonePermission();
        }

        function editVRTimeZone(timeZoneObj) {
            var onVRTimeZoneUpdated = function (timeZoneObj) {
                gridAPI.itemUpdated(timeZoneObj);
            }

            VRCommon_VRTimeZoneService.editVRTimeZone(timeZoneObj.Entity.TimeZoneId, onVRTimeZoneUpdated);
        }
              
    }

    return directiveDefinitionObject;

}]);
