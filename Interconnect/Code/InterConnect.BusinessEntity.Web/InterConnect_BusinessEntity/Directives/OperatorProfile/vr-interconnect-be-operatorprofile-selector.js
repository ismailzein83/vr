'use strict';
app.directive('vrInterconnectBeOperatorprofileSelector', ['InterConnect_BE_OperatorProfileAPIService', 'InterConnect_BE_OperatorProfileService', 'UtilsService', 'VRUIUtilsService',
    function (InterConnect_BE_OperatorProfileAPIService, InterConnect_BE_OperatorProfileService, UtilsService, VRUIUtilsService) {

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

                $scope.addNewOperatorProfile = function () {
                    var onOperatorProfileAdded = function (operatorProfileObj) {
                        ctrl.datasource.push(operatorProfileObj.Entity);
                        if ($attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(operatorProfileObj.Entity);
                        else
                            ctrl.selectedvalues = operatorProfileObj.Entity;
                    };
                    InterConnect_BE_OperatorProfileService.addOperatorProfile(onOperatorProfileAdded);
                }


                var ctor = new operatorProfileCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

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
                return getOperatorProfileTemplate(attrs);
            }

        };


        function getOperatorProfileTemplate(attrs) {

            var multipleselection = "";
            var label = "Operator Profile";
            if (attrs.ismultipleselection != undefined) {
                label = "Operator Profiles";
                multipleselection = "ismultipleselection";
            }


            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewOperatorProfile"';

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="OperatorProfileId" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="OperatorProfile" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>'
        }

        function operatorProfileCtor(ctrl, $scope, attrs) {

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

                    return getOperatorProfilesInfo(attrs, ctrl, selectedIds, serializedFilter);
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('OperatorProfileId', attrs, ctrl);
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        function getOperatorProfilesInfo(attrs, ctrl, selectedIds, serializedFilter) {
            return InterConnect_BE_OperatorProfileAPIService.GetOperatorProfilsInfo(serializedFilter).then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'OperatorProfileId', attrs, ctrl);
                }
            });
        }
        return directiveDefinitionObject;
    }]);