﻿"use strict";

app.directive("vrCpRatepreviewGrid", ["WhS_CP_CodePreparationPreviewAPIService", "WhS_CP_CodeChangeTypeEnum", 'UtilsService', "VRNotificationService", 'VRDateTimeService',
function (WhS_CP_CodePreparationPreviewAPIService, WhS_CP_CodeChangeTypeEnum, UtilsService, VRNotificationService, VRDateTimeService) {

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
        templateUrl: "/Client/Modules/WhS_CodePreparation/Directives/RatePreview/Templates/CodePreparationRatePreviewGridTemplate.html"

    };

    function RatePreviewGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var zoneName;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.ratesPreview = [];
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
                return WhS_CP_CodePreparationPreviewAPIService.GetFilteredRatesPreview(dataRetrievalInput)
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
            var today = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
            var bed = UtilsService.createDateFromString(dataItem.Entity.BED);
            if (bed > today) {
                var roundedRate = UtilsService.round(dataItem.Entity.Rate, 4);
                dataItem.Entity.Rate = roundedRate + ' (Future)';
            }
        }

    }
    return directiveDefinitionObject;

}]);
