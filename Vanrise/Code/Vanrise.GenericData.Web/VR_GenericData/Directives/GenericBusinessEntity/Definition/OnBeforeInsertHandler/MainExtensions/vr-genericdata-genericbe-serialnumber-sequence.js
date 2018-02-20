"use strict";

app.directive("vrGenericdataGenericbeSerialnumberSequence", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_GenericData_DateCounterTypeEnum",
function (UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_DateCounterTypeEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope:
        {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new SequenceCtor($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/OnBeforeInsertHandler/MainExtensions/Templates/SerialNumberSequenceTemplate.html"
    };

    function SequenceCtor($scope, ctrl, $attrs) {

        var context;

        var dataRecordTypeFieldsSelectorAPI;
        var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        this.initializeController = initializeController;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.dateCounterTypes = UtilsService.getArrayEnum(VR_GenericData_DateCounterTypeEnum);

            $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                dataRecordTypeFieldsSelectorAPI = api;
                dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers.SequenceSerialNumberPart, Vanrise.GenericData.MainExtensions",
                    InfoType: $scope.scopeModel.infoType,
                    IncludePartnerId: $scope.scopeModel.includePartnerId,
                    DateCounterType: $scope.scopeModel.selectedDateCounterType != undefined ? $scope.scopeModel.selectedDateCounterType.value : null,
                    PaddingLeft: $scope.scopeModel.paddingLeft,
                    SequenceKeyFieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds()
                };
            };

            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    context = payload.context;
                    if (payload.settings != undefined) {
                        $scope.scopeModel.selectedDateCounterType = UtilsService.getItemByVal($scope.scopeModel.dateCounterTypes, payload.settings.DateCounterType, 'value');
                        $scope.scopeModel.infoType = payload.settings.InfoType;
                        $scope.scopeModel.paddingLeft = payload.settings.PaddingLeft;
                        $scope.scopeModel.includePartnerId = payload.settings.IncludePartnerId;
                    }
                }
                var getDataRecordTypeFieldPromise = loadDataRecordTypeFieldsSelector();
                promises.push(getDataRecordTypeFieldPromise);

                function loadDataRecordTypeFieldsSelector() {
                    var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                        var typeFieldsPayload = {
                            dataRecordTypeId: getContext().getDataRecordTypeId(),
                            selectedIds: payload != undefined && payload.settings != undefined ? payload.settings.SequenceKeyFieldName : undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, typeFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                    });
                    return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }

    }

    return directiveDefinitionObject;

}
]);