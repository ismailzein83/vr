(function (app) {

    'use strict';

    GenericBusinessEntityGridDirective.$inject = ['VR_GenericData_GenericBEDefinitionAPIService', 'VRNotificationService', 'VR_GenericData_GenericBusinessEntityAPIService', 'VRUIUtilsService', 'UtilsService'];

    function GenericBusinessEntityGridDirective(VR_GenericData_GenericBEDefinitionAPIService, VRNotificationService, VR_GenericData_GenericBusinessEntityAPIService, VRUIUtilsService, UtilsService) {
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

            var gridAPI;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.columns = [];
                $scope.scopeModel.menuActions = [];
                $scope.scopeModel.businessEntities = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                    return VR_GenericData_GenericBusinessEntityAPIService.GetFilteredGenericBusinessEntities(dataRetrievalInput).then(function (response) {
                        console.log(response);
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                //  var vrCase = response.Data[i];
                                //Retail_BE_AccountBEService.defineAccountViewTabs(vrCaseDefinitionId, vrCase, gridAPI, vrCaseViewDefinitions);
                                // Retail_BE_AccountActionService.defineAccountMenuActions(vrCaseDefinitionId, vrCase, gridAPI, vrCaseViewDefinitions, vrCaseActionDefinitions);
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
            //function editGenericBusinessEntity(genericBusinessEntity) {
            //    var onGenericBusinessEntityUpdated = function (updatedGenericBusinessEntity) {
            //        gridDrillDownTabsObj.setDrillDownExtensionObject(updatedGenericBusinessEntity);
            //        gridAPI.itemUpdated(updatedGenericBusinessEntity);
            //    };
            //    VR_GenericData_GenericBusinessEntityService.editGenericBusinessEntity(genericBusinessEntity.Entity.GenericBusinessEntityId, genericBusinessEntity.Entity.BusinessEntityDefinitionId, onGenericBusinessEntityUpdated);
            //}

            //function deleteGenericBusinessEntity(genericBusinessEntity) {
            //    var onGenericBusinessEntityDeleted = function () {
            //        gridAPI.itemDeleted(genericBusinessEntity);
            //    };
            //    VR_GenericData_GenericBusinessEntityService.deleteGenericBusinessEntity($scope, genericBusinessEntity, onGenericBusinessEntityDeleted);
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

                        //Loading VRCaseGridColumns
                        var businessEntityGridColumnsLoadPromise = getBusinessEntityGridColumnsLoadPromise();
                        promises.push(businessEntityGridColumnsLoadPromise);

                        ////Loading AccountViewDefinitions
                        //var vrCaseViewDefinitionsLoadPromise = getAccountViewDefinitionsLoadPromise();
                        //promises.push(vrCaseViewDefinitionsLoadPromise);

                        ////Loading AccountViewDefinitions
                        //var vrCaseActionDefinitionsLoadPromise = getAccountActionDefinitionsLoadPromise();
                        //promises.push(vrCaseActionDefinitionsLoadPromise);
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

                    //function getAccountViewDefinitionsLoadPromise() {
                    //    var vrCaseViewDefinitionsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    //    Retail_BE_AccountBEDefinitionAPIService.GetAccountViewDefinitions(vrCaseDefinitionId).then(function (response) {
                    //        vrCaseViewDefinitions = response;
                    //        vrCaseViewDefinitionsLoadPromiseDeferred.resolve();
                    //    }).catch(function (error) {
                    //        vrCaseViewRuntimeEditorsLoadPromiseDeferred.reject(error);
                    //    });

                    //    return vrCaseViewDefinitionsLoadPromiseDeferred.promise;
                    //}
                    //function getAccountActionDefinitionsLoadPromise() {
                    //    var vrCaseActionDefinitionsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    //    Retail_BE_AccountBEDefinitionAPIService.GetAccountActionDefinitions(vrCaseDefinitionId).then(function (response) {
                    //        vrCaseActionDefinitions = response;
                    //        vrCaseActionDefinitionsLoadPromiseDeferred.resolve();
                    //    }).catch(function (error) {
                    //        vrCaseViewRuntimeEditorsLoadPromiseDeferred.reject(error);
                    //    });

                    //    return vrCaseActionDefinitionsLoadPromiseDeferred.promise;
                    //}

                    function buildGridQuery(gridQuery) {
                        return {
                            BusinessEntityDefinitionId: businessEntityDefinitionId,
                           // Columns: gridColumnFieldNames,
                        };
                    }

                    return gridLoadDeferred.promise;
                };

                api.onBusinessEntityAdded = function (addedBusinessEntity) {
                    //Retail_BE_AccountBEService.defineAccountViewTabs(vrCaseDefinitionId, addedAccount, gridAPI, vrCaseViewDefinitions);
                    //Retail_BE_AccountActionService.defineAccountMenuActions(vrCaseDefinitionId, addedAccount, gridAPI, vrCaseViewDefinitions, vrCaseActionDefinitions);
                    gridAPI.itemAdded(addedBusinessEntity);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getDirectiveAPI() {
                var api = {};
                 
                api.load = function (payload) {
                    var runtimeGrid;
                    var gridQuery;

                    if (payload != undefined) {
                        runtimeGrid = payload.runtimeGrid;
                        gridQuery = payload.gridQuery;
                    }

                  
                    return gridAPI.retrieveData(gridQuery);
                };

                api.onGenericBusinessEntityAdded = function (addedGenericBusinessEntity) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedGenericBusinessEntity);
                    gridAPI.itemAdded(addedGenericBusinessEntity);
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataGenericbusinessentityGrid', GenericBusinessEntityGridDirective);

})(app);