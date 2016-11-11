(function (app) {

    'use strict';

    DataStoreSelectorDirective.$inject = ['VR_GenericData_DataStoreAPIService', 'VR_GenericData_DataStoreService', 'UtilsService', 'VRUIUtilsService'];

    function DataStoreSelectorDirective(VR_GenericData_DataStoreAPIService, VR_GenericData_DataStoreService, UtilsService, VRUIUtilsService) {
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
                    return VR_GenericData_DataStoreAPIService.HasAddDataStore();
                };

                var dataStoreSelector = new DataStoreSelector(ctrl, $scope, $attrs);
                dataStoreSelector.initializeController();
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
                return getDirectiveTemplate(attrs);
            }
        };

        function DataStoreSelector(ctrl, $scope, attrs) {
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
                            ctrl.onAddDataStore = onAddDataStore;
                        selectedIds = payload.selectedIds;
                    }

                    return VR_GenericData_DataStoreAPIService.GetDataStoresInfo().then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'DataStoreId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('DataStoreId', attrs, ctrl);
                };

                return api;
            }
            function onAddDataStore() {
                var onDataStoreAdded = function (dataStoreObj) {
                    ctrl.datasource.push(dataStoreObj.Entity);
                    if (attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(dataStoreObj.Entity);
                    else
                        ctrl.selectedvalues = dataStoreObj.Entity;
                };
                VR_GenericData_DataStoreService.addDataStore(onDataStoreAdded);
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Data Store';
            if (attrs.ismultipleselection != undefined) {
                label = 'Data Stores';
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
                    + 'onaddclicked="ctrl.onAddDataStore"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="DataStoreId"'
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

    app.directive('vrGenericdataDatastoreSelector', DataStoreSelectorDirective);

})(app);