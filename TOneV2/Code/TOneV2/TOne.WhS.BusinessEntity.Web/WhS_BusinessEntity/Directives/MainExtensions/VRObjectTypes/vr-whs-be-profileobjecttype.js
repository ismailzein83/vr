(function (app) {

    'use strict';

    ProfileObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function ProfileObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var customerObjectType = new ProfileDierctiveObjectType($scope, ctrl, $attrs);
                customerObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl:  '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/ProfileObjectTypeTemplate.html'


        };
        function ProfileDierctiveObjectType($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.ProfileObjectType, TOne.WhS.BusinessEntity.MainExtensions"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsBeProfileobjecttype', ProfileObjectType);

})(app);