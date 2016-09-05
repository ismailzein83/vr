(function (app) {

    'use strict';

    VolumeCommitmentGridDirective.$inject = ['WhS_BE_VolumeCommitmentAPIService', 'WhS_BE_VolumeCommitmentService', 'VRNotificationService'];

    function VolumeCommitmentGridDirective(WhS_BE_VolumeCommitmentAPIService, WhS_BE_VolumeCommitmentService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dealGrid = new VolumeCommitmentGrid($scope, ctrl, $attrs);
                dealGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/Deal/Templates/VolumeCommitmentTemplate.html'
        };

        function VolumeCommitmentGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataSource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_BE_VolumeCommitmentAPIService.GetFilteredVolumeCommitments(dataRetrievalInput).then(function (response) {

                        if (response != undefined && response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                            }
                        }

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

                api.onVolumeCommitmentAdded = function (addedVolumeCommitment) {
                    gridAPI.itemAdded(addedVolumeCommitment);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editVolumeCommitment,
                }];

                function editVolumeCommitment(dataItem) {
                    var onVolumeCommitmentUpdated = function (updatedVolumeCommitment) {
                        gridAPI.itemUpdated(updatedVolumeCommitment);
                    };
                    WhS_BE_VolumeCommitmentService.editVolumeCommitment(dataItem.Entity.VolumeCommitmentId, onVolumeCommitmentUpdated);
                }
            }
        }
    }

    app.directive('vrWhsBeVolumecommitmentGrid', VolumeCommitmentGridDirective);

})(app);