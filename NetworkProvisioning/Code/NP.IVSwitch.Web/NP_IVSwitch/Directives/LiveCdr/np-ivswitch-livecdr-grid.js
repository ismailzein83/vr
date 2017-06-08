"use strict";

app.directive('npIvswitchLivecdrGrid', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'NP_IVSwitch_LiveCdrAPIService',
function (VRNotificationService, VRUIUtilsService, UtilsService, NP_IVSwitch_LiveCdrAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new LiveCdrGrid($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/NP_IVSwitch/Directives/LiveCdr/Templates/LiveCdrGridTemplate.html"

    };

    function LiveCdrGrid($scope, ctrl, $attrs) {
        var gridAPI;

        function initializeController() {
            $scope.liveCdrs = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        $scope.sortingCalls = (query.sorting == 1) ? 'DESC' :'ASC';
                            return gridAPI.retrieveData(query.query);

                            
                    };

                    return directiveAPI;
                };
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return NP_IVSwitch_LiveCdrAPIService.GetFilteredLiveCdrs(dataRetrievalInput)
                   .then(function (response) {

                       if (response && response.Data) {
                          
                       }
                       onResponseReady(response);

                   })
                   
            };

        };

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);