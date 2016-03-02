
app.service('CP_SupplierPricelist_SupplierPriceListService', ['LabelColorsEnum', 'CP_SupplierPricelist_PriceListResultEnum', 'CP_SupplierPricelist_PriceListStatusEnum',
    function (labelColorsEnum, cpSupplierPricelistResultEnum, cpSupplierPricelistStatusEnum) {
        function getSupplierPriceListStatusColor(value) {
            switch (value) {
                case cpSupplierPricelistStatusEnum.New.value:
                    return labelColorsEnum.New.color;
                case cpSupplierPricelistStatusEnum.Uploaded.value:
                    return labelColorsEnum.Success.color;
                case cpSupplierPricelistStatusEnum.SuspendedDueToConfigurationErrors.value:
                    return labelColorsEnum.Failed.color;
                default:
                    return undefined;
            }
        }

        function getSupplierPriceListResultColor(value) {
            switch (value) {
                case cpSupplierPricelistResultEnum.NotCompleted.value:
                    return undefined;
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
