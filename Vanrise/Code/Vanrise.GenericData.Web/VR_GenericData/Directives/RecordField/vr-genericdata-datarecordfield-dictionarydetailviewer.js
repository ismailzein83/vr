(function (app) {

    'use strict';

    vrGenericdataDatarecordfieldDictionarydetailviewer.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordStorageAPIService'];

    function vrGenericdataDatarecordfieldDictionarydetailviewer(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordStorageAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DictionarydetailviewerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordField/Templates/DictionaryDetailViewer.html"
        };

        function DictionarydetailviewerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.detailItem = payload.detailItem;
                    $scope.fieldValues = [];

                    if (payload.fieldValue != undefined && payload.fieldValue.Value != undefined) {
                        for (var kvn in payload.fieldValue.Value) {
                            if (kvn != "$type") {
                                $scope.fieldValues.push({ key: kvn, value: payload.fieldValue.Value[kvn] });
                            }
                        }
                    }
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataDatarecordfieldDictionarydetailviewer', vrGenericdataDatarecordfieldDictionarydetailviewer);

})(app);