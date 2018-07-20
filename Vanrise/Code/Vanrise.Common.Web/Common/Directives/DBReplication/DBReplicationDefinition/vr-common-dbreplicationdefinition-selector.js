﻿'use strict';

app.directive('vrCommonDbreplicationdefinitionSelector', ['UtilsService', 'VRUIUtilsService', 'VRCommon_DBReplicationDefinitionAPIService',
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

                var ctor = new DbReplicationSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'selectorCtrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function DbReplicationSelectorCtor(ctrl, $scope, attrs) {
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
                    var filter;

                    if (payload != undefined) {
                        dbReplicationDefinitionId = payload.dbReplicationDefinitionId;
                        filter = payload.filter;
                    }

                    return VRCommon_DBReplicationDefinitionAPIService.GetDBReplicationDefinitionsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (dbReplicationDefinitionId != undefined) {
                                VRUIUtilsService.setSelectedValues(dbReplicationDefinitionId, 'DBReplicationDefinitionId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('DBReplicationDefinitionId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }


        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "DB Replication Definition";
            var hideremoveicon = '';

            if (attrs.ismultipleselection != undefined) {
                label = "DB Replication Definitions";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            if (attrs.hideremoveicon != undefined)
                hideremoveicon = 'hideremoveicon';

            return  '<vr-select label="' + label + '" ' + multipleselection + ' ' + hideremoveicon + ' datatextfield="Name" datavaluefield="DBReplicationDefinitionId" isrequired="selectorCtrl.isrequired" '
                       + ' datasource="selectorCtrl.datasource" on-ready="selectorCtrl.onSelectorReady" selectedvalues="selectorCtrl.selectedvalues" onselectionchanged="selectorCtrl.onselectionchanged" ' 
                       + ' customvalidate="selectorCtrl.customvalidate">'
                    + '</vr-select>';
        }
    }]);