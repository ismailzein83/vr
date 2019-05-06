'use strict';

app.directive('retailRaPerioddefinitionSelector', ['VRUIUtilsService', 'UtilsService', 'Retail_RA_PeriodDefinitionAPIService', 'Retail_Be_TrafficTypeEnum',
    function (VRUIUtilsService, UtilsService, Retail_RA_PeriodDefinitionAPIService, Retail_Be_TrafficTypeEnum) {

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

                var periodCtor = new PeriodCtor(ctrl, $scope, $attrs);
                periodCtor.initializeController();
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
                + '  datavaluefield="PeriodDefinitionId"'
                + 'datadisabledfield="disablePeriodField"'
                + '  datatextfield="PeriodDefinitionName" hideselectall'
                + '  ' + multipleselection
                + '  isrequired="ctrl.isrequired"'
                //       + ' entityName="ctrl.fieldTitle"'
                + '  >'
                + '</vr-select>'

                + '</vr-columns>';
        }

        function PeriodCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            ctrl.useRemoteSelector = false;
            var filter = {};
            var selectedIds;
            var selectorAPI;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.fieldTitle = "Period";

                if (attrs.ismultipleselection != undefined) {
                    ctrl.fieldTitle = "Periods";
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
                var operatorId;
                var trafficType;


                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        operatorId = payload.operatorId;
                        trafficType = payload.trafficType;
                    }
                    var input = {
                        OperatorId: operatorId,
                        TrafficType : trafficType
                    };
                    var loadSelectorPromise = Retail_RA_PeriodDefinitionAPIService.GetPeriodDefinitionInfo(input).then(function (response) {
                        if (response != undefined) {
                            for (var i = 0; i < response.length; i++) {
                                var periodDefinition = response[i];
                                extendDataItem(periodDefinition);
                                ctrl.datasource.push(periodDefinition);
                            }
                            if (selectedIds != undefined)
                                VRUIUtilsService.setSelectedValues(selectedIds, 'PeriodDefinitionId', attrs, ctrl);
                        }
                    });

                    return loadSelectorPromise.promise;
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('PeriodDefinitionId', attrs, ctrl);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            function extendDataItem(dataItem) {
                dataItem.disablePeriodField = dataItem.HasDeclaration && dataItem.PeriodDefinitionId != selectedIds;
            }

        }

        return directiveDefinitionObject;

    }]);