(function (app) {

    'use strict';

    DataRecordStorageSelectorDirective.$inject = ['VR_GenericData_DataRecordStorageAPIService', 'VR_GenericData_DataRecordStorageService', 'UtilsService', 'VRUIUtilsService'];

    function DataRecordStorageSelectorDirective(VR_GenericData_DataRecordStorageAPIService, VR_GenericData_DataRecordStorageService, UtilsService, VRUIUtilsService) {
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
                customlabel: "@",
                customvalidate: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var dataRecordStorageSelector = new DataRecordStorageSelector(ctrl, $scope, $attrs);
                dataRecordStorageSelector.initializeController();
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

        function DataRecordStorageSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                var isReadyTrigged = false;

                ctrl.onSelectorReady = function (api) {
                    if (isReadyTrigged)
                        return;
                    isReadyTrigged = true;
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }
            function getDirectiveAPI() {
                var api = {};

                api.clearDataSource = function () {
                    selectorAPI.clearDataSource();
                };

                api.load = function (payload) {
                    var selectedIds;
                    var filter = null;
                    if (payload != undefined) {
                        filter = {};
                        filter.DataRecordTypeId = payload.DataRecordTypeId;
                        filter.Filters = payload.filters;
                        if (payload.showaddbutton)
                            ctrl.onAddDataStorageRecord = onAddDataStorageRecord;
                        selectedIds = payload.selectedIds;
                    };

                    return VR_GenericData_DataRecordStorageAPIService.GetDataRecordsStorageInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'DataRecordStorageId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('DataRecordStorageId', attrs, ctrl);
                };

                return api;
            }

            function onAddDataStorageRecord() {
                var onDataStorageRecordAdded = function (dataRecordStorageObj) {
                    ctrl.datasource.push(dataRecordStorageObj.Entity);
                    if (attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(dataRecordStorageObj.Entity);
                    else
                        ctrl.selectedvalues = dataRecordStorageObj.Entity;
                };
                VR_GenericData_DataRecordStorageService.addDataRecordStorage(onDataStorageRecordAdded);
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Data Storage Record';
            if (attrs.ismultipleselection != undefined) {
                label = 'Data Storage Records';
                multipleselection = 'ismultipleselection';
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            var lableplace = "";
            if (attrs.hidelable == undefined)
                lableplace = ' label="' + label + '"';

            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;
            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            var customvalidate = '';
            if (attrs.customvalidate != undefined) {
                customvalidate=' customvalidate="ctrl.customvalidate()"';
            }

            return '<div>'
                        + '<vr-select on-ready="ctrl.onSelectorReady"'
                                 + ' datasource="ctrl.datasource"'
                                 + ' selectedvalues="ctrl.selectedvalues"'
                                 + ' onselectionchanged="ctrl.onselectionchanged"'
                                 + ' onaddclicked="ctrl.onAddDataStorageRecord"'
                                 + ' onselectitem="ctrl.onselectitem"'
                                 + ' ondeselectitem="ctrl.ondeselectitem"'
                                 + ' datavaluefield="DataRecordStorageId"'
                                 + ' datatextfield="Name"'
                                 + ' ' + customvalidate
                                 + ' ' + multipleselection
                                 + ' ' + hideselectedvaluessection
                                 + ' ' + hideremoveicon
                                 + ' isrequired="ctrl.isrequired"'
                                 + ' ' + lableplace
                                 + ' entityName="' + label + '"'
                        + '</vr-select>'
                 + '</div>';
        }
    }

    app.directive('vrGenericdataDatarecordstorageSelector', DataRecordStorageSelectorDirective);

})(app);