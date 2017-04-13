"use strict";

app.directive("vrWhsBeSalepricelistGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SalePricelistAPIService", "FileAPIService"
                                            , "WhS_BE_SalePriceListOwnerTypeEnum", "WhS_BE_SalePriceListChangeService",
function (utilsService, vrNotificationService, whSBeSalePricelistApiService, fileApiService, whSBeSalePriceListOwnerTypeEnum, whSBeSalePriceListPreviewService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
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

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.salepricelist = [];
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
                return whSBeSalePricelistApiService.GetFilteredSalePriceLists(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        vrNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
        }


        function defineMenuActions() {
            $scope.gridMenuActions = function (dataItem) {
                var menuActions = [];
                var labelSendValue = "Resend";
                var salePriceListPreview =
                {
                    name: "Preview PriceList",
                    clicked: PreviewPriceList
                };
                menuActions.push(salePriceListPreview);
                if (dataItem.Entity.OwnerType === whSBeSalePriceListOwnerTypeEnum.Customer.value) {
                    var downloadPricelist = {
                        name: "Download",
                        clicked: downloadPriceList
                    };
                    menuActions.push(downloadPricelist);
                }
                if (dataItem.Entity.IsSent === false) {
                    labelSendValue = "Send";
                }
                var salePriceListSend =
                    {
                        name: labelSendValue,
                        clicked: SendPriceList
                    }
                menuActions.push(salePriceListSend);
                return menuActions;
            };

        }
        function SendPriceList(priceListObj) {
            whSBeSalePricelistApiService.SendPriceList(priceListObj.Entity.PriceListId)
                .then(function (response) {
                    vrNotificationService.showSuccess('Email Sended Successfully');
                });
        }
        function PreviewPriceList(priceListObj) {
            whSBeSalePriceListPreviewService.previewPriceList(priceListObj.Entity.PriceListId);
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
