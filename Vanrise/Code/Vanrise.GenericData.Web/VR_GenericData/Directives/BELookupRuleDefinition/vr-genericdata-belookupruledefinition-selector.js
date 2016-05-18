(function (app) {

    'use strict';

    BelookupruledefinitionSelectorDirective.$inject = ['VR_GenericData_DataRecordStorageAPIService', 'VR_GenericData_DataRecordStorageService', 'UtilsService', 'VRUIUtilsService'];

    function BelookupruledefinitionSelectorDirective(VR_GenericData_DataRecordStorageAPIService, VR_GenericData_DataRecordStorageService, UtilsService, VRUIUtilsService) {
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

                var belookupruledefinitionSelector = new BelookupruledefinitionSelector(ctrl, $scope, $attrs);
                belookupruledefinitionSelector.initializeController();
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

        function BelookupruledefinitionSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                }
            }

            function getDirectiveAPI() {
                var api = {};

                api.clearDataSource = function () {
                    selectorAPI.clearDataSource();
                }

                api.load = function (payload) {
                    var selectedIds;
                    var filter = null;
                    if (payload != undefined) {
                        filter = {};
                        selectedIds = payload.selectedIds;
                    }

                    return VR_GenericData_DataRecordStorageAPIService.GetDataRecordsStorageInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds) {
                                VRUIUtilsService.setSelectedValues(selectedIds, '', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('', attrs, ctrl);
                };

                return api;
            }
            
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = '';
            if (attrs.ismultipleselection != undefined) {
                label = '';
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
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="DataRecordStorageId"'
                    + ' datatextfield="Name"'
                    + ' ' + multipleselection
                    + ' ' + hideselectedvaluessection
                    + ' isrequired="ctrl.isrequired"'
                    + ' ' + hideremoveicon
                    + ' vr-disabled="ctrl.isdisabled"'
                    + ' label="' + label + '"'
                    + ' entityName="' + label + '"'
                + '</vr-select>'
            + '</div>';
        }
    }

    app.directive('vrGenericdataBelookupruledefinitionSelector', BelookupruledefinitionSelectorDirective);

})(app);