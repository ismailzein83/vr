'use strict';
app.directive('vrGenericdataGenericbusinessentityRuntimeSection', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new SectionCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/Editor/Templates/SectionTemplate.html';
            }

        };

        function SectionCtor(ctrl, $scope) {
            var currentContext;

            function initializeController() {
                ctrl.rows = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload.rows != undefined) {
                        currentContext = payload.context;
                        var promises = [];

                        for (var i = 0; i < payload.rows.length; i++) {
                            var row = payload.rows[i];
                            row.readyPromiseDeferred = UtilsService.createPromiseDeferred();
                            row.loadPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(row.loadPromiseDeferred.promise);
                            prepareExtendedFieldsObject(row);
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    var rows = {};
                    for (var i = 0; i < ctrl.rows.length; i++) {
                        var row = ctrl.rows[i];
                        rows = UtilsService.mergeObject(rows, row.fieldsAPI.getData(), false);
                    }
                    return rows;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function prepareExtendedFieldsObject(row) {

                row.onRowDirectiveReady = function (api) {
                    row.fieldsAPI = api;
                    row.readyPromiseDeferred.resolve();
                };
                var payload = { context: getContext(), fields: row.Fields };
                if (row.readyPromiseDeferred != undefined) {
                    row.readyPromiseDeferred.promise.then(function () {
                        row.readyPromiseDeferred = undefined;

                        VRUIUtilsService.callDirectiveLoad(row.fieldsAPI, payload, row.loadPromiseDeferred);
                    });
                }
                ctrl.rows.push(row);
            }

            function getContext() {
                var context = UtilsService.cloneObject(currentContext, false);
                return context;
            }


            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }
]);