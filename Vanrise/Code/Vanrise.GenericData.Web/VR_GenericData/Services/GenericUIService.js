(function (appControllers) {

    'use strict';

    GenericUIService.$inject = ['$rootScope', 'UtilsService'];

    function GenericUIService($rootScope, UtilsService) {
        return ({
            createGenericUIObj: createGenericUIObj
        });


        function createGenericUIObj(fields) {
            return new genericUIObj(fields);
        };


        function genericUIObj(fields) {
            var fields = fields;
            var fieldContexts;

            var isLoading = true;
            var pendingNotifications = [];

            var getFieldContext = function (field) {
                var returnedContext = undefined;
                for (var x = 0; x < fieldContexts.length; x++) {
                    var currentFieldContext = fieldContexts[x];
                    if (currentFieldContext.field == field) {
                        returnedContext = currentFieldContext.context;
                        break;
                    }
                }
                return returnedContext;
            };

            var loadingFinish = function () {
                isLoading = false;
                if (pendingNotifications.length > 0) {

                    setTimeout(function () {
                        for (var x = 0; x < pendingNotifications.length; x++) {
                            var currentNotification = pendingNotifications[x];
                            currentNotification.genericUIContext.onvaluechanged(currentNotification.relatedField, currentNotification.selectedvalue);
                        }
                        UtilsService.safeApply($rootScope);
                    });
                }
            };

            function initialize() {
                fieldContexts = buildFieldContexts(fields);
            };

            initialize();

            function buildFieldContexts(fields) {
                var contexts = [];
                angular.forEach(fields, function (currentField) {
                    var context = { field: currentField, context: buildGenericUIContext(fields, currentField) };
                    contexts.push(context);
                });
                return contexts;
            }

            function buildGenericUIContext(fields, relatedField) {
                var context = {
                    getFields: function () { return fields; },
                    notifyValueChanged: function (selectedvalue) {
                        angular.forEach(fields, function (currentField) {
                            if (currentField.genericUIContext != undefined && currentField.genericUIContext.onvaluechanged != undefined && typeof (currentField.genericUIContext.onvaluechanged) == "function") {
                                if (!isLoading) {
                                    currentField.genericUIContext.onvaluechanged(relatedField, selectedvalue);
                                }
                                else
                                    pendingNotifications.push({ genericUIContext: currentField.genericUIContext, relatedField: relatedField, selectedvalue: selectedvalue });
                            }
                        });
                    }
                };
                return context;
            };

            this.getFieldContext = getFieldContext;
            this.loadingFinish = loadingFinish;
        }
    };

    appControllers.service('VR_GenericData_GenericUIService', GenericUIService);

})(appControllers);