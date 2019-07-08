"use strict";

app.directive("vrInvoicetypeDatasourcesettingsGeneric", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_GenericData_GenericBEDefinitionAPIService", "VR_GenericData_DataRecordFieldAPIService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_DataRecordFieldAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new GenericDataSourceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceDataSourceSettings/MainExtensions/DataSource/Templates/GenericDataSourceItems.html"

        };

        function GenericDataSourceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            var dataRecordTypeId;
            var dataRecordTypeFields = [];
            var beDefinitionId;
            var filterGroup;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var fieldNamesFieldSelectorAPI;
            var fieldNamesFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedFieldNames;

            var filterSelectorAPI;
            var filterReadyDeferred = UtilsService.createPromiseDeferred();

            var onBusinessEntityDefinitionSelectionChangePromiseDeferred;

            function initializeController() {
                $scope.scopeModel = {};


                $scope.scopeModel.onBEDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    beDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onFieldNamesFieldSelectorReady = function (api) {
                    fieldNamesFieldSelectorAPI = api;
                    fieldNamesFieldSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onFilterReady = function (api) {
                    filterSelectorAPI = api;
                    filterReadyDeferred.resolve();
                };


                $scope.scopeModel.onBusinessEntityDefinitionSelectionChange = function (beDefinition) {
                    if (beDefinition != undefined) {
                        if (onBusinessEntityDefinitionSelectionChangePromiseDeferred != undefined) {
                            onBusinessEntityDefinitionSelectionChangePromiseDeferred.resolve();
                        }
                        else {
                            dataRecordTypeId = undefined;
                            beDefinitionId = beDefinition.BusinessEntityDefinitionId;
                            if (beDefinitionId != undefined) {
                                VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(beDefinitionId).then(function (response) {
                                    dataRecordTypeId = response.DataRecordTypeId;

                                    var fieldNamesPayload = {
                                        dataRecordTypeId: dataRecordTypeId
                                    };
                                    var fieldNamesSetLoader = function (value) { $scope.scopeModel.isFieldNamesLoading = value; };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fieldNamesFieldSelectorAPI, fieldNamesPayload, fieldNamesSetLoader);

                                    getDataRecordFieldsInfo().then(function () {
                                        var filterPayload = {
                                            context: getContext()
                                        };
                                        var filterSetLoader = function (value) { $scope.scopeModel.isfilterLoading = value; };
                                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterSelectorAPI, filterPayload, filterSetLoader);
                                    });

                                });
                            }
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.dataSourceEntity != undefined) {
                            onBusinessEntityDefinitionSelectionChangePromiseDeferred = UtilsService.createPromiseDeferred();

                            beDefinitionId = payload.dataSourceEntity.BusinessEntityDefinitionId;
                            selectedFieldNames = payload.dataSourceEntity.FieldNames;
                            filterGroup = payload.dataSourceEntity.FilterGroup;

                            promises.push(getDataRecordTypeId());
                        }
                    }
                    promises.push(loadBEDefinitionSelector());
                    var rootPromiseNode = {
                        promises: promises,
                        getChildNode: function () {
                            var selectorsPromises = [];

                            if (dataRecordTypeId != undefined) {
                                selectorsPromises.push(loadFieldNamesFieldSelectorDirective());
                                selectorsPromises.push(getDataRecordFieldsInfo());
                            }
                            return {
                                promises: selectorsPromises,
                                getChildNode: function () {
                                    return {
                                        promises: [loadFilterDirective()],
                                        getChildNode: function () {
                                            onBusinessEntityDefinitionSelectionChangePromiseDeferred = undefined;
                                            return {
                                                promises: []
                                            };
                                        }
                                    }
                                }
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    var filter = filterSelectorAPI.getData();

                    return {
                        $type: "Vanrise.Invoice.MainExtensions.GenericInvoiceDataSource ,Vanrise.Invoice.MainExtensions",
                        FieldNames: fieldNamesFieldSelectorAPI.getSelectedIds(),
                        BusinessEntityDefinitionId: beDefinitionSelectorAPI.getSelectedIds(),
                        FilterGroup: filter != undefined ? filter.filterObj : undefined

                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getDataRecordTypeId() {
                var dataRecordTypeGetPromise = UtilsService.createPromiseDeferred();

                VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(beDefinitionId).then(function (response) {
                    dataRecordTypeId = response.DataRecordTypeId;

                    return dataRecordTypeGetPromise.resolve();

                });
                return dataRecordTypeGetPromise.promise;
            }

            function loadFieldNamesFieldSelectorDirective() {
                var fieldNamesFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                fieldNamesFieldSelectorReadyDeferred.promise.then(function () {
                    var fieldNamesFieldSelectorPayload = {
                        dataRecordTypeId: dataRecordTypeId
                    };

                    if (selectedFieldNames != undefined) {
                        fieldNamesFieldSelectorPayload.selectedIds = selectedFieldNames;
                    }
                    VRUIUtilsService.callDirectiveLoad(fieldNamesFieldSelectorAPI, fieldNamesFieldSelectorPayload, fieldNamesFieldSelectorLoadDeferred);
                });


                return fieldNamesFieldSelectorLoadDeferred.promise;
            }

            function loadBEDefinitionSelector() {
                var beDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                beDefinitionSelectorReadyDeferred.promise.then(function () {
                    var beDefinitionSelectorPayload;
                    if (beDefinitionId != undefined)
                        beDefinitionSelectorPayload = {
                            selectedIds: beDefinitionId
                        };
                    VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, beDefinitionSelectorPayload, beDefinitionSelectorLoadDeferred);
                });
                return beDefinitionSelectorLoadDeferred.promise;
            }

            function loadFilterDirective() {
                var filterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                filterReadyDeferred.promise.then(function () {
                    var filterPayload = {
                        context: getContext()
                    };
                    if (filterGroup != undefined)
                        filterPayload.FilterGroup = filterGroup;

                    VRUIUtilsService.callDirectiveLoad(filterSelectorAPI, filterPayload, filterDirectiveLoadDeferred);
                });
                return filterDirectiveLoadDeferred.promise;
            }

            function getDataRecordFieldsInfo() {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId, null).then(function (response) {
                    dataRecordTypeFields.length = 0;
                    if (response != undefined)
                        for (var i = 0; i < response.length; i++) {
                            var currentField = response[i];
                            dataRecordTypeFields.push({
                                FieldName: currentField.Entity.Name,
                                FieldTitle: currentField.Entity.Title,
                                Type: currentField.Entity.Type
                            });
                        }
                });
            }

            function getContext() {
                var currentContext = context;

                if (currentContext == undefined)
                    currentContext = {};

                currentContext.getFields = function () {
                    return dataRecordTypeFields;
                };

                return currentContext;
            };

        }

        return directiveDefinitionObject;

    }
]);