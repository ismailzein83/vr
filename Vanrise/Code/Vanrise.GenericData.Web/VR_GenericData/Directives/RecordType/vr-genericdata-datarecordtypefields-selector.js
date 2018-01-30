'use strict';

app.directive('vrGenericdataDatarecordtypefieldsSelector', ['VR_GenericData_DataRecordFieldAPIService', 'UtilsService', '$compile', 'VRUIUtilsService',
    function (VR_GenericData_DataRecordFieldAPIService, UtilsService, $compile, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                type: "=",
                label: "@",
                ismultipleselection: "@",
                hideselectedvaluessection: '@',
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "=",
                showaddbutton: '@',
                hidelabel: '@',
                onselectitem: "=",
                ondeselectitem: "=",
                customvalidate: '=',
                onbeforeselectionchanged: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                ctrl.datasource = [];

                var ctor = new recordTypeFieldsCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {
            var label;
            if (attrs.hidelabel == undefined)
                if (attrs.label == undefined)
                    label = ' label="Field"';
                else
                    label = ' label="' + attrs.label + '"';

            var disabled = "";
            if (attrs.isdisabled)
                disabled = "vr-disabled='true'";

            var hideselectedvaluessection = "";
            if (attrs.hideselectedvaluessection != undefined)
                hideselectedvaluessection = "hideselectedvaluessection";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined)
                multipleselection = "ismultipleselection";

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            return ' <vr-select  datasource="ctrl.datasource" customvalidate="ctrl.customvalidate" on-ready="ctrl.onSelectorReady" isrequired="ctrl.isrequired" ' + hideselectedvaluessection + ' ' +hideremoveicon + ' selectedvalues="ctrl.selectedvalues" ' +disabled + ' onselectionchanged="ctrl.onselectionchanged" datatextfield="Title" datavaluefield="Name" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                   + 'entityname="Field" ' + label + ' ' + multipleselection + ' onbeforeselectionchanged="ctrl.onbeforeselectionchanged"></vr-select>';

        }

        function recordTypeFieldsCtor(ctrl, $scope, $attrs) {
            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {
                };

                api.load = function (payload) {
                    
                    var promises = [];
                    var selectedIds;
                    var dataRecordTypeId;
                    var filter;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        dataRecordTypeId = payload.dataRecordTypeId;
                        filter = payload.filter;
                    }

                    var serializedFilter;
                    if (filter != undefined) {
                        serializedFilter = UtilsService.serializetoJson(filter);
                    }

                    return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId, serializedFilter).then(function (response) {
                        api.clearDataSource();
                        if (response != undefined)
                            for (var i = 0; i < response.length; i++) {
                                var currentField = response[i];
                                ctrl.datasource.push(currentField.Entity);
                            }

                        if (selectedIds != undefined)
                            VRUIUtilsService.setSelectedValues(selectedIds, 'Name', $attrs, ctrl);
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Name', $attrs, ctrl);
                };
                api.getSelectedValue = function () {
                    return ctrl.selectedvalues;
                };
                api.clearDataSource = function () {
                    selectorAPI.clearDataSource();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

