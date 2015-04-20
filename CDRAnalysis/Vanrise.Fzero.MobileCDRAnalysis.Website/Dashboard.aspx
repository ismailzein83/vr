<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/MasterPage.master"
    CodeFile="Dashboard.aspx.cs" Inherits="Dashboard" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>
<asp:Content runat="server" ContentPlaceHolderID="body">
    <div class="row-fluid">
        <!--BEGIN METRO STATES-->
        <div class="metro-nav metro-fix-view">
            <div class="span3">
                <div class="space40"></div>
                <div class="metro-nav-block nav-light-brown " style="margin-right: 10px;">
                    <a href="TrunckSwitches.aspx" data-original-title="" id="aTrunkscount"  runat="server" >
                        <i class="icon-link"></i>
                        <div class="info" id="divTruncksCount" runat="server"></div>
                        <div class="status"></div>
                    </a>
                </div>
                <div class="metro-nav-block nav-block-blue">
                    <a href="SwitchProfiles.aspx" data-original-title=""  id="aSwitchesCount"  runat="server">
                        <i class="icon-fullscreen"></i>
                        <div class="info" id="divSwitchesCount" runat="server"></div>
                        <div class="status">Sources</div>
                    </a>
                </div>
                <br />
                <div class="metro-nav-block nav-block-purple double" style="margin-top: 10px;">
                    <a href="NormalizationRules.aspx" data-original-title=""  id="aRulesCount"  runat="server">
                        <i class="icon-list"></i>
                        <div class="info" id="divRulesCount" runat="server"></div>
                        <div class="status">Normalization Rules</div>
                    </a>
                </div>
               
            </div>
            

            <div class="span1"></div>






            <div class="span3">
                <div class="space40"></div>
                <div class="metro-nav-block nav-olive" style="margin-right: 10px;">
                    <a href="Strategies.aspx" data-original-title="" id="aStrategies"  runat="server">
                        <i class="icon-edit"></i>
                        <div class="info" id="divStrategyCount" runat="server"></div>
                        <div class="status">Strategies</div>
                    </a>
                </div>
                <div class="metro-nav-block nav-block-yellow">
                    <a href="#" data-original-title="">
                        <i class="icon-th"></i>
                        <div class="info" id="div2" runat="server"></div>
                        <div class="status">....</div>
                    </a>
                </div>
                <br />




                <div class="metro-nav-block nav-block-red " style="margin-top: 10px; margin-right: 10px;">
                    <a href="ReportManagement.aspx" data-original-title="" id="aReportedCases"  runat="server">
                        <i class="icon-tasks"></i>
                        <div class="info" id="divReportedCases" runat="server"></div>
                        <div class="status">Reported Cases</div>
                    </a>
                </div>


                <br />
                <div class="metro-nav-block nav-deep-thistle " style="margin-top: 10px;">
                    <a href="ReportManagement.aspx" data-original-title=""  id="aReportManagement"  runat="server">
                        <i class="icon-file"></i>
                        <div class="info" id="divReports" runat="server"></div>
                        <div class="status">Reports</div>
                    </a>
                </div>

              <%--  <div class="metro-nav-block nav-block-red " style="margin-top: 10px; margin-right: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-search"></i>
                        <div class="info" id="div1" runat="server"></div>
                        <div class="status">....</div>
                    </a>
                </div>


                <br />
                <div class="metro-nav-block nav-block-orange " style="margin-top: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-envelope"></i>
                        <div class="info" id="div3" runat="server"></div>
                        <div class="status">...</div>
                    </a>
                </div>

                <div class="metro-nav-block nav-block-red " style="margin-top: 10px; margin-right: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-star"></i>
                        <div class="info" id="div4" runat="server"></div>
                        <div class="status">....</div>
                    </a>
                </div>


                <br />
                <div class="metro-nav-block nav-block-orange " style="margin-top: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-user"></i>
                        <div class="info" id="div5" runat="server"></div>
                        <div class="status">...</div>
                    </a>
                </div>

                <div class="metro-nav-block nav-block-red " style="margin-top: 10px; margin-right: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-film"></i>
                        <div class="info" id="div6" runat="server"></div>
                        <div class="status">....</div>
                    </a>
                </div>


                <br />
                <div class="metro-nav-block nav-block-orange " style="margin-top: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-th-large"></i>
                        <div class="info" id="div7" runat="server"></div>
                        <div class="status">...</div>
                    </a>
                </div>

                <div class="metro-nav-block nav-block-red " style="margin-top: 10px; margin-right: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-th"></i>
                        <div class="info" id="div8" runat="server"></div>
                        <div class="status">....</div>
                    </a>
                </div>


                <br />
                <div class="metro-nav-block nav-block-orange " style="margin-top: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-th-list"></i>
                        <div class="info" id="div9" runat="server"></div>
                        <div class="status">...</div>
                    </a>
                </div>

                <div class="metro-nav-block nav-block-red " style="margin-top: 10px; margin-right: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-cog"></i>
                        <div class="info" id="div10" runat="server"></div>
                        <div class="status">....</div>
                    </a>
                </div>


                <br />
                <div class="metro-nav-block nav-block-red " style="margin-top: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-signal"></i>
                        <div class="info" id="div11" runat="server"></div>
                        <div class="status">...</div>
                    </a>
                </div>

                <div class="metro-nav-block nav-block-red " style="margin-top: 10px; margin-right: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-home"></i>
                        <div class="info" id="div12" runat="server"></div>
                        <div class="status">....</div>
                    </a>
                </div>


                <br />
                <div class="metro-nav-block nav-block-orange " style="margin-top: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-info-sign"></i>
                        <div class="info" id="div13" runat="server"></div>
                        <div class="status">...</div>
                    </a>
                </div>

                <div class="metro-nav-block nav-light-yellow " style="margin-top: 10px; margin-right: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-edit"></i>
                        <div class="info" id="div14" runat="server"></div>
                        <div class="status">....</div>
                    </a>
                </div>


                <br />
                <div class="metro-nav-block nav-deep-terques" style="margin-top: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-print"></i>
                        <div class="info" id="div15" runat="server"></div>
                        <div class="status">...</div>
                    </a>
                </div>

                <div class="metro-nav-block nav-block-red " style="margin-top: 10px; margin-right: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-book"></i>
                        <div class="info" id="div16" runat="server"></div>
                        <div class="status">....</div>
                    </a>
                </div>


                <br />
                <div class="metro-nav-block nav-block-yellow " style="margin-top: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-thumbs-up"></i>
                        <div class="info" id="div17" runat="server"></div>
                        <div class="status">...</div>
                    </a>
                </div>--%>


            </div>

            <div class="span1"></div>
            <div class="span3">
                <%-- <h4> Switch CDRs Statistics</h4>--%>
                <Telerik:RadComboBox ID="ddlSwitches" runat="server" SkinID="None"  Skin="Metro"   Width="248px" BorderColor="LightGray" BorderStyle="Solid" BorderWidth="1px"
                    AutoPostBack="true" OnSelectedIndexChanged="ddlSwitches_SelectedIndexChanged" >
                </Telerik:RadComboBox>
                <div class="metro-nav-block nav-block-green double" style="margin-bottom: 10px;">
                    <a href="#" data-original-title="">
                        <i class="icon-calendar"></i>
                        <div class="info" id="divCDRsDate" runat="server"></div>
                        <div class="status">Last New CDRs Date</div>
                    </a>
                </div>

                <div class="metro-nav-block nav-deep-red  double">
                    <a href="#" data-original-title="">
                        <i class="icon-retweet"></i>
                        <div class="info" id="divCDRsCount" runat="server"></div>
                        <div class="status">New CDRs</div>
                    </a>
                </div>
                <div class="metro-nav-block nav-block-orange double" style="margin-top: 10px;">
                    <a href="UnNormalizedCalls.aspx" data-original-title=""  id="aUnNormalizedCGPN"  runat="server">
                        <i class="icon-warning-sign"></i>
                        <div class="info" id="divUnNormalizedCGPN" runat="server"></div>
                        <div class="status">UnNormalized CGPN</div>
                    </a>
                </div>
                  <div class="metro-nav-block nav-block-grey double" style="margin-top: 10px;">
                    <a href="UnNormalizedCalls.aspx" data-original-title=""  id="aUnNormalizedCDPN"  runat="server">
                        <i class="icon-warning-sign"></i>
                        <div class="info" id="divUnNormalizedCDPN" runat="server"></div>
                        <div class="status">UnNormalized CDPN</div>
                    </a>
                </div>
            </div>






        </div>
    </div>
</asp:Content>
