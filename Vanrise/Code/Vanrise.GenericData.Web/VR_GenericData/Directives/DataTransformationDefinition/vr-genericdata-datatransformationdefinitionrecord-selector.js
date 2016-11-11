'use strict';
app.directive('vrGenericdataDatatransformationdefinitionrecordSelector', ['UtilsService', '$compile', 'VRUIUtilsService', 'VR_GenericData_DataTransformationDefinitionAPIService',
    function (UtilsService, $compile, VRUIUtilsService, VR_GenericData_DataTransformationDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                isdisabled: "=",
                customlabel: "@"
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new DataTransformationDefiniton(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return getDataTransformationDefinitonTemplate(attrs);
            }

        };


        function getDataTransformationDefinitonTemplate(attrs) {

            var multipleselection = "";

            var label = "Data Transformation Record";
            if (attrs.ismultipleselection != undefined) {
                label = "Data Transformation Record";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="RecordName" datavaluefield="RecordName" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Data Transformation Record" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }

        function DataTransformationDefiniton(ctrl, $scope, attrs) {

            var selectorApi;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                    ctrl.selectedvalues = undefined;
                };

                api.load = function (payload) {

                    var dataTransformationDefinitionId;
                    var selectedIds;
                    var filter = {};

                    if (payload) {
                        var payloadFilter = payload.filter;
                        if (payloadFilter != undefined)
                            filter = UtilsService.serializetoJson(payloadFilter);

                        dataTransformationDefinitionId = payload.dataTransformationDefinitionId;
                        selectedIds = payload.selectedIds;
                    }


                    return VR_GenericData_DataTransformationDefinitionAPIService.GetDataTransformationRecordsInfo(dataTransformationDefinitionId, filter).then(function (response) {
                        api.clearDataSource();
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'RecordName', attrs, ctrl);
                        }


                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('RecordName', attrs, ctrl);
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);

