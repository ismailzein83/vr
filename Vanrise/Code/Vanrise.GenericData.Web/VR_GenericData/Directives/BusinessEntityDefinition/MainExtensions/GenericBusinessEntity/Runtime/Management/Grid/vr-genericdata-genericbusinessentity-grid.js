(function (app) {

    'use strict';

    GenericBusinessEntityGridDirective.$inject = ['VR_GenericData_GenericBEDefinitionAPIService', 'VRNotificationService', 'VR_GenericData_GenericBusinessEntityAPIService', 'VRUIUtilsService', 'UtilsService','VR_GenericData_GenericBEActionService'];

    function GenericBusinessEntityGridDirective(VR_GenericData_GenericBEDefinitionAPIService, VRNotificationService, VR_GenericData_GenericBusinessEntityAPIService, VRUIUtilsService, UtilsService, VR_GenericData_GenericBEActionService) {
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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Runtime/Management/Grid/Templates/GenericBusinessEntityGridTemplate.html'
        };

        function GenericBusinessEntityGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var businessEntityDefinitionId;
            var gridColumnFieldNames = [];
            var genericBEActions = [];
            var genericBEGridActions = [];
            var idFieldType;

            var gridAPI;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.columns = [];
                //$scope.scopeModel.menuActions = [];

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
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                    return VR_GenericData_GenericBusinessEntityAPIService.GetFilteredGenericBusinessEntities(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var genericBusinessEntity = response.Data[i];
                                //VR_GenericData_GenericBEService.defineGenericBEViewTabs(vrCaseDefinitionId, vrCase, gridAPI, vrCaseViewDefinitions);
                                VR_GenericData_GenericBEActionService.defineGenericBEMenuActions(businessEntityDefinitionId, genericBusinessEntity, gridAPI, genericBEActions, genericBEGridActions, idFieldType);
                            }
                        }
                        onResponseReady(response);

                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
            }

            //function hasEditGenericBEPermission(genericBusinessEntity) {
            //    return VR_GenericData_GenericBusinessEntityAPIService.DoesUserHaveEditAccess(genericBusinessEntity.Entity.BusinessEntityDefinitionId);
            //}

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var gridQuery;

                    if (payload != undefined) {
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        gridQuery = payload.query;
                    }

                    if ($scope.scopeModel.columns.length == 0) {

                        //Loading gridColumnsAttributes
                        var businessEntityGridColumnsLoadPromise = getBusinessEntityGridColumnsLoadPromise();
                        promises.push(businessEntityGridColumnsLoadPromise);

                        //Loading GenericBEDefinitionSettings
                        var genericBEDefinitionSettingsLoadPromise = getGenericBEDefinitionSettingsLoadPromise();
                        promises.push(genericBEDefinitionSettingsLoadPromise);

                        //Loading GenericBEIdFieldType
                        var idFieldTypeForGenericBELoadPromise = getIdFieldTypeForGenericBELoadPromise();
                        promises.push(idFieldTypeForGenericBELoadPromise);

                    }

                    var gridLoadDeferred = UtilsService.createPromiseDeferred();

                    //Retrieving Data
                    UtilsService.waitMultiplePromises(promises).then(function () {
                        gridQuery = buildGridQuery(gridQuery);
                        gridAPI.retrieveData(gridQuery);
                        gridLoadDeferred.resolve();
                    }).catch(function (error) {
                        gridLoadDeferred.reject(error);
                    });

                    function getBusinessEntityGridColumnsLoadPromise() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEGridColumnAttributes(businessEntityDefinitionId).then(function (response) {
                            var businessEntityGridColumnAttributes = response;
                            if (businessEntityGridColumnAttributes != undefined) {
                                for (var index = 0; index < businessEntityGridColumnAttributes.length; index++) {
                                    var businessEntityGridColumnAttribute = businessEntityGridColumnAttributes[index];
                                    gridColumnFieldNames.push(businessEntityGridColumnAttribute.Name);
                                    $scope.scopeModel.columns.push(businessEntityGridColumnAttribute.Attribute);
                                }
                            }
                        });
                    }

                    function getGenericBEDefinitionSettingsLoadPromise() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessEntityDefinitionId).then(function (response) {
                            if (response != undefined) {
                                genericBEActions = response.GenericBEActions;
                                if (response.GridDefinition != undefined)
                                    genericBEGridActions = response.GridDefinition.GenericBEGridActions;
                            }
                        });
                    }

                    function getIdFieldTypeForGenericBELoadPromise() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetIdFieldTypeForGenericBE(businessEntityDefinitionId).then(function (response) {
                            if (response != undefined) {
                                idFieldType = response;
                                if (idFieldType != undefined)
                                    $scope.scopeModel.idFieldName = 'FieldValues.'+ idFieldType.Name+'.Description';
                            }
                        });
                    }

                    function buildGridQuery(gridQuery) {
                        return {
                            BusinessEntityDefinitionId: businessEntityDefinitionId,
                        };
                    }

                    return gridLoadDeferred.promise;
                };

                api.onGenericBEAdded = function (addedBusinessEntity) {
                    //VR_GenericData_GenericBEService.defineGenericBEViewTabs(vrCaseDefinitionId, addedAccount, gridAPI, vrCaseViewDefinitions);
                    VR_GenericData_GenericBEActionService.defineGenericBEMenuActions(businessEntityDefinitionId, addedBusinessEntity, gridAPI, genericBEActions, genericBEGridActions, idFieldType);
                    gridAPI.itemAdded(addedBusinessEntity);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }
    }

    app.directive('vrGenericdataGenericbusinessentityGrid', GenericBusinessEntityGridDirective);

})(app);