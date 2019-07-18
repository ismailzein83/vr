'use strict';
app.directive('vrGenericdataFieldtypeAttachmentRuntimeeditor', ['UtilsService', 'VRUIUtilsService',
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
                var ctor = new attachmentTypeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return '<vr-columns colnum="12" vr-loader="scopeModel.isDirectiveLoading">'
                    + '<vr-genericdata-attachmentfieldtype-management on-ready="ctrl.onDirectiveReady" ></vr-genericdata-attachmentfieldtype-management>'
                    + '</vr-columns>';
            }

        };

        function attachmentTypeCtor(ctrl, $scope) {
            var fieldName;

            var directiveReadyAPI;
            var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                ctrl.onDirectiveReady = function (api) {
                    directiveReadyAPI = api;
                    directiveReadyPromiseDeferred.resolve();
                };
                directiveReadyPromiseDeferred.promise.then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var payloadVariable;
                    if (payload != undefined) {
                        fieldName = payload.fieldName;
                        payloadVariable = {
                            attachementFieldTypes: payload.fieldValue,
                            readOnly: payload.readOnly
                        };
                    
                    }

                    promises.push(directiveReadyAPI.loadGrid(payloadVariable));

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return directiveReadyAPI.getData();
                };

                api.setFieldValues = function (fieldValuesByNames) {
                    if (fieldValuesByNames == undefined || !(fieldName in fieldValuesByNames))
                        return;

                    var setFieldValuesDeferred = UtilsService.createPromiseDeferred();

                    var payloadVariable = {
                        attachementFieldTypes: fieldValuesByNames[fieldName]
                    };

                    ctrl.isDirectiveLoading = true;

                    directiveReadyAPI.loadGrid(payloadVariable).then(function () {
                        setFieldValuesDeferred.resolve();
                        ctrl.isDirectiveLoading = true;
                    });

                    return setFieldValuesDeferred.promise;
                };
                api.setOnlyViewMode = function () {
                    UtilsService.setContextReadOnly($scope);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);