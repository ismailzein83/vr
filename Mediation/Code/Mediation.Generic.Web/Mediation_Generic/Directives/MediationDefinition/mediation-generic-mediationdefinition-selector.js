'use strict';
app.directive('mediationGenericMediationdefinitionSelector', ['Mediation_Generic_MediationDefinitionAPIService', 'VRUIUtilsService',
    function (Mediation_Generic_MediationDefinitionAPIService, VRUIUtilsService) {

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
                var mediationDefinitionSelectorCtor = new MediationDefinitionSelectorCtor(ctrl, $scope, $attrs);
                mediationDefinitionSelectorCtor.initializeController();
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getMediationDefinitionTemplate(attrs);
            }

        };


        function getMediationDefinitionTemplate(attrs) {
            var label = "Mediation Definition";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Mediation Definitions";
                multipleselection = "ismultipleselection";
            }
            var hideremoveicon = (attrs.hideremoveicon != undefined && attrs.hideremoveicon != null) ? 'hideremoveicon' : null;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                   + '<vr-select on-ready="ctrl.onSelectorReady"'
                   + '  selectedvalues="ctrl.selectedvalues"'
                   + '  onselectionchanged="ctrl.onselectionchanged"'
                   + '  datasource="ctrl.datasource"'
                   + '  datavaluefield="MediationDefinitionId"'
                   + '  datatextfield="Name"'
                   + '  ' + multipleselection
                   + '  isrequired="ctrl.isrequired"'
                   + '  label="' + label + '"'
                   + ' entityName="' + label + '"'
                   + hideremoveicon
                   + '  >'
                   + '</vr-select>'
                   + '</vr-columns>'
        }

        function MediationDefinitionSelectorCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var filter;
            var selectorAPI;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(defineAPI());
                    }
                }


                ctrl.search = function (nameFilter) {
                    return Mediation_Generic_MediationDefinitionAPIService.GetMediationDefinitionsInfo(nameFilter);
                }

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

                    return Mediation_Generic_MediationDefinitionAPIService.GetMediationDefinitionsInfo(undefined).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'MediationDefinitionId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('MediationDefinitionId', attrs, ctrl);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;

            }

            function GetMediationDefinitionsInfo(attrs, ctrl, selectedIds) {
                ctrl.datasource = [];
                return Mediation_Generic_MediationDefinitionAPIService.GetMediationDefinitionsInfoByIds(selectedIds).then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.datasource.push(item);
                    });
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'MediationDefinitionId', attrs, ctrl);
                });
            }
        }

        return directiveDefinitionObject;

    }]);