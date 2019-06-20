'use strict';
app.directive('vrGenericdataFieldtypeText', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new textTypeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Text/Templates/TextFieldTypeTemplate.html';
            }

        };

        function textTypeCtor(ctrl, $scope) {

            var textTypeSelectorApi;
            var textTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
         
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onTextTypeSelectorReady = function (api) {
                    textTypeSelectorApi = api;
                    textTypeSelectorReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var textTypePayload;
                    if (payload != undefined) {
                        $scope.hint = payload.Hint;
                        textTypePayload = { selectedIds: payload.TextType };
                    }
                    var textTypeLoadDeferred = UtilsService.createPromiseDeferred();
                    textTypeSelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(textTypeSelectorApi, textTypePayload, textTypeLoadDeferred);
                    });
                    promises.push(textTypeLoadDeferred.promise);

                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions",
                        Hint: $scope.hint,
                        TextType: textTypeSelectorApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);