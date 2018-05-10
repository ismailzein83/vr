"use strict";
app.directive("vrGenericdataDatastoresettingBusinessobject", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var businessObject = new BusinessObject($scope, ctrl, $attrs);
            businessObject.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStoreSetting/Templates/BusinessObjectTemplate.html"
    };


    function BusinessObject($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                if (payload != undefined) {
                }
            };

            api.getData = function () {

                return {
                    $type: "Vanrise.GenericData.Business.BusinessObjectDataStoreSettings, Vanrise.GenericData.Business",
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}
]);