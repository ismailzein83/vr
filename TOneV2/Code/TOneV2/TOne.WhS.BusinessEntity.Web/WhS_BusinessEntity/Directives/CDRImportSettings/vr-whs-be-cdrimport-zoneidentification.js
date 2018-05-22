'use strict';

app.directive('vrWhsBeCdrimportZoneidentification', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new cdrImportZoneIdentificationEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CDRImportSettings/Templates/CDRImportZoneIdentificationTemplate.html"
        };

        function cdrImportZoneIdentificationEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var sellingNumberPlanSelectorAPI;
            var sellingNumberPlanSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSellingNumberPlanSelectorReady = function (api) {
                    sellingNumberPlanSelectorAPI = api;
                    sellingNumberPlanSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var secondarySellingNumberPlanId;

                    if (payload != undefined && payload.cdrImportZoneIdentification != undefined) {
                        secondarySellingNumberPlanId = payload.cdrImportZoneIdentification.SecondarySellingNumberPlanId;
                    }

                    var promises = [];

                    //Loading SellingNumberPlan Selector
                    var sellingNumberPlanSelectorLoadPromise = getSellingNumberPlanSelectorLoadPromise();
                    promises.push(sellingNumberPlanSelectorLoadPromise);


                    function getSellingNumberPlanSelectorLoadPromise() {
                        var sellingNumberPlanSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        sellingNumberPlanSelectorReadyDeferred.promise.then(function () {
                            var sellingNumberPlanSelectorPayload;
                            if (secondarySellingNumberPlanId != undefined) {
                                sellingNumberPlanSelectorPayload = { selectedIds: secondarySellingNumberPlanId };
                            }
                            VRUIUtilsService.callDirectiveLoad(sellingNumberPlanSelectorAPI, sellingNumberPlanSelectorPayload, sellingNumberPlanSelectorLoadDeferred);

                        });

                        return sellingNumberPlanSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        SecondarySellingNumberPlanId: sellingNumberPlanSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);