(function (app) {

    'use strict';

    TextValuePropertyEvaluator.$inject = ['UtilsService', 'VRUIUtilsService'];

    function TextValuePropertyEvaluator(UtilsService, VRUIUtilsService) {
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
                var textValuePropertyEvaluatorSecurity = new TextValuePropertyEvaluatorSecurity($scope, ctrl, $attrs);
                textValuePropertyEvaluatorSecurity.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/MainExtensions/VRObjectTypes/Templates/TextValuePropertyEvaluatorTemplate.html'
        };

        function TextValuePropertyEvaluatorSecurity($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                //$scope.scopeModel.userFields = UtilsService.getArrayEnum(VR_Sec_TextValuePropertyEnum);

                //$scope.scopeModel.onSelectorReady = function (api) {
                //    selectorAPI = api;
                //    defineAPI();
                //};
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    //if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                    //    $scope.scopeModel.selectedUserField = UtilsService.getItemByVal($scope.scopeModel.userFields, payload.objectPropertyEvaluator.UserField, "value");
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Security.MainExtensions.VRObjectTypes.TextValuePropertyEvaluator, Vanrise.Security.MainExtensions",
                        TextField: "Value"
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrCommonTextvaluepropertyevaluator', TextValuePropertyEvaluator);

})(app);
