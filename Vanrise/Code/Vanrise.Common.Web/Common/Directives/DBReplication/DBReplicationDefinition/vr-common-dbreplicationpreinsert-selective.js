'use strict';

app.directive('vrCommonDbreplicationpreinsertSelective', ['UtilsService', 'VRUIUtilsService', 'VRCommon_DBReplicationDefinitionAPIService',
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

                var ctor = new DbReplicationPreInsertSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'selectorCtrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function DbReplicationPreInsertSelectorCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                ctrl.onDirectiveReady = function (api) {
                    directiveAPI = api;
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var promises = [];
                    var readyPreInsertSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(getDBReplicationPreInsertConfigs());

                    function getDBReplicationPreInsertConfigs() {
                        return VRCommon_DBReplicationDefinitionAPIService.GetDBReplicationPreInsert().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    ctrl.datasource.push(response[i]);
                                }

                                if (payload != undefined && payload.DBReplicationPreInsert != undefined) {
                                    UtilsService.getItemByVal(ctrl.datasource, payload.DBReplicationPreInsert.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;
                    if (ctrl.selectedvalues != undefined && directiveAPI != undefined) {
                        data = directiveAPI.getData();
                        if (data != undefined)
                            data.ConfigId = ctrl.selectedvalues.ExtensionConfigurationId;
                    }
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }


        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "DB Replication Pre Insert";
            var hideremoveicon = '';

            if (attrs.ismultipleselection != undefined) {
                label = "DB Replication Pre Insert";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            if (attrs.hideremoveicon != undefined)
                hideremoveicon = 'hideremoveicon';

            return '<vr-select' + ' ' + multipleselection + ' ' + hideremoveicon + ' datatextfield="Title" datavaluefield="DBReplicationDefinitionId" isrequired="selectorCtrl.isrequired" '
                       + ' datasource="selectorCtrl.datasource" on-ready="selectorCtrl.onSelectorReady" selectedvalues="selectorCtrl.selectedvalues" onselectionchanged="selectorCtrl.onselectionchanged" '
                       + ' customvalidate="selectorCtrl.customvalidate">'
                    + '</vr-select>'

            + '<vr-directivewrapper ng-if="selectorCtrl.selectedvalues != undefined" directive="selectorCtrl.selectedvalues.Editor"'
                 + 'on-ready="selectorCtrl.onDirectiveReady">'
                + '</vr-directivewrapper>';
        }
    }]);