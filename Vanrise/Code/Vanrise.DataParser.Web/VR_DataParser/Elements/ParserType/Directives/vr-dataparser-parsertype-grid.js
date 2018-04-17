"use strict";

app.directive("vrDataparserParsertypeGrid", ["UtilsService", "VRNotificationService", "VR_DataParser_ParserTypeAPIService", "VR_DataParser_ParserTypeService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VR_DataParser_ParserTypeAPIService, VR_DataParser_ParserTypeService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var parserTypeGrid = new ParserTypeGrid($scope, ctrl, $attrs);
            parserTypeGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/Templates/ParserTypeGrid.html"
    };

    function ParserTypeGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.parserTypes = [];
            $scope.onGridReady = function (api) {
                     gridAPI = api;
                        if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                                ctrl.onReady(getDirectiveAPI());
                    function getDirectiveAPI() {
                                 var directiveAPI = {};
                                 directiveAPI.loadGrid = function (query) {
                                 return gridAPI.retrieveData(query);
                                 };

                        directiveAPI.onParserTypeAdded = function (parserTypeObject) {
                        gridAPI.itemAdded(parserTypeObject);
                    };
                    return directiveAPI;
                }
            };
          $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_DataParser_ParserTypeAPIService.GetFilteredParserTypes(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editParsertype

            }];
        }

        function editParsertype(ParserTypeObj) {
            var onParserTypeUpdated = function (ParserTypeObj) {
                gridAPI.itemUpdated(ParserTypeObj);
            };
           
            VR_DataParser_ParserTypeService.editParserType(ParserTypeObj.Entity.ParserTypeId, onParserTypeUpdated);
        }

    }

    return directiveDefinitionObject;

}]);