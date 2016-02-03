'use strict';
app.directive('vrGenericdataDynamiccontrolsBusinessentity', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectionmode: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.selectionmode == "multiple")
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];
                var ctor = new selectorCtor(ctrl, $scope, $attrs);
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
                return getTemplate(attrs);
            }

        };


        function getTemplate(attrs) {
            var multipleselection = "";
            //var label = "";
            if (attrs.selectionmode == "multiple") {
                //label = "";
                multipleselection = "ismultipleselection";
            }
            //var required = "";
            //if (attrs.isrequired != undefined)
            //    required = "isrequired";

            //var hideremoveicon = "";
            //if (attrs.hideremoveicon != undefined)
            //    hideremoveicon = "hideremoveicon";

            return '<vr-directivewrapper directive="selector.directive" on-ready="selector.onDirectiveReady"'
                + multipleselection + '></vr-directivewrapper>'
                + '<vr-directivewrapper directive="dynamic.directive" on-ready="dynamic.onDirectiveReady"></vr-directivewrapper>'
        }

        function selectorCtor(ctrl, $scope, $attrs) {

            var selectorApi;

            function initializeController() {
                //ctrl.showSelector = ($attrs.selectionmode == "dynamic");

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var fieldType;

                    if (payload != undefined) {
                        fieldType = payload.fieldType;
                    }

                    if (fieldType != undefined) {
                        //fieldType.BusinessEntityDefinitionId;
                        var businessEntityDef = { Settings: { SelectorUIControl: 'vr-whs-be-salezone-selector', GroupSelectorUIControl: 'vr-whs-be-salezonegroup' } };
                        if (businessEntityDef.Settings != null)
                        {
                            if ($attrs.selectionmode == "dynamic") {
                                $scope.dynamic = {};
                                $scope.dynamic.directive = businessEntityDef.Settings.GroupSelectorUIControl;
                                $scope.dynamic.onDirectiveReady = function (api) {
                                    $scope.dynamic.directiveAPI = api;
                                    $scope.dynamic.directiveAPI.load(undefined);
                                }
                            }
                            else
                            {
                                $scope.selector = {};
                                $scope.selector.directive = businessEntityDef.Settings.SelectorUIControl;
                                $scope.selector.onDirectiveReady = function (api) {
                                    $scope.selector.directiveAPI = api;
                                    $scope.selector.directiveAPI.load(undefined);
                                }
                            }
                        }
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);