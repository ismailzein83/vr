(function (app) {

    'use strict';

    UserProfilePropertyEvaluator.$inject = ['VR_Sec_UserProfilePropertyEnum', 'UtilsService', 'VRUIUtilsService'];

    function UserProfilePropertyEvaluator(VR_Sec_UserProfilePropertyEnum, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var userProfilePropertyEvaluatorSecurity = new UserProfilePropertyEvaluatorSecurity($scope, ctrl, $attrs);
                userProfilePropertyEvaluatorSecurity.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Security/Directives/MainExtensions/VRObjectTypes/Templates/UserProfilePropertyEvaluatorTemplate.html'
        };

        function UserProfilePropertyEvaluatorSecurity($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.userFields = UtilsService.getArrayEnum(VR_Sec_UserProfilePropertyEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        $scope.scopeModel.selectedUserField = UtilsService.getItemByVal($scope.scopeModel.userFields, payload.objectPropertyEvaluator.UserField, "value");
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Security.MainExtensions.VRObjectTypes.UserProfilePropertyEvaluator, Vanrise.Security.MainExtensions",
                        UserField: $scope.scopeModel.selectedUserField.value
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrSecUserprofilepropertyevaluator', UserProfilePropertyEvaluator);

})(app);
