﻿(function (app) {

    'use strict';

    GenericBusinessEntityGridDirective.$inject = ['VR_GenericData_GenericBusinessEntityAPIService', 'VRNotificationService'];

    function GenericBusinessEntityGridDirective(VR_GenericData_GenericBusinessEntityAPIService, VRNotificationService) {
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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericBusinessEntityGridTemplate.html'
        };

        function GenericBusinessEntityGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            
            var gridAPI;

            function initializeController() {
                ctrl.dataSource = [];
                ctrl.columns = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    console.log('in');
                    return VR_GenericData_GenericBusinessEntityAPIService.GetFilteredGenericBusinessEntities(dataRetrievalInput).then(function (response) {
                        console.log(response);
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
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
                    setTimeout(function () {
                        return gridAPI.retrieveData(gridQuery);
                    });

                    function setGridColumns() {
                        if (runtimeGrid != undefined && runtimeGrid.Columns != undefined) {
                            for (var i = 0; i < runtimeGrid.Columns.length; i++) {
                                ctrl.columns.push(runtimeGrid.Columns[i]);
                            }
                        }
                        console.log(ctrl.columns);
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