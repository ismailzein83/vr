'use strict';

app.directive('vrCommonDbdefinitionSelector', ['UtilsService', 'VRUIUtilsService', 'VRCommon_DBReplicationDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VRCommon_DBReplicationDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new DbDefinitionSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'selectorCtrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function DbDefinitionSelectorCtor(ctrl, $scope, attrs) {
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
                    var dbReplicationDefinitionId;
                    var selectedIds;
                    var filter;

                    if (payload != undefined) {
                        dbReplicationDefinitionId = payload.dbReplicationDefinitionId;
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return VRCommon_DBReplicationDefinitionAPIService.GetDBDefinitionsInfo(dbReplicationDefinitionId, UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            ctrl.datasource = response;
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'DBDefinitionId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    var selectedDBDefinitions = VRUIUtilsService.getIdSelectedIds('DBDefinitionId', attrs, ctrl);
                    var selectedDBDefinitionIds = [];
                    for (var i = 0; i < selectedDBDefinitions.length; i++) {
                        selectedDBDefinitionIds.push({
                            DatabaseDefinitionId: selectedDBDefinitions[i]
                        });
                    }
                    return selectedDBDefinitionIds;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }


        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "DB Definition";
            var hideremoveicon = '';

            if (attrs.ismultipleselection != undefined) {
                label = "DB Definitions";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            if (attrs.hideremoveicon != undefined)
                hideremoveicon = 'hideremoveicon';

            return '<vr-select label="' + label + '" ' + multipleselection + ' ' + hideremoveicon + ' datatextfield="Name" datavaluefield="DBDefinitionId" isrequired="selectorCtrl.isrequired" '
                       + ' datasource="selectorCtrl.datasource" on-ready="selectorCtrl.onSelectorReady" selectedvalues="selectorCtrl.selectedvalues" onselectionchanged="selectorCtrl.onselectionchanged" '
                       + ' customvalidate="selectorCtrl.customvalidate">'
                    + '</vr-select>';
        }
    }]);