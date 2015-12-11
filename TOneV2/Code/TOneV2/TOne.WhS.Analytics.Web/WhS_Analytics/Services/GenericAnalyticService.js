(function (appControllers) {

    "use strict";

    analyticsServiceObj.$inject = ['LabelColorsEnum', 'GenericAnalyticMeasureEnum','VRModalService','WhS_Analytics_GenericAnalyticDimensionEnum'];

    function analyticsServiceObj(LabelColorsEnum, GenericAnalyticMeasureEnum, VRModalService, WhS_Analytics_GenericAnalyticDimensionEnum) {
   
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
            if (coldef.tag.value === GenericAnalyticMeasureEnum.ACD.value)
                return getACDColor(dataItem.MeasureValues[GenericAnalyticMeasureEnum.ACD.name], dataItem.MeasureValues[GenericAnalyticMeasureEnum.Attempts.name], parameters);
            else if (coldef.tag.value === GenericAnalyticMeasureEnum.ASR.value)
                return getASRColor(dataItem.MeasureValues[GenericAnalyticMeasureEnum.ASR.name], dataItem.MeasureValues[GenericAnalyticMeasureEnum.Attempts.name], parameters);
            return undefined;
        }

        function showCdrLog(parameters) {
            VRModalService.showModal('/Client/Modules/WhS_Analytics/Views/CDR/CDRLog.html', parameters, {
                useModalTemplate: true,
                width: "80%",
                maxHeight: "800px",
                title: "CDR Log"
            });
        }

        function updateParametersFromDimentions(parameters, ctrl, dataItem) {
            var dimentions = loadDimention();
            if (ctrl === undefined)
                return;

            for (var i = 0; i < dimentions.length; i++) {
                var dimention = dimentions[i];
                for (var j = 0; j < ctrl.filters.length; j++) {
                    if (dimention.value == ctrl.filters[j].Dimension) {

                        switch (dimention.value) {
                            case WhS_Analytics_GenericAnalyticDimensionEnum.Zone.value:
                                parameters.saleZoneIds = ctrl.filters[j].FilterValues;
                                break;
                            case WhS_Analytics_GenericAnalyticDimensionEnum.Customer.value:
                                parameters.customerIds = ctrl.filters[j].FilterValues;
                                break;
                            case WhS_Analytics_GenericAnalyticDimensionEnum.Supplier.value:
                                parameters.supplierIds = ctrl.filters[j].FilterValues;
                                break;
                            case WhS_Analytics_GenericAnalyticDimensionEnum.Switch.value:
                                parameters.switchIds = ctrl.filters[j].FilterValues;
                                break;
                            case WhS_Analytics_GenericAnalyticDimensionEnum.SupplierZone.value:
                                parameters.supplierZoneIds = ctrl.filters[j].FilterValues;
                                break;
                        }
                    }

                }
            }
            for (var i = 0; i < dimentions.length; i++) {
                var dimention = dimentions[i];
                for (var j = 0; j < ctrl.dimensionFields.length; j++) {
                    if (dimention.value == ctrl.dimensionFields[j].value) {
                        
                        switch (dimention.value) {
                            case WhS_Analytics_GenericAnalyticDimensionEnum.Zone.value:
                                parameters.saleZoneIds.push(dataItem.DimensionValues[j].Id);
                                break;
                            case WhS_Analytics_GenericAnalyticDimensionEnum.Customer.value:
                                parameters.customerIds.push(dataItem.DimensionValues[j].Id);
                                break;
                            case WhS_Analytics_GenericAnalyticDimensionEnum.Supplier.value:
                                parameters.supplierIds.push(dataItem.DimensionValues[j].Id);
                                break;
                            case WhS_Analytics_GenericAnalyticDimensionEnum.Switch.value:
                                parameters.switchIds.push(dataItem.DimensionValues[j].Id);
                                break;
                            case WhS_Analytics_GenericAnalyticDimensionEnum.SupplierZone.value:
                                parameters.supplierZoneIds.push(dataItem.DimensionValues[j].Id);
                                break;
                        }
                    }
                    
                }
            }
      
            

          //  updateParametersFromGroupKeys(parameters, scope.gridParentScope, scope.dataItem);
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