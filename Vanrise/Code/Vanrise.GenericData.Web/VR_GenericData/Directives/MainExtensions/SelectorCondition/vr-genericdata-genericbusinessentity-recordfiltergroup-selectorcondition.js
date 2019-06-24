"use strict";
app.directive("vrGenericdataGenericbusinessentityRecordfiltergroupSelectorcondition", ["UtilsService", "VRUIUtilsService", "VR_GenericData_GenericBEDefinitionAPIService",
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBusinessEntityRecordFilterGroupSelectorSelectorConditionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/SelectorCondition/Templates/RecordFilterGroupSelectorConditionTemplate.html"
        };

        function GenericBusinessEntityRecordFilterGroupSelectorSelectorConditionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionId;
            var fields = [];

            var recordFilterAPI;
            var recordFilterReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterAPI = api;
                    recordFilterReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var loadRecordFilterPromise;
                    var recordFilter;

                    if (payload != undefined) {
                        beDefinitionId = payload.beDefinitionId;
                        if (payload.genericBESelectorCondition != undefined)
                            recordFilter = payload.genericBESelectorCondition.RecordFilter;
                    }

                    var loadDataRecordTypeFieldsPromise = loadDataRecordTypeFields();
                    promises.push(loadDataRecordTypeFieldsPromise);

                    var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadDirectivePromiseDeferred.promise);

                    loadDataRecordTypeFieldsPromise.then(function () {
                        loadRecordFilterPromise = loadRecordFilterDirective();
                        loadRecordFilterPromise.then(function () { loadDirectivePromiseDeferred.resolve(); });
                    });

                    function loadDataRecordTypeFields() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetDataRecordTypeFieldsListByBEDefinitionId(beDefinitionId).then(function (response) {
                            for (var i = 0; i < response.length; i++) {
                                var currentItem = response[i];
                                fields.push({
                                    FieldName: currentItem.Name,
                                    FieldTitle: currentItem.Title,
                                    Type: currentItem.Type
                                });
                            }
                        });
                    }

                    function loadRecordFilterDirective() {
                        var loadRecordFilterDirectiveDeferred = UtilsService.createPromiseDeferred();
                        if (tryGetRecordFields()) {
                            recordFilterReadyDeferred.promise.then(function () {
                                var recordFilterPayload = {
                                    context: buildContext(),
                                    FilterGroup: recordFilter
                                };

                                VRUIUtilsService.callDirectiveLoad(recordFilterAPI, recordFilterPayload, loadRecordFilterDirectiveDeferred);
                            });
                        }
                        else
                            loadRecordFilterDirectiveDeferred.resolve();
                        return loadRecordFilterDirectiveDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var result = recordFilterAPI.getData();
                    if (result != undefined && result.filterObj != undefined) {
                        return {
                            $type: "Vanrise.GenericData.Business.GenericBERecordFilterGroupSelectorCondition,Vanrise.GenericData.Business",
                            RecordFilter: result.filterObj
                        };
                    }
                    return null;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function tryGetRecordFields() {
                $scope.scopeModel.hasRecordFields = fields.length > 0;
                return $scope.scopeModel.hasRecordFields;
            }

            function buildContext() {
                var context = {
                    getFields: function () {
                        return fields;
                    }
                };
                return context;
            }
        }
        return directiveDefinitionObject;
    }
]);