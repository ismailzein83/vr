appControllers.controller('RouteOverrideController2',
    function RouteOverrideController2($scope, $http) {


        $scope.optionsSuppliers = {
            selectedvalues: [],
            datasource: [],
            selectedgroup:false
        };
        $scope.subViewConnector.getActionData = function () {
            return {
                $type: "TOne.LCR.Entities.OverrideRouteActionData, TOne.LCR.Entities",
                Options: $scope.fillOptionsData()
            }
        }
        
        $scope.selectedSuppliers = [];
        $scope.suppliers = [];
        $http.get($scope.baseurl + "/api/BusinessEntity/GetCarriers",
         {
             params: {
                 carrierType: 2
             }
             })
            .success(function (response) {
                $scope.optionsSuppliers.datasource = response;
             if ($scope.routeRule) {
                 var tab = [];
                 $.each($scope.routeRule.ActionData.Options, function (i, value) {
                     $scope.optionsSuppliers.selectedvalues.length = 0;
                     var existobj = $scope.findExsiteObj($scope.optionsSuppliers.datasource, value.SupplierId, 'CarrierAccountID')
                     if (existobj != null) {
                         tab[i] = {
                             CarrierAccountID: value.SupplierId,
                             Name: existobj.Name,
                             Percentage: $scope.routeRule.ActionData.Options[i].Percentage
                         }
                     }

                 });
                 $scope.optionsSuppliers.selectedvalues = tab;
             }
         });

        
        $scope.fillOptionsData = function () {
            var tab = [];
            $.each($scope.optionsSuppliers.selectedvalues, function (i, value) {
                tab[i] = {
                    SupplierId: value.CarrierAccountID,
                    Percentage: value.Percentage
                }

            });
            return tab;
        }       
       
        $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };
        $scope.selectSupplier = function ($event, s) {
            $event.preventDefault();
            $event.stopPropagation();
            var index = null;
            try {
                var index = $scope.findExsite($scope.optionsSuppliers.selectedvalues, s.CarrierAccountID, 'CarrierAccountID');
            }
            catch (e) {

            }
            if (index >= 0) {
                $scope.optionsSuppliers.selectedvalues.splice(index, 1);
            }
            else
                $scope.optionsSuppliers.selectedvalues.push(s);
        };
    });