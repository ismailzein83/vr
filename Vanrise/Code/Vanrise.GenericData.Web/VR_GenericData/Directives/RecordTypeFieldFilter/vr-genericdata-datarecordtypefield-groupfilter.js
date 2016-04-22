'use strict';
app.directive('vrGenericdataDatarecordtypefieldGroupfilter', ['VR_GenericData_DataRecordTypeAPIService', 'UtilsService', 'VRUIUtilsService',
    function (VR_GenericData_DataRecordTypeAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                datarecordtypeid: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new recordTypeFieldGroupFilterCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordTypeFieldFilter/Templates/DataRecordTypeFieldGroupFilter.html"

        };

        function recordTypeFieldGroupFilterCtor(ctrl, $scope, $attrs) {
            var dataRecordTypeId;
            var context;
            ctrl.addRule = function () {
                var rule = {
                    onRuleFilterReady: function (api) {
                        var payload = {
                            dataRecordTypeId: dataRecordTypeId,
                            context: context
                        };
                        api.load(payload);
                    }
                };
                ctrl.rules.push(rule);
            }
            ctrl.addGroup = function () {
                var group = {
                    onGroupFilterReady: function (api) {
                        var payload = {
                            dataRecordTypeId: dataRecordTypeId,
                            context: context
                        };
                        api.load(payload);
                    }
                };


                ctrl.groups.push(group);
            }
            ctrl.groups = [];
            ctrl.rules = [];

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        dataRecordTypeId = payload.dataRecordTypeId;
                        context = payload.context;
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

