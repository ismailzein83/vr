(function (app) {

    'use strict';

    GenericBERuntimeManagementDirective.$inject = ['VR_GenericData_GenericBusinessEntityService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_RecordQueryLogicalOperatorEnum', 'VR_GenericData_GenericBusinessEntityAPIService'];

    function GenericBERuntimeManagementDirective(VR_GenericData_GenericBusinessEntityService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_RecordQueryLogicalOperatorEnum, VR_GenericData_GenericBusinessEntityAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBERuntimeManagementCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEViewDefinition/Templates/GenericBERuntimeManagementTemplate.html'
        };

        function GenericBERuntimeManagementCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var definitionId;

            var filterDirectiveAPI;
            var filterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var viewId;
            var bulkActionId;
            var bulkAction;

            var businessEntityDefinitionId;
            var genericBEDefinitionSettings;

            var businessEntityDefinitionAPI;
            var businessEntityDefinitionReadyDeferred = UtilsService.createPromiseDeferred();
            var businessEntityDefinitionSelectedDeferred;

            var gridDirectiveAPI;
            var gridDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var runtimeManagement;
            var context;

            var showAddFromAccess = true;
            var showUploadFromAccess = true;

            var showAddFromDefinitionSettings = true;
            var showUploadFromDefinitionSettings = true;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.showAddButton = false;
                $scope.scopeModel.showUploadButton = false;
                $scope.scopeModel.hasFilter = false;

                $scope.scopeModel.showActionButtons = false;
                $scope.scopeModel.showMenuActions = false;
                $scope.scopeModel.deselectAllClicked = function () {
                    gridDirectiveAPI.deselectAllBusinessEntities();
                };
                $scope.scopeModel.selectAllClicked = function () {
                    gridDirectiveAPI.selectAllBusinessEntities();
                };

                $scope.scopeModel.addBulkActions = [];

                $scope.scopeModel.onFilterDirectiveReady = function (api) {
                    filterDirectiveAPI = api;
                    filterDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onGridDirectiveReady = function (api) {
                    gridDirectiveAPI = api;
                    gridDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    businessEntityDefinitionAPI = api;
                    businessEntityDefinitionReadyDeferred.resolve();
                };
                $scope.scopeModel.onBusinessEntityDefinitionSelectionChange = function () {
                    if (businessEntityDefinitionAPI != undefined) {
                        if (businessEntityDefinitionAPI.getSelectedIds() != undefined) {
                            checkDoesUserHaveAddAccess(businessEntityDefinitionAPI.getSelectedIds()).then(function () {
                                $scope.scopeModel.showAddButton = showAddFromDefinitionSettings && showAddFromAccess;
                                $scope.scopeModel.showUploadButton = showUploadFromDefinitionSettings && showUploadFromAccess;
                            });

                            if (businessEntityDefinitionSelectedDeferred != undefined)
                                businessEntityDefinitionSelectedDeferred.resolve();
                            else {
                                $scope.scopeModel.isLoading = true;

                                loadBusinessEntityDefinitionSettings().then(function () {
                                    loadSearchCriteria().finally(function () {
                                        $scope.scopeModel.isLoading = false;
                                    }).catch(function (error) {
                                        VRNotificationService.notifyException(error, $scope);
                                    });
                                }).catch(function (error) {
                                    VRNotificationService.notifyException(error, $scope);
                                });
                            }
                        }
                    }
                };

                function checkDoesUserHaveAddAccess(definitionId) {
                    return VR_GenericData_GenericBusinessEntityAPIService.DoesUserHaveAddAccess(definitionId).then(function (response) {
                        showAddFromAccess = response;
                        showUploadFromAccess = response;
                    });

                }


                $scope.search = function () {
                    return gridDirectiveAPI.load(getGridFilter());
                };

                $scope.addBusinessEntity = function () {
                    var onGenericBusinessEntityAdded = function (addedGenericBusinessEntity) {
                        gridDirectiveAPI.onGenericBEAdded(addedGenericBusinessEntity);
                    };
                    var editorSize = undefined;//genericBEDefinitionSettings != undefined ? genericBEDefinitionSettings.editorSize : undefined;
                    VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(onGenericBusinessEntityAdded, businessEntityDefinitionAPI.getSelectedIds(), editorSize);
                };

                $scope.scopeModel.uploadBusinessEntity = function () {
                    var editorSize = undefined;
                    VR_GenericData_GenericBusinessEntityService.uploadGenericBusinessEntity(businessEntityDefinitionAPI.getSelectedIds(), editorSize);
                };

                UtilsService.waitMultiplePromises([businessEntityDefinitionReadyDeferred.promise]).then(function () {
                    defineAPI();
                });

            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    $scope.scopeModel.hasFilter = false;
                    $scope.scopeModel.isGridLoading = true;
                    if (payload != undefined) {
                        viewId = payload.viewId;
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        bulkAction = payload.bulkAction;
                    }
                    bulkActionId = bulkAction != undefined ? bulkAction.GenericBEBulkActionId : undefined;
                    if (bulkActionId != undefined) {
                        $scope.scopeModel.showActionButtons = true;
                        $scope.scopeModel.showActionInformation = true;
                    }
                    return UtilsService.waitMultipleAsyncOperations([loadBEDefinitionsSelectorAndSubsections]).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.isLoading = false;
                    });
                };
                api.finalizeBulkActionDraft = function () {
                    return gridDirectiveAPI.finalizeBulkActionDraft();
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function loadBEDefinitionsSelectorAndSubsections() {
                var loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred = UtilsService.createPromiseDeferred();

                var promises = [];
                businessEntityDefinitionSelectedDeferred = UtilsService.createPromiseDeferred();
                promises.push(businessEntityDefinitionSelectedDeferred.promise);
                promises.push(loadBusinessEntityDefinitionSelector());

                UtilsService.waitMultiplePromises(promises).then(function () {
                    businessEntityDefinitionSelectedDeferred = undefined;
                    $scope.scopeModel.hideBusinessEntityDefinitionSelector = businessEntityDefinitionAPI.hasSingleItem();

                    loadBusinessEntityDefinitionSettings().then(function () {

                        $scope.scopeModel.showAddButton = showAddFromDefinitionSettings && showAddFromAccess;
                        $scope.scopeModel.showUploadButton = showUploadFromDefinitionSettings && showUploadFromAccess;

                        loadSearchCriteria().then(function () {
                            loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.resolve();
                        }).catch(function (error) {
                            loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.reject(error);
                        });
                    }).catch(function (error) {
                        loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.reject(error);
                    });

                }).catch(function (error) {
                    loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.reject(error);
                });

                return loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.promise.then(function (response) {

                    loadBulkActions();
                });
            }

            function loadBusinessEntityDefinitionSettings() {
                businessEntityDefinitionId = businessEntityDefinitionAPI.getSelectedIds();
                return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessEntityDefinitionId).then(function (response) {
                    genericBEDefinitionSettings = response;
                    if (genericBEDefinitionSettings != undefined) {
                        if (genericBEDefinitionSettings.FilterDefinition != undefined && genericBEDefinitionSettings.FilterDefinition.Settings != undefined) {
                            $scope.scopeModel.filterDirective = genericBEDefinitionSettings.FilterDefinition.Settings.RuntimeEditor;
                        }
                        showAddFromDefinitionSettings = !genericBEDefinitionSettings.HideAddButton;
                        showUploadFromDefinitionSettings = genericBEDefinitionSettings.ShowUpload;
                    }
                });
            }

            function loadSearchCriteria() {
                return UtilsService.waitMultipleAsyncOperations([loadFilterDirective]).then(function () {
                    $scope.scopeModel.hasFilter = filterDirectiveAPI != undefined ? filterDirectiveAPI.hasFilters() : false;
                    loadGridDirective();
                });
            }

            function loadFilterDirective() {
                if ($scope.scopeModel.filterDirective != undefined) {
                    var filterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    filterDirectiveReadyDeferred.promise.then(function () {
                        var filterDirectivePayload = {
                            settings: genericBEDefinitionSettings.FilterDefinition.Settings,
                            dataRecordTypeId: genericBEDefinitionSettings.DataRecordTypeId
                        };
                        VRUIUtilsService.callDirectiveLoad(filterDirectiveAPI, filterDirectivePayload, filterDirectiveLoadDeferred);
                    });
                    return filterDirectiveLoadDeferred.promise;
                }
            }

            function loadGridDirective() {
                gridDirectiveReadyDeferred.promise.then(function () {
                    gridDirectiveAPI.load(getGridFilter());
                });
            }

            function getGridFilter() {
                var filterData = filterDirectiveAPI != undefined ? filterDirectiveAPI.getData() : undefined;
                var filterGroup;
                var filters;

                if (filterData != undefined) {
                    if (filterData.RecordFilter != undefined) {
                        if (filterData.RecordFilter.$type.indexOf("RecordFilterGroup") < 0) {
                            filterGroup = filterData.RecordFilter;
                        } else {
                            filterGroup = {
                                $type: "Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities",
                                LogicalOperator: VR_GenericData_RecordQueryLogicalOperatorEnum.And.value,
                                Filters: [filterData.RecordFilter]
                            };
                        }
                    }

                    if (filterData.Filters != undefined) {
                        filters = [];
                        for (var i = 0; i < filterData.Filters.length; i++) {
                            filters.push(filterData.Filters[i]);
                        }
                    }
                }


                var gridPayload = {
                    query: {
                        FilterGroup: filterGroup,
                        FromTime: filterData != undefined ? filterData.FromTime : undefined,
                        ToTime: filterData != undefined ? filterData.ToTime : undefined,
                        Filters: filters,
                        LimitResult: filterData != undefined ? filterData.LimitResult : undefined
                    },
                    businessEntityDefinitionId: businessEntityDefinitionAPI.getSelectedIds(),
                    context: getGridContext(),
                    bulkActionId: bulkActionId
                };
                return gridPayload;
            }

            function loadBusinessEntityDefinitionSelector() {
                var payLoad = {
                    filter: {
                        Filters: [{
                            $type: "Vanrise.GenericData.Business.GenericBEDefinitionViewFilter, Vanrise.GenericData.Business",
                            ViewId: viewId
                        }]
                    },
                    selectFirstItem: true
                };
                return businessEntityDefinitionAPI.load(payLoad);
            }

            function loadBulkActions() {
                $scope.scopeModel.addBulkActions.length = 0;
                return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessEntityDefinitionId).then(function (response) {
                    genericBEDefinitionSettings = response;
                    if (genericBEDefinitionSettings && genericBEDefinitionSettings.GenericBEBulkActions && genericBEDefinitionSettings.GenericBEBulkActions.length != 0) {
                        if (bulkActionId == undefined) {
                            $scope.scopeModel.showMenuActions = true;
                            var genericBEBulkActions = genericBEDefinitionSettings.GenericBEBulkActions;
                            for (var i = 0; i < genericBEBulkActions.length; i++) {
                                var genericBEBulkAction = genericBEBulkActions[i];
                                addMenuAction(genericBEBulkAction);
                            }
                            function addMenuAction(genericBEBulkAction) {
                                $scope.scopeModel.addBulkActions.push({
                                    name: genericBEBulkAction.Title,
                                    clicked: function () {
                                        VR_GenericData_GenericBusinessEntityService.openBulkActionsEditor(viewId, genericBEBulkAction, businessEntityDefinitionId, genericBEDefinitionSettings.DataRecordTypeId);
                                    }
                                });
                            }
                        }
                        else {
                            $scope.scopeModel.showMenuActions = false;
                        }
                    }
                    else {
                        $scope.scopeModel.showMenuActions = false;
                    }

                });
            }

            function getGridContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};

                currentContext.setSelectAllEnablity = function (value) {
                    $scope.scopeModel.enableSelectAll = value;
                };
                currentContext.setDeselectAllEnablity = function (value) {
                    $scope.scopeModel.enableDeselectAll = value;

                };
                currentContext.setActionsEnablity = function (value) {

                    $scope.scopeModel.enableBulkActions = value;
                };
                return currentContext;
            }
        }
    }

    app.directive('vrGenericdataGenericbeRuntimeManagement', GenericBERuntimeManagementDirective);

})(app);