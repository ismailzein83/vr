﻿'use strict';

app.directive('vrWhsSalesBulkactionTypeBedAllzones', ['WhS_Sales_BulkActionUtilsService', 'UtilsService', 'VRUIUtilsService', function (WhS_Sales_BulkActionUtilsService, UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var bedBulkActionType = new BEDBulkActionType($scope, ctrl, $attrs);
            bedBulkActionType.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function BEDBulkActionType($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var bulkActionContext;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onBEDChanged = function () {
                WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
            };

            defineAPI();
        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                if (payload != undefined) {
                    bulkActionContext = payload.bulkActionContext;
                    if (payload.bulkAction != undefined)
                        $scope.scopeModel.beginEffectiveDate = payload.bulkAction.BED;
                }
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Sales.MainExtensions.BEDAllZonesBulkActionType, TOne.WhS.Sales.MainExtensions',
                    BED: $scope.scopeModel.beginEffectiveDate
                };
            };

            api.getSummary = function () {
                var bedAsString = ($scope.scopeModel.beginEffectiveDate != undefined) ? UtilsService.getShortDate($scope.scopeModel.beginEffectiveDate) : 'None';
                return 'BED: ' + bedAsString;
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }

    function getTemplate(attrs) {
        return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-datetimepicker type="date" label="BED" value="scopeModel.beginEffectiveDate" onvaluechanged="scopeModel.onBEDChanged" isrequired="ctrl.isrequired"></vr-datetimepicker></vr-columns>';
    }
}]);