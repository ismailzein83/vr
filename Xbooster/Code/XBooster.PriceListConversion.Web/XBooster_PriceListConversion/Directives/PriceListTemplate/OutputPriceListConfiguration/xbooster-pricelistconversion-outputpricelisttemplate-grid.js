"use strict";

app.directive("xboosterPricelistconversionOutputpricelisttemplateGrid", ["UtilsService", "VRNotificationService", "XBooster_PriceListConversion_PriceListTemplateAPIService", "XBooster_PriceListConversion_PriceListTemplateService",
    function (UtilsService, VRNotificationService, XBooster_PriceListConversion_PriceListTemplateAPIService, XBooster_PriceListConversion_PriceListTemplateService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var pricelistTemplateGrid = new PricelistTemplateGrid($scope, ctrl, $attrs);
                pricelistTemplateGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/XBooster_PriceListConversion/Directives/PriceListTemplate/OutputPriceListConfiguration/Templates/OutputPriceListTemplateGrid.html"

        };

        function PricelistTemplateGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            function initializeController() {

                $scope.priceListTemplates = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onPriceListTemplateAdded = function (priceListTemplateObj) {
                            gridAPI.itemAdded(priceListTemplateObj);
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return XBooster_PriceListConversion_PriceListTemplateAPIService.GetFilteredInputPriceListTemplates(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };

                defineMenuActions();
            }


            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editPriceListTemplate,
                    haspermission: hasUpdateOutputPriceListTemplatePermission
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function hasUpdateOutputPriceListTemplatePermission() {
                return XBooster_PriceListConversion_PriceListTemplateAPIService.HasUpdateOutputPriceListTemplatePermission();
            };
            function editPriceListTemplate(dataItem) {
                var onPriceListTemplateUpdated = function (priceListTemplateObj) {
                    gridAPI.itemUpdated(priceListTemplateObj);
                };
                XBooster_PriceListConversion_PriceListTemplateService.editOutputPriceListTemplate(dataItem.Entity.PriceListTemplateId, onPriceListTemplateUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);