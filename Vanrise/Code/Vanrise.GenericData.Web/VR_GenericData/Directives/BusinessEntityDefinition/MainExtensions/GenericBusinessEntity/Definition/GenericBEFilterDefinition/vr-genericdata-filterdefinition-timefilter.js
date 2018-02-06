(function (app) {

    'use strict';

    GenericBETimeFilterDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function GenericBETimeFilterDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TimeFilterCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/TimeFilterDefiitionTemplate.html'
        };

        function TimeFilterCtor($scope, ctrl) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {};


                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.TimeFilterDefinitionSettings, Vanrise.GenericData.MainExtensions"
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataFilterdefinitionTimefilter', GenericBETimeFilterDirective);

})(app);