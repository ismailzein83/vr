(function (app) {

    'use strict';

    VRDataRecordObjectType.$inject = ["UtilsService", 'VRUIUtilsService'];

    function VRDataRecordObjectType(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordObjectType = new DataRecordObjectType($scope, ctrl, $attrs);
                dataRecordObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/VROjectType/Templates/VRDaraRecordObjectTypeTemplate.html"

        };
        function DataRecordObjectType($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var recordTypeId;

                    if (payload != undefined) {
                        recordTypeId = payload.recordTypeId;
                    }

                    if (recordTypeId != undefined) {
                        $scope.scopeModel.recordTypeId = recordTypeId;
                    }
                };

                api.getData = function() {
                    var data = {
                        $type: "Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordObjectType, Vanrise.GenericData.MainExtensions",
                        ClassName: $scope.scopeModel.recordTypeId
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataDatarecordobjecttypePropertyevaluator', VRDataRecordObjectType);

})(app);