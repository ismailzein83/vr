(function (appControllers) {

    'use strict';

    TestCallService.$inject = ['LabelColorsEnum', 'Qm_CliTester_CallTestResultEnum', 'Qm_CliTester_CallTestStatusEnum', 'VRModalService'];

    function TestCallService(LabelColorsEnum, Qm_CliTester_CallTestResultEnum, Qm_CliTester_CallTestStatusEnum, VRModalService) {
        var drillDownDefinitions = [];
        return {
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            getCallTestStatusColor: getCallTestStatusColor,
            getCallTestResultColor: getCallTestResultColor,
            sendTestCall: sendTestCall
        };
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        function sendTestCall(supplierName,userName, countryName, zoneName,callTestStatusDescription, callTestResultDescription, scheduleName, 
            pdd, mos, creationDate, source, destination, receivedCli, releaseCode, ringDuration, duration, onSendTestCall) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCountryUpdated = onSendTestCall;
            };
            var parameters = {
                SupplierName: supplierName,
                UserName: userName,
                CountryName: countryName,
                ZoneName: zoneName,
                CallTestStatusDescription: callTestStatusDescription,
                CallTestResultDescription: callTestResultDescription,
                ScheduleName: scheduleName,
                PDD: pdd,
                MOS: mos,
                CreationDate: creationDate,
                Source: source,
                Destination: destination,
                ReceivedCli: receivedCli,
                ReleaseCode: releaseCode,
                RingDuration: ringDuration,
                Duration: duration
            };

            VRModalService.showModal('/Client/Modules/QM_CLITester/Views/HistoryTestCall/HistoryTestCallEditor.html', parameters, settings);
        }

        function getCallTestStatusColor(value) {
            switch (value) {
                case Qm_CliTester_CallTestStatusEnum.New.value:
                    return LabelColorsEnum.New.color;
                    break;
                case Qm_CliTester_CallTestStatusEnum.Initiated.value:
                    return LabelColorsEnum.Primary.color;
                    break;
                case Qm_CliTester_CallTestStatusEnum.InitiationFailedWithRetry.value:
                    return LabelColorsEnum.Warning.color;
                    break;
                case Qm_CliTester_CallTestStatusEnum.InitiationFailedWithNoRetry.value:
                    return LabelColorsEnum.WarningLevel2.color;
                    break;
                case Qm_CliTester_CallTestStatusEnum.PartiallyCompleted.value:
                    return LabelColorsEnum.Processing.color;
                    break;
                case Qm_CliTester_CallTestStatusEnum.Completed.value:
                    return LabelColorsEnum.Success.color;
                    break;
                case Qm_CliTester_CallTestStatusEnum.GetProgressFailedWithRetry.value:
                    return LabelColorsEnum.WarningLevel1.color;
                    break;
                case Qm_CliTester_CallTestStatusEnum.GetProgressFailedWithNoRetry.value:
                    return LabelColorsEnum.Failed.color;
                    break;
                default:
                    return undefined;
            }
        }

        function getCallTestResultColor(value) {
            switch (value) {
                case Qm_CliTester_CallTestResultEnum.NotCompleted.value:
                    return LabelColorsEnum.Error.color;
                    break;
                case Qm_CliTester_CallTestResultEnum.Succeeded.value:
                    return LabelColorsEnum.Success.color;
                    break;
                case Qm_CliTester_CallTestResultEnum.PartiallySucceeded.value:
                    return LabelColorsEnum.WarningLevel1.color;
                    break;
                case Qm_CliTester_CallTestResultEnum.Failed.value:
                    return LabelColorsEnum.Failed.color;
                    break;
                case Qm_CliTester_CallTestResultEnum.NotAnswered.value:
                    return LabelColorsEnum.Warning.color;
                    break;
                case Qm_CliTester_CallTestResultEnum.Fas.value:
                    return LabelColorsEnum.Failed.color;
                    break;
                default:
                    return undefined;
            }
        }
    }
    appControllers.service('Qm_CliTester_TestCallService', TestCallService);

})(appControllers);