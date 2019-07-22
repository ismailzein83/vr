(function (app) {

    'use strict';

    ExecuteActionEditorOnButtonClickedDefinition.$inject = ["UtilsService", "VRUIUtilsService"];

    function ExecuteActionEditorOnButtonClickedDefinition(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ExecuteActionEditorOnButtonClickedDefinitionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorExecuteActionSetting/CallRestAPI/Templates/ExecuteActionOnButtonClickedDefinitionTemplate.html"

        };

        function ExecuteActionEditorOnButtonClickedDefinitionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var buttonTypesSelectorAPI;
            var buttonTypesSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCallOnButtonClickedChanged = function () {
                    $scope.scopeModel.selectedButton = undefined;
                };

                $scope.scopeModel.onButtonTypesSelectorReady = function (api) {
                    buttonTypesSelectorAPI = api;
                    buttonTypesSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var context;
                    var definitionSettings;
                    var buttonType;

                    if (payload != undefined) {
                        context = payload.context;
                        definitionSettings = payload.definitionSettings;
                        if (definitionSettings != undefined) {
                            buttonType = definitionSettings.VRButtonType;
                        }
                    }

                    if (context != undefined) {
                        $scope.scopeModel.dataRecordFields = context.getFields();
                    }

                    loadStaticData();

                    var buttonTypesSelectorLoadedPromise = loadButtonTypesSelector();
                    promises.push(buttonTypesSelectorLoadedPromise);

                    function loadStaticData() {
                        if (definitionSettings != undefined) {
                            $scope.scopeModel.callOnButtonClicked = definitionSettings.CallOnButtonClicked;
                        }
                    }

                    function loadButtonTypesSelector() {
                        var buttonTypesSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        buttonTypesSelectorReadyPromiseDeferred.promise.then(function () {
                            var buttonTypesSelectorPayload;
                            if (buttonType != undefined) {
                                buttonTypesSelectorPayload = { selectedIds: buttonType };
                            }

                            VRUIUtilsService.callDirectiveLoad(buttonTypesSelectorAPI, buttonTypesSelectorPayload, buttonTypesSelectorLoadPromiseDeferred);
                        });
                        return buttonTypesSelectorLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        CallOnButtonClicked: $scope.scopeModel.callOnButtonClicked,
                        VRButtonType: $scope.scopeModel.callOnButtonClicked ? buttonTypesSelectorAPI.getSelectedIds() : undefined
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataExecuteactioneditorsettingDefinitionOnbuttonclicked', ExecuteActionEditorOnButtonClickedDefinition);
})(app);