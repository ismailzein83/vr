(function (app) {

    'use strict';

    LimitResultFilterRuntimeSettingsDirective.$inject = ['UtilsService'];

    function LimitResultFilterRuntimeSettingsDirective(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new LimitResultFilterRuntimeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEFilterDefinition/Templates/LimitResultFilterRuntimeTemplate.html"
        };


        function LimitResultFilterRuntimeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var dataRecordTypeId;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                
                api.load = function (payload) {
                    
                    var promises = [];
                    if (payload != undefined) {
                        $scope.scopeModel.limitResult = payload.settings.DefaultLimitResult;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        LimitResult: $scope.scopeModel.limitResult
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterruntimeLimitresult', LimitResultFilterRuntimeSettingsDirective);

})(app);