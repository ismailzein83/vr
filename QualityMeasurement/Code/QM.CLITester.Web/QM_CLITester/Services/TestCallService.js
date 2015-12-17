
app.service('Qm_CliTester_TestCallService', ['LabelColorsEnum', 'Qm_CliTester_CallTestResultEnum', 'Qm_CliTester_CallTestStatusEnum',
    function (LabelColorsEnum, Qm_CliTester_CallTestResultEnum, Qm_CliTester_CallTestStatusEnum) {
        var drillDownDefinitions = [];
        return ({
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            getCallTestStatusColor: getCallTestStatusColor,
            getCallTestResultColor: getCallTestResultColor
        });

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
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
                default:
                    return undefined;
            }
        }

    }]);
