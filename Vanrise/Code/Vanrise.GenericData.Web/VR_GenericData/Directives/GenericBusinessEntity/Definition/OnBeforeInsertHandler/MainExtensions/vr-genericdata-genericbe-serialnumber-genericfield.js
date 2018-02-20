"use strict";

app.directive("vrGenericdataGenericbeSerialnumberGenericfield", ["UtilsService", "VRNotificationService","VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new GenericFieldCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/OnBeforeInsertHandler/MainExtensions/Templates/SerialNumberGenericFieldTemplate.html"
        };

        function GenericFieldCtor($scope, ctrl, $attrs) {

            var context;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers.GenericFieldSerialNumberPart, Vanrise.GenericData.MainExtensions",
                        FieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds()
                    };
                };

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                    }
                    var getDataRecordTypeFieldPromise = loadDataRecordTypeFieldsSelector();
                    promises.push(getDataRecordTypeFieldPromise);

                    function loadDataRecordTypeFieldsSelector() {
                        var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var typeFieldsPayload = {
                                dataRecordTypeId: getContext().getDataRecordTypeId(),
                                selectedIds: payload != undefined && payload.settings != undefined ? payload.settings.FieldName : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, typeFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                        });
                        return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };
               
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
            
        }

        return directiveDefinitionObject;

    }
]);