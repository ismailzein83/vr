'use strict';

app.directive('vrCommonGridcolumnwidthEditor', ['UtilsService', 'VRUIUtilsService', 'VRCommon_GridWidthFactorEnum',
    function (UtilsService, VRUIUtilsService, VRCommon_GridWidthFactorEnum) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectorColNum: '@',
                textboxColNum: '@'

            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                $scope.selectorColNum = 8;
                $scope.textboxColNum = 4;

                if (ctrl.selectorColNum != undefined)
                    $scope.selectorColNum = ctrl.selectorColNum;

                if (ctrl.textboxColNum != undefined)
                    $scope.textboxColNum = ctrl.textboxColNum;

                var ctor = new GridColumnWidthEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function GridColumnWidthEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var widthFactorSelectorAPI;
            var widthFactorSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fixedWidthValue = VRCommon_GridWidthFactorEnum.FixedWidth.value;
                $scope.scopeModel.onGridWidthFactorSelectorReady = function (api) {
                    widthFactorSelectorAPI = api;
                    widthFactorSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onGridWidthFactorSelectionChange = function () {
                    if (widthFactorSelectorAPI.getSelectedIds() != $scope.scopeModel.fixedWidthValue)
                        $scope.scopeModel.fixedWidth = undefined;
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var data;
                    if (payload != undefined) {
                        data = payload.data;
                    }
                    $scope.scopeModel.fixedWidth = data && data.FixedWidth ? data.FixedWidth : undefined;
                    var widthFactorSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();


                    widthFactorSelectorReadyPromiseDeferred.promise.then(function () {
                        var selectorPayload = {
                            selectedIds: data ? data.Width : VRCommon_GridWidthFactorEnum.Normal.value
                        };
                        VRUIUtilsService.callDirectiveLoad(widthFactorSelectorAPI, selectorPayload, widthFactorSelectorLoadPromiseDeferred);
                    });

                    return widthFactorSelectorLoadPromiseDeferred.promise;



                };

                api.getData = function () {

                    return {
                        Width: widthFactorSelectorAPI.getSelectedIds(),
                        FixedWidth: $scope.scopeModel.fixedWidth
                    }

                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);


            }


        }
        function getDirectiveTemplate(attrs) {
            var label = attrs.label ? attrs.label : 'Width';

            var hidelabel = ' ';
            var fixedLabel = ' label ="Fixed Width"';
            if (attrs.hidelabel != undefined) {
                hidelabel = "hidelabel";
                fixedLabel = " ";
            }

            return '<vr-common-gridwidthfactor-selector normal-col-num="{{selectorColNum}}" on-ready="scopeModel.onGridWidthFactorSelectorReady" onselectionchanged="scopeModel.onGridWidthFactorSelectionChange" selectedvalues="scopeModel.widthOption" isrequired="true" '
                   + hidelabel
                   + ' ></vr-common-gridwidthfactor-selector>'
                   + '<vr-columns colnum="{{textboxColNum}}" >'
                   + '<vr-textbox type="number" '
                    + fixedLabel +
                   ' ng-show="scopeModel.widthOption.value == scopeModel.fixedWidthValue" value="scopeModel.fixedWidth" decimalprecision="0" isrequired="scopeModel.widthOption.value == scopeModel.fixedWidthValue "></vr-textbox>'
                   + '</vr-columns>';
        }
    }]);