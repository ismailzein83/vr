(function (app) {

    'use strict';

    HistoryGenericBEDefinitionViewDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function HistoryGenericBEDefinitionViewDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HistoryGenericBEDefinitionViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/GenericBEStaticEditorDefiitionTemplate.html'
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
                    var settings;
                    if (payload != undefined)
                        settings = payload.settings;

                    if (settings != undefined)
                        $scope.scopeModel.directiveName = settings.DirectiveName;

                };


                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.StaticEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        DirectiveName: $scope.scopeModel.directiveName
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataStaticeditorDefinition', HistoryGenericBEDefinitionViewDirective);

})(app);