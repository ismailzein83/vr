'use strict';
app.directive('vrGenericdataFieldtypeCustomobjectRulefiltereditor', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            selectionmode: '@',
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            $scope.scopeModel = {};
            var ctor = new customobjectCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ruleFilterEditorCtrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        template: function (element, attrs) {
            return getDirectiveTemplate(attrs);
        }
    };

    function customobjectCtor(ctrl, $scope, $attrs) {

        function initializeController() {
            $scope.scopeModel = {};


            defineAPI();
        }


        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var fieldType;
                var innerSectionPromises = [];

               
                if (payload != undefined && payload.dataRecordTypeField != undefined) {
                    var dataRecordTypeField = payload.dataRecordTypeField;
                    var filterObj = payload.filterObj;

                    $scope.scopeModel.fieldTitle = dataRecordTypeField.FieldTitle;
                    fieldType = dataRecordTypeField.Type;
                }

              
                if (fieldType != undefined && fieldType.Settings != undefined) {

                    if ($scope.selector == undefined) {
                        $scope.selector = {
                            directive: fieldType.Settings.RuleFilterSelectorUIControl,
                            directiveReadyPromiseDeferred : UtilsService.createPromiseDeferred()
                        };
                    }
                    $scope.selector.onDirectiveReady = function (api) {
                        $scope.selector.directiveAPI = api;
                        $scope.selector.directiveReadyPromiseDeferred.resolve();
                    };

                    $scope.selector.directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    innerSectionPromises.push($scope.selector.directiveLoadPromiseDeferred.promise);
                    $scope.selector.onselectionchanged = function (selectedvalue) {

                    };

                    $scope.selector.directiveReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            fieldTitle: $scope.scopeModel.fieldTitle,
                            fieldValue: filterObj != undefined ? filterObj : null
                        };
                        VRUIUtilsService.callDirectiveLoad($scope.selector.directiveAPI, payload, $scope.selector.directiveLoadPromiseDeferred);
                    });
                }
                return UtilsService.waitMultiplePromises(innerSectionPromises);
            };

            api.getData = function () {
                return $scope.selector.directiveAPI.getData();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getDirectiveTemplate(attrs) {
       
            var multipleselection = "";

            if (attrs.selectionmode == "dynamic" || attrs.selectionmode == "multiple") {
                multipleselection = "ismultipleselection";
            }

            return '<vr-columns width="1/2row"><vr-directivewrapper directive="selector.directive"   on-ready="selector.onDirectiveReady" onselectionchanged="selector.onselectionchanged" '
                + multipleselection + ' isrequired="ruleFilterEditorCtrl.isrequired"></vr-directivewrapper></vr-columns>';
    }

    return directiveDefinitionObject;
}]);

