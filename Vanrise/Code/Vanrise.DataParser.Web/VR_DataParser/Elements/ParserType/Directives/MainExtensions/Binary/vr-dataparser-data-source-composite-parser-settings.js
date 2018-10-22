"use strict";
app.directive("vrDataparserDataSourceCompositeParserSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new dateFromDayNumberCompositeParserEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/DataSourceCompositeRecordParser.html"


    };

    function dateFromDayNumberCompositeParserEditor($scope, ctrl) {

        var context;
        var dataRecordTypeFieldSelectorAPI;
        var dataRecordTypeFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.onDataRecordTypeFieldsSelectorReady = function (api) {
                dataRecordTypeFieldSelectorAPI = api;
                dataRecordTypeFieldSelectorReadyPromiseDeferred.resolve();
            };
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    if (payload.ValueParser != undefined)
                    {
                        $scope.scopeModel.dataSourceFieldName = payload.ValueParser.DataSourceFieldName;

                    }
                    context = payload.context;

                }
            

                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
				return {
					$type: "Vanrise.DataParser.MainExtensions.CompositeFieldParsers.DataSourceCompositeParser,Vanrise.DataParser.MainExtensions",
					DataSourceFieldName: $scope.scopeModel.dataSourceFieldName
				};
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }




    return directiveDefinitionObject;

}]);