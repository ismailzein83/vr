'use strict';
app.directive('reprocessReprocessdefinitionSelector', ['Reprocess_ReprocessDefinitionAPIService', 'UtilsService', 'VRUIUtilsService',
    function (Reprocess_ReprocessDefinitionAPIService, UtilsService, VRUIUtilsService) {

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
                isdisabled: "=",
                showaddbutton: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new reprocessDefinitionCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return getReprocessDefinitionTemplate(attrs);
            }

        };


        function getReprocessDefinitionTemplate(attrs) {

            var multipleselection = "";
            var label = "Reprocess Definition";
            if (attrs.ismultipleselection != undefined) {
                label = "Reprocess Definitions";
                multipleselection = "ismultipleselection";
            }


            var addCliked = '';

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="ReprocessDefinitionId" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="ReprocessDefinition" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }

        function reprocessDefinitionCtor(ctrl, $scope, attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var serializedFilter = {};
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        if (payload.filter != undefined) {
                            serializedFilter = UtilsService.serializetoJson(payload.filter);
                        }
                    }
                    return getReprocessDefinitionsInfo(attrs, ctrl, selectedIds, serializedFilter);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ReprocessDefinitionId', attrs, ctrl);
                };

                api.setSelectedValues = function (selectedIds) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'ReprocessDefinitionId', attrs, ctrl);
                };
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        function getReprocessDefinitionsInfo(attrs, ctrl, selectedIds, serializedFilter) {
            return Reprocess_ReprocessDefinitionAPIService.GetReprocessDefinitionsInfo(serializedFilter).then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'ReprocessDefinitionId', attrs, ctrl);
                }
            });
        }
        return directiveDefinitionObject;
    }]);