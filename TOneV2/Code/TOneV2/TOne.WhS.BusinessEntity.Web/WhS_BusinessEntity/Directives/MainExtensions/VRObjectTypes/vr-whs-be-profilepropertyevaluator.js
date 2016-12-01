(function (app) {

    'use strict';

    ProfilePropertyEvaluator.$inject = ['WhS_BE_ProfilePropertyEnum', 'UtilsService', 'VRUIUtilsService'];

    function ProfilePropertyEvaluator(WhS_BE_ProfilePropertyEnum, UtilsService, VRUIUtilsService) {
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
                var selector = new ProfileSelectorPropertyEvaluator($scope, ctrl, $attrs);
                selector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/ProfilePropertyEvaluatorTemplate.html'
        };

        function ProfileSelectorPropertyEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.profileFields = UtilsService.getArrayEnum(WhS_BE_ProfilePropertyEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        $scope.scopeModel.selectedProfileField = UtilsService.getItemByVal($scope.scopeModel.profileFields, payload.objectPropertyEvaluator.ProfileField, "value");
                };

                api.getData = function () {

                    var data = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.ProfilePropertyEvaluator, TOne.WhS.BusinessEntity.MainExtensions",
                        ProfileField: $scope.scopeModel.selectedProfileField.value
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrWhsBeProfilepropertyevaluator', ProfilePropertyEvaluator);

})(app);
