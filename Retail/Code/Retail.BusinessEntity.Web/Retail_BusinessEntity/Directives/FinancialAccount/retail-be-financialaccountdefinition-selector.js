'use strict';
app.directive('retailBeFinancialaccountdefinitionSelector', ['Retail_BE_FinancialAccountDefinitionAPIService', 'VRUIUtilsService','UtilsService',
function (Retail_BE_FinancialAccountDefinitionAPIService, VRUIUtilsService,UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                isdisabled: "=",
                selectedvalues: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];
                var ctor = new FinancialAccountDefinitionSelectorCtor(ctrl, $scope, $attrs);
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
            var label = "Financial Account Type";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Financial Account Types";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                       + ' <vr-select on-ready="ctrl.onSelectorReady"'
                       + '  selectedvalues="ctrl.selectedvalues"'
                       + '  onselectionchanged="ctrl.onselectionchanged"'
                       + '  datasource="ctrl.datasource"'
                       + '  datavaluefield="FinancialAccountDefinitionId"'
                       + '  datatextfield="Name"'
                       + '  ' + multipleselection
                       + '  ' + hideremoveicon
                       + '  isrequired="ctrl.isrequired"'
                       + '  label="' + label + '"'
                       + ' entityName="' + label + '"'
                       + '  >'
                       + '</vr-select>'
              + '</vr-columns>';
        }

        function FinancialAccountDefinitionSelectorCtor(ctrl, $scope, attrs) {

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
                    selectorAPI.clearDataSource();

                    var selectedIds;
                    var filter;
                    var selectfirstitem;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        selectfirstitem = payload.selectfirstitem != undefined && payload.selectfirstitem == true;
                    }

                    return Retail_BE_FinancialAccountDefinitionAPIService.GetFinancialAccountDefinitionsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'FinancialAccountDefinitionId', attrs, ctrl);
                            }
                            else if (selectfirstitem == true) {
                                var defaultValue = ctrl.datasource[0].FinancialAccountDefinitionId;
                                if (attrs.ismultipleselection != undefined)
                                    defaultValue = [defaultValue];;
                                VRUIUtilsService.setSelectedValues(defaultValue, 'FinancialAccountDefinitionId', attrs, ctrl);
                            }

                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('FinancialAccountDefinitionId', attrs, ctrl);
                };

                api.hasSingleItem = function () {
                    return ctrl.datasource.length == 1;
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }]);