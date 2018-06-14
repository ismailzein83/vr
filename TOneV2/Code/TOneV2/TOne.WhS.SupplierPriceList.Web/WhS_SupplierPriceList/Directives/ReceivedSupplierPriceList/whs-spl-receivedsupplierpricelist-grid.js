"use strict";

app.directive("whsSplReceivedsupplierpricelistGrid", ["UtilsService", "VRNotificationService", "WhS_SupPL_ReceivedSupplierPricelistAPIService","FileAPIService","WhS_BE_SupplierPriceListService","BusinessProcess_BPInstanceService",
function (UtilsService, VRNotificationService, WhS_SupPL_ReceivedSupplierPricelistAPIService, FileAPIService, WhS_BE_SupplierPriceListService,BusinessProcess_BPInstanceService) {
    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new ReceivedSupplierPricelistGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_SupplierPricelist/Directives/ReceivedSupplierPricelist/Templates/ReceivedSupplierPricelistGridTemplate.html"

    };

    function ReceivedSupplierPricelistGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.receivedPricelists = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_SupPL_ReceivedSupplierPricelistAPIService.GetFilteredReceivedSupplierPricelist(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            $scope.viewDetails = function (pricelistObj) {
                viewTracker(pricelistObj);
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = function (dataItem) {
                var menuActions = [];
                var processInstanceId = dataItem.ReceivedPricelist.ProcessInstanceId;
                if (dataItem.ReceivedPricelist.FileId !== 0 && dataItem.ReceivedPricelist.FileId != null) {
                    var downloadPricelistAction = {
                        name: "Download",
                        clicked: downloadPricelist
                    };
                    menuActions.push(downloadPricelistAction);
                }

                if (processInstanceId != null && dataItem.StatusDescription=="Succeeded") {
                    var additionalMenuActions = WhS_BE_SupplierPriceListService.getAdditionalActionOfSupplierPricelistGrid();
                    for (var i = 0, length = additionalMenuActions.length; i < length; i++) {
                        var additionalMenuAction = additionalMenuActions[i];
                        var menuAction = {
                            name: additionalMenuAction.name,
                            clicked: function (dataItem) {
                                var payload = {
                                    processInstanceId: dataItem.ReceivedPricelist.ProcessInstanceId
                                };
                                additionalMenuAction.clicked(payload);
                            }
                        };
                        menuActions.push(menuAction);
                    }
                }

                if (processInstanceId != null) {
                    var viewTrackerMenuAction = {
                        name: "View Tracker",
                        clicked: viewTracker,
                    };
                    menuActions.push(viewTrackerMenuAction);
                }
                return menuActions;
            };
        }

        function downloadPricelist(pricelistObj) {
            FileAPIService.DownloadFile(pricelistObj.ReceivedPricelist.FileId)
                    .then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });
        }

        function viewTracker(pricelistObj) {
            return BusinessProcess_BPInstanceService.openProcessTracking(pricelistObj.ReceivedPricelist.ProcessInstanceId);
        }
    }

    return directiveDefinitionObject;

}]);
