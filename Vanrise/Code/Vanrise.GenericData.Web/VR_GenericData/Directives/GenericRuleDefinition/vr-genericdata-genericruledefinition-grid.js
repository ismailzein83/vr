(function (app) {

    'use strict';

    GenericRuleDefinitionGridDirective.$inject = ['VR_GenericData_GenericRuleDefinitionAPIService', 'VRNotificationService'];

    function GenericRuleDefinitionGridDirective(VR_GenericData_GenericRuleDefinitionAPIService, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericRuleDefinitionGrid = new GenericRuleDefinitionGrid($scope, ctrl, $attrs);
                genericRuleDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/GenericData/Directives/GenericRuleDefinition/Templates/GenericRuleDefinitionGridTemplate.html'
        };

        function GenericRuleDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {

                $scope.genericRuleDefinitions = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_GenericRuleDefinitionAPIService.GetFilteredGenericRuleDefinitions(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function getDirectiveAPI() {

                var directiveAPI = {};

                directiveAPI.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                directiveAPI.onGenericRuleDefinitionAdded = function (addedGenericRuleDefinition) {
                    gridAPI.itemAdded(addedGenericRuleDefinition);
                };

                return directiveAPI;
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: 'Edit',
                    clicked: editGenericRuleDefinition,
                }, {
                    name: 'Delete',
                    clicked: deleteGenericRuleDefinition,
                }];
            }

            function editGenericRuleDefinition(genericRuleDefinition) {
                var onUserUpdated = function (userObj) {
                    gridAPI.itemUpdated(userObj);
                }

                VR_Sec_UserService.editUser(userObj.Entity.UserId, onUserUpdated);
            }

            function deleteGenericRuleDefinition(genericRuleDefinition) {

            }
        }

        return directiveDefinitionObject;
    }

    app.directive('vrGenericdataGenericruledefinitionGrid', GenericRuleDefinitionGridDirective);

})(app);