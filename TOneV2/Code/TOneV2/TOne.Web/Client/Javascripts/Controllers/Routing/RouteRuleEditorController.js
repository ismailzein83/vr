appControllers.controller('RouteRuleEditorController',
    function RouteRuleEditorController($scope, $http, $routeParams, notify) {
        $scope.routeRule = null;
        $scope.customers = [];  
        $scope.selectedCustomers = [];
        $scope.ruletype = { name: 'Select ..', url: '' };
        $scope.templates = [
         { name: 'Override Route', url: '/Client/Templates/PartialTemplate/RouteOverrideTemplate.html' },
         
         { name: 'Priority Rule', url: '/Client/Templates/PartialTemplate/PriorityTemplate.html' },
        { name: 'Block Route', url: '' }
        ]

        $http.get($scope.baseurl + "/api/BusinessEntity/GetCarriers",
         {
             params: {
                 carrierType: 1
             }
         })
         .success(function (response) {
             $scope.customers = response;
             if (typeof ($routeParams.RouteRuleId) != undefined) {
                 $http.get($scope.baseurl + "/api/Routing/GetRouteRuleDetails",
                 {
                     params: {
                         RouteRuleId: $routeParams.RouteRuleId
                     }
                 })
                  .success(function (response) {
                      $scope.routeRule = response;
                      var tab = [];
                      $.each($scope.routeRule.CarrierAccountSet.Customers.SelectedValues, function (i, value) {
                          var existobj = $scope.findExsiteObj($scope.customers, value, 'CarrierAccountID')
                          if (existobj != null)
                              tab[i] = existobj;

                      });
                      $scope.selectedCustomers = tab;
                      $scope.BEDDate = $scope.routeRule.BeginEffectiveDate;
                      $scope.EEDDate = $scope.routeRule.EndEffectiveDate;
                      $scope.Reason = $scope.routeRule.Reason;
                      $scope.ruletype = null;
                      //alert($scope.getRuletypeIndex($scope.routeRule.ActionData.$type))
                      $scope.ruletype = $scope.templates[$scope.getRuletypeIndex($scope.routeRule.ActionData.$type)]
                         // $scope.getRuletype($scope.routeRule.ActionData.$type)

                  });
             }
         });
        $http.get($scope.baseurl + "/api/BusinessEntity/GetCarriers",
        {
            params: {
                carrierType: 2
            }
            })
        .success(function (response) {
            $scope.suppliers = response;
        });
        $scope.selectedSuppliers = [];
        $scope.onloadRuletype = function () {
            $scope.selectedSuppliers.length = 0;      
            var tab = [];
            $.each($scope.routeRule.ActionData.Options, function (i, value) {
                var existobj = $scope.findExsiteObj($scope.suppliers, value.SupplierId, 'CarrierAccountID')
                if (existobj != null) {
                    //alert(value.Percentage + "//" + value.Force)
                    //tab[i].Percentage = value.Percentage;
                    //tab[i].Force = value.Force;
                    tab[i] = {
                        CarrierAccountID: value.SupplierId,
                        Name: existobj.Name,
                        Force: value.Force,
                        Percentage: $scope.routeRule.ActionData.Options[i].Percentage,
                        Priority: value.Priority,


                    }
                }                      
               
            });
            console.log(tab)
            $scope.selectedSuppliers = tab;
            
        }
        
        $('.dropdown').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

         //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('.dropdown').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });
        
        var dropdownHidingTimeoutHandler;

        $('.dropdown-custom').on('mouseenter', function () {
            var $this = $(this);
            clearTimeout(dropdownHidingTimeoutHandler);
            if (!$this.hasClass('open')) {
                $('.dropdown-toggle', $this).dropdown('toggle');
            }
        });

        $('.dropdown-custom').on('mouseleave', function () {
            var $this = $(this);
            dropdownHidingTimeoutHandler = setTimeout(function () {
                if ($this.hasClass('open')) {
                    $('.dropdown-toggle', $this).dropdown('toggle');
                }
            }, 150);
        });        
        $scope.muteAction = function (e) {
            e.preventDefault();
            e.stopPropagation();
        }
      
        $scope.update = function (val, model) {
            if (model == 'routetype' && val.name != 'Customer') {
                $scope[model] = $scope.routeTemplates[0];
                notify.closeAll();
                notify({ message: 'This module is under construction.',classes:"alert alert-danger"});
            }
            else {
                $scope[model] = val;
            }
           
        }       
        
        
     
        $scope.editorTemplates = [
            { name: 'Zone', url: '/Client/Templates/PartialTemplate/ZoneTemplate.html' },
            { name: 'Code', url: '/Client/Templates/PartialTemplate/CodeTemplate.html' }
        ]
        $scope.editortype = $scope.editorTemplates[0];         
        $scope.routeTemplates = [
          { name: 'Customer', url: '/Client/Templates/PartialTemplate/CustomerTemplate.html' },
          { name: 'Pool', url: '/Client/Templates/PartialTemplate/PoolTemplate.html' },
          { name: 'Product', url: '/Client/Templates/PartialTemplate/ProductTemplate.html' }
        ]
        $scope.routetype = $scope.routeTemplates[0];
        $scope.subViewConnector = {}     
        $scope.saveRule = function () {
            var routeRule =
                {
                    CodeSet: $scope.subViewConnector.getCodeSet(),
                    CarrierAccountSet: $scope.subViewConnector.getCarrierAccountSet(),
                    ActionData: $scope.subViewConnector.getActionData(),
                    Type: "RouteRule",
                    BeginEffectiveDate: $scope.BEDDate,
                    EndEffectiveDate: ($scope.EEDDate),
                    Reason: $scope.Reason
                };
            $http.post($scope.baseurl + "/api/routing/SaveRouteRule",
                         routeRule)
                     .success(function (response) {

                     });
        }
        $scope.findExsiteObj = function (arr, value, attname) {
            var obj = null;
            for (var i = 0; i < arr.length; i++) {
                if (arr[i][attname] == value) {
                    obj = arr[i];
                }
            }
            return obj;
        }

        $scope.getRuletypeIndex = function (type) {

            switch (type) {
                case "TOne.LCR.Entities.OverrideRouteActionData, TOne.LCR.Entities":
                    return 0;
                    break;
                case "TOne.LCR.Entities.PriorityRouteActionData, TOne.LCR.Entities":
                    return 1;
                    break;
                case "TOne.LCR.Entities.BlockRouteActionData, TOne.LCR.Entities":
                    return 2;
                    break;

                default:
                    return 0;
                    break;
            }
        }

    });

function waitAlert(msg) {
    showAlertBootStrapMsg("attention", msg, true);
}
function hideWaitAlert() {
    $(".attention").fadeOut('slow');
    $('#notification').html('');
}
function showAlertBootStrapMsg(type, msg, wait) {
    $('#notification').html('<div class="alert ' + type + '" style="display: none;"> ' + msg + '  <img src="images/alert/close.png" alt="" class="close" data-dismiss="alert" ></div>');
    $("." + type).fadeIn('slow');
    if (!wait) {
        setTimeout(function () {
            $("." + type).fadeOut('slow');
            $('#notification').html('');
        }, 7000);
    }
    else {
        setTimeout(function () {
            $("." + type).fadeOut('slow');
            $('#notification').html('');
        }, wait);

    }
}
