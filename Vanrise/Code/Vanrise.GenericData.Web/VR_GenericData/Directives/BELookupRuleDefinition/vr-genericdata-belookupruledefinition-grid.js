﻿(function (app) {

    'use strict';

    BELookupRuleDefinitionGridDirective.$inject = ['VR_GenericData_BELookupRuleDefinitionAPIService', 'VRNotificationService'];

    function BELookupRuleDefinitionGridDirective(VR_GenericData_BELookupRuleDefinitionAPIService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var beLookupRuleDefinitionGrid = new BELookupRuleDefinitionGrid($scope, ctrl, $attrs);
                beLookupRuleDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BELookupRuleDefinition/Templates/BELookupRuleDefinitionGridTemplate.html'
        };

        function BELookupRuleDefinitionGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;

            function initializeController()
            {
                $scope.scopeModel = {};
                $scope.scopeModel.dataSource = [];

                $scope.scopeModel.onGridReady = function (api)
                {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady)
                {
                    return VR_GenericData_BELookupRuleDefinitionAPIService.GetFilteredBELookupRuleDefinitions(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
            }

            function defineAPI()
            {
                var api = {};

                api.load = function (query)
                {
                    return gridAPI.retrieveData(query);
                };

                api.onBELookupRuleDefinitionAdded = function (addedBELookupRuleDefinition)
                {
                    gridAPI.itemAdded(addedBELookupRuleDefinition);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions()
            {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editBELookupRuleDefinition
                }, {
                    name: 'Delete',
                    clicked: deleteBELookupRuleDefinition
                }];

                function editBELookupRuleDefinition(dataItem)
                {
                    console.log('edit');
                }

                function deleteBELookupRuleDefinition(dataItem)
                {
                    console.log('delete');
                }
            }
        }
    }

    app.directive('vrGenericdataBelookupruledefinitionGrid', BELookupRuleDefinitionGridDirective);

})(app);