(function (app) {

    'use strict';

   SalePriceListObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

   function SalePriceListObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var splObjectType = new SPLDierctiveObjectType($scope, ctrl, $attrs);
                splObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl:  '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/SalePriceListObjectTypeTemplate.html'


        };
        function SPLDierctiveObjectType($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SalePriceListObjectType, TOne.WhS.BusinessEntity.MainExtensions",
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

   app.directive('vrWhsBeSalepricelistobjecttype', SalePriceListObjectType);

})(app);