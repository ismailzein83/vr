'use strict';

app.directive('vrInvoiceConvertorEditor', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var invoiceConvertor = new InvoiceConvertor($scope, ctrl, $attrs);
                invoiceConvertor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Invoice/Directives/Extensions/Invoice/Templates/InvoiceConvertorTemplate.html'
        };

        function InvoiceConvertor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var gridDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.staticObjects = [];
                $scope.scopeModel.partnerInfoObjects = [];
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.scopeModel.addPartnerinfo = function () {
                    var dataItem = {
                        id: $scope.scopeModel.partnerInfoObjects + 1,
                        ObjectName: '',
                        InfoType: ''
                    };

                    $scope.scopeModel.partnerInfoObjects.push(dataItem);
                }
                $scope.scopeModel.removePartnerInfo = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.partnerInfoObjects, dataItem.id, 'id');
                    $scope.scopeModel.partnerInfoObjects.splice(index, 1);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    fillStaticObjects();
                    if (payload != undefined) {
                        if (payload.PartnerInfoObjects != undefined) {
                            for (var i = 0; i < payload.PartnerInfoObjects.length; i++) {
                                var item = payload.PartnerInfoObjects[i];
                                item.id = i;
                                $scope.scopeModel.partnerInfoObjects.push(item);
                            }
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.Convertors.InvoiceToVRObjectConvertor, Vanrise.Invoice.MainExtensions",
                        PartnerInfoObjects: $scope.scopeModel.partnerInfoObjects
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function fillStaticObjects() {
                $scope.scopeModel.staticObjects.push({
                    ObjectName: 'Invoice',
                    InfoType: 'Invoice'
                });
            }
        }
    }]);
