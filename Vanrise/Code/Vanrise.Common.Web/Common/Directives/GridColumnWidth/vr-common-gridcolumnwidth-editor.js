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
                $scope.hintColNum = 1;
                $scope.showHint = false;
                if (ctrl.selectorColNum != undefined)
                    $scope.selectorColNum = ctrl.selectorColNum;

                if (ctrl.textboxColNum != undefined)
                    $scope.textboxColNum = ctrl.textboxColNum;

                if ($attrs.showhint != undefined) {

                    $scope.selectorColNum = 6;
                    $scope.textboxColNum = 2;
                    if ($attrs.showhint != undefined)
                        $scope.showHint = true;
                }

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
                    $scope.scopeModel.listViewWidth = data && data.ListViewWidth ? data.ListViewWidth : undefined;
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
                        FixedWidth: $scope.scopeModel.fixedWidth,
                        ListViewWidth: $scope.scopeModel.listViewWidth
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
            var listViewLabel = ' label ="List View Width"';
            var withchild = "";
            if (attrs.hidelabel != undefined || attrs.showhint != undefined) {
                hidelabel = "hidelabel";
                fixedLabel = " ";
                listViewLabel = " ";
                withchild = "haschildcolumns";
            }

            return '<vr-common-gridwidthfactor-selector normal-col-num="{{selectorColNum}}" on-ready="scopeModel.onGridWidthFactorSelectorReady" onselectionchanged="scopeModel.onGridWidthFactorSelectionChange" selectedvalues="scopeModel.widthOption" isrequired="true" '
                   + hidelabel
                   + ' ></vr-common-gridwidthfactor-selector>'
                   + '<vr-columns colnum="{{textboxColNum}}" ' + withchild + ' ng-show="scopeModel.widthOption.value == scopeModel.fixedWidthValue" >'
                   + '<vr-textbox type="number" '
                    + fixedLabel +
                   '  value="scopeModel.fixedWidth" decimalprecision="0" isrequired="scopeModel.widthOption.value == scopeModel.fixedWidthValue "></vr-textbox>'
                   + '</vr-columns>'
                    + '<vr-columns colnum="{{hintColNum}}" ' + withchild + ' ng-show="scopeModel.widthOption.value == scopeModel.fixedWidthValue && showHint " >'
                        + '<vr-hint value="Fixed Width"></vr-hint>'
                   + '</vr-columns>'
                     + '<vr-columns colnum="{{textboxColNum}}" ' + withchild + '>'
                   + '<vr-textbox type="number" '
                    + listViewLabel +
                   'value="scopeModel.listViewWidth" decimalprecision="0"></vr-textbox>'
                   + '</vr-columns>'
                   + '<vr-columns colnum="{{hintColNum}}" ' + withchild + ' ng-show="showHint">'
                        + '<vr-hint value="List View Width"></vr-hint>'
                   + '</vr-columns>' ;
        }
    }]);