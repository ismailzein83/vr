"use strict";

app.directive("vrWhsBeRateTypeGrid", ["UtilsService", "VRNotificationService", "WhS_BE_RateTypeAPIService", "WhS_BE_MainService",
function (UtilsService, VRNotificationService, WhS_BE_RateTypeAPIService, WhS_BE_MainService) {

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
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/RateType/Templates/RateTypeGridTemplate.html"

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
                    }
                    directiveAPI.onRateTypeAdded = function (rateTypeObject) {
                        gridAPI.itemAdded(rateTypeObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_RateTypeAPIService.GetFilteredRateTypes(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                setDataItemExtension(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }

        function setDataItemExtension(dataItem) {

            var extensionObject = {};
            var query = {
                RateTypesIds: [dataItem.RateTypeId],
            }
            extensionObject.onGridReady = function (api) {
                extensionObject.onGridReady = undefined;
            };
            dataItem.extensionObject = extensionObject;

        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editRateType
            }
            ];
        }

        function editRateType(rateTypeObj) {
            var onRateTypeUpdated = function (rateTypeObj) {
                setDataItemExtension(rateTypeObj);
                gridAPI.itemUpdated(rateTypeObj);
            }

            WhS_BE_MainService.editRateType(rateTypeObj, onRateTypeUpdated);
        }

    }

    return directiveDefinitionObject;

}]);
