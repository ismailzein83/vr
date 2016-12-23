'use strict';

app.directive('vrCommonGridcolumnwidthEditor', ['UtilsService', 'VRUIUtilsService', 'VRCommon_GridWidthFactorEnum',
    function (UtilsService, VRUIUtilsService, VRCommon_GridWidthFactorEnum) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GridColumnWidthEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/GridColumnWidth/Templates/GridColumnWidthEditorTemplate.html"
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
                    $scope.scopeModel.fixedWidth = data &&  data.FixedWidth ? data.FixedWidth: undefined;
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
 }]);