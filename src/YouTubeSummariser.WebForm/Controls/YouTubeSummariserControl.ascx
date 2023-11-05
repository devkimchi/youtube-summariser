<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YouTubeSummariserControl.ascx.cs" Inherits="YouTubeSummariser.WebForm.Controls.YouTubeSummariserControl" %>

<div class="container">

    <div class="row">
        <h2>YouTube Summariser</h2>
    </div>

    <div class="row">
        <div class="col-md-4">
            <div class="mb-3">
                <label for="<%= YouTubeLinkUrl.ClientID %>" class="form-label"><strong>YouTube Link</strong></label>
                <asp:TextBox runat="server" CssClass="form-control" ID="YouTubeLinkUrl" TextMode="SingleLine" placeholder="Add YouTube link here" />
            </div>
        </div>
        <div class="col-md-4">
            <div class="mb-3">
                <label for="<%= VideoLanguageCode.ClientID %>" class="form-label"><strong>Video Language</strong></label>
                <asp:DropDownList runat="server" CssClass="form-select" ID="VideoLanguageCode" AutoPostBack="false">
                    <asp:ListItem Text="English" Value="en" Selected="True" />
                    <asp:ListItem Text="Korean" Value="ko" />
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-md-4">
            <div class="mb-3">
                <label for="<%= SummaryLanguageCode.ClientID %>" class="form-label"><strong>Summary Language</strong></label>
                <asp:DropDownList runat="server" CssClass="form-select" ID="SummaryLanguageCode" AutoPostBack="false">
                    <asp:ListItem Text="English" Value="en" Selected="True" />
                    <asp:ListItem Text="Korean" Value="ko" />
                </asp:DropDownList>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="mb-3">
            <div class="col-md-12">
                <asp:Button runat="server" CssClass="btn btn-primary" ID="Summarise" Text="Summarise!" OnClick="Complete_Click" />
                <asp:Button runat="server" CssClass="btn btn-secondary" ID="Clear" Text="Clear!" OnClick="Clear_Click" />
            </div>
        </div>
    </div>

    <div class="row">
        <div class="mb-3">
            <div class="progress" id="progressbar">
                <div class="progress-bar progress-bar-striped progress-bar-animated indeterminate" role="progressbar" aria-valuenow="75" aria-valuemin="0" aria-valuemax="100" style="width: 75%"></div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="mb-3">
            <div class="col-md-12">
                <label for="<%= Summary.ClientID %>" class="form-label"><strong>Summary</strong></label>
                <asp:TextBox runat="server" CssClass="form-control" ID="Summary" TextMode="MultiLine" Rows="10" placeholder="Result will show here" ReadOnly="True" />
            </div>
        </div>
    </div>

</div>

<script>
    $(document).ready(function () {
        $("#progressbar").hide();

        $("#<%= Summarise.ClientID %>").click(function () {
            $("#progressbar").show();

            if ($("#<%= Summary.ClientID %>").val()) {
                $("#progressbar").hide();
            }
        });

        $("#<%= Clear.ClientID %>").click(function () {
            $("#progressbar").hide();
        });
    });
</script>
