(function (appControllers) {

    "use strict";

    TimeFormatEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','TimeFormateTemplatesEnum'];

    function TimeFormatEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService,TimeFormateTemplatesEnum) {

        var timeFormatValue;
        var context;
        var expressionBuilderDirectiveAPI;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                timeFormatValue = parameters.timeFormatValue;
                context = parameters.Context;
            }
        }

        function defineScope() {


            $scope.timeFormatparts = [{
                value: 0,
                description: "Year"
            },
            {
                value: 1,
                description: "Month"
            },
            {
                value: 2,
                description: "Day"
            },
            {
                value: 3,
                description: "Hour"
            }
            ,
            {
                value: 4,
                description: "Minute"
            },
            {
                value: 5,
                description: "Second"
            },
            {
                value: 6,
                description: "Seconds Fraction"
            },
             {
                 value: 7,
                 description: 'A.M. or P.M.'
             }
            ];



            $scope.timeFormatTemplates = UtilsService.getArrayEnum(TimeFormateTemplatesEnum);

            $scope.expressionTemplates = [{
                value: 0,
                timeValue: 0,
                description: "yyyy (ex: 2008)",
                expression: "yyyy"

            }, {
                value: 1,
                timeValue: 0,
                description: "yyy (ex: 008)",
                expression: "yyy"
            }, {
                value: 2,
                timeValue: 0,
                description: "yy (ex: 08)",
                expression: "yy"
            }, {
                value: 3,
                timeValue: 1,
                description: "MMMM (ex: March)",
                expression: "MMMM"
            }, {
                value: 4,
                timeValue: 1,
                description: "MMM (ex: Mar)",
                expression: "MMM"
            }, {
                value: 5,
                timeValue: 1,
                description: "MM (ex: 03)",
                expression: "MM"
            }, {
                value: 6,
                timeValue: 1,
                description: "M (ex: 3)",
                expression: "M"
            }, {
                value: 9,
                timeValue: 2,
                description: "dd (ex: 09)",
                expression: "dd"
            }, {
                value: 10,
                timeValue: 2,
                description: "d (ex: 9)",
                expression: "d"
            }, {
                value: 11,
                timeValue: 3,
                description: "HH (ex: 16)",
                expression: "HH"
            }, {
                value: 12,
                timeValue: 3,
                description: "H (ex: 16)",
                expression: "H"
            }, {
                value: 13,
                timeValue: 3,
                description: "hh (ex: 04)",
                expression: "hh"
            }, {
                value: 14,
                timeValue: 3,
                description: "h (ex: 4)",
                expression: "h"
            }, {
                value: 15,
                timeValue: 4,
                description: "mm (ex: 05)",
                expression: "mm"
            }, {
                value: 16,
                timeValue: 4,
                description: "m (ex: 5)",
                expression: "m"
            }, {
                value: 17,
                timeValue: 5,
                description: "ss (ex: 07)",
                expression: "ss"
            }, {
                value: 18,
                timeValue: 5,
                description: "s (ex: 7)",
                expression: "s"
            }, {
                value: 19,
                timeValue: 6,
                description: "ffff (ex: 1230)",
                expression: "ffff"
            }, {
                value: 20,
                timeValue: 6,
                description: "fff (ex: 123)",
                expression: "fff"
            }, {
                value: 21,
                timeValue: 6,
                description: "ff (ex: 12)",
                expression: "ff"
            }, {
                value: 22,
                timeValue: 6,
                description: "f (ex: 1)",
                expression: "f"
            }
            , {
                value: 23,
                timeValue: 7,
                description: "tt (ex: PM)",
                expression: "tt"
            }
            , {
                value: 24,
                timeValue: 7,
                description: "t (ex: P)",
                expression: "t"
            }];
            $scope.showTimeParts = false;
            $scope.onTimeFormatSelectionChanged = function () {
                if ($scope.selectedTimeFormatTemplate.value == TimeFormateTemplatesEnum.Custom.value)
                    $scope.showTimeParts = true;
                else
                    $scope.showTimeParts = false;
            };

            $scope.onTimeSelectionChanged = function () {
                if ($scope.selectedTimeFormatParts != undefined) {
                    $scope.selectedExpression = undefined;
                    $scope.filteredExpressionTemplates = UtilsService.getFilteredArrayFromArray($scope.expressionTemplates, $scope.selectedTimeFormatParts.value, "timeValue");

                }
            };

            $scope.isbuttonDisabled = function () {
            };



            $scope.addExpression = function () {
                if ($scope.timeFormatValue != undefined)
                    $scope.timeFormatValue = $scope.timeFormatValue.concat($scope.selectedExpression.expression);
                else
                    $scope.timeFormatValue = $scope.selectedExpression.expression;
            };

            $scope.saveTimeFormatBuilder = function () {
                return saveTimeFormat();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.isFormatValueRequired = function () {
                if ($scope.selectedTimeFormatTemplate.value == TimeFormateTemplatesEnum.Custom.value && $scope.timeFormatValue == undefined)
                    return true;
                else
                    return false;
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadTextAreaSection, setTitle]).then(function () {
                })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    })
                   .finally(function () {
                       $scope.isLoading = false;
                   });


                function setTitle() {
                    $scope.title = 'Time Format Builder';
                }



                function loadTextAreaSection() {
                    if (timeFormatValue != undefined) {
                        var timeFormatTemplate = UtilsService.getEnum(TimeFormateTemplatesEnum, 'expression', timeFormatValue);
                        if (timeFormatTemplate != undefined)
                        {
                            $scope.selectedTimeFormatTemplate = timeFormatTemplate;
                        }else
                        {
                            $scope.selectedTimeFormatTemplate = TimeFormateTemplatesEnum.Custom;
                            $scope.timeFormatValue = timeFormatValue;
                        }

                    }
                }
            }
        }

        function buildTimeFormatObjFromScope() {
            if ($scope.selectedTimeFormatTemplate.value != TimeFormateTemplatesEnum.Custom.value)
                return $scope.selectedTimeFormatTemplate.expression;
            return $scope.timeFormatValue;
        }

        function saveTimeFormat() {

            var timeFormatValueObject = buildTimeFormatObjFromScope();
            if ($scope.onSetTimeFormatBuilder != undefined)
                $scope.onSetTimeFormatBuilder(timeFormatValueObject);
            $scope.modalContext.closeModal();

        }
    }

    appControllers.controller('VR_TimeFormatEditorController', TimeFormatEditorController);
})(appControllers);
