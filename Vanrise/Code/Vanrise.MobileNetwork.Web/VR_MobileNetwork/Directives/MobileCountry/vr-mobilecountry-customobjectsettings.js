(function (app) {
    'use strict';
    mobileCountrySettingsType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function mobileCountrySettingsType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new MNCSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_MobileNetwork/Directives/MobileCountry/Templates/MobileCountrySettingsCustomObjectSettingsTemplate.html"
        };
        function MNCSettings($scope, ctrl, $attrs) {
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
                        $type: "Vanrise.MobileNetwork.Business.MobileCountrySettingsCustomObjectTypeSettings, Vanrise.MobileNetwork.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrMobilecountryCustomobjectsettings', mobileCountrySettingsType);

})(app);
