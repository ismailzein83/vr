"use strict";

app.directive("vrWhsBeSalepricelistcodechangeGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SalePriceListChangeAPIService", "WhS_BE_CodeChangeTypeEnum",
function (utilsService, vrNotificationService, whSBeSalePricelistChangeApiService, whSBeCodeChangeTypeEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new CodeChangeGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListChange/Templates/SalePriceListCodeChangeTemplate.html"
    };

    function CodeChangeGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.CodeChange = [];
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
                return whSBeSalePricelistChangeApiService.GetFilteredSalePriceListCodeChanges(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var item = response.Data[i];
                                SetCodeChange(item);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        vrNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };
        }
        function SetCodeChange(dataItem) {
            switch (dataItem.ChangeType) {
                case whSBeCodeChangeTypeEnum.New.value:
                    dataItem.CodeChangeTypeIcon = whSBeCodeChangeTypeEnum.New.iconUrl;
                    dataItem.CodeChangeTypeIconTooltip = whSBeCodeChangeTypeEnum.New.description;
                    dataItem.CodeChangeTypeIconType = whSBeCodeChangeTypeEnum.New.iconType;
                    break;
                case whSBeCodeChangeTypeEnum.Closed.value:
                    dataItem.CodeChangeTypeIcon = whSBeCodeChangeTypeEnum.Closed.iconUrl;
                    dataItem.CodeChangeTypeIconTooltip = whSBeCodeChangeTypeEnum.Closed.description;
                    dataItem.CodeChangeTypeIconType = whSBeCodeChangeTypeEnum.Closed.iconType;
                    break;

                case whSBeCodeChangeTypeEnum.Moved.value:
                    dataItem.CodeChangeTypeIcon = whSBeCodeChangeTypeEnum.Moved.iconUrl;
                    dataItem.CodeChangeTypeIconTooltip = whSBeCodeChangeTypeEnum.Moved.description;
                    dataItem.CodeChangeTypeIconType = whSBeCodeChangeTypeEnum.Moved.iconType;
                    break;
            }
        }
    }
    return directiveDefinitionObject;
}]);
