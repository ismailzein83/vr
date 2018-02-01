'use strict';
app.directive('vrGenericdataFieldtypeCustomobject', ['UtilsService','VRUIUtilsService',
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

                var ctor = new customObjectTypeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/CustomObject/Templates/CustomObjectFieldTypeTemplate.html';
            }

        };

        function customObjectTypeCtor(ctrl, $scope) {

            var fieldTypeCustomObjectAPI;
            var fieldTypeCustomObjectReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onFieldTypeCustomObjectDirectiveReady = function (api) {
                    fieldTypeCustomObjectAPI = api;
                    fieldTypeCustomObjectReadyPromiseDeferred.resolve();
                };


                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined)
                        ctrl.isNullable = payload.IsNullable;
                  
                    promises.push(loadFieldTypeCustomObject());

                    function loadFieldTypeCustomObject() {
                        var loadFieldTypeCustomObjectPromiseDeferred = UtilsService.createPromiseDeferred();
                        fieldTypeCustomObjectReadyPromiseDeferred.promise.then(function () {
                            var datapPayload = {
                                settings: payload != undefined && payload.Settings || undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(fieldTypeCustomObjectAPI, datapPayload, loadFieldTypeCustomObjectPromiseDeferred);
                        });

                        return loadFieldTypeCustomObjectPromiseDeferred.promise;
                    }


                    return UtilsService.waitMultiplePromises(promises)
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldCustomObjectType,Vanrise.GenericData.MainExtensions",
                        IsNullable: ctrl.isNullable,
                        Settings: fieldTypeCustomObjectAPI.getData()
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