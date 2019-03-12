(function (app) {

    'use strict';

    GenericBusinessEntityGridDirective.$inject = ['VR_GenericData_GenericBEDefinitionAPIService', 'VRNotificationService', 'VR_GenericData_GenericBusinessEntityAPIService', 'VRUIUtilsService', 'UtilsService', 'VR_GenericData_GenericBEActionService', 'VR_GenericData_GenericBusinessEntityService', 'VRCommon_VRBulkActionDraftService'];

    function GenericBusinessEntityGridDirective(VR_GenericData_GenericBEDefinitionAPIService, VRNotificationService, VR_GenericData_GenericBusinessEntityAPIService, VRUIUtilsService, UtilsService, VR_GenericData_GenericBEActionService, VR_GenericData_GenericBusinessEntityService, VRCommon_VRBulkActionDraftService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBusinessEntityGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/Management/Grid/Templates/GenericBusinessEntityGridTemplate.html'
        };

        function GenericBusinessEntityGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var businessEntityDefinitionId;
            var genericBEActions = [];
            var genericBEGridActions = [];
            var genericBEGridViews = [];
            var fieldValues;
            var idFieldType;
            var bulkActionId;
            var bulkActionDraftInstance;
            var context;
            var gridQuery;

            var gridAPI;
            var gridAPIPromiseDeferred = UtilsService.createPromiseDeferred();
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.showGrid = false;
                $scope.scopeModel.columns = [];
                //$scope.scopeModel.menuActions = [];
                $scope.scopeModel.showDrillDown = function () {
                    if (genericBEGridViews == undefined || genericBEGridViews.length == 0) {
                        return false;
                    }
                    return true;
                };
                $scope.scopeModel.menuActions = function (genericBusinessEntity) {
                    var menuActions = [];
                    if (genericBusinessEntity.menuActions != undefined) {
                        for (var i = 0; i < genericBusinessEntity.menuActions.length; i++)
                            menuActions.push(genericBusinessEntity.menuActions[i]);
                    }
                    return menuActions;
                };

                $scope.scopeModel.businessEntities = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridAPIPromiseDeferred.resolve();
                };

                $scope.scopeModel.showDirectiveTabs = function () {
                    if (bulkActionId != undefined) return false;
                    return true;
                };


                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_GenericBusinessEntityAPIService.GetFilteredGenericBusinessEntities(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var businessEntity = response.Data[i];
                                if (bulkActionId != undefined && idFieldType != undefined && idFieldType.Name != undefined) {
                                    businessEntity.isSelected = bulkActionDraftInstance.isItemSelected(businessEntity.FieldValues[idFieldType.Name].Value);
                                } else {

                                    VR_GenericData_GenericBusinessEntityService.defineGenericBEViewTabs(businessEntityDefinitionId, businessEntity, gridAPI, genericBEGridViews, idFieldType);
                                    VR_GenericData_GenericBEActionService.defineGenericBEMenuActions(businessEntityDefinitionId, businessEntity, gridAPI, genericBEActions, genericBEGridActions, genericBEGridViews, idFieldType, fieldValues);
                                }
                            }
                        }
                        onResponseReady(response);

                        if (bulkActionId != undefined) {
                            bulkActionDraftInstance.reEvaluateButtonsStatus();
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };


                $scope.scopeModel.onBusinessEntitySelect = function (businessEntity) {
                    if (bulkActionDraftInstance != undefined && idFieldType != undefined && idFieldType.Name != undefined) {
                        bulkActionDraftInstance.onSelectItem({ ItemId: businessEntity.FieldValues[idFieldType.Name].Value }, businessEntity.isSelected);
                    }
                };

                defineAPI();
            }

            //function hasEditGenericBEPermission(genericBusinessEntity) {
            //    return VR_GenericData_GenericBusinessEntityAPIService.DoesUserHaveEditAccess(genericBusinessEntity.Entity.BusinessEntityDefinitionId);
            //}

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        if (businessEntityDefinitionId != undefined && businessEntityDefinitionId != payload.businessEntityDefinitionId) {
                            $scope.scopeModel.columns.length = 0; gridAPI.clearUpdatedItems();
                        }
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        bulkActionId = payload.bulkActionId;
                        context = payload.context;
                        gridQuery = payload.query;
                        if (gridQuery != undefined)
                            fieldValues = gridQuery.fieldValues;
                    }

                    if ($scope.scopeModel.columns.length == 0) {

                        //Loading gridColumnsAttributes
                        var businessEntityGridColumnsLoadPromise = getBusinessEntityGridColumnsLoadPromise();
                        businessEntityGridColumnsLoadPromise.finally(function () {
                            $scope.scopeModel.showGrid = true;
                        });
                        promises.push(businessEntityGridColumnsLoadPromise);

                        //Loading GenericBEDefinitionSettings
                        var genericBEDefinitionSettingsLoadPromise = getGenericBEDefinitionSettingsLoadPromise();
                        promises.push(genericBEDefinitionSettingsLoadPromise);

                        //Loading GenericBEIdFieldType
                        var idFieldTypeForGenericBELoadPromise = getIdFieldTypeForGenericBELoadPromise();
                        promises.push(idFieldTypeForGenericBELoadPromise);

                        promises.push(gridAPIPromiseDeferred.promise);
                    }

                    var gridLoadDeferred = UtilsService.createPromiseDeferred();

                    //Retrieving Data
                    UtilsService.waitMultiplePromises(promises).finally(function () {
                        if (bulkActionId != undefined) {
                            $scope.scopeModel.showMultipleSelection = true;
                            bulkActionDraftInstance = VRCommon_VRBulkActionDraftService.createBulkActionDraft(getContext());
                        }

                        var promise = gridAPI.retrieveData(buildGridQuery(gridQuery));
                        if (promise != undefined) {
                            promise.then(function () {
                                gridLoadDeferred.resolve();

                            }).catch(function (error) {
                                gridLoadDeferred.reject(error);
                            });
                        }
                        else {
                            gridLoadDeferred.resolve();
                        }

                    }).catch(function (error) {
                        gridLoadDeferred.reject(error);
                    });

                    function getBusinessEntityGridColumnsLoadPromise() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEGridColumnAttributes(businessEntityDefinitionId).then(function (response) {
                            var businessEntityGridColumnAttributes = response;
                            if (businessEntityGridColumnAttributes != undefined) {
                                for (var index = 0; index < businessEntityGridColumnAttributes.length; index++) {
                                    var businessEntityGridColumnAttribute = businessEntityGridColumnAttributes[index];
                                    if (fieldValues == undefined) {
                                        $scope.scopeModel.columns.push(businessEntityGridColumnAttribute.Attribute);
                                        continue;
                                    }

                                    var fieldValue = fieldValues[businessEntityGridColumnAttribute.Name];
                                    if (fieldValue == undefined || !fieldValue.isHidden) {
                                        $scope.scopeModel.columns.push(businessEntityGridColumnAttribute.Attribute);
                                        continue;
                                    }

                                }
                            }
                        });
                    }

                    function getGenericBEDefinitionSettingsLoadPromise() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessEntityDefinitionId).then(function (response) {
                            if (response != undefined) {
                                genericBEActions = response.GenericBEActions;
                                if (response.GridDefinition != undefined) {
                                    genericBEGridActions = response.GridDefinition.GenericBEGridActions;
                                    genericBEGridViews = response.GridDefinition.GenericBEGridViews;
                                }
                            }
                        });
                    }

                    function getIdFieldTypeForGenericBELoadPromise() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetIdFieldTypeForGenericBE(businessEntityDefinitionId).then(function (response) {
                            if (response != undefined) {
                                idFieldType = response;
                                if (idFieldType != undefined)
                                    $scope.scopeModel.idFieldName = 'FieldValues.' + idFieldType.Name + '.Description';
                            }
                        });
                    }

                };

                api.onGenericBEAdded = function (addedBusinessEntity) {
                    VR_GenericData_GenericBusinessEntityService.defineGenericBEViewTabs(businessEntityDefinitionId, addedBusinessEntity, gridAPI, genericBEGridViews, idFieldType);
                    VR_GenericData_GenericBEActionService.defineGenericBEMenuActions(businessEntityDefinitionId, addedBusinessEntity, gridAPI, genericBEActions, genericBEGridActions, genericBEGridViews, idFieldType, fieldValues);
                    gridAPI.itemAdded(addedBusinessEntity);
                };

                api.deselectAllBusinessEntities = function () {
                    bulkActionDraftInstance.deselectAllItems();
                };

                api.selectAllBusinessEntities = function () {
                    bulkActionDraftInstance.selectAllItems();
                };

                api.finalizeBulkActionDraft = function () {
                    return bulkActionDraftInstance.finalizeBulkActionDraft();
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {

                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                currentContext.triggerRetrieveData = function () {
                    if (gridQuery == undefined)
                        gridQuery = {};
                    gridQuery.BulkActionState = bulkActionDraftInstance.getBulkActionState();
                    gridAPI.retrieveData(buildGridQuery(gridQuery));
                };

                currentContext.hasItems = function () {
                    return $scope.scopeModel.businessEntities.length > 0;
                };
                return currentContext;
            }



            function buildGridQuery(gridQuery) {
                if (gridQuery == undefined)
                    gridQuery = {};
                gridQuery.BusinessEntityDefinitionId = businessEntityDefinitionId;
                return gridQuery;
            }


        }
    }

    app.directive('vrGenericdataGenericbusinessentityGrid', GenericBusinessEntityGridDirective);

})(app);