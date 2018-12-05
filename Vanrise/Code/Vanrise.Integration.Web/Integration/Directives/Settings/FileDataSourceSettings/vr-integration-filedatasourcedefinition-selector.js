"use strict";

app.directive('vrIntegrationFiledatasourcedefinitionSelector', ['VR_Integration_DataSourceSettingAPIService', 'UtilsService', 'VRUIUtilsService', 'VR_Integration_DataSourceSettingService',
    function (VR_Integration_DataSourceSettingAPIService, UtilsService, VRUIUtilsService, VR_Integration_DataSourceSettingService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                hideremoveicon: '@',
                normalColNum: '@',
                isdisabled: '=',
                customlabel: '@',
                includeviewhandler : '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;

                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new FileDataSourceDefinitionsSelector(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getFileDataSourceDefinitionsSelectorTemplate(attrs);
            }
        };

        function FileDataSourceDefinitionsSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.addNewFileDataSourceDefinition = function () {
                    var onFileDataSourceDefinitionAdded = function (fileDataSourceDefinitionObj) {
                        ctrl.datasource.push(fileDataSourceDefinitionObj);
                        if (attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(fileDataSourceDefinitionObj);
                        else
                            ctrl.selectedvalues = fileDataSourceDefinitionObj;
                    };
                    VR_Integration_DataSourceSettingService.addFileDataSourceDefinition(onFileDataSourceDefinitionAdded, true);
                };

                $scope.scopeModel.onViewIconClicked = function (item) {
                    VR_Integration_DataSourceSettingService.viewFileDataSourceDefinition(item.FileDataSourceDefinitionId, true);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var selectedIds;
                    var filter;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    return VR_Integration_DataSourceSettingAPIService.GetFileDataSourceDefinitionInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'FileDataSourceDefinitionId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('FileDataSourceDefinitionId', attrs, ctrl);
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }

        function getFileDataSourceDefinitionsSelectorTemplate(attrs) {

            var multipleselection = "";
            var label = "File Data Source Definition";

            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }
            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="scopeModel.addNewFileDataSourceDefinition"';

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = "hideremoveicon";
            }

            var onviewclicked = "";
            if (attrs.includeviewhandler != undefined)
                onviewclicked = "onviewclicked='scopeModel.onViewIconClicked'";

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<span vr-disabled="ctrl.isdisabled">'
                + '<vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield= "Name" datavaluefield="FileDataSourceDefinitionId" isrequired= "ctrl.isrequired"'
                + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged"' + addCliked + ' ' + onviewclicked + ' entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'
                + '</vr-select></span></vr-columns>';
        }

        return directiveDefinitionObject;
    }]);