"use strict";

app.directive("vrGenericdataGenericbeAftersavehandlerSavehistorygenericbe", ["UtilsService", "VRUIUtilsService", "VR_GenericData_GenericBEDefinitionAPIService",
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SaveHistoryGenericBEHandler($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "saveHistoryGenericBECtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/OnAfterSaveHandler/Templates/SaveHistoryGenericBEHandlerTemplate.html"
        };

        function SaveHistoryGenericBEHandler($scope, ctrl, $attrs) {

            var context;
            var dataRecordTypeId;
            var historyBusinessEntityDefinitionId;
            var parentFieldName;

            var businessEntityDefinitionSelectorAPI;
            var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedBusinessEntityDefinitionDeferred;

            var parentFieldSelectorAPI;
            var parentFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    businessEntityDefinitionSelectorAPI = api;
                    businessEntityDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onParentFieldSelectorReady = function (api) {
                    parentFieldSelectorAPI = api;
                    parentFieldSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onBusinessEntityDefinitionSelectorChanged = function (value) {
                    if (value != undefined) {
                        if (selectedBusinessEntityDefinitionDeferred != undefined)
                            selectedBusinessEntityDefinitionDeferred.resolve();
                        else {
                            getDataRecordTypeId(value.BusinessEntityDefinitionId).then(function () {
                                var parentFieldSelectorPayload = {
                                    dataRecordTypeId: dataRecordTypeId
                                };
                                var setLoader = function (value) {
                                    $scope.scopeModel.isParentFieldSelectorLoading = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, parentFieldSelectorAPI, parentFieldSelectorPayload, setLoader);
                            });
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var settings;
                    var rootPromiseNode = { promises: [] };

                    if (payload != undefined) {
                        settings = payload.settings;
                        selectedBusinessEntityDefinitionDeferred = UtilsService.createPromiseDeferred();

                        if (settings != undefined) {
                            historyBusinessEntityDefinitionId = settings.HistoryBEDefintionID;
                            rootPromiseNode.promises.push(loadBusinessEntityDefinitionSelector(historyBusinessEntityDefinitionId));

                            if (historyBusinessEntityDefinitionId != undefined) {
                                parentFieldName = settings.ParentBEFieldName;
                                rootPromiseNode.getChildNode = function () {
                                    return {
                                        promises: [getDataRecordTypeId(historyBusinessEntityDefinitionId)],
                                        getChildNode: function () {
                                            return { promises: [loadParentFieldSelector()] };
                                        }
                                    };
                                };
                            }
                        }
                        else {
                            rootPromiseNode.promises.push(loadBusinessEntityDefinitionSelector());
                        }
                    }
                    return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
                        selectedBusinessEntityDefinitionDeferred = undefined;
                    });
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnAfterSaveHandlers.SaveHistoryGenericBEAfterSaveHandler, Vanrise.GenericData.MainExtensions",
                        HistoryBEDefintionID: businessEntityDefinitionSelectorAPI.getSelectedIds(),
                        ParentBEFieldName: parentFieldSelectorAPI.getSelectedIds(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadBusinessEntityDefinitionSelector(businessEntityDefinitionId) {
                var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                businessEntityDefinitionSelectorReadyDeferred.promise.then(function () {

                    var payloadSelector = {
                        filter: {
                            Filters: [{
                                $type: "Vanrise.GenericData.Business.GenericBusinessEntityDefinitionFilter, Vanrise.GenericData.Business",
                            }]
                        },
                        selectedIds: businessEntityDefinitionId
                    };
                    VRUIUtilsService.callDirectiveLoad(businessEntityDefinitionSelectorAPI, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
                });
                return businessEntityDefinitionSelectorLoadDeferred.promise;
            }

            function getDataRecordTypeId(businessEntityDefinitionId) {
                return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessEntityDefinitionId).then(function (response) {
                    if (response != undefined) {
                        dataRecordTypeId = response.DataRecordTypeId;
                    }
                });
            }

            function loadParentFieldSelector() {
                var loadParentFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                UtilsService.waitPromiseNode({ promises: [parentFieldSelectorReadyPromiseDeferred.promise, selectedBusinessEntityDefinitionDeferred.promise] }).then(function () {

                    var parentFieldSelectorPayload = {
                        dataRecordTypeId: dataRecordTypeId,
                        selectedIds: parentFieldName,
                    };
                    VRUIUtilsService.callDirectiveLoad(parentFieldSelectorAPI, parentFieldSelectorPayload, loadParentFieldSelectorPromiseDeferred);
                });
                return loadParentFieldSelectorPromiseDeferred.promise;
            }
        }
        return directiveDefinitionObject;
    }
]);