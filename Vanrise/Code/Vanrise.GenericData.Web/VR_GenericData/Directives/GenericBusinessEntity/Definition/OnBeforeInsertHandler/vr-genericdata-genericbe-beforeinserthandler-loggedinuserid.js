"use strict";

app.directive("vrGenericdataGenericbeBeforeinserthandlerLoggedinuserid", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new LoggedInUserId($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/OnBeforeInsertHandler/Templates/LoggedInUserIdHandlerEditor.html"
        };

        function LoggedInUserId($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var context;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.selectedField;

                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                var settings;

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        settings = payload.settings;
                    }

                    var dataRecordTypeId = getContext().getDataRecordTypeId();
                    if (dataRecordTypeId != undefined) {

                        var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var fieldsPayload = {
                                dataRecordTypeId: dataRecordTypeId,
                                selectedIds: settings != undefined ? settings.UserIdFieldName : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, fieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                        });

                        promises.push(loadDataRecordTypeFieldsSelectorPromiseDeferred.promise);
                    }

                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
            
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers.SetLoggedInUserIdBeforeSaveHandler, Vanrise.GenericData.MainExtensions",
                        UserIdFieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds()
                    };
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