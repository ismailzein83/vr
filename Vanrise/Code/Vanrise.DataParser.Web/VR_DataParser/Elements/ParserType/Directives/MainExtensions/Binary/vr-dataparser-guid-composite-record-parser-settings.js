"use strict";
app.directive("vrDataparserGuidCompositeRecordParserSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new guidCompositeParserEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/GuidCompositeRecordParser.html"


    };

    function guidCompositeParserEditor($scope, ctrl) {

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
                    { $scope.scopeModel.fieldName = payload.ValueParser.FieldName; }
                    context = payload.context;

                }
            

                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
				return {
					$type: "Vanrise.DataParser.MainExtensions.CompositeFieldParsers.GuidCompositeParser,Vanrise.DataParser.MainExtensions",
					FieldName: $scope.scopeModel.fieldName
				};
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }




    return directiveDefinitionObject;

}]);