'use strict';

app.directive('vrCommonStatusdefinitionSelector', ['VR_Common_StatusDefinitionAPIService', 'UtilsService', 'VRUIUtilsService',

    function (VR_Common_StatusDefinitionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];
                ctrl.label = "Status Definition";
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined) {
                    ctrl.label = "Status Definitions";
                    ctrl.selectedvalues = [];
                }
                    

                var statusDefinitionSelector = new StatusDefinitionSelector(ctrl, $scope, $attrs);
                statusDefinitionSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function StatusDefinitionSelector(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    var selectfirstitem;
                    var filter;
                    if (payload != undefined) {
                        if (payload.fieldTitle != undefined)
                            ctrl.label = payload.fieldTitle;
                        selectedIds = payload.selectedIds;
                        selectfirstitem = payload.selectfirstitem != undefined && payload.selectfirstitem == true;
                        filter = payload.filter;
                        var businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        if(businessEntityDefinitionId != undefined)
                        {
                            if (filter == undefined)
                                filter = {};
                            filter.BusinessEntityDefinitionId = businessEntityDefinitionId;                           
                        }

                    }

                    return VR_Common_StatusDefinitionAPIService.GetStatusDefinitionsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'StatusDefinitionId', attrs, ctrl);
                            }
                            else if (selectfirstitem) {
                                var defaultValue = attrs.ismultipleselection != undefined ? [ctrl.datasource[0].StatusDefinitionId] : ctrl.datasource[0].StatusDefinitionId;
                                VRUIUtilsService.setSelectedValues(defaultValue, 'StatusDefinitionId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('StatusDefinitionId', attrs, ctrl);
                };
                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "{{ctrl.label}}";

            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                   '<vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="StatusDefinitionId" isrequired="ctrl.isrequired" label="' + label +
                       '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Status" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">' +
                   '</vr-select>' +
                   '</vr-columns>';
        }

    }]);