
app.service('CP_SupplierPricelist_SupplierPriceListService', ['LabelColorsEnum', 'CP_SupplierPricelist_PriceListResultEnum', 'CP_SupplierPricelist_PriceListStatusEnum',
    function (labelColorsEnum, cpSupplierPricelistResultEnum, cpSupplierPricelistStatusEnum) {
        function getSupplierPriceListStatusColor(value) {
            switch (value) {
                case cpSupplierPricelistStatusEnum.New.value:
                    return labelColorsEnum.New.color;
                case cpSupplierPricelistStatusEnum.Recieved.value:
                    return labelColorsEnum.Primary.color;
                case cpSupplierPricelistStatusEnum.SuspendedDueToBusinessErrors.value:
                    return labelColorsEnum.Warning.color;
                case cpSupplierPricelistStatusEnum.AwaitingWarningsConfirmation.value:
                    return labelColorsEnum.WarningLevel2.color;
                case cpSupplierPricelistStatusEnum.Processing.value:
                    return labelColorsEnum.Processing.color;
                case cpSupplierPricelistStatusEnum.ProcessedSuccessfuly.value:
                    return labelColorsEnum.Success.color;
                case cpSupplierPricelistStatusEnum.SuspendedDueToConfigurationErrors.value:
                    return labelColorsEnum.WarningLevel1.color;
                case cpSupplierPricelistStatusEnum.FailedDuetoSheetError.value:
                    return labelColorsEnum.Failed.color;
                case cpSupplierPricelistStatusEnum.AwaitingSaveConfirmationbySystemparam.value:
                    return labelColorsEnum.WarningLevel1.color;
                case cpSupplierPricelistStatusEnum.Processedwithnochanges.value:
                    return labelColorsEnum.WarningLevel1.color;
                default:
                    return undefined;
            }
        }

        function getSupplierPriceListResultColor(value) {
            switch (value) {
                case cpSupplierPricelistResultEnum.NotCompleted.value:
                    return labelColorsEnum.Error.color;
                case cpSupplierPricelistResultEnum.Succeeded.value:
                    return labelColorsEnum.Success.color;
                case cpSupplierPricelistResultEnum.PartiallySucceeded.value:
                    return labelColorsEnum.WarningLevel1.color;
                case cpSupplierPricelistResultEnum.Failed.value:
                    return labelColorsEnum.Failed.color;
                case cpSupplierPricelistResultEnum.NotAnswered.value:
                    return labelColorsEnum.Warning.color;
                default:
                    return undefined;
            }
        }

        return ({
            getSupplierPriceListStatusColor: getSupplierPriceListStatusColor,
            getSupplierPriceListResultColor: getSupplierPriceListResultColor
        });
    }]);
