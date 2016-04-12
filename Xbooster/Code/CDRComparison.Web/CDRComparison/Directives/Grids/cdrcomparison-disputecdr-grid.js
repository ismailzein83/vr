(function (app) {

    'use strict';

    DisputeCDRGridDirective.$inject = ['CDRComparison_DisputeCDRAPIService', 'VRNotificationService'];

    function DisputeCDRGridDirective(CDRComparison_DisputeCDRAPIService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var disputeCDRGrid = new DisputeCDRGrid($scope, ctrl, $attrs);
                disputeCDRGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/CDRComparison/Directives/Grids/Templates/DisputeCDRGridTemplate.html'
        };

        function DisputeCDRGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.disputeCDRs = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return CDRComparison_DisputeCDRAPIService.GetFilteredDisputeCDRs(dataRetrievalInput).then(function (response) {
                        if (response && response.Data)
                        {
                            for(var i=0;i<response.Data.length ; i++)
                            {
                                response.Data[i].DifferenceDurationInSec = response.Data[i].PartnerDurationInSec - response.Data[i].SystemDurationInSec;
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('cdrcomparisonDisputecdrGrid', DisputeCDRGridDirective);

})(app);