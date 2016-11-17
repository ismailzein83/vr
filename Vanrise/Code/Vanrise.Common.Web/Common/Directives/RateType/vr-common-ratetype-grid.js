"use strict";
app.directive("vrCommonRatetypeGrid", ["UtilsService", "VRNotificationService", "VRCommon_RateTypeAPIService", "VRCommon_RateTypeService",
function (UtilsService, VRNotificationService, VRCommon_RateTypeAPIService, VRCommon_RateTypeService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var rateTypeGrid = new RateTypeGrid($scope, ctrl, $attrs);
            rateTypeGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/RateType/Templates/RateTypeGridTemplate.html"

    };

    function RateTypeGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.rateTypes = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onRateTypeAdded = function (rateTypeObject) {
                        gridAPI.itemAdded(rateTypeObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_RateTypeAPIService.GetFilteredRateTypes(dataRetrievalInput)
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
                clicked: editRateType,
                haspermission: hasUpdateRateTypePermission
            }];
        }

        function hasUpdateRateTypePermission() {
            return VRCommon_RateTypeAPIService.HasUpdateRateTypePermission();
        }

        function editRateType(rateType) {
            var onRateTypeUpdated = function (updatedRateType) {
                gridAPI.itemUpdated(updatedRateType);
            };
            VRCommon_RateTypeService.editRateType(rateType.Entity.RateTypeId, onRateTypeUpdated);
        }
    }

    return directiveDefinitionObject;

}]);