(function (app) {

    'use strict';

    CompanySettingObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function CompanySettingObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var companySettingObjectTypeSecurity = new CompanySettingObjectTypeSecurity($scope, ctrl, $attrs);
                companySettingObjectTypeSecurity.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/MainExtensions/VRObjectTypes/Templates/CompanySettingObjectTypeTemplate.html"

        };
        function CompanySettingObjectTypeSecurity($scope, ctrl, $attrs) {
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
                        $type: "Vanrise.Common.MainExtensions.VRObjectTypes.CompanySettingObjectType, Vanrise.Common.MainExtensions",
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrCommonCompanysettingobjecttype', CompanySettingObjectType);

})(app);