'use strict';
app.directive('vrCommonExclusivesessiontypeSelector', ['VRCommon_VRExclusiveSessionTypeAPIService', 'VRUIUtilsService',
    function (VRCommon_VRExclusiveSessionTypeAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                isdisabled: "=",
                selectedvalues: '=',
                normalColNum: '@',
                onselectitem: "=",
                ondeselectitem: "=",
                hideselectedvaluessection: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];
                var ctor = new ExclusiveSessionTypeSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;
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
                return getTemplate(attrs);
            }
        };


        function getTemplate(attrs) {
            var label = "Session Type";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Session Types";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = "";
            if (attrs.hideremoveicon!=undefined)
                hideremoveicon = "hideremoveicon";

            var hideselectedvaluessection = "";
            if (attrs.hideselectedvaluessection != undefined)
                hideselectedvaluessection = "hideselectedvaluessection";

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                       + ' <vr-select on-ready="ctrl.onSelectorReady"'
                       + '  selectedvalues="ctrl.selectedvalues"'
                       + '  onselectionchanged="ctrl.onselectionchanged"'
                       + '  datasource="ctrl.datasource"'
                       + '  onselectitem="ctrl.onselectitem"'
                       + '  ondeselectitem="ctrl.ondeselectitem"'
                       + '  datavaluefield="VRExclusiveSessionTypeId"'
                       + '  datatextfield="Name"'
                       + '  ' + multipleselection
                       + '  ' + hideremoveicon
                       + '  ' + hideselectedvaluessection
                       + '  isrequired="ctrl.isrequired"'
                       + '  label="' + label + '"'
                       + ' entityName="' + label + '" >'
                       + '</vr-select>'
              + '</vr-columns>';
        }

        function ExclusiveSessionTypeSelectorCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(defineAPI());
                    }
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var selectedIds;
                    var filter;
                    var selectfirstitem;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        selectfirstitem = payload.selectfirstitem != undefined && payload.selectfirstitem == true;
                    }

                    return VR_AccountBalance_BalanceAccountTypeAPIService.GetVRExclusiveSessionTypeInfos(filter).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'VRExclusiveSessionTypeId', attrs, ctrl);
                            }
                            else if (selectfirstitem==true) {
                                var defaultValue = attrs.ismultipleselection != undefined ? [ctrl.datasource[0].Id] : ctrl.datasource[0].Id;
                                VRUIUtilsService.setSelectedValues(defaultValue, 'VRExclusiveSessionTypeId', attrs, ctrl);
                            }

                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('VRExclusiveSessionTypeId', attrs, ctrl);
                };

                api.hasSingleItem = function () {
                    return ctrl.datasource.length == 1;
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;
            }
        }

        return directiveDefinitionObject;

    }]);