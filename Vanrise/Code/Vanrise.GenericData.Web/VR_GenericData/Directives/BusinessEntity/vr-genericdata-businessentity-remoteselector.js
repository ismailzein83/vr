'use strict';

app.directive('vrGenericdataBusinessentityRemoteselector', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_VRRestAPIBusinessEntityAPIService', 'VR_GenericData_BusinessEntityDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_VRRestAPIBusinessEntityAPIService, VR_GenericData_BusinessEntityDefinitionAPIService) {
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

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new BusinessEntityRemoteSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function BusinessEntityRemoteSelectorCtor(ctrl, $scope, attrs) {
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

                    var promises = [];

                    var businessEntityDefinitionId;
                    var businessEntityDefinition;
                    var fieldTitle;
                    var selectedIds;

                    if (payload != undefined) {
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        fieldTitle = payload.fieldTitle;
                        selectedIds = payload.selectedIds;
                    }

                    var loadPromise = UtilsService.createPromiseDeferred();

                    getBusinessEntityDefintion().then(function () {
                        getBusinessEntitySelectorLoadPromise();
                    });


                    function getBusinessEntityDefintion() {
                        return VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(businessEntityDefinitionId).then(function (response) {
                            businessEntityDefinition = response;

                            if (attrs.ismultipleselection != undefined) {
                                ctrl.customLabel = businessEntityDefinition.Settings.PluralTitle;
                            } else {
                                ctrl.customLabel = businessEntityDefinition.Settings.SingularTitle;
                            }
                        });
                    }
                    function getBusinessEntitySelectorLoadPromise() {
                        return VR_GenericData_VRRestAPIBusinessEntityAPIService.GetBusinessEntitiesInfo(businessEntityDefinitionId).then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    ctrl.datasource.push(response[i]);
                                }

                                if (selectedIds != undefined) {
                                    VRUIUtilsService.setSelectedValues(selectedIds, 'BusinessEntityId', attrs, ctrl);
                                }
                            }

                            loadPromise.resolve();
                        });
                    }

                    return loadPromise.promise;
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('BusinessEntityId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";

            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                        '<vr-label>{{ctrl.customLabel}}</vr-label>' +
                        '<vr-select ' + multipleselection + ' datatextfield="Description" datavaluefield="BusinessEntityId" isrequired="ctrl.isrequired" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" '
                            + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                            + ' hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">' +
                        '</vr-select>' +
                   '</vr-columns>';
        }
    }]);