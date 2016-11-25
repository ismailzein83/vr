"use strict";

app.directive("vrNpSalecodeGrid", ["UtilsService", "VRNotificationService", "Vr_NP_SaleCodeAPIService",
function (UtilsService, VRNotificationService, Vr_NP_SaleCodeAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SaleCodeGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_NumberingPlan/Directives/SaleCode/Templates/SaleCodeGridTemplate.html"

    };

    function SaleCodeGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.showGrid = false;
            $scope.salecodes = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                $scope.hidesalezonecolumn = false;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {
                        $scope.hidesalezonecolumn = payload.hidesalezonecolumn;
                        return gridAPI.retrieveData(payload.queryHandler);
                    };
                   
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
               
                return Vr_NP_SaleCodeAPIService.GetFilteredSaleCodes(dataRetrievalInput)
                    .then(function (response) {
                        $scope.showGrid = true;
                         onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }

    }

    return directiveDefinitionObject;

}]);
