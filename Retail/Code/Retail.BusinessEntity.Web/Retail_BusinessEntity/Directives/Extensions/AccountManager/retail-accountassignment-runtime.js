'use strict';

app.directive('retailAccountassignmentRuntime', ['UtilsService', 'VRUIUtilsService', 'VRNavigationService',
function (UtilsService, VRUIUtilsService, VRNavigationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new AccountAssignmentRuntimeCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Extensions/AccountManager/Templates/AccountAssignmentRuntimeTemplate.html"
    };

    function AccountAssignmentRuntimeCtor(ctrl, $scope, $attrs) {

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function () {
            };
            api.getData = function () {
                var obj = {
                    $type: "Retail.BusinessEntity.Business.RetailAccountAssignmentRuntime,Retail.BusinessEntity.Business",
                };

                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

    }

    return directiveDefinitionObject;
}]);