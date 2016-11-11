"use strict";

app.directive("vrGenericdataSummarygroupingcolumnsGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_KeyFieldMappingService",
    function (UtilsService, VRNotificationService, VR_GenericData_KeyFieldMappingService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var summaryGroupingColumns = new SummaryGroupingColumns($scope, ctrl, $attrs);
                summaryGroupingColumns.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/SummaryTransformationDefinition/Templates/SummaryGroupingColumnsManagement.html"

        };

        function SummaryGroupingColumns($scope, ctrl, $attrs) {
            var rawDataRecordTypeId;
            var summaryDataRecordTypeId;
            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {







                defineAPI();
            }

            function defineAPI() {
                var api = {};


                api.load = function (payload) {
                    if (payload != undefined) {

                        if (payload.rawDataRecordTypeId)
                            rawDataRecordTypeId = payload.rawDataRecordTypeId;

                        if (payload.summaryDataRecordTypeId)
                            summaryDataRecordTypeId = payload.summaryDataRecordTypeId;

                        if (payload.KeyFieldMappings && payload.KeyFieldMappings.length > 0) {
                            for (var i = 0; i < payload.KeyFieldMappings.length; i++) {
                                var dataItem = payload.KeyFieldMappings[i];
                                ctrl.datasourceColumnGrouping.push(dataItem);
                            }
                        }

                        if (payload.rawDataRecordTypeId == undefined || payload.summaryDataRecordTypeId == undefined)
                            ctrl.enableAddColumnGrouping = false;
                        else
                            ctrl.enableAddColumnGrouping = true;
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }




        }

        return directiveDefinitionObject;

    }
]);