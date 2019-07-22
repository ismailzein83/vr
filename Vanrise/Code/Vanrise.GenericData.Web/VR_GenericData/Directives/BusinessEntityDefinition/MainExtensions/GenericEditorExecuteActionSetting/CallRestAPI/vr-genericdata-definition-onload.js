(function (app) {

    'use strict';

    ExecuteActionEditorOnLoadDefinition.$inject = ["UtilsService"];

    function ExecuteActionEditorOnLoadDefinition(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ExecuteActionEditorOnLoadDefinitionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorExecuteActionSetting/CallRestAPI/Templates/ExecuteActionOnLoadDefinitionTemplate.html"

        };

        function ExecuteActionEditorOnLoadDefinitionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var context;
                    var definitionSettings;

                    if (payload != undefined) {
                        context = payload.context;
                        definitionSettings = payload.definitionSettings;
                    }

                    loadStaticData();

                    function loadStaticData() {
                        if (definitionSettings != undefined) {
                            $scope.scopeModel.callOnLoad = definitionSettings.CallOnLoad;
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        CallOnLoad: $scope.scopeModel.callOnLoad
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataExecuteactioneditorsettingDefinitionOnload', ExecuteActionEditorOnLoadDefinition);

})(app);