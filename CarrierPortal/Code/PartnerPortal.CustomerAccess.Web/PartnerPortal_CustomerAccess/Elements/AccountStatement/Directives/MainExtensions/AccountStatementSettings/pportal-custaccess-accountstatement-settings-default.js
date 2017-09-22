'use strict';


app.directive('pportalCustaccessAccountstatementSettingsDefault', ['UtilsService',
function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new defaultAccountStatementExtendedSettingsCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/PartnerPortal_CustomerAccess/Elements/AccountStatement/Directives/MainExtensions/AccountStatementSettings/Templates/DefaultAccountStatementExtendedSettings.html'
    };


    function defaultAccountStatementExtendedSettingsCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

            };

            api.getData = function () {
                var obj = {
                    $type: "PartnerPortal.CustomerAccess.MainExtensions.DefaultAccountStatementExtendedSettings, PartnerPortal.CustomerAccess.MainExtensions",
                };
                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);