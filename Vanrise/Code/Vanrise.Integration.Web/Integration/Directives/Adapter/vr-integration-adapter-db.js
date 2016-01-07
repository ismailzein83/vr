"use strict";

app.directive("vrIntegrationAdapterDb", ['UtilsService',
function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/Integration/Directives/Adapter/Templates/AdapterDBTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;


        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            $scope.connectionString = undefined;
            $scope.query = undefined;
            $scope.lastImportedId = 0;
          
            var api = {};
                        
            api.getData = function () {
                return {
                    $type: "Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterArgument, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments",
                    ConnectionString: $scope.connectionString,
                    Query: $scope.query
                };
            };
            api.getStateData = function () {
                return {
                    $type: "Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterState, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments",
                    LastImportedId: $scope.lastImportedId
                };
            };


            api.load = function (payload) {

                if (payload != undefined) {
                    var argumentData = payload.adapterArgument;
                    if (argumentData != undefined) {
                        $scope.connectionString = argumentData.ConnectionString;
                        $scope.query = argumentData.Query;
                    }

                    var adapterState = payload.adapterState;
                    if (adapterState != undefined) {
                        $scope.lastImportedId = adapterState.LastImportedId;
                    }
                }
              
                
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
