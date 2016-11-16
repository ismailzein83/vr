"use strict";

app.directive("whsSplCodepreviewGrid", ["WhS_SupPL_SupplierPriceListPreviewPIService", "WhS_SupPL_CodeChangeTypeEnum", "VRNotificationService",
function (WhS_SupPL_SupplierPriceListPreviewPIService, WhS_SupPL_CodeChangeTypeEnum, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var codePreviewGrid = new CodePreviewGrid($scope, ctrl, $attrs);
            codePreviewGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/CodePreview/Templates/SupplierPriceListCodePreviewGridTemplate.html"

    };

    function CodePreviewGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var zoneName;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.changedCodes = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.load = function (query) {
                        zoneName = query.ZoneName;
                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };


            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_SupPL_SupplierPriceListPreviewPIService.GetFilteredCodePreview(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                mapDataNeeded(response.Data[i]);
                            }
                        }

                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }


        function mapDataNeeded(dataItem) {
            dataItem.showMovedTo = false;
            dataItem.showMovedFrom = false;
            switch (dataItem.Entity.ChangeType) {
                case WhS_SupPL_CodeChangeTypeEnum.New.value:
                    dataItem.codeStatusIconUrl = WhS_SupPL_CodeChangeTypeEnum.New.icon;
                    dataItem.codeStatusIconTooltip = WhS_SupPL_CodeChangeTypeEnum.New.description;
                    break;

                case WhS_SupPL_CodeChangeTypeEnum.Deleted.value:
                    dataItem.codeStatusIconUrl = WhS_SupPL_CodeChangeTypeEnum.Deleted.icon;
                    dataItem.codeStatusIconTooltip = WhS_SupPL_CodeChangeTypeEnum.Deleted.description;
                    break;

                case WhS_SupPL_CodeChangeTypeEnum.Moved.value:
                    dataItem.showMovedTo = true;
                    dataItem.showMovedFrom = true;
                    if (dataItem.Entity.ZoneName != zoneName) {
                        dataItem.codeStatusIconUrl = WhS_SupPL_CodeChangeTypeEnum.MovedFrom.icon;
                            dataItem.codeStatusIconTooltip = WhS_SupPL_CodeChangeTypeEnum.Moved.description + " to " + dataItem.Entity.ZoneName;
                        }
                        else {
                        dataItem.codeStatusIconUrl = WhS_SupPL_CodeChangeTypeEnum.MovedTo.icon;
                            dataItem.codeStatusIconTooltip = WhS_SupPL_CodeChangeTypeEnum.Moved.description + " from " + dataItem.Entity.RecentZoneName;
                        }
                    break;
            }
            
            if (zoneName == dataItem.Entity.ZoneName)
                dataItem.showMovedTo = false;

            if (dataItem.Entity.RecentZoneName== null || zoneName == dataItem.Entity.RecentZoneName)
                dataItem.showMovedFrom = false;
        }

    }

    return directiveDefinitionObject;

}]);
