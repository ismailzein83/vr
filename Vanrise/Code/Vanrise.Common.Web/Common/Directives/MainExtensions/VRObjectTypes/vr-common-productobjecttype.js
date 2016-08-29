(function (app) {

    'use strict';

    ProductObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function ProductObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrProductObjectType = new VRProductObjectType($scope, ctrl, $attrs);
                vrProductObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/MainExtensions/VRObjectTypes/Templates/ProductObjectTypeTemplate.html"

        };
        function VRProductObjectType($scope, ctrl, $attrs) {
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
                        $type: "Vanrise.Common.MainExtensions.VRObjectTypes.ProductObjectType, Vanrise.Common.MainExtensions",
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrCommonProductobjecttype', ProductObjectType);

})(app);