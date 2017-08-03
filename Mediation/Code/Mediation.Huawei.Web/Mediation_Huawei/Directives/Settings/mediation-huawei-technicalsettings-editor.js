'use strict';

app.directive('mediationHuaweiTechnicalsettingsEditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService',
function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new HuaweiTechnicalsettingsEditor($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Mediation_Huawei/Directives/Settings/Templates/HuaweiSettingsEditorTemplate.html'
    };

    function HuaweiTechnicalsettingsEditor($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var cdrRecordFilterDirectiveAPI;
        var cdrRecordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var smsRecordFilterDirectiveAPI;
        var smsRecordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var cdrRecordTypeId = 'F4021D2B-9C88-42CA-B99E-0AE187BCC289';
        var smsRecordTypeId = '037BEC85-BE42-4A28-81F7-1687D3E6CEED';

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onCDRRecordFilterDirectiveReady = function (api) {
                cdrRecordFilterDirectiveAPI = api;
                cdrRecordFilterDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onSMSRecordFilterDirectiveReady = function (api) {
                smsRecordFilterDirectiveAPI = api;
                smsRecordFilterDirectiveReadyDeferred.resolve();
            };
            defineAPI();
        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                var promises = [];

                var cdrFilterGroup;
                var smsFilterGroup;

                if (payload != undefined && payload.data != undefined) {
                    var technicalSettingData = payload.data;

                    if (technicalSettingData != undefined) {

                        cdrFilterGroup = technicalSettingData.CDR_FilterGroup;
                        smsFilterGroup = technicalSettingData.SMS_FilterGroup;
                    }
                }

                var recordFilterDirectiveLoadPromise = getCDRRecordFilterDirectiveLoadPromise();
                promises.push(recordFilterDirectiveLoadPromise);

                var smsRecordFilterDirectiveLoadPromise = getSMSRecordFilterDirectiveLoadPromise();
                promises.push(smsRecordFilterDirectiveLoadPromise);

                function getCDRRecordFilterDirectiveLoadPromise() {
                    var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises([cdrRecordFilterDirectiveReadyDeferred.promise]).then(function () {

                        VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(cdrRecordTypeId).then(function (response) {
                            var dataRecordFieldsInfo = response;

                            var recordFilterDirectivePayload = {
                                context: buildContext(dataRecordFieldsInfo)
                            };
                            if (cdrFilterGroup != undefined) {
                                recordFilterDirectivePayload.FilterGroup = cdrFilterGroup
                            }
                            VRUIUtilsService.callDirectiveLoad(cdrRecordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                        });
                    });

                    return recordFilterDirectiveLoadDeferred.promise;
                }

                function getSMSRecordFilterDirectiveLoadPromise() {
                    var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises([smsRecordFilterDirectiveReadyDeferred.promise]).then(function () {

                        VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(smsRecordTypeId).then(function (response) {
                            var dataRecordFieldsInfo = response;

                            var recordFilterDirectivePayload = {
                                context: buildContext(dataRecordFieldsInfo)
                            };
                            if (smsFilterGroup != undefined) {
                                recordFilterDirectivePayload.FilterGroup = smsFilterGroup
                            }
                            VRUIUtilsService.callDirectiveLoad(smsRecordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                        });
                    });

                    return recordFilterDirectiveLoadDeferred.promise;
                }


                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var data = {
                    $type: 'Mediation.Huawei.Entities.HuaweiTechnicalSettingData, Mediation.Huawei.Entities',
                    CDR_FilterGroup: cdrRecordFilterDirectiveAPI.getData().filterObj,
                    SMS_FilterGroup: smsRecordFilterDirectiveAPI.getData().filterObj
                };

                return data;
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }

        function buildContext(dataRecordFieldsInfo) {
            var context = {
                getFields: function () {
                    var fields = [];
                    if (dataRecordFieldsInfo != undefined) {
                        for (var i = 0; i < dataRecordFieldsInfo.length; i++) {
                            var dataRecordField = dataRecordFieldsInfo[i].Entity;

                            fields.push({
                                FieldName: dataRecordField.Name,
                                FieldTitle: dataRecordField.Title,
                                Type: dataRecordField.Type
                            });
                        }
                    }
                    return fields;
                }
            };
            return context;
        }
    }
}]);