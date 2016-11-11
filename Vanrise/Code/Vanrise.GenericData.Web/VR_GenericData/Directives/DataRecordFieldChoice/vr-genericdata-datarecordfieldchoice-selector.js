(function (app) {

    'use strict';

    DataRecordFieldChoiceSelectorDirective.$inject = ['VR_GenericData_DataRecordFieldChoiceAPIService', 'VR_GenericData_DataRecordFieldChoiceService', 'UtilsService', 'VRUIUtilsService'];

    function DataRecordFieldChoiceSelectorDirective(VR_GenericData_DataRecordFieldChoiceAPIService, VR_GenericData_DataRecordFieldChoiceService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectitem: "=",
                ondeselectitem: "=",
                onselectionchanged: '=',
                ismultipleselection: "@",
                isrequired: "=",
                isdisabled: "=",
                customlabel: "@",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;
                ctrl.haspermission = function () {
                    return VR_GenericData_DataRecordFieldChoiceAPIService.HasAddDataRecordFieldChoice();
                };

                var dataRecordFieldChoiceSelector = new DataRecordFieldChoiceSelector(ctrl, $scope, $attrs);
                dataRecordFieldChoiceSelector.initializeController();
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
                return getDirectiveTemplate(attrs);
            }
        };

        function DataRecordFieldChoiceSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            
            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;

                    if (payload != undefined) {
                        if (payload.showaddbutton)
                            ctrl.onAddDataRecordFieldChoice = onAddDataRecordFieldChoice;
                        selectedIds = payload.selectedIds;
                    }

                    return VR_GenericData_DataRecordFieldChoiceAPIService.GetDataRecordFieldChoicesInfo().then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'DataRecordFieldChoiceId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('DataRecordFieldChoiceId', attrs, ctrl);
                };

                return api;
            }
            function onAddDataRecordFieldChoice() {
                var onDataRecordFieldChoiceAdded = function (dataRecordFieldChoiceObj) {
                    ctrl.datasource.push(dataRecordFieldChoiceObj.Entity);
                    if (attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(dataRecordFieldChoiceObj.Entity);
                    else
                        ctrl.selectedvalues = dataRecordFieldChoiceObj.Entity;
                };
                VR_GenericData_DataRecordFieldChoiceService.addDataRecordFieldChoice(onDataRecordFieldChoiceAdded);
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Data Record Field Choice';
            if (attrs.ismultipleselection != undefined) {
                label = 'Data Record Field Choices';
                multipleselection = 'ismultipleselection';
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;
            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            return '<div>'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
                    + ' datasource="ctrl.datasource"'
                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + 'onaddclicked="ctrl.onAddDataRecordFieldChoice"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="DataRecordFieldChoiceId"'
                    + ' datatextfield="Name"'
                    + ' ' + multipleselection
                    + ' ' + hideselectedvaluessection
                    + ' isrequired="ctrl.isrequired"'
                    + ' ' + hideremoveicon
                    + ' vr-disabled="ctrl.isdisabled"'
                    + ' label="' + label + '"'
                    + ' entityName="' + label + '"'
                    + ' haspermission="ctrl.haspermission" '
                + '</vr-select>'
            + '</div>';
        }
    }

    app.directive('vrGenericdataDatarecordfieldchoiceSelector', DataRecordFieldChoiceSelectorDirective);

})(app);