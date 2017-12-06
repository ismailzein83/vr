'use strict';

app.directive('vrAccountmanagerviewHistory', ['UtilsService', 'VRUIUtilsService', 'VRNavigationService',
function (UtilsService, VRUIUtilsService, VRNavigationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new AccountManagerHistoryCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/Template/AccountManagerHistoryTemplate.html'
    };

    function AccountManagerHistoryCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;

        var context;
        var subViewEntity;

        function initializeController() {
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function () {
            };
            api.getData = function () {
                var obj = {
                    $type: " Vanrise.AccountManager.Business.AccountManagerSubviewHistory,Vanrise.AccountManager.Business",

                };

                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

    }

    return directiveDefinitionObject;
}]);