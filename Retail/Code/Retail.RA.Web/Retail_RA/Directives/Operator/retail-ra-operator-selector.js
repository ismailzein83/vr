'use strict';

app.directive('retailRaOperatorSelector', ['VRUIUtilsService', 'UtilsService','Retail_RA_OperatorDefinitionAPIService',
    function (VRUIUtilsService, UtilsService, Retail_RA_OperatorDefinitionAPIService) {

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
                customlabel: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var operatorCtor = new OperatorCtor(ctrl, $scope, $attrs);
                operatorCtor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getAccountTemplate(attrs);
            }
        };


        function getAccountTemplate(attrs) {
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }


            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<vr-label>{{ctrl.fieldTitle}}</vr-label>'
                + '<vr-select on-ready="ctrl.onSelectorReady"' //DataSource Selector
                + '  selectedvalues="ctrl.selectedvalues"'
                + '  onselectionchanged="ctrl.onselectionchanged"'
                + '  datasource="ctrl.datasource"'
                + '  datavaluefield="OperatorId"'
                + '  datatextfield="OperatorName" hideselectall'
                + '  ' + multipleselection
                + '  isrequired="ctrl.isrequired"'
                //       + ' entityName="ctrl.fieldTitle"'
                + '  >'
                + '</vr-select>'

                + '</vr-columns>';
        }

        function OperatorCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            ctrl.useRemoteSelector = false;
            var filter = {};

            var selectorAPI;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.fieldTitle = "Operator";

                if (attrs.ismultipleselection != undefined) {
                    ctrl.fieldTitle = "Operators";
                }

                if (attrs.customlabel != undefined) {
                    ctrl.fieldTitle = attrs.customlabel;
                }

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                ctrl.search = function (nameFilter) {
                    return GetAccountsInfo(nameFilter);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var selectedIds;
                    var loadSelectorPromise = Retail_RA_OperatorDefinitionAPIService.GetOperatorDefinitionInfo().then(function (response) {
                        if (response != undefined) {
                            for (var i = 0; i < response.length; i++) {
                                var operator = response[i];
                                ctrl.datasource.push(operator);
                            }
                        }
                    });
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        if (selectedIds != undefined)
                            VRUIUtilsService.setSelectedValues(selectedIds, 'OperatorId', attrs, ctrl);
                    }
                    return loadSelectorPromise.promise;
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('OperatorId', attrs, ctrl);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
         
        }

        return directiveDefinitionObject;

    }]);