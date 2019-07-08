(function (app) {

    'use strict';

    NewOrExistingGenericBEDefinitionCustomActionDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function NewOrExistingGenericBEDefinitionCustomActionDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AddCustomActionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBECustomActionDefinition/Templates/NewOrExistingGenericBEDefinitionCustomActionTemplate.html'
        };

        function AddCustomActionCtor($scope, ctrl) {

            this.initializeController = initializeController;

            var editorDefinitionSettingDirectiveAPI;
            var editorDefinitionSettingDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var modalWidthSelectorAPI;
            var modalWidthReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onModalWidthSelectorReady = function (api) {
                    modalWidthSelectorAPI = api;
                    modalWidthReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEFieldEditorDefinitionDirectiveReady = function (api) {
                    editorDefinitionSettingDirectiveAPI = api;
                    editorDefinitionSettingDirectiveReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }


            function defineAPI() {
                var api = {};
                var context;
                var editorDefinitionSetting;
                var containerType;

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        containerType = payload.containerType;
                        var settings = payload.settings;
                        if (settings != undefined) {
                            editorDefinitionSetting = settings.EditorDefinitionSetting;
                        }
                    }
                    var promises = [];

                    var rootNodePromises = {
                        promises: promises
                    };

                    promises.push(loadFieldEditorDefinition());
                    function loadFieldEditorDefinition() {
                        var editorDefinitionSettingDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        editorDefinitionSettingDirectiveReadyPromiseDeferred.promise.then(function () {

                            var payload = {
                                settings: editorDefinitionSetting,
                                context: context,
                                containerType: containerType
                            };
                            VRUIUtilsService.callDirectiveLoad(editorDefinitionSettingDirectiveAPI, payload, editorDefinitionSettingDirectiveLoadPromiseDeferred);
                        });
                        return editorDefinitionSettingDirectiveLoadPromiseDeferred.promise;
                    }

                    promises.push(loadModalWidthSelector());
                    function loadModalWidthSelector() {
                        var loadModalWidthSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        modalWidthReadyPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                selectedIds: settings != undefined && settings.EditorSize != undefined ? settings.EditorSize : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(modalWidthSelectorAPI, selectorPayload, loadModalWidthSelectorPromiseDeferred);
                        });

                        return loadModalWidthSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode(rootNodePromises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBECustomAction.GenericBENewOrExistingCustomAction, Vanrise.GenericData.MainExtensions",
                        EditorDefinitionSetting: editorDefinitionSettingDirectiveAPI.getData(),
                        EditorSize: modalWidthSelectorAPI.getSelectedIds()
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrGenericdataGenericbeNeworexistingCustomactionDefinition', NewOrExistingGenericBEDefinitionCustomActionDirective);

})(app);