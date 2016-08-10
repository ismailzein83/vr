(function (appControllers) {

    'use strict';

    ExpressionBuilderService.$inject = ['VRModalService'];

    function ExpressionBuilderService(VRModalService) {
        return ({
            getFilterGroupFieldNames: getFilterGroupFieldNames,
        });

        function getFilterGroupFieldNames(filterGroup) {
            var fieldNames = [];
            if(filterGroup !=undefined)
            {
                if(filterGroup.Filters != undefined)
                {
                    for(var i=0;i<filterGroup.Filters.length;i++)
                    {
                        var filter = filterGroup.Filters[i];
                        fieldNames.push(filter.FieldName);
                        if (filter.Filters != undefined)
                            getFilterFieldNames(filter, fieldNames)
                    }
                }
                return fieldNames;
            }
        }
        function getFilterFieldNames(filter, fieldNames) {
            if (filter != undefined) {
                if (filter.Filters != undefined) {
                    for (var i = 0; i < filter.Filters.length; i++) {
                        var filterObj = filter.Filters[i];
                        fieldNames.push(filterObj.FieldName);
                        if (filterObj.Filters != undefined)
                            getFilterFieldNames(filterObj, fieldNames);
                    }
                }
            }
        }

    };

    appControllers.service('VR_GenericData_RecordFilterService', ExpressionBuilderService);

})(appControllers);
