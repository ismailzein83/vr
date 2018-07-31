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

            var context;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.selectedvalues = undefined;

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        ctrl.isLoadingDirective = value;
                    };
                    var directivePayload = { context: getContext() };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var promises = [];

                    if (payload != undefined) {
                        context = payload.context;

                        if (payload.DBReplicationPreInsert != undefined) {
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            promises.push(loadDirective());
                        }
                    }

                    promises.push(getDBReplicationPreInsertConfigs());

                    function getDBReplicationPreInsertConfigs() {
                        return VRCommon_DBReplicationDefinitionAPIService.GetDBReplicationPreInsert().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    ctrl.datasource.push(response[i]);
                                }
                                if (payload != undefined && payload.DBReplicationPreInsert != undefined) {
                                    $scope.scopeModel.selectedvalues = UtilsService.getItemByVal(ctrl.datasource, payload.DBReplicationPreInsert.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    function loadDirective() {
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { context: getContext() };
                            if (payload != undefined)
                                directivePayload.Settings = payload.DBReplicationPreInsert;

                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedvalues != undefined && directiveAPI != undefined) {
                        data = directiveAPI.getData();
                        if (data != undefined)
                            data.ConfigId = $scope.scopeModel.selectedvalues.ExtensionConfigurationId;
                    }
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
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
                       + ' datasource="selectorCtrl.datasource" on-ready="selectorCtrl.onSelectorReady" selectedvalues="scopeModel.selectedvalues" onselectionchanged="selectorCtrl.onselectionchanged" '
                       + ' customvalidate="selectorCtrl.customvalidate">'
                    + '</vr-select>'
            + '<vr-directivewrapper ng-if="scopeModel.selectedvalues != undefined" directive="scopeModel.selectedvalues.Editor" vr-loader="selectorCtrl.isLoadingDirective"'
                 + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{selectorCtrl.normalColNum}}" isrequired="selectorCtrl.isrequired" customvalidate="selectorCtrl.customvalidate">'
            + '</vr-directivewrapper>';
        }
    }]);