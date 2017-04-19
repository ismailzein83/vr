(function (appControllers) {

    "use strict";

    parserTypeManagementController.$inject = ['$scope', 'VR_DataParser_ParserTypeService'];

    function parserTypeManagementController($scope, VR_DataParser_ParserTypeService) {

        var gridAPI;
        var filter = {};

        defineScope();
        load();

              function defineScope() {
                 $scope.searchClicked = function () {
                        getFilterObject();
                        return gridAPI.loadGrid(filter);
                    };

                 function getFilterObject() {
                        filter = {
                            Name: $scope.name
                        };
                    }

                  $scope.onGridReady = function (api) {
                        gridAPI = api;
                        api.loadGrid(filter);
                  };

                  $scope.addNewParserType = addNewParserType;
                }

              function load() {
                    loadAllControls();
                }

              function loadAllControls()
                {}

              function addNewParserType() {
                    var onParserTypeAdded = function (parserTypeObj) {
                        gridAPI.onParserTypeAdded(parserTypeObj);
                    };
                    VR_DataParser_ParserTypeService.addParserType(onParserTypeAdded);
                }

        
    }

    appControllers.controller('VR_DataParser_ParserTypeManagementController', parserTypeManagementController);
})(appControllers);