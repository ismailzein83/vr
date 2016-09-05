'use strict';

app.directive('retailBeOperatordeclaredinfoGrid', ['Retail_BE_OperatorDeclaredInfoAPIService', 'Retail_BE_OperatorDeclaredInfoService', 'VRNotificationService', function (Retail_BE_OperatorDeclaredInfoAPIService, Retail_BE_OperatorDeclaredInfoService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var operatorDeclaredInfoGrid = new OperatorDeclaredInfoGrid($scope, ctrl, $attrs);
            operatorDeclaredInfoGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/OperatorDeclaredInfo/Templates/OperatorDeclaredInfoGridTemplate.html'
    };

    function OperatorDeclaredInfoGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.operatorDeclaredInfos = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_OperatorDeclaredInfoAPIService.GetFilteredOperatorDeclaredInfos(dataRetrievalInput).then(function (response) {
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            defineMenuActions();
        }
        function defineAPI() {
            var api = {};

            api.loadGrid = function (query) {
                return gridAPI.retrieveData(query);
            };

            api.onOperatorDeclaredInfoAdded = function (addedOperatorDeclaredInfo) {
                gridAPI.itemAdded(addedOperatorDeclaredInfo);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editOperatorDeclaredInfo//,
               /// haspermission: hasEditOperatorDeclaredInfoPermission
            });
        }
        function editOperatorDeclaredInfo(OperatorDeclaredInfo) {
            var onOperatorDeclaredInfoUpdated = function (updatedOperatorDeclaredInfo) {
                gridAPI.itemUpdated(updatedOperatorDeclaredInfo);
            };
            Retail_BE_OperatorDeclaredInfoService.editOperatorDeclaredInfo(OperatorDeclaredInfo.Entity.OperatorDeclaredInfoId, onOperatorDeclaredInfoUpdated);
        }
        //function hasEditOperatorDeclaredInfoPermission() {
        //    return Retail_BE_OperatorDeclaredInfoAPIService.HasUpdateOperatorDeclaredInfoPermission();
        //}
    }
}]);
