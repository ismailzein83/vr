﻿'use strict';

app.directive('whsRoutingActionBlocksalezone', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockSaleZoneAction = new BlockSaleZoneAction($scope, ctrl, $attrs);
            blockSaleZoneAction.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockSaleZone/Templates/BlockSaleZoneAction.html'
    };

    function BlockSaleZoneAction($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        function initializeController() {
        }

        function defineAPI() {

            var api = {};


            api.load = function (payload) {

                var promises = [];

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Routing.MainExtensions.BlockSaleZoneAction,TOne.WhS.Routing.MainExtensions',
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);