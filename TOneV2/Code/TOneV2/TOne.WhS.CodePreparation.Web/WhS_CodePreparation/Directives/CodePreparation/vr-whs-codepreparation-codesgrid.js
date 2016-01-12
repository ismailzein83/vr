"use strict";

app.directive('vrWhsCodepreparationCodesgrid', ['VRNotificationService', 'VRUIUtilsService', 'WhS_CodePrep_CodePrepAPIService', 'UtilsService','WhS_CP_CodeItemStatusEnum',
function (VRNotificationService, VRUIUtilsService, WhS_CodePrep_CodePrepAPIService, UtilsService, WhS_CP_CodeItemStatusEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            selectedcodes: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new saleCodesGrid($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_CodePreparation/Directives/CodePreparation/Templates/CodePreparationSaleCodesGridTemplate.html"

    };

    function saleCodesGrid($scope, ctrl, $attrs) {
        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.salecodes = [];
            $scope.ZoneName;

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                     
                        $scope.ZoneName = query.ZoneName;
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onCodeAdded = function (codeItemObject) {
                        gridAPI.itemAdded(codeItemObject);
                    }
                    directiveAPI.clearUpdatedItems = gridAPI.clearUpdatedItems;
                    directiveAPI.getSelectedCodes = function () {
                        var selectedCodes = [];
                        angular.forEach($scope.salecodes, function (itm) {
                            if (itm.IsSelected)
                                selectedCodes.push(itm);
                        });
                        return selectedCodes;
                    };


                    return directiveAPI;
                }
            };

            $scope.onCodeChecked = function (dataItem) {
                if (ctrl.selectedcodes != undefined) {
                    var index = UtilsService.getItemIndexByVal(ctrl.selectedcodes, dataItem.Code, 'Code');
                    if (index >= 0 && !dataItem.IsSelected) {
                        ctrl.selectedcodes.splice(index, 1);
                    }
                    else if (index <= 0 && dataItem.IsSelected) {
                        ctrl.selectedcodes.push(dataItem);
                    }
                }
            }

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_CodePrep_CodePrepAPIService.GetCodeItems(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined)
                        {
                            for (var i = 0 ; i < response.Data.length; i++) {
                                mapDataNeeded(response.Data[i]);
                            }
                        }
                       
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            function mapDataNeeded(dataItem)
            {
                if (dataItem.Status == WhS_CP_CodeItemStatusEnum.ExistingMoved.value)
                {
                    dataItem.MovedTo = dataItem.OtherCodeZoneName;
                    dataItem.MovedFrom = "";
                } 
                else if (dataItem.Status == WhS_CP_CodeItemStatusEnum.NewMoved.value)
                {
                    dataItem.MovedFrom = dataItem.OtherCodeZoneName;
                    dataItem.MovedTo = "";
                }
                    
            }
        }
    }

    return directiveDefinitionObject;

}]);
