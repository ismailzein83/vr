(function (app) {

    'use strict';

    GenericFieldsEditorSettingsDefinitionDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function GenericFieldsEditorSettingsDefinitionDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericFieldsEditorDefinitionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/GenericFieldsEditorDefinitionSettingTemplate.html'
        };

        function GenericFieldsEditorDefinitionCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var context;
            var genericFieldsDirectiveAPI;
            var genericFieldsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGenericFieldsDirectiveReady = function (api) {
                    genericFieldsDirectiveAPI = api;
                    genericFieldsDirectiveReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.datasource = [];
                    var settings;
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        settings = payload.settings;

                        var genericFieldsDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        genericFieldsDirectiveReadyPromiseDeferred.promise.then(function () {
                            var genericFieldsPayload = {
                                context: context,
                                fields: settings != undefined ? settings.Fields: undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(genericFieldsDirectiveAPI, genericFieldsPayload, genericFieldsDirectiveLoadPromiseDeferred);
                        });
                        promises.push(genericFieldsDirectiveLoadPromiseDeferred.promise);

                        return UtilsService.waitPromiseNode({ promises: promises });
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericFieldsEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        Fields: genericFieldsDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }



        }
    }

    app.directive('vrGenericdataGenericfieldseditorsettingDefinition', GenericFieldsEditorSettingsDefinitionDirective);

})(app);