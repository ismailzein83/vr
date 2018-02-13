"use strict";

app.directive("vrGenericdataGenericbeFiltergroupcondition", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FilterGroupCondition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/GenericBeCondition/MainExtensions/Templates/FilterGroupConditionTemplate.html"
        };

        function FilterGroupCondition($scope, ctrl, $attrs) {

            var recordFilterAPI;
            var recordFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var context;
            ctrl.isValid = function () {
                if (recordFilterAPI == undefined) return null;
                if (recordFilterAPI.getData() != undefined && recordFilterAPI.getData().filterObj == null)
                    return "You Should add at least one filter expression.";
                return null;
            };
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onGenericDataRecordFilterDirectiveReady = function (api) {
                    recordFilterAPI = api;
                    recordFilterReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    console.log(recordFilterAPI.getData())
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEConditions.GenericFilterGroupCondition, Vanrise.GenericData.MainExtensions",
                        FilterGroup: recordFilterAPI.getData().filterObj
                    };
                };

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                    }

                    var loadDirectivePromise = loadDataRecordFilterDirective();
                    promises.push(loadDirectivePromise);

                   
                    function loadDataRecordFilterDirective() {
                        var loadDataRecordFilterPromiseDeferred = UtilsService.createPromiseDeferred();

                        recordFilterReadyPromiseDeferred.promise.then(function () {
                            var directivePayload = {
                                context: getContext()
                            };
                            if (payload != undefined && payload.filterGroup) {
                                directivePayload.FilterGroup = payload.filterGroup;
                            };
                            VRUIUtilsService.callDirectiveLoad(recordFilterAPI, directivePayload, loadDataRecordFilterPromiseDeferred);
                        });

                        return loadDataRecordFilterPromiseDeferred.promise;
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