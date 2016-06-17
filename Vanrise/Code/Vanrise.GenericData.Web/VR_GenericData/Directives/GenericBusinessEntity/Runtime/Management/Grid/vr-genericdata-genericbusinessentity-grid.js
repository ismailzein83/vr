﻿(function (app) {

    'use strict';

    GenericBusinessEntityGridDirective.$inject = [ 'VR_GenericData_GenericBusinessEntityAPIService', 'VRNotificationService','VR_GenericData_GenericBusinessEntityService'];

    function GenericBusinessEntityGridDirective(VR_GenericData_GenericBusinessEntityAPIService, VRNotificationService, VR_GenericData_GenericBusinessEntityService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericBusinessEntityGrid = new GenericBusinessEntityGrid($scope, ctrl, $attrs);
                genericBusinessEntityGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/Management/Grid/Templates/GenericBusinessEntityGridTemplate.html'
        };

        function GenericBusinessEntityGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            
            var gridAPI;

            function initializeController() {
                ctrl.dataSource = [];
                ctrl.columns = [];
                $scope.isValueBool= function(column,value,fieldDescription)
                {
                    if (column.Attribute.Type.indexOf("Boolean")>-1)
                    {
                      
                        column.fieldValue = value;
                        return true;
                    }else
                    {
                        column.fieldValue = fieldDescription;
                        return false;
                    }
                }
                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_GenericBusinessEntityAPIService.GetFilteredGenericBusinessEntities(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                setMenuActions();
            }

            function setMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editGenericBusinessEntity
                }, {
                    name: 'Delete',
                    clicked: deleteGenericBusinessEntity
                }];
            }

            function editGenericBusinessEntity(genericBusinessEntity) {
                var onGenericBusinessEntityUpdated = function (updatedGenericBusinessEntity) {
                    gridAPI.itemUpdated(updatedGenericBusinessEntity);
                };
                VR_GenericData_GenericBusinessEntityService.editGenericBusinessEntity(genericBusinessEntity.Entity.GenericBusinessEntityId, genericBusinessEntity.Entity.BusinessEntityDefinitionId, onGenericBusinessEntityUpdated);
            }

            function deleteGenericBusinessEntity(genericBusinessEntity) {
                var onGenericBusinessEntityDeleted = function () {
                    gridAPI.itemDeleted(genericBusinessEntity);
                };
                VR_GenericData_GenericBusinessEntityService.deleteGenericBusinessEntity($scope, genericBusinessEntity, onGenericBusinessEntityDeleted);
            }

            function getDirectiveAPI() {
                var api = {};
                 
                api.load = function (payload) {
                    var runtimeGrid;
                    var gridQuery;

                    if (payload != undefined) {
                        runtimeGrid = payload.runtimeGrid
                        gridQuery = payload.gridQuery;
                    }

                    setGridColumns();
                    return gridAPI.retrieveData(gridQuery);

                    function setGridColumns() {
                        if (runtimeGrid != undefined && runtimeGrid.Columns != undefined) {
                            ctrl.columns.length = 0;
                            for (var i = 0; i < runtimeGrid.Columns.length; i++) {
                                ctrl.columns.push(runtimeGrid.Columns[i]);
                            }
                        }
                    }
                };

                api.onGenericBusinessEntityAdded = function (addedGenericBusinessEntity) {
                    gridAPI.itemAdded(addedGenericBusinessEntity);
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataGenericbusinessentityGrid', GenericBusinessEntityGridDirective);

})(app);