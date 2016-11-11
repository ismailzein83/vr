(function (app) {

    'use strict';

    BusinessentityRecordtypeSelectorDirective.$inject = ['VR_GenericData_GenericUIRuntimeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function BusinessentityRecordtypeSelectorDirective(VR_GenericData_GenericUIRuntimeAPIService, UtilsService, VRUIUtilsService) {
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
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var businessentityRecordtype = new BusinessentityRecordtype(ctrl, $scope, $attrs);
                businessentityRecordtype.initializeController();
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

        function BusinessentityRecordtype(ctrl, $scope, attrs) {
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
                    var filter;
                    var selectedIds;
                    var businessEntityId;
                    if (payload != undefined) {
                        businessEntityId = payload.businessEntityId;
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return VR_GenericData_GenericUIRuntimeAPIService.GetDataRecordTypesInfo(businessEntityId, UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'DataRecordTypeId', attrs, ctrl);
                        }
                    });
                };
                api.getIfSingleItem = function () {
                    if (ctrl.datasource.length == 1)
                        return ctrl.datasource[0];
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('DataRecordTypeId', attrs, ctrl);
                };

                return api;
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Record Type';
            if (attrs.ismultipleselection != undefined) {
                label = 'Record Types';
                multipleselection = 'ismultipleselection';
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            return  '<vr-columns colnum={{ctrl.normalColNum}}"><vr-select on-ready="ctrl.onSelectorReady"'
                    + ' datasource="ctrl.datasource"'
                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="DataRecordTypeId"'
                    + ' datatextfield="Name"'
                    + ' ' + multipleselection
                    + ' ' + hideselectedvaluessection
                    + ' isrequired="ctrl.isrequired"'
                    + ' ' + hideremoveicon
                    + ' label="' + label + '"'
                    + ' entityName="' + label + '"'
                + '</vr-select></vr-columns>';
        }
    }

    app.directive('vrGenericdataBusinessentityRecordtypeSelector', BusinessentityRecordtypeSelectorDirective);

})(app);