using System;

using Nito.AsyncEx;

using YouTubeSummariser.WebForm.Facade;

namespace YouTubeSummariser.WebForm.Controls;
public partial class YouTubeSummariserControl : System.Web.UI.UserControl
{
    public YouTubeSummariserClient YouTube { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public void Complete_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(this.YouTubeLinkUrl.Text))
        {
            return;
        }

        var request = new SummariseRequestModel
        {
            VideoUrl = this.YouTubeLinkUrl.Text,
            VideoLanguageCode = this.VideoLanguageCode.SelectedValue,
            SummaryLanguageCode = this.SummaryLanguageCode.SelectedValue,
        };

        var response = default(string);
        try
        {
            response = AsyncContext.Run<string>(() => this.YouTube.SummariseAsync(request));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            response = ex.Message;
        }

        this.Summary.Text = response;
    }

    public void Clear_Click(object sender, EventArgs e)
    {
        this.YouTubeLinkUrl.Text = default;
        this.VideoLanguageCode.SelectedItem.Value = "en";
        this.SummaryLanguageCode.SelectedItem.Value = "en";
        this.Summary.Text = default;
    }
}
