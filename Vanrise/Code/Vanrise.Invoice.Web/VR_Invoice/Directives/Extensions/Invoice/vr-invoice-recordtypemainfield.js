"use strict";
app.directive("vrInvoiceRecordtypemainfield", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var recordTypeMainField = new RecordTypeMainField($scope, ctrl, $attrs);
            recordTypeMainField.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/VR_Invoice/Directives/Extensions/Invoice/Templates/InvoiceRecordTypeMainFieldTemplate.html'
    };


    function RecordTypeMainField($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var dataRecordTypeDirectiveAPI;
        var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {

                dataRecordTypeDirectiveAPI = api;
                dataRecordTypeSelectorReadyPromiseDeferred.resolve();

                var setLoader = function (value) {
                    $scope.scopeModel.isDirectiveLoading = value;
                };

                var daPayload = { filter: { Filters: [] }, partnerFieldType: dataRecordTypeDirectiveAPI.getData() }; //Check dataRecordTypeId
                daPayload.filter.Filters.push({ $type: "Vanrise.Invoice.Business.InvoiceRecordTypeMainFields, Vanrise.Invoice.Business" });

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordTypeDirectiveAPI, daPayload, setLoader, dataRecordTypeSelectorReadyPromiseDeferred);
            };
            defineAPI();
        }

        function defineAPI() {
            
            var api = {};

            api.load = function (payload) {

                var promises = [];
                var loadDataRecordTypePromiseDeferred = UtilsService.createPromiseDeferred(); 

                dataRecordTypeSelectorReadyPromiseDeferred.promise.then(function () {
                    var dataRecordTypePayload = { filter: { Filters: [] } };
                    dataRecordTypePayload.filter.Filters.push({ $type: "Vanrise.Invoice.Business.InvoiceRecordTypeMainFields, Vanrise.Invoice.Business" });

                    if (payload != undefined) {

                        dataRecordTypePayload.partnerFieldType = payload.PartnerFieldType;


                        dataRecordTypePayload.partnerFieldTitle = payload.PartnerFieldTitle;
                        $scope.scopeModel.partnerFieldTitle = payload.PartnerFieldTitle;
                        console.log(payload);
                    }
                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeDirectiveAPI, dataRecordTypePayload.partnerFieldType, loadDataRecordTypePromiseDeferred);
                });
                promises.push(loadDataRecordTypePromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                console.log(dataRecordTypeDirectiveAPI.getData());
                return {
                    $type: "Vanrise.Invoice.Business.InvoiceRecordTypeMainFields, Vanrise.Invoice.Business",
                    PartnerFieldTitle: $scope.scopeModel.partnerFieldTitle,
                    PartnerFieldType: dataRecordTypeDirectiveAPI.getData(),
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}
]);