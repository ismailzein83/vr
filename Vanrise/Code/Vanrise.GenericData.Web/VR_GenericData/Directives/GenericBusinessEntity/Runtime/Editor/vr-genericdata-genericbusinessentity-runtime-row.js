'use strict';
app.directive('vrGenericdataGenericbusinessentityRuntimeRow', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new RowCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/Editor/Templates/RowTemplate.html';
            }

        };

        function RowCtor(ctrl, $scope) {
            var currentContext;

            function initializeController() {
                ctrl.fields = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload.fields != undefined) {
                        currentContext = payload.context;
                        var promises = [];
                        for (var i = 0; i < payload.fields.length; i++) {
                            var field = payload.fields[i];
                            field.readyPromiseDeferred = UtilsService.createPromiseDeferred();
                            field.loadPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(field.loadPromiseDeferred.promise);
                            prepareFieldObject(field);
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    var fields = {};
                    for (var i = 0; i < ctrl.fields.length; i++) {
                        var field = ctrl.fields[i];
                        if (field.fieldAPI != undefined)
                            fields[field.FieldPath] = field.fieldAPI.getData();
                    }
                    return fields;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function prepareFieldObject(field) {
                if (currentContext != undefined && field.FieldType != undefined)
                    field.runTimeEditor = currentContext.getRuntimeEditor(field.FieldType.ConfigId);
                field.onFieldDirectiveReady = function (api) {
                    field.fieldAPI = api;
                    field.readyPromiseDeferred.resolve();
                };
                var payload = {
                    fieldTitle: field.FieldTitle,
                    fieldType: field.FieldType,
                    fieldValue: currentContext !=undefined? currentContext.getFieldPathValue(field.FieldPath):undefined
                };
                if (field.readyPromiseDeferred != undefined) {
                    field.readyPromiseDeferred.promise.then(function () {
                        field.readyPromiseDeferred = undefined;

                        VRUIUtilsService.callDirectiveLoad(field.fieldAPI, payload, field.loadPromiseDeferred);
                    });
                }
                ctrl.fields.push(field);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);