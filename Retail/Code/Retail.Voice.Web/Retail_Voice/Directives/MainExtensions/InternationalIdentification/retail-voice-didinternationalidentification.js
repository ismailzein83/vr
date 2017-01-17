'use strict';

app.directive('retailVoiceDidinternationalidentification', ['UtilsService',
function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new didInternationalIdentification(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Retail_Voice/Directives/MainExtensions/InternationalIdentification/Templates/DIDInternationalIdentification.html'
    };


    function didInternationalIdentification(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                
            };

            api.getData = function () {
                var obj = {
                    $type: "Retail.Voice.MainExtensions.DIDInternationalIdentification, Retail.Voice.MainExtensions",
                };
                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);