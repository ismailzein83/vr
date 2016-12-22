'use strict';

app.directive('retailBeCreditclassGrid', ['Retail_BE_CreditClassAPIService', 'Retail_BE_CreditClassService', 'VRNotificationService',
    function (Retail_BE_CreditClassAPIService, Retail_BE_CreditClassService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var creditClassGrid = new RetailBeCreditClassGrid($scope, ctrl, $attrs);
                creditClassGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/CreditClass/Templates/CreditClassGridTemplate.html'
        };

        function RetailBeCreditClassGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.creditClass = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_CreditClassAPIService.GetFilteredCreditClasss(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onCreditClassAdded = function (addedCreditClass) {
                    gridAPI.itemAdded(addedCreditClass);
                };

                api.onCreditClassUpdated = function (updatedCreditClass) {
                    gridAPI.itemUpdated(updatedCreditClass);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editCreditClass,
                    haspermission: hasEditCreditClassPermission
                });
            }
            function hasEditCreditClassPermission() {
                return Retail_BE_CreditClassAPIService.HasUpdateCreditClassPermission()
            }
            function editCreditClass(creditClassItem) {
                var onCreditClassUpdated = function (updatedCreditClass) {
                    gridAPI.itemUpdated(updatedCreditClass);
                };

                Retail_BE_CreditClassService.editCreditClass(creditClassItem.Entity.CreditClassId, onCreditClassUpdated);
            }
        }
    }]);
