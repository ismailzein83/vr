(function (appControllers) {

    "use strict";

    analyticsServiceObj.$inject = ['LabelColorsEnum', 'WhS_Analytics_GenericAnalyticMeasureEnum', 'VRModalService', 'WhS_Analytics_GenericAnalyticDimensionEnum'];

    function analyticsServiceObj(LabelColorsEnum, WhS_Analytics_GenericAnalyticMeasureEnum, VRModalService, WhS_Analytics_GenericAnalyticDimensionEnum) {
   
        function getACDColor(acdValue, attemptsValue, parameters)
        {
            if (attemptsValue > parameters.attempts && acdValue < parameters.acd)
                return LabelColorsEnum.WarningLevel1.color;
            return undefined;
        }

        function getASRColor(asrValue, attemptsValue, parameters)
        {
            if (attemptsValue > parameters.attempts && asrValue < parameters.asr)
                return LabelColorsEnum.WarningLevel2.color;
            return undefined;
        }

        function getMeasureColor(dataItem, coldef, parameters)
        {
            if (coldef.tag.value === WhS_Analytics_GenericAnalyticMeasureEnum.ACD.value)
                return getACDColor(dataItem.MeasureValues[WhS_Analytics_GenericAnalyticMeasureEnum.ACD.name], dataItem.MeasureValues[WhS_Analytics_GenericAnalyticMeasureEnum.Attempts.name], parameters);
            else if (coldef.tag.value === WhS_Analytics_GenericAnalyticMeasureEnum.ASR.value)
                return getASRColor(dataItem.MeasureValues[WhS_Analytics_GenericAnalyticMeasureEnum.ASR.name], dataItem.MeasureValues[WhS_Analytics_GenericAnalyticMeasureEnum.Attempts.name], parameters);
            return undefined;
        }

        function showCdrLog(parameters) {
            VRModalService.showModal('/Client/Modules/WhS_Analytics/Views/CDR/CDRLog.html', parameters, {
                useModalTemplate: true,
                width: "80%",
                title: "CDR Log"
            });
        }

        function updateParametersFromDimentions(parameters, ctrl, dataItem) {
           
            var dimentions = loadDimention();

            for (var j = 0; j < ctrl.filters.length; j++) {
                for (var i = 0; i < dimentions.length; i++) {

                    var dimention = dimentions[i];
                    if (dimention.value == ctrl.filters[j].Dimension) {
                        var filtervalues = ctrl.filters[j].FilterValues;
                        for (var k = 0; k < filtervalues.length; k++)
                            addIdToParameters(dimention.value, filtervalues[k]);
                    }
                }
            }

            for (var j = 0; j < ctrl.dimensionFields.length; j++) {
                for (var i = 0; i < dimentions.length; i++) {
                    var dimention = dimentions[i];
                    if (dimention.value == ctrl.dimensionFields[j].value) {
                        addIdToParameters(dimention.value, dataItem.DimensionValues[j].Id);
                    }
                }
            }

            function addIdToParameters(dimentionValue,idToBeAdded) {
                switch (dimentionValue) {
                    case WhS_Analytics_GenericAnalyticDimensionEnum.Zone.value:
                        parameters.saleZoneIds.push(idToBeAdded);
                        break;
                    case WhS_Analytics_GenericAnalyticDimensionEnum.Customer.value:
                        parameters.customerIds.push(idToBeAdded);
                        break;
                    case WhS_Analytics_GenericAnalyticDimensionEnum.Supplier.value:
                        parameters.supplierIds.push(idToBeAdded);
                        break;
                    case WhS_Analytics_GenericAnalyticDimensionEnum.Switch.value:
                        parameters.switchIds.push(idToBeAdded);
                        break;
                    case WhS_Analytics_GenericAnalyticDimensionEnum.SupplierZone.value:
                        parameters.supplierZoneIds.push(idToBeAdded);
                        break;
                    case WhS_Analytics_GenericAnalyticDimensionEnum.Date.value:
                        parameters.fromDate=idToBeAdded;
                        break;
                    case WhS_Analytics_GenericAnalyticDimensionEnum.Day.value:
                        parameters.fromDate = idToBeAdded;
                        break;
                }
            }

        }

        function loadDimention() {
            var dimentions=[];
            for(var p in WhS_Analytics_GenericAnalyticDimensionEnum )
            {
                dimentions.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
            }
            return dimentions;
        }

        return {
            updateParametersFromDimentions: updateParametersFromDimentions,
            showCdrLog:showCdrLog,
            getMeasureColor: getMeasureColor
        };
    }

    appControllers.service('WhS_Analytics_GenericAnalyticService', analyticsServiceObj);


})(appControllers);