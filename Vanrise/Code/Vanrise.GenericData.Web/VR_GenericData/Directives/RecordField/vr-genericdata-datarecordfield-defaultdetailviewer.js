(function (app) {

    'use strict';
    
    vrGenericdataDatarecordfieldDefaultdetailviewer.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordStorageAPIService'];

    function vrGenericdataDatarecordfieldDefaultdetailviewer(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordStorageAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DefaultdetailviewerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordField/Templates/DefaultDetailViewer.html"
        };

        function DefaultdetailviewerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.detailItem = payload.detailItem;
                    $scope.fieldValue = payload.fieldValue;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataDatarecordfieldDefaultdetailviewer', vrGenericdataDatarecordfieldDefaultdetailviewer);

})(app);