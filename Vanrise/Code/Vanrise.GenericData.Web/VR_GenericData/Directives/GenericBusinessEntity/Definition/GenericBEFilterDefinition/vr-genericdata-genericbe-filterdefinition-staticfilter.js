(function (app) {

    'use strict';

    GenericBEStaticFilterDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function GenericBEStaticFilterDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new StaticFilterCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/StaticFilterDefiitionTemplate.html'
        };

        function StaticFilterCtor($scope, ctrl) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var settings;
                    if (payload != undefined)
                        settings = payload.settings;

                    if (settings != undefined)
                        $scope.scopeModel.directiveName = settings.DirectiveName;

                };


                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.StaticFilterDefinitionSettings, Vanrise.GenericData.MainExtensions",
                        DirectiveName: $scope.scopeModel.directiveName
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterdefinitionStaticfilter', GenericBEStaticFilterDirective);

})(app);