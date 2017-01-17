'use strict';

app.directive('retailVoiceDidaccountidentification', ['UtilsService',
function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new didAccountIdentification(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Retail_Voice/Directives/MainExtensions/AccountIdentification/Templates/DIDAccountIdentification.html'
    };


    function didAccountIdentification(ctrl, $scope, $attrs) {
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
                    $type: "Retail.Voice.MainExtensions.DIDAccountIdentification, Retail.Voice.MainExtensions",
                };
                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);