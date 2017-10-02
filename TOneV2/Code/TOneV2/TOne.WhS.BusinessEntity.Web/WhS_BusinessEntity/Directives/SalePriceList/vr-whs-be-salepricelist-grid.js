"use strict";

app.directive("vrWhsBeSalepricelistGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SalePricelistAPIService", "FileAPIService"
                                            , "WhS_BE_SalePriceListOwnerTypeEnum", "WhS_BE_SalePriceListChangeService",
function (utilsService, vrNotificationService, whSBeSalePricelistApiService, fileApiService, whSBeSalePriceListOwnerTypeEnum, whSBeSalePriceListPreviewService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            defaultSortDirection: "@",
            defaultSortByFieldName: "@"

        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SalePriceListGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SalePriceList/Templates/SalePriceListGridTemplate.html"

    };

    function SalePriceListGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;
        var context;

        function initializeController() {
            $scope.salePriceLists = [];
            $scope.hideSelectedColumn = true;
            defineMenuActions();

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.load = function (payload) {
                        var query;

                        if (payload != undefined) {
                            query = payload.query;
                            context = payload.context;
                            if (payload.HideSelectedColumn != undefined) {
                                $scope.hideSelectedColumn = payload.HideSelectedColumn;
                            }
                        }
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.toggleSelection = function (toggleValue) {
                        for (var i = 0; i < $scope.salePriceLists.length; i++) {
                            var item = $scope.salePriceLists[i];
                            item.isSelected = toggleValue;
                        }
                    };
                    directiveAPI.getData = function () {
                        var selectedPriceListIds = [];
                        var notSelectedPriceListIds = [];

                        for (var i = 0; i < $scope.salePriceLists.length; i++) {
                            var salePriceList = $scope.salePriceLists[i];
                            if (salePriceList.isSelected)
                                selectedPriceListIds.push(salePriceList.Entity.PriceListId);
                            else
                                notSelectedPriceListIds.push(salePriceList.Entity.PriceListId);
                        }

                        return {
                            selectedPriceListIds: selectedPriceListIds,
                            notSelectedPriceListIds: notSelectedPriceListIds
                        };
                    };
                    directiveAPI.previewIfSinglePriceList = function () {
                        var priceListToPreview;
                        if ($scope.salePriceLists.length == 1)
                            priceListToPreview = $scope.salePriceLists[0];
                        else {
                            var numberOfSelectedPriceLists = 0;
                            for (var i = 0; i < $scope.salePriceLists.length; i++) {
                                if ($scope.salePriceLists[i].isSelected) {
                                    priceListToPreview = $scope.salePriceLists[i];
                                    numberOfSelectedPriceLists++;
                                }
                            }
                            if (numberOfSelectedPriceLists > 1)
                                return false;
                        }
                        if (priceListToPreview != undefined) {
                            PreviewPriceList(priceListToPreview, true);
                            return true;
                        }
                        return false;
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return whSBeSalePricelistApiService.GetFilteredSalePriceLists(dataRetrievalInput)
                    .then(function (response) {
                        if (response != undefined && response.Data != null) {
                            var selectAllValue = context.getSelectAllValue();
                            for (var i = 0; i < response.Data.length; i++) {
                                response.Data[i].isSelected = selectAllValue;
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        vrNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };
        }
        function defineMenuActions() {
            $scope.gridMenuActions = function (dataItem) {
                var menuActions = [];
                var sourceId = dataItem.Entity.SourceId;
                if (typeof sourceId == 'undefined' || sourceId == null) {
                    var labelSendValue = "Resend";

                    var salePriceListPreview = {
                        name: "Preview",
                        clicked: PreviewPriceList
                    };
                    menuActions.push(salePriceListPreview);
                    if (context == undefined || context.processInstanceId == undefined) {
                        if (dataItem.Entity.IsSent === false) {
                            labelSendValue = "Send";
                        }
                        var salePriceListSend =
                        {
                            name: labelSendValue,
                            clicked: SendPriceList
                        };
                        // menuActions.push(salePriceListSend);
                    }
                }
                if (dataItem.Entity.FileId !== 0) {
                    var downloadPricelist = {
                        name: "Download",
                        clicked: downloadPriceList
                    };
                    menuActions.push(downloadPricelist);
                }
                return menuActions;
            };
        }

        function SendPriceList(priceListObj) {
            whSBeSalePricelistApiService.SendPriceList(priceListObj.Entity.PriceListId)
                .then(function (response) {
                    vrNotificationService.showSuccess('Email Sent Successfully');
                });
        }
        function PreviewPriceList(priceListObj, shouldOpenEmailPage) {
            var onSalePriceListPreviewClosed = (context != undefined) ? context.onSalePriceListPreviewClosed : undefined;
            whSBeSalePriceListPreviewService.previewPriceList(priceListObj.Entity.PriceListId, onSalePriceListPreviewClosed, shouldOpenEmailPage);
        }
        function downloadPriceList(priceListObj) {
            fileApiService.DownloadFile(priceListObj.Entity.FileId)
                    .then(function (response) {
                        utilsService.downloadFile(response.data, response.headers);
                    });
        }
    }

    return directiveDefinitionObject;

}]);
