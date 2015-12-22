"use strict";

app.directive("vrSplRatepreviewGrid", ["WhS_SupPL_SupplierPriceListPreviewPIService", "VRNotificationService", "WhS_SupPL_RateChangeTypeEnum",
function (WhS_SupPL_SupplierPriceListPreviewPIService, VRNotificationService, WhS_SupPL_RateChangeTypeEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ratePreviewGrid = new RatePreviewGrid($scope, ctrl, $attrs);
                ratePreviewGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/RatePreview/Templates/SupplierPriceListRatePreviewGridTemplate.html"

        };

        function RatePreviewGrid($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
              
                $scope.changedRates = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());
                    function getDirectiveAPI() {

                        var directiveAPI = {};
                        directiveAPI.load = function (query) {

                            return gridAPI.retrieveData(query);
                        }
                        
                        return directiveAPI;
                    }
                };
                function getValueType(enumname, value) {
                    switch (value) {
                        case enumname.NotChanged.value:
                            return enumname.NotChanged.description;
                            break;
                        case enumname.New.value:
                            return enumname.New.description;
                            break;
                        case enumname.Increase.value:
                            return enumname.Increase.description;
                            break;
                        case enumname.Decrease.value:
                            return enumname.Decrease.description;
                            break;
                        default:
                            return undefined;
                    }
                }
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_SupPL_SupplierPriceListPreviewPIService.GetFilteredRatePreview(dataRetrievalInput)
                        .then(function (response) {
                            for (var i = 0 ; i < response.Data.length ; i++) {
                                response.Data[i].ChangeTypeText = getValueType(WhS_SupPL_RateChangeTypeEnum, response.Data[i].ChangeType)
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };
            }
        }

        return directiveDefinitionObject;
        
}]);
