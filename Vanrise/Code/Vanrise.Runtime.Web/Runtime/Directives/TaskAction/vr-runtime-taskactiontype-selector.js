'use strict';
app.directive('vrRuntimeTaskactiontypeSelector', ['VR_Runtime_SchedulerTaskActionTypeAPIService', 'UtilsService', 'VRUIUtilsService',
    function (VR_Runtime_SchedulerTaskActionTypeAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];          

                var ctor = new taskActionTypeCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTaskActionTypeTemplate(attrs);
            }

        };


        function getTaskActionTypeTemplate(attrs) {

            var multipleselection = "";
            var label = "Type";
            if (attrs.ismultipleselection != undefined) {
                label = "Types";
                multipleselection = "ismultipleselection";
            }

        
            return '<div>'
                + '<vr-select ' + multipleselection + ' on-ready="ctrl.onSelectorReady" datatextfield="Name" datavaluefield="ActionTypeId" isrequired="ctrl.isrequired"'
               + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" entityName="'+label+'" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
               + '</div>';
        }

        function taskActionTypeCtor(ctrl, $scope, attrs) {

            var selectorApi;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var filter;
                    var selectFirstItem;
                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        selectFirstItem = payload.selectFirstItem != undefined && payload.selectFirstItem == true;
                    }
                    return getTaskActionTypeInfos(attrs, ctrl, selectedIds, filter, selectFirstItem);

                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ActionTypeId', attrs, ctrl);
                };
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        function getTaskActionTypeInfos(attrs, ctrl, selectedIds, filter, selectFirstItem) {
            return VR_Runtime_SchedulerTaskActionTypeAPIService.GetSchedulerTaskActionTypes(filter).then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    ctrl.datasource.push(itm);
                });
                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'ActionTypeId', attrs, ctrl);
                }
                else if (selectFirstItem == true) {
                    var defaultValue = attrs.ismultipleselection != undefined ? [ctrl.datasource[0].ActionTypeId] : ctrl.datasource[0].ActionTypeId;
                    VRUIUtilsService.setSelectedValues(defaultValue, 'ActionTypeId', attrs, ctrl);
                }
            });
        }
        return directiveDefinitionObject;
    }]);