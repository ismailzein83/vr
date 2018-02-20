(function (app) {

    'use strict';

    HistoryGenericBEDefinitionViewDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function HistoryGenericBEDefinitionViewDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HistoryGenericBEDefinitionViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEViewDefinition/Templates/HistoryGenericBEDefinitionViewTemplate.html'
        };

        function HistoryGenericBEDefinitionViewCtor($scope, ctrl) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                };


                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.HistoryGenericBEDefinitionView, Vanrise.GenericData.MainExtensions"
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeHistorygridviewDefinition', HistoryGenericBEDefinitionViewDirective);

})(app);