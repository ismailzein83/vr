'use strict';
app.directive('vrGenericdataDatarecordtypefieldNonemptyfilter', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new recordTypeFieldNotEmptyFilterCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordTypeFieldFilter/Templates/DataRecordTypeFieldNonEmptyFilter.html"

    };

    function recordTypeFieldNotEmptyFilterCtor(ctrl, $scope, $attrs) {

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function () {

            };

            api.getData = function () {
                return {
                    $type: "Vanrise.GenericData.Entities.NonEmptyRecordFilter, Vanrise.GenericData.Entities"
                };
            };

            api.getExpression = function () {
                return 'Is Not Empty';
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);