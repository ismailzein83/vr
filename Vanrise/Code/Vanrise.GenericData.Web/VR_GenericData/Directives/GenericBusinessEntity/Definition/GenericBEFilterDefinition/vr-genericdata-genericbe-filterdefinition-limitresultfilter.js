(function (app) {

    'use strict';

    GenericBELimitResultFilterDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function GenericBELimitResultFilterDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new LimitResultFilterCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/LimitResultFilterDefinitionTemplate.html'
        };

        function LimitResultFilterCtor($scope, ctrl) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                
                api.load = function (payload) {
                    if(payload != undefined && payload.settings != undefined) 
                        $scope.scopeModel.limitResult = payload.settings.DefaultLimitResult;
                };


                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.LimitResultFilterDefinitionSettings, Vanrise.GenericData.MainExtensions",
                        DefaultLimitResult: $scope.scopeModel.limitResult
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterdefinitionLimitresultfilter', GenericBELimitResultFilterDirective);

})(app);