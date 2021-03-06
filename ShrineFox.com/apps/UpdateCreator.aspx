<%@ Page Title="PS4 Update Creator" MaintainScrollPositionOnPostback="true" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UpdateCreator.aspx.cs" Inherits="ShrineFox.com.UpdateCreator" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="navipath">
		<a href="https://shrinefox.com/"><i class="fa fa-home" aria-hidden="true"></i> ShrineFox.com</a> 
		<i class="fa fa-angle-right" aria-hidden="true"></i> <a href="https://shrinefox.com/WebApps">Apps</a> 
        <i class="fa fa-angle-right" aria-hidden="true"></i> <%: Page.Title %>
	</div>
	<h1><%: Page.Title %></h1>
    <p>
        Generate a PS4 Game Update with your choice of patches.
        <br>Powered by <a href="https://github.com/zarroboogs/ppp">Zarroboogs' ps4 persona patches (ppp)</a>.
    </p>
    <asp:ScriptManager EnablePartialRendering="true" ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel">
        <ProgressTemplate>
            <div class="modal active modal-sm">
                <span class="modal-overlay"></span>
                <div class="loading loading-lg"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
        <div>
            <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <!--Notice-->
                    <asp:PlaceHolder ID="NoticePlaceHolder" runat="server">
                        <div class="notice yellow"><p>Some patches cannot be disabled, such as <b>Mod Support</b> and <b>Intro Skip</b>.
                            <br>I did this to exponentially reduce the number of PKGs/eboots I would need to create.
                            <br>These default patches are harmless and generally only beneficial.
                            <br>If you would still prefer a different combination of patches, see <a href="https://shrinefox.com/guides/2021/12/28/manually-patching-ps4-persona-games/">this guide regarding patching games manually</a>.
                    </p></div>
                    </asp:PlaceHolder>
                    <!--Game & Region Select-->
                    <ul class="tab tab-block">
                        <li class="tab-item active" id="p5rtab" runat="server"><asp:LinkButton id="p5r" runat="server" OnClick="GameTab_Click" OnClientClick="ShowProgress();">P5R</asp:LinkButton></li>
                        <li class="tab-item" id="p3dtab" runat="server"><asp:LinkButton id="p3d" runat="server" OnClick="GameTab_Click" OnClientClick="ShowProgress();" class="badge" data-badge="new">P3D</asp:LinkButton></li>
                        <li class="tab-item" id="p4dtab" runat="server"><asp:LinkButton id="p4d" runat="server" OnClick="GameTab_Click" OnClientClick="ShowProgress();" class="badge" data-badge="new">P4D</asp:LinkButton></li>
                        <li class="tab-item" id="p5dtab" runat="server"><asp:LinkButton id="p5d" runat="server" OnClick="GameTab_Click" OnClientClick="ShowProgress();" class="badge" data-badge="new">P5D</asp:LinkButton></li>
                        <li class="tab-item tab-action">
                            <div class="input-group input-inline">
                                <label class="form-radio">
                                    <asp:LinkButton id="usa" runat="server" OnClick="Radio_Click" OnClientClick="ShowProgress();"><i class="fas fa-circle"></i> USA</asp:LinkButton>
                                </label>
                                <label class="form-radio">
                                    <asp:LinkButton id="eur" runat="server" OnClick="Radio_Click" OnClientClick="ShowProgress();"><i class="far fa-circle"></i> EUR</asp:LinkButton>
                                </label>
                            </div>
                        </li>
                    </ul>
                    <!--Patch Select-->
                    <ul class="tab">
                        <li class="tab-item active" id="mod_support_tab" runat="server"><asp:LinkButton id="mod_support" runat="server" OnClick="PatchTab_Click" OnClientClick="ShowProgress();">Mod Support <i class="fas fa-check-square"></i></asp:LinkButton></li>
                        <li class="tab-item" id="_0505_tab" runat="server"><asp:LinkButton id="_0505" runat="server" OnClick="PatchTab_Click" OnClientClick="ShowProgress();">5.05 Backport <i class="fas fa-check-square"></i></asp:LinkButton></li>
                        <li class="tab-item" id="intro_skip_tab" runat="server"><asp:LinkButton id="intro_skip" runat="server" OnClick="PatchTab_Click" OnClientClick="ShowProgress();">Intro Skip <i class="fas fa-check-square"></i></asp:LinkButton></li>
                        <li class="tab-item" id="all_dlc_tab" runat="server"><asp:LinkButton id="all_dlc" runat="server" OnClick="PatchTab_Click" OnClientClick="ShowProgress();">Content Enabler</asp:LinkButton></li>
                        <li class="tab-item" id="no_trp_tab" runat="server"><asp:LinkButton id="no_trp" runat="server" OnClick="PatchTab_Click" OnClientClick="ShowProgress();">Disable Trophies</asp:LinkButton></li>
                        <li class="tab-item" id="square_tab" runat="server"><asp:LinkButton id="square" runat="server" OnClick="PatchTab_Click" OnClientClick="ShowProgress();">Global Square Menu <i class="fas fa-check-square"></i></asp:LinkButton></li>
                        <li class="tab-item" id="p5_save_tab" runat="server"><asp:LinkButton id="p5_save" runat="server" OnClick="PatchTab_Click" OnClientClick="ShowProgress();">P5 Save Bonus <i class="fas fa-check-square"></i></asp:LinkButton></li>
                        <li class="tab-item d-none" id="overlay_tab" runat="server"><asp:LinkButton id="overlay" runat="server" OnClick="PatchTab_Click" OnClientClick="ShowProgress();">Disable Screenshot Overlay</asp:LinkButton></li>
                    </ul>
                    <!--Selected Patch Info & Toggle-->
                    <div class="card">
                        <div class="card-image"><img class="img-responsive" id="image" runat="server" src="https://66.media.tumblr.com/c3f99e21c7edb1df53e7f2fa02117621/tumblr_inline_pl680q6yWy1rp7sxh_500.gif"></div>
                        <div class="card-header">
                            <div class="card-title h5" id="patch_name" runat="server">Mod Support</div>
                            <div class="card-subtitle text-muted" id="description" runat="server">mod.cpk file replacement via FTP</div>
                            <div class="card-body" id="description_long" runat="server">
                                Loads modded files from a <kbd>mod.cpk</kbd> file from <code>/data/p5r</code> on the PS4's internal memory via FTP.
                                <br><b>Enabled by default.</b>
                            </div>
                        </div>
                        <div class="card-footer" style="font-size:16pt;">
                            <label class="form-checkbox float-right">
                                <asp:LinkButton id="enable" enabled="false" runat="server" OnClick="Enable_Click" OnClientCheckedChanged="ShowProgress();"><i class="fas fa-check-square"></i> Enable This Patch</asp:LinkButton>
                            </label>
                        </div>
                    </div>
                    <!--Download Button-->
                    <div class="card">
                        <div class="card-header">
                            <div class="card-title h5" id="titleID" runat="server">Patches Applied to <b>CUSA17416</b> (Persona 5 Royal, USA):</div>
                            <div class="card-subtitle text-muted" id="appliedPatches" runat="server">Mod Support, 5.05 Backport, DLC Msg Skip, Square Menu</div>
                        </div>
                        <div class="card-footer">
                            <label class="form-checkbox float-right">
                                <div class="dropdown dropdown-right"><a class="btn btn-primary dropdown-toggle" tabindex="0">Download As... <i class="icon icon-caret"></i></a>
                                    <ul class="menu text-left">
                                        <li class="menu-item"><asp:HyperLink runat="server" id="pkg" NavigateUrl="http://up-4.net/d/QmCj" target="_blank" Text="PKG"/></li>
                                        <li class="menu-item"><asp:HyperLink runat="server" id="eboot" NavigateUrl="http://up-4.net/d/QmAs" target="_blank" Text="eboot.bin"/></li>
                                    </ul>
                                </div>
                            </label>
                        </div>
                    </div>
                    <!--PKG Info-->
                    <div class="card">
                        <div class="card-header">
                            <div class="card-title h5">Base Game FPKG Hash</div>
                            <div class="card-subtitle text-muted">In order to install the Update <kbd>PKG</kbd>, you must have installed the same base game <kbd>FPKG</kbd> I generated it from.
                                <br>Use a program like <a href="https://download.cnet.com/HashTab/3000-2094_4-84837.html">HashTab</a> to check if your base game <kbd>FPKG</kbd> matches.
                                <br>If it doesn't match, grab the <kbd>eboot.bin</kbd> instead and follow <a href="https://shrinefox.com/guides/2021/12/28/manually-patching-ps4-persona-games/">the guide</a> to create your own.
                                <br><br>Read more about installing PKGs and setting up the PS4 for modding <a href="https://shrinefox.com/guides/2020/09/30/modding-persona-5-royal-jp-on-ps4-fw-6-72/">here</a>.
                            </div>
                        </div>
                        <div class="card-footer">
                            <table class="table table-striped table-hover">
                                <tbody>
                                    <tr>
                                        <td><b>CRC32</b></td>
                                        <td id="crc32" runat="server">E2452B1C</td>
                                    </tr>
                                    <tr>
                                        <td><b>MD5</b></td>
                                        <td id="md5" runat="server">E669D7F9F9AB3989A2ED9D8D615547BD</td>
                                    </tr>
                                    <tr>
                                        <td><b>SHA-1</b></td>
                                        <td id="sha1" runat="server">25ABE8EFBD0D0CB7307927CD6AE6F1BB5ED506F4</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="p5r" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="p3d" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="p4d" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="p5d" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="usa" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="eur" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="mod_support" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="_0505" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="intro_skip" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="all_dlc" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="no_trp" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="square" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="p5_save" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="overlay" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="enable" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    <script type="text/javascript">
    function ShowProgress()
    {
        document.getElementById('<% Response.Write(UpdateProgress1.ClientID); %>').style.display = "inline";
    }
    </script>
</asp:Content>
