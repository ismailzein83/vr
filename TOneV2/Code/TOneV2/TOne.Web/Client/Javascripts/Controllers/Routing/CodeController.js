appControllers.controller('CodeController',
    function CodeController($scope, $http) {
       
        $scope.subViewConnector.getCodeSet = function () {
            return {
                $type: "TOne.LCR.Entities.CodeSelectionSet, TOne.LCR.Entities",
                Code: $scope.code,
                WithSubCodes: $scope.subCodes,
                ExcludedCodes: $scope.codeList
            };
        };

        //$scope.RouteRule.CodeSet = $scope.CodeSet;
        $('#CodeListddl').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('#CodeListddl').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });
        $scope.code = "";
        $scope.codeList = [];
        $scope.codeInpute ='';
        $scope.getCodes = function () {
            var label='';
            if ($scope.codeList.length == 0)
                label = "Fill codes...";
            else if ($scope.codeList.length ==1) {
                label += $scope.codeList[0] ;
            }
            else if ($scope.codeList.length < 5){
                $.each($scope.codeList, function (i, value) {
                    if (i < $scope.codeList.length-1)
                        label += value + ',';
                    else
                        label += value ;
                });
            }
            else
                label = $scope.codeList.length + " Codes selected";
           // RouteRule.CodeSet.ExcludedCodes = $scope.codeList;
            return label;
        };

        $scope.addCodeEnter = function (e) {        
                     
            if (e.keyCode == 13) {
                $scope.addCode(e);
            }
        }
        $scope.addCode = function (e) {
            e.preventDefault();
            e.stopPropagation();
            var valid = $scope.isNumber($scope.codeInpute);
            if (valid) {
                var index = null;
                var index = $scope.codeList.indexOf($scope.codeInpute);
                if (index >= 0) {
                    $scope.codeInpute = '';
                    return;
                }
                else {
                    $scope.codeList.push($scope.codeInpute);
                    $scope.codeInpute = '';
                }

            }
            else {
                $scope.codeInpute = '';
            }
        }
        $scope.removeCode = function (e,s) {
            e.preventDefault();
            e.stopPropagation();
            var index = $scope.codeList.indexOf(s);
            $scope.codeList.splice(index, 1);
        }
       
    });

