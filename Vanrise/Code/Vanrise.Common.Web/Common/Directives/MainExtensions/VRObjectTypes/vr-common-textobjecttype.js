(function (app) {

    'use strict';

    TextObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function TextObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrTextObjectType = new VRTextObjectType($scope, ctrl, $attrs);
                vrTextObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/MainExtensions/VRObjectTypes/Templates/TextObjectTypeTemplate.html"

        };
        function VRTextObjectType($scope, ctrl, $attrs) {
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
                        $type: "Vanrise.Common.MainExtensions.VRObjectTypes.TextObjectType, Vanrise.Common.MainExtensions",
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrCommonTextobjecttype', TextObjectType);

})(app);