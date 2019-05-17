'use strict';

app.directive('vrGenericdataGenericbusinessentityRuntimeSection', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SectionCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/Editor/Templates/SectionTemplate.html';
            }
        };

        function SectionCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var currentContext;
            var genericContext;
            var fieldValuesByName;
            var parentFieldValues;

            function initializeController() {
                ctrl.rows = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload.rows != undefined) {
                        currentContext = payload.context;
                        genericContext = payload.genericContext;
                        fieldValuesByName = payload.fieldValuesByName;
                        parentFieldValues = payload.parentFieldValues;

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

                //api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                //    var _promises = [];

                //    for (var i = 0; i < ctrl.rows.length; i++) {
                //        var row = ctrl.rows[i];
                //        if (row.fieldsAPI.onFieldValueChanged != undefined && typeof (row.fieldsAPI.onFieldValueChanged) == "function") {
                //            var onFieldValueChangedPromise = row.fieldsAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                //            if (onFieldValueChangedPromise != undefined)
                //                _promises.push(onFieldValueChangedPromise);
                //        }
                //    }

                //    return UtilsService.waitMultiplePromises(_promises);
                //};

                //api.setFieldValues = function (fieldValuesByNames) {
                //    var _promises = [];

                //    for (var i = 0; i < ctrl.rows.length; i++) {
                //        var row = ctrl.rows[i];
                //        if (row.fieldsAPI.setFieldValues != undefined && typeof (row.fieldsAPI.setFieldValues) == "function") {
                //            var onFieldValueSettedPromise = row.fieldsAPI.setFieldValues(fieldValuesByNames);
                //            if (onFieldValueSettedPromise != undefined)
                //                _promises.push(onFieldValueSettedPromise);
                //        }
                //    }

                //    return UtilsService.waitMultiplePromises(_promises);
                //};

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function prepareExtendedFieldsObject(row) {

                row.onRowDirectiveReady = function (api) {
                    row.fieldsAPI = api;
                    row.readyPromiseDeferred.resolve();
                };

                if (row.readyPromiseDeferred != undefined) {
                    row.readyPromiseDeferred.promise.then(function () {
                        row.readyPromiseDeferred = undefined;

                        var payload = {
                            fields: row.Fields,
                            context: getContext(),
                            //genericContext: genericContext,
                            //fieldValuesByName: fieldValuesByName,
                            //parentFieldValues: parentFieldValues
                        };
                        VRUIUtilsService.callDirectiveLoad(row.fieldsAPI, payload, row.loadPromiseDeferred);
                    });
                }

                ctrl.rows.push(row);
            }

            function getContext() {
                var context = UtilsService.cloneObject(currentContext, false);
                return context;
            }
        }

        return directiveDefinitionObject;
    }
]);