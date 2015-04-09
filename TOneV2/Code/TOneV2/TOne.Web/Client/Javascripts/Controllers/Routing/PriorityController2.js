appControllers.controller('PriorityController2',
    function PriorityController2($scope, $http, CarriersService) {

       
       
        $scope.subViewConnector.getActionData = function () {           
            return {
                $type: "TOne.LCR.Entities.PriorityRouteActionData, TOne.LCR.Entities",
                Options: $scope.fillOptionsData()
            }
        }
        $scope.fillOptionsData = function () {
            var tab = [];
            $.each($scope.optionsSuppliers.selectedvalues, function (i, value) {
                tab[i] = {
                    SupplierId: value.CarrierAccountID,
                    Priority: ($scope.optionsSuppliers.selectedvalues.length) - i,
                    Force:(value.Force==true)?true:false,
                    Percentage: value.Percentage
                }

            });
            return tab;
        }
        $scope.optionsSuppliers = {
            selectedvalues: [],
            datasource: [],
            selectedgroup: false
        };
        
        CarriersService.getSuppliers()
            .then(function (response) {
                $scope.optionsSuppliers.datasource = response;
                $scope.optionsSuppliers.selectedvalues.length = 0;
                if ($scope.routeRule) {
                    var tab = [];
                    $.each($scope.routeRule.ActionData.Options, function (i, value) {
                        var existobj = $scope.findExsiteObj($scope.optionsSuppliers.datasource, value.SupplierId, 'CarrierAccountID')
                        if (existobj != null) {
                            tab[i] = {
                                CarrierAccountID: value.SupplierId,
                                Name: existobj.Name,
                                Force: value.Force,
                                Percentage: $scope.routeRule.ActionData.Options[i].Percentage,
                                Priority: value.Priority
                            }
                        }

                    });
                    $scope.optionsSuppliers.selectedvalues = tab;
                }

         })
      
        $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };
        $scope.selectSupplier = function ($event, s) {
            $event.preventDefault();
            $event.stopPropagation();

            var index = $scope.findExsite($scope.optionsSuppliers.selectedvalues, s.CarrierAccountID, 'CarrierAccountID');

            if (index >= 0) {
                $scope.optionsSuppliers.selectedvalues.splice(index, 1);
            }
            else {
                $scope.optionsSuppliers.selectedvalues.push(s);
            }

        };
       
    });