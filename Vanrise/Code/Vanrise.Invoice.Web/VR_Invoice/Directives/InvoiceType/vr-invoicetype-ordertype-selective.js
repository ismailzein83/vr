'use strict';

app.directive('vrInvoicetypeOrdertypeSelective', ['VR_Invoice_OrderTypeEnum', 'UtilsService', 'VRUIUtilsService', function (VR_Invoice_OrderTypeEnum, UtilsService, VRUIUtilsService) {

    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var orderTypeSelector = new OrderTypeSelector(ctrl, $scope, $attrs);
            orderTypeSelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getUserTemplate(attrs);
        }
    };

    function OrderTypeSelector(ctrl, $scope, attrs) {

        this.initializeController = initializeController;

        var selectorAPI;

        var directiveAPI;
        var directiveReadyDeferred;
        var context;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
            $scope.scopeModel.datasource = [];
            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var payload;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, directiveReadyDeferred);
            };
            $scope.scopeModel.onSelectionChanged = function (value) {
                if (value != undefined && value.directiveEditor == undefined)
                    directiveAPI = undefined;
            };

        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                selectorAPI.clearDataSource();

                var orderTypeEntity;
                var promises = [];
                $scope.scopeModel.datasource = UtilsService.getArrayEnum(VR_Invoice_OrderTypeEnum);

                if (payload != undefined) {
                    orderTypeEntity = payload.orderTypeEntity;
                    context = payload.context;
                }

                if (orderTypeEntity != undefined) {
                    $scope.scopeModel.selectedValue = UtilsService.getItemByVal($scope.scopeModel.datasource, orderTypeEntity.OrderType, 'value');
                    if ($scope.scopeModel.selectedValue != undefined && $scope.scopeModel.selectedValue.directiveEditor != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }
                }

                function loadDirective() {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        var directivePayload;
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    });

                    return directiveLoadDeferred.promise;
                }
                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
                return $scope.scopeModel.selectedValue != undefined ? $scope.scopeModel.selectedValue.value : undefined;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getUserTemplate(attrs) {

        var multipleselection = "";
        var label = "Order Type";

        if (attrs.ismultipleselection != undefined) {
            label = "Order Types";
            multipleselection = "ismultipleselection";
        }
        return '<vr-row>'
                    + '<vr-columns colnum="{{ctrl.normalColNum}}">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.datasource"'
                            + ' selectedvalues="scopeModel.selectedValue"'
                            + ' datavaluefield="value"'
                            + ' datatextfield="description"'
                            + 'isrequired="ctrl.isrequired"'
            + 'onselectionchanged="scopeModel.onSelectionChanged"'
                            + 'label="Order Type" '
                            + ' >'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'
         + '<vr-directivewrapper ng-if="scopeModel.selectedValue != undefined" directive="scopeModel.selectedValue.directiveEditor"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';
    }

}]);