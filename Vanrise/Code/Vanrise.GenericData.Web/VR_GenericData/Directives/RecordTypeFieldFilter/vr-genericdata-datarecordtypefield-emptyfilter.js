'use strict';
app.directive('vrGenericdataDatarecordtypefieldEmptyfilter', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new recordTypeFieldEmptyFilterCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordTypeFieldFilter/Templates/DataRecordTypeFieldEmptyFilter.html"

    };

    function recordTypeFieldEmptyFilterCtor(ctrl, $scope, $attrs) {

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function () {

            };

            api.getData = function () {
                return {
                    $type: "Vanrise.GenericData.Entities.EmptyRecordFilter, Vanrise.GenericData.Entities"
                };
            };

            api.getExpression = function () {
                return 'Is Empty';
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);